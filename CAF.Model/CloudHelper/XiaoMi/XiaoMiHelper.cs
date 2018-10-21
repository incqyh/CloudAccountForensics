using CAF.Model.Common;
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
        public bool isFirstTime;
        public string syncIgnoreTag;
        public string syncTag;
        public string syncThreadingTag;
        public bool lastPage;
    }

    public partial class XiaoMiHelper : ICloudHelper
    {
        RuntimeData runtimeData = null;

        public void InitHelper()
        {
            InitFetcher();
        }

        public async Task SyncCallRecordAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                syncTag = "",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchCallRecordAsync();
                ParseCallRecord(data);
            }
        }

        public async Task SyncContactsAsync()
        {
            runtimeData = new RuntimeData
            {
                isFirstTime = true,
                syncTag = "0",
                syncIgnoreTag = "0",
                lastPage = false
            };
            while (!runtimeData.lastPage)
            {
                string data = await FetchContactsAsync();
                ParseContacts(data);
            }
        }

        public async Task SyncFileAsync()
        {
            string data = await FetchFileAsync();
            ParseFile(data);
        }

        public async Task SyncLocationAsync()
        {
            string data = await FetchGpsAsync();
            ParseGps(data);
        }

        public async Task SyncMessageAsync()
        {
            // runtimeData = new RuntimeData
            // {
            //     isFirstTime = true,
            //     syncTag = "0",
            //     syncThreadingTag = "",
            //     lastPage = "false"
            // };
            // while (runtimeData.lastPage != "true")
            // {
                string data = await FetchMessageAsync();
                ParseMessage(data);
            // }
        }

        public async Task SyncNoteAsync()
        {
            string data = await FetchNoteAsync();
            ParseNote(data);
        }

        public async Task SyncPictureAsync()
        {
            string data = await FetchPictureAsync();
            ParsePicture(data);
        }

        public async Task SyncRecordAsync()
        {
            string data = await FetchRecordAsync();
            ParseRecord(data);
        }

        public async Task DownloadNoteAsync(int index)
        {
            string id = DataManager.Notes[index].Id;

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
            string note = json.data.entry.content;
            System.IO.File.WriteAllText(string.Format(@"{0}\{1}.txt", Setting.DownloadFolder, id), note);
        }

        public async Task DownloadFileAsync(int index)
        {
            string id = DataManager.Files[index].Id;
            string name = DataManager.Files[index].Name;

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

        public async Task DownloadPictureAsync(int index)
        {
            string id = DataManager.Pictures[index].Id;
            string name = DataManager.Pictures[index].Name;

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

        public async Task DownloadRecordAsync(int index)
        {
            string id = DataManager.Records[index].Id;
            string name = DataManager.Records[index].Name;

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
