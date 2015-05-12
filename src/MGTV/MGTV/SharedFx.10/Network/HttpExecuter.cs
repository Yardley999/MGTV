using SharedFx.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SharedFx.Network
{
    public class Error
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public Error()
        {
            ErrorCode = -1;
            Message = string.Empty;
        }
    }

    public class HttpMultipartPostRequestExecuter : HttpPOSTRequestExecuter
    {
        public HttpMultipartPostRequestExecuter(string url) 
            : base(url)
        {
        }

        public override void AddPostData(string key, string value)
        {
            if (dataWriter == null)
            {
                dataWriter = new StreamWriter(dataStream);
            }

            dataWriter.WriteLine(string.Format("--{0}", Http.MULTIPART_BOUNDARY));
            dataWriter.WriteLine(string.Format(@"Content-Disposition: form-data; name=""{0}""", key));
            dataWriter.WriteLine();
            dataWriter.WriteLine(value);
            dataWriter.Flush();

            base.AddPostData(key, value);
        }

        public void AddMultiparFileData(string key, string filename, byte[] value)
        {
            if (dataWriter == null)
            {
                dataWriter = new StreamWriter(dataStream);
            }

            dataWriter.WriteLine(string.Format("--{0}", Http.MULTIPART_BOUNDARY));
            dataWriter.WriteLine(string.Format(@"Content-Disposition: form-data; name={0}; filename={1}", key, filename));
            dataWriter.WriteLine("Content-Type: application/octet-stream");
            dataWriter.WriteLine();
            dataWriter.Flush();
            dataWriter.BaseStream.Write(value, 0, value.Length);
            dataWriter.Write("\r\n");
            dataWriter.Flush();
        }

        public override async Task RunAsync(Action<string> onSuccess, Action<Error> onFail)
        {
            try
            {
                byte[] dataToPost = null;

                if (dataWriter != null)
                {
                    dataWriter.WriteLine(string.Format("--{0}--", Http.MULTIPART_BOUNDARY));
                    dataWriter.Flush();
                }

                dataToPost = new byte[dataStream.Length];
                dataStream.Position = 0;
                dataStream.Read(dataToPost, 0, dataToPost.Length);

                dataWriter.Dispose();

                await Http.POSTAsync(apiUrl, dataToPost, additionalRequestHeaders, (result) =>
                {
                    if (onSuccess != null)
                    {
                        onSuccess.Invoke(result.Content);
                    }
                }, true);
            }
            catch (Exception e)
            {
                if (onFail != null)
                {
                    onFail.Invoke(new Error() { ErrorCode = e.HResult, Message = e.Message });
                }
            };
        }

    }

    public class HttpPOSTRequestExecuter : HttpPOSTRequestExecuter<string, Error>
    {
        protected string apiUrl;
        protected Dictionary<string, string> postData;
        protected Dictionary<string, string> additionalRequestHeaders;
        protected MemoryStream dataStream;
        protected StreamWriter dataWriter;

        public HttpPOSTRequestExecuter(string url)
        {
            this.apiUrl = url;
            dataStream = new MemoryStream();
            dataWriter = new StreamWriter(dataStream);
        }

        public void AddRequestHeader(string key, string value)
        {
            if (additionalRequestHeaders == null)
            {
                additionalRequestHeaders = new Dictionary<string, string>();
                additionalRequestHeaders.Add(key, value);
                return;
            }

            if (additionalRequestHeaders.ContainsKey(key))
            {
                additionalRequestHeaders[key] = value;
            }
            else
            {
                additionalRequestHeaders.Add(key, value);
            }
        }

        public virtual void AddPostData(string key, string value)
        {
            if (postData == null)
            {
                postData = new Dictionary<string, string>();
                postData.Add(key, value);
                return;
            }

            if (postData.ContainsKey(key))
            {
                postData[key] = value;
            }
            else
            {
                postData.Add(key, value);
            }
        }

        public virtual async Task RunAsync(Action<string> onSuccess, Action<Error> onFail)
        {
            try
            {
                byte[] dataToPost = Encoding.UTF8.GetBytes(Http.CombineURLParameters(postData)); ;
                await Http.POSTAsync(apiUrl, dataToPost, additionalRequestHeaders, (result) =>
                    {
                        if (onSuccess != null)
                        {
                            onSuccess.Invoke(result.Content);
                        }
                    }, false);
            }
            catch (Exception e)
            {
                if (onFail != null)
                {
                    onFail.Invoke(new Error() { ErrorCode = e.HResult, Message = e.Message });
                }
            }
        }
    }


    public class HttpGETRequestExecuter : HttpPOSTRequestExecuter<string, Error>
    {
        private string apiUrl;
        private Dictionary<string, string> additionalRequestHeaders;

        public HttpGETRequestExecuter(string url)
        {
            this.apiUrl = url;
        }

        public void AddRequestHeader(string key, string value)
        {
            if (additionalRequestHeaders == null)
            {
                additionalRequestHeaders = new Dictionary<string, string>();
                additionalRequestHeaders.Add(key, value);
                return;
            }

            if (additionalRequestHeaders.ContainsKey(key))
            {
                additionalRequestHeaders[key] = value;
            }
            else
            {
                additionalRequestHeaders.Add(key, value);
            }
        }

        public async virtual Task RunAsync(Action<string> onSuccess, Action<Error> onFail)
        {
            try
            {
                await Http.GETAsync(apiUrl, additionalRequestHeaders, (result) =>
                {
                    if (onSuccess != null)
                    {
                        onSuccess.Invoke(result.Content);
                    }
                });
            }
            catch (Exception e)
            {
                if (onFail != null)
                {
                    onFail.Invoke(new Error() { ErrorCode = e.HResult, Message = e.Message });
                }
            }
        }
    }

}
