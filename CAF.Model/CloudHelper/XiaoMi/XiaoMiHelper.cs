﻿using CAF.Model.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAF.Model.CloudHelper.XiaoMi
{
    class RuntimeData
    {
        public string syncIgnoreTag;
        public string syncTag;
        // public string syncThreadingTag;
        public bool lastPage;
    }

    partial class XiaoMiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        public void InitHelper()
        {
            InitFetcher();
        }

        public async Task<List<CallRecord>> SyncCallRecordAsync()
        {
            List<CallRecord> callRecords = new List<CallRecord>();

            runtimeData = new RuntimeData
            {
                syncTag = "",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchCallRecordAsync();
                callRecords.AddRange(ParseCallRecord(data));
            }

            return callRecords;
        }

        public async Task<List<Contact>> SyncContactAsync()
        {
            List<Contact> contacts = new List<Contact>();

            runtimeData = new RuntimeData
            {
                syncTag = "0",
                syncIgnoreTag = "0",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchContactAsync();
               contacts.AddRange(ParseContact(data));
            }

            return contacts;
        }

        public async Task<List<Common.File>> SyncFileAsync(Common.File file)
        {
            runtimeData = new RuntimeData();
            string id;
            if (file == null)
            {
                id = "0";
            }
            else
            {
                id = file.Id;
                if (file.Type != "folder")
                    throw new Exception("文件没有下级目录");
            }

            string data = await FetchFileAsync(id);
            return ParseFile(data);
        }

        public async Task<List<Gps>> SyncLocationAsync()
        {
            string data = await FetchGpsAsync();
            return ParseGps(data);
        }

        public async Task<List<Message>> SyncMessageAsync()
        {
            // runtimeData = new RuntimeData
            // {
            //     syncTag = "0",
            //     syncThreadingTag = "",
            //     lastPage = "false"
            // };
            // while (runtimeData.lastPage != "true")
            // {
                string data = await FetchMessageAsync();
                return ParseMessage(data);
            // }
        }

        public async Task<List<Note>> SyncNoteAsync()
        {
            string data = await FetchNoteAsync();
            return ParseNote(data);
        }

        public async Task<List<Picture>> SyncPictureAsync()
        {
            string data = await FetchPictureAsync();
            return ParsePicture(data);
        }

        public async Task<List<Record>> SyncRecordAsync()
        {
            string data = await FetchRecordAsync();
            return ParseRecord(data);
        }

        public async Task DownloadNoteAsync(Note note)
        {
            string id = note.Id; 

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "_dc", currentTimeStamp },
                {"uuid", uuid},
            };
            string url = WebHelper.MakeGetUrl(String.Format("https://i.mi.com/note/{0}/note/{1}/", uuid, id), content);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();

            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;
            data = json.data.entry.content;
            System.IO.File.WriteAllText(string.Format(@"{0}\{1}.txt", Setting.DownloadFolder, id), data);
        }

        public async Task DownloadFileAsync(Common.File file)
        {
            string id = file.Id;
            string name = file.Name;

            string currentTimeStamp = TimeConverter.GetTimeStamp();

            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "jsonpCallback", string.Format("dlFileJsonpCb{0}", currentTimeStamp) },
            };
            string url = WebHelper.MakeGetUrl(string.Format("https://i.mi.com/drive/user/{0}/files/{1}", uuid, id), content);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.data.storage.jsonpUrl;
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            response = await client.SendAsync(request);

            data = await response.Content.ReadAsStringAsync();
            string pattern = @"\((.*)\)";
            data = Regex.Match(data, pattern).Result("$1");
            json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.url;
            string meta = json.meta;
            var postForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("meta", meta),
            });
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = postForm,
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            Stream res = await response.Content.ReadAsStreamAsync();
            using (var fs = new FileStream(string.Format(@"{0}\{1}", Setting.DownloadFolder, name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        public async Task DownloadPictureAsync(Picture picture)
        {
            string id = picture.Id;
            string name = picture.Name;

            string currentTimeStamp = TimeConverter.GetTimeStamp();
            Dictionary<string, string> content = new Dictionary<string, string>
            {
                { "ts", currentTimeStamp },
                { "id", id },
            };
            string url = WebHelper.MakeGetUrl(string.Format("https://i.mi.com/gallery/{0}/storage", uuid), content);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.data.url;
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            response = await client.SendAsync(request);

            data = await response.Content.ReadAsStringAsync();
            string pattern = @"dl_img_cb\((.*)\)";
            data = Regex.Match(data, pattern).Result("$1");
            json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.url;
            string meta = json.meta;
            var postForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("meta", meta),
            });
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = postForm,
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            Stream res = await response.Content.ReadAsStreamAsync();
            using (var fs = new FileStream(string.Format(@"{0}\{1}", Setting.DownloadFolder, name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }

        public async Task DownloadRecordAsync(Record record)
        {
            string id = record.Id;
            string name = record.Name;

            string currentTimeStamp = TimeConverter.GetTimeStamp();

            string url = string.Format("https://i.mi.com/sfs/{0}/ns/recorder/file/{1}/storage", uuid, id);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string data = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.data.url;
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url.ToString())
            };
            response = await client.SendAsync(request);
            data = await response.Content.ReadAsStringAsync();

            string pattern = @"dl_sfs_cb\((.*)\)";
            data = Regex.Match(data, pattern).Result("$1");
            json = Newtonsoft.Json.Linq.JToken.Parse(data) as dynamic;

            url = json.url;
            string meta = json.meta;
            var postForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("meta", meta),
            });
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = postForm,
            };
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            Stream res = await response.Content.ReadAsStreamAsync();
            using (var fs = new FileStream(string.Format(@"{0}\{1}", Setting.DownloadFolder, name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await res.CopyToAsync(fs);
            }
        }
    }
}
