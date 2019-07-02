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
    /// <summary>
    /// 获取数据时的临时参数
    /// </summary>
    class RuntimeData
    {
        public bool isEnd;
        public uint totalCount;
    }

    /// <summary>
    /// 同步文件数据使用的参数
    /// </summary>
    class FileSyncContent
    {
        public string ascending;
        public string cursor;
        public string order;
        public string fileType;
        public string traceId;
        public List<string> paths = new List<string>();
    }

    /// <summary>
    /// 下载文件时使用的参数
    /// </summary>
    class FileDownloadContent
    {
        public string fileType;
        public string traceId;
        public List<string> files = new List<string>();
        public List<string> fields = new List<string>();
    }

    /// <summary>
    /// 下载图片时使用的参数
    /// </summary>
    class PictureDownloadContent
    {
        public Dictionary<string, string> fileList = new Dictionary<string, string>();
        public string traceId;
        public string type;
    }

    /// <summary>
    /// 获取图片信息时使用的参数
    /// </summary>
    class PictureInfoContent
    {
        public Dictionary<string, string> fileList = new Dictionary<string, string>();
        public Dictionary<string, string> lcd = new Dictionary<string, string>();
        public string traceId;
    }

    /// <summary>
    /// 最开始设计hepler类的时候希望将数据获取与数据解析分开
    /// 这样可以使逻辑更加清楚，调试也更加方便
    /// 但是由于云服务网站的不断更新，与一些接口的复杂性
    /// 所以该类最后变得有些混乱，获取与解析的耦合较严重
    /// 但是对外提供的接口没有变化
    /// </summary>
    public partial class HuaWeiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        /// <summary>
        /// 初始化获取器与解析器
        /// </summary>
        public void InitHelper()
        {
            InitFetcher();
            InitParser();
        }

        public bool IsLogIn()
        {
            return false;
        }

        /// <summary>
        /// 华为云服务的通话记录，由于要获取完整的童话记录
        /// 需要多次重复请求并解析，最后整合出完整的数据
        /// 所以此处使用一个获取终止的flag
        /// </summary>
        /// <returns></returns>
        public async Task<List<CallRecord>> SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
            };

            return await FetchCallRecordAsync();
        }

        /// <summary>
        /// 华为的通讯录获取比较特殊，使用的数据类型是protobuf
        /// </summary>
        /// <returns></returns>
        public async Task<List<Contact>> SyncContactAsync()
        {
            var data = await FetchContactAsync();
            return ParseContact(data);
        }

        /// <summary>
        /// 文件列表信息获取
        /// 程序中递归搜索所有的文件，忽略文件夹
        /// 华为的请求将获取文件夹和获取文件分开了
        /// 分别为queryFolder和queryFileLazy
        /// </summary>
        /// <returns></returns>
        public async Task<List<Common.File>> SyncFileAsync()
        {
            // 获取文件需要userId头，先提取一下
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

            // 初始的默认参数
            Queue<string> folders = new Queue<string>();
            folders.Enqueue("%2F");
            
            // 拆开所有的文件夹
            while(folders.Count != 0)
            {
                string path = folders.Dequeue();

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
                    file.LocalUrl = Path.Combine(Directory.GetCurrentDirectory(),
                        Setting.FileFolder, file.Name.Replace("/", "_"));
                    files.Add(file);
                }
            }

            return files;
        }

        /// <summary>
        /// 获取gps数据，需要当前账户有关联的手机开着定位服务
        /// </summary>
        /// <returns></returns>
        public async Task<List<Gps>> SyncLocationAsync()
        {
            // 此处获取uid需要正则匹配"useid"字串
            // 大概率是程序员少写了'r'
            // 以后他们有可能会修改
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

        /// <summary>
        /// 同步信息的请求和其他的请求少许有些不同
        /// 不需要重复获取数据来保证数据完整，只要把count参数设置够大即可
        /// </summary>
        /// <returns></returns>
        public async Task<List<Message>> SyncMessageAsync()
        {
            runtimeData = new RuntimeData
            {
                isEnd = false,
                totalCount = 1000000
            };
            return await FetchMessageAsync();
        }

        /// <summary>
        /// 获取备忘录的请求，简单
        /// </summary>
        /// <returns></returns>
        public async Task<List<Note>> SyncNoteAsync()
        {
            List<string> data = await FetchNoteAsync();
            var notes = ParseNote(data);
            return notes;
        }

        /// <summary>
        /// 华为的图片获取接口又是比较复杂
        /// 由于获取图片数据的时候发送了一下非常复杂的参数，难以读懂
        /// 所以选择折中的实现方案，使用前端的浏览器来辅助获取部分参数
        /// 浏览器获取到的原始参数放在pictureInfo表中
        /// </summary>
        /// <returns></returns>
        public async Task<List<Picture>> SyncPictureAsync()
        {
            int cnt = 0;
            while (!getPictureInfoDone)
            {
                Thread.Sleep(1000);
                cnt += 1;
                if (cnt == 30)
                {
                    getPictureInfoDone = true;
                    throw new Exception("无图片获取");
                }
            }

            List<Picture> pictures = new List<Picture>();

            // 这里是初步解析
            foreach (var json in pictureInfo)
            {
                pictures.AddRange(ParsePicture(json));
            }

            // 解析完之后还需要拿解析完的数据重新发送一次请求
            // 以获得所需要的完整数据
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
                    ulong timeStamp = json.successList[0]["createTime"];
                    picture.Time = TimeConverter.UInt64ToDateTime(timeStamp);
                    picture.LocalUrl = Path.Combine(Directory.GetCurrentDirectory(), Setting.PictureFolder, picture.Name);
                }
            }
            pictureInfo.Clear();
            return pictures;
        }

        /// <summary>
        /// 同步语音数据，简单
        /// </summary>
        /// <returns></returns>
        public async Task<List<Record>> SyncRecordAsync()
        {
            string data = await FetchRecordAsync();
            return ParseRecord(data);
        }

        /// <summary>
        /// 下载文件接口
        /// 文件下载时需要先获取当前文件下载的url
        /// 然后对此url作get请求即可
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task DownloadFileAsync(Common.File file)
        {
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

            // 这里只等待到获取返回头
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            // 然后流式写文件，防止文件过大导致内存错误，也省去内存中转的开销
            Stream res = await response.Content.ReadAsStreamAsync();
            if (!Directory.Exists(Setting.FileFolder))
                Directory.CreateDirectory(Setting.FileFolder);
            using (var fs = new FileStream(file.LocalUrl, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        /// <summary>
        /// 下载完整的备忘录数据
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public async Task DownloadNoteAsync(Note note)
        {
            string id = note.Id; 

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
            System.IO.File.WriteAllText(note.LocalUrl, data);
        }

        /// <summary>
        /// 下载图片，方法本质上和下载文件没太大区别
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        public async Task DownloadPictureAsync(Picture picture)
        {
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
            using (var fs = new FileStream(picture.LocalUrl, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        /// <summary>
        /// 下载录音，方法本质上和下载文件没太大区别
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task DownloadRecordAsync(Record record)
        {
            string url = "https://cloud.huawei.com/nspservice/getDownLoadInfo";
            FileDownloadContent form = new FileDownloadContent();
            form.traceId = GetTraceId("09103");
            form.files.Add(record.Name);
            form.fields.Add("url");
            form.fileType = "9";

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
            using (var fs = new FileStream(record.LocalUrl, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        /// <summary>
        /// 下载缩略图，这主要是用在前端显示
        /// 如果在现实的时候就都下载完整的图片会非常的慢
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
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
