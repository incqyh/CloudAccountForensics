using CAF.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper.HuaWei
{
    class RuntimeData
    {
        public bool isEnd;
        public uint totalCount;
    }

    class FileSyncContent
    {
        public string ascending;
        public string cursor;
        public string order;
        public string fileType;
        public string traceId;
        public List<string> paths = new List<string>();
    }

    class FileDownloadContent
    {
        public string fileType;
        public string traceId;
        public List<string> files = new List<string>();
        public List<string> fields = new List<string>();
    }

    class PictureDownloadContent
    {
        public Dictionary<string, string> fileList = new Dictionary<string, string>();
        public string traceId;
        public string type;
    }

    class PictureInfoContent
    {
        public Dictionary<string, string> fileList = new Dictionary<string, string>();
        public Dictionary<string, string> lcd = new Dictionary<string, string>();
        public string traceId;
    }

    public partial class HuaWeiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        public void InitHelper()
        {
            InitFetcher();
            InitParser();
        }

        public bool IsLogIn()
        {
            return false;
        }

        public async Task<List<CallRecord>> SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
            };

            return await FetchCallRecordAsync();
        }

        public async Task<List<Contact>> SyncContactAsync()
        {
            string data = await FetchContactAsync();
            return ParseContact(data);
        }

        public async Task<List<Common.File>> SyncFileAsync(Common.File withoutUse)
        {
            try
            {
                string homeUrl = "https://cloud.huawei.com/home";
                string re = await client.GetStringAsync(homeUrl);
                string pattern = @"uid = '(\S+)'";
                string uid = Regex.Match(re, pattern).Result("$1");
                client.DefaultRequestHeaders.Remove("userId");
                client.DefaultRequestHeaders.Add("userId", uid);
            }
            catch(Exception)
            {
                throw new Exception("通讯录获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }

            List<Common.File> files = new List<Common.File>();

            Queue<string> folders = new Queue<string>();
            folders.Enqueue("%2F");
            
            while(folders.Count != 0)
            {
                string path = folders.Dequeue();

                // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("05003"));
                // while (!WebHelper.GetTraceIDDone)
                // {
                //     Thread.Sleep(10);
                // }
                // WebHelper.GetTraceIDDone = false;
                string url = "https://cloud.huawei.com/nspservice/queryFolder";
                FileSyncContent form = new FileSyncContent();
                form.ascending = "1";
                form.cursor = "";
                form.fileType = "6";
                form.order = "name";
                form.traceId = GetTraceId("05003");
                form.paths.Add(path);

                var jsonString = JsonConvert.SerializeObject(form);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string data = await response.Content.ReadAsStringAsync();
                dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
                foreach (var i in json.fileList)
                {
                    string name = i.name;
                    folders.Enqueue(string.Format("%2F{0}%2F", name.Substring(1)));
                }

                // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("05003"));
                // while (!WebHelper.GetTraceIDDone)
                // {
                //     Thread.Sleep(10);
                // }
                // WebHelper.GetTraceIDDone = false;
                url = "https://cloud.huawei.com/nspservice/queryFileLazy";
                form = new FileSyncContent();
                form.ascending = "1";
                form.cursor = "";
                form.fileType = "6";
                form.order = "name";
                form.traceId = GetTraceId("05003");
                form.paths.Add(path);
 
                jsonString = JsonConvert.SerializeObject(form);
                content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, content);
                data = await response.Content.ReadAsStringAsync();
                json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

                foreach (var i in json.fileList)
                {
                    Common.File file = new Common.File();
                    file.CreateTime = i.createTime;
                    file.ModifyTime = i.accessTime;
                    file.Size = i.size;
                    file.Name = i.name;
                    files.Add(file);
                }
            }

            return files;
        }

        public async Task<List<Gps>> SyncLocationAsync()
        {
            try
            {
                string callPageUrl = "https://cloud.huawei.com/v1/mobile";
                string re = await client.GetStringAsync(callPageUrl);
                string pattern = @"CSRFToken = ""(\S+)""";
                string CSRFToken = Regex.Match(re, pattern).Result("$1");
                UpdateCSRFToken(CSRFToken);
                pattern = @"useid=""(\S+)""";
                string uid = Regex.Match(re, pattern).Result("$1");
                client.DefaultRequestHeaders.Add("uid", uid);
            }
            catch(Exception)
            {
                throw new Exception("GPS获取出错，请尝试重新获取数据，请检查登陆是否失效");
            }
            List<Gps> gpses = new List<Gps>();

            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("01001"));
            // while (!WebHelper.GetTraceIDDone)
            //     Thread.Sleep(10);
            // WebHelper.GetTraceIDDone = false;
            string timeStamp = TimeConverter.GetTimeStamp();
            string url = string.Format("https://cloud.huawei.com/v1/mobile/getMobileDeviceList.action?dt={0}", timeStamp);
            var content = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
            if (json.deviceList.Count == 0)
                return gpses;
            var token = response.Headers.GetValues("CSRFToken");
            UpdateCSRFToken(token.First());

            string deviceId = json.deviceList[0].deviceID;
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("01001"));
            // while (!WebHelper.GetTraceIDDone)
            //     Thread.Sleep(10);
            // WebHelper.GetTraceIDDone = false;
            timeStamp = TimeConverter.GetTimeStamp();
            url = string.Format("https://cloud.huawei.com/v1/mobile/queryLocateResult.action?dt={0}", timeStamp);
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("currentDate", timeStamp),
                new KeyValuePair<string, string>("deviceId", deviceId),
                new KeyValuePair<string, string>("destDeviceType", "0"),
                new KeyValuePair<string, string>("mobileTraceId", string.Format("{0}_100202305_BKL-AL208.0.0.202C00GT", GetTraceId("01001"))),
                new KeyValuePair<string, string>("sequence", "1"),
            });
            response = await client.PostAsync(url, formContent);
            data = await response.Content.ReadAsStringAsync();
            token = response.Headers.GetValues("CSRFToken");
            UpdateCSRFToken(token.First());

            client.DefaultRequestHeaders.Remove("uid");

            return ParseGps(data);
        }

        public async Task<List<Message>> SyncMessageAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
                totalCount = 1000000
            };
            return await FetchMessageAsync();
        }

        public async Task<List<Note>> SyncNoteAsync()
        {
            List<string> data = await FetchNoteAsync();
            var notes = ParseNote(data);
            return notes;
        }

        public async Task<List<Picture>> SyncPictureAsync()
        {
            // EventManager.getHuaWeiPictureEventManager.RaiseEvent();
            int cnt = 0;
            while (!WebHelper.GetPictureDone)
            {
                Thread.Sleep(1000);
                cnt += 1;
                if (cnt == 30)
                {
                    WebHelper.GetPictureDone = true;
                    throw new Exception("无图片获取");
                }
            }

            List<Picture> pictures = new List<Picture>();
            foreach (var json in WebHelper.HuaWeiPicture)
            {
                pictures.AddRange(ParsePicture(json));
            }

            foreach (var picture in pictures)
            {
                var form = new PictureInfoContent();
                form.traceId = GetTraceId("04101");
                form.fileList.Add("albumId", picture.AlbumId);
                form.fileList.Add("uniqueId", picture.UniqueId);
                form.lcd.Add("thumbHeight", "1920");
                form.lcd.Add("thumbWidth", "1920");
                form.lcd.Add("thumbType", "imgcrop");
                form.lcd.Add("type", "2");

                var jsonString = JsonConvert.SerializeObject(form);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                string url = "https://cloud.huawei.com/album/getThumbLcdUrl";
                var response = await client.PostAsync(url, content);
                var data = await response.Content.ReadAsStringAsync();
                var json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

                if (json.successList != null)
                {
                    picture.Name = json.successList[0]["fileName"];
                    picture.Time = json.successList[0]["sdsctime"];
                    picture.Url = Path.Combine(Directory.GetCurrentDirectory(), Setting.PictureFolder, picture.Name);
                }
            }
            WebHelper.HuaWeiPicture.Clear();
            return pictures;
        }

        public async Task<List<Record>> SyncRecordAsync()
        {
            string data = await FetchRecordAsync();
            return ParseRecord(data);
        }

        public async Task DownloadFileAsync(Common.File file)
        {
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("09103"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;
            string url = "https://cloud.huawei.com/nspservice/getDownLoadInfo";
            FileDownloadContent form = new FileDownloadContent();
            form.traceId = GetTraceId("09103");
            form.files.Add(file.Name);
            form.fileType = "6";
            form.fields.Add("sslUrl");
 
            string jsonString = JsonConvert.SerializeObject(form);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.successList[0].sslUrl;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            Stream res = await response.Content.ReadAsStreamAsync();
            if (!Directory.Exists(Setting.FileFolder))
                Directory.CreateDirectory(Setting.FileFolder);
            using (var fs = new FileStream(Setting.FileFolder + file.Name.Replace("/", "_"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        public async Task DownloadNoteAsync(Note note)
        {
            string id = note.Id; 

            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("08101"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;

            string url = "https://cloud.huawei.com/notepad/note/query";

            var values = new Dictionary<string, string>
            {
                { "guid", note.Id },
                { "ctagNoteInfo", note.ctagNoteInfo },
                { "ctagNoteTag", note.ctagNoteTag },
                { "traceId", GetTraceId("08101") },
            };
            string jsonString = JsonConvert.SerializeObject(values);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();

            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
            data = json.rspInfo.data;

            json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
            data = json.content.content;
            data = data.Replace("<>><><<<Text|", "\n");

            if (!Directory.Exists(Setting.NoteFolder))
                Directory.CreateDirectory(Setting.NoteFolder);
            System.IO.File.WriteAllText(Setting.NoteFolder + id + ".txt", data);
        }

        public async Task DownloadPictureAsync(Picture picture)
        {
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("04101"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;

            string url = "https://cloud.huawei.com/album/getSingleUrl";
            var form = new PictureDownloadContent();
            form.traceId = GetTraceId("04101");
            form.type = "0";
            form.fileList.Add("albumId", picture.AlbumId);
            form.fileList.Add("uniqueId", picture.UniqueId);
 
            var jsonString = JsonConvert.SerializeObject(form);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var data = await response.Content.ReadAsStringAsync();
            var json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
            url = json.urlList[0].url;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            // string name = response.Content.Headers.ContentDisposition.Parameters.First().Value;
            // picture.Name = name.Replace("\"", "").Replace("\\", "");
            Stream res = await response.Content.ReadAsStreamAsync();
            if (!Directory.Exists(Setting.PictureFolder))
                Directory.CreateDirectory(Setting.PictureFolder);
            using (var fs = new FileStream(Setting.PictureFolder + picture.Name, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        public async Task DownloadRecordAsync(Record record)
        {
            // EventManager.runJSEventManager.GetTraceID(new RunJSEventArgs("09103"));
            // while (!WebHelper.GetTraceIDDone)
            // {
            //     Thread.Sleep(10);
            // }
            // WebHelper.GetTraceIDDone = false;
            string url = "https://cloud.huawei.com/nspservice/getAttr";
            FileDownloadContent form = new FileDownloadContent();
            form.traceId = GetTraceId("09103");
            form.files.Add(record.Name);
            form.fields.Add("url");

            string jsonString = JsonConvert.SerializeObject(form);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.successList[0].url;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            Stream res = await response.Content.ReadAsStreamAsync();
            if (!Directory.Exists(Setting.RecordFolder))
                Directory.CreateDirectory(Setting.RecordFolder);
            using (var fs = new FileStream(Setting.RecordFolder + record.Name.Replace("/", "_"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        public async Task DownloadThumbnailAsync(Picture picture)
        {
            string url = picture.BigThumbnailUrl;

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };
            HttpResponseMessage response = await client.SendAsync(request);
            picture.Thumbnail = await response.Content.ReadAsByteArrayAsync();
        }
    }
}
