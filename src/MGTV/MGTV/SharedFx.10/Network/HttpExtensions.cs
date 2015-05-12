using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SharedFx.Network
{
    public static class HttpExtensions
    {
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var taskComplete = new TaskCompletionSource<HttpWebResponse>();
            request.BeginGetResponse(asyncResponse =>
            {
                try
                {
                    HttpWebRequest responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
                    HttpWebResponse someResponse =
                       (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);
                    taskComplete.TrySetResult(someResponse);
                }
                catch (WebException webExc)
                {
                    HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);
            return taskComplete.Task;
        }
    }

    public class HttpRequestResult
    {
        public string Content { get; set; }
        public WebHeaderCollection ResponseHeaders { get; set; }

        public HttpRequestResult()
        {
            Content = string.Empty;
            ResponseHeaders = new WebHeaderCollection();
        }
    }

    public class Http
    {
        public static string CombineURLParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentException("parameters is null");
            }

            List<string> paras = new List<string>();
            foreach (var kv in parameters)
            {
                paras.Add(string.Format("{0}={1}", kv.Key, Uri.EscapeDataString(kv.Value)));
            }

            return string.Join("&", paras.ToArray());
        }

        public const string MULTIPART_BOUNDARY = "6CUS_DxALdoqFbRCOr86dQnoOjTVpFFR";

        public static async Task POSTAsync(string url,
                                           byte[] data,
                                           Dictionary<string, string> additionalHttpReqHeaders = null,
                                           Action<HttpRequestResult> callback = null, bool multipart = false)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            if (multipart)
            {
                httpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", MULTIPART_BOUNDARY);
            }

            if (additionalHttpReqHeaders != null)
            {
                foreach (var kv in additionalHttpReqHeaders)
                {
                    httpWebRequest.Headers[kv.Key] = kv.Value;
                }
            }

            if (data != null)
            {
                using (Stream requestStream = await httpWebRequest.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }

            HttpWebResponse response = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (Stream respStream = response.GetResponseStream())
            {
                HttpRequestResult result = new HttpRequestResult();
                result.Content = (new StreamReader(respStream)).ReadToEnd();
                result.ResponseHeaders = response.Headers;
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            }

        }


        public static async Task GETAsync(string url,
                                          Dictionary<string, string> additionalHttpReqHeaders = null,
                                          Action<HttpRequestResult> callback = null)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "GET";

            if (additionalHttpReqHeaders != null)
            {
                foreach (var kv in additionalHttpReqHeaders)
                {
                    httpWebRequest.Headers[kv.Key] = kv.Value;
                }
            }

            HttpWebResponse response = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (Stream respStream = response.GetResponseStream())
            {
                HttpRequestResult result = new HttpRequestResult();
                result.Content = (new StreamReader(respStream)).ReadToEnd();
                result.ResponseHeaders = response.Headers;
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            }


        }

    }
}
