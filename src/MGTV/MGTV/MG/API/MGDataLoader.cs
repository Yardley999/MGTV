using SharedFx.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedFx.Data;
using MGTV.MG.DataModels;
using SharedFx.Interface;
using Windows.Data.Json;

namespace MGTV.MG.API
{
    public class DataLoader
    {
        public async Task LoadDataAsync(string url, Action<string> onSuccess, Action<Error> onFail)
        {
            HttpGETRequestExecuter executer = new HttpGETRequestExecuter(url);
            await executer.RunAsync(response =>
            {
                try
                {
                    if (onSuccess != null)
                    {
                        onSuccess.Invoke(response);
                    }
                }
                catch (Exception e)
                {
                    if (onFail != null)
                    {
                        onFail.Invoke(new Error() { ErrorCode = e.HResult, Message = e.Message });
                    }
                }

            }, error =>
            {
                if (onFail != null)
                {
                    onFail.Invoke(error);
                }
            });
        }
    }

    public class MGDataLoader<T>
    {
        public const string HOST = "http://win-dev.api.hunantv.com";
        public const string VERSION = "v1";

        private string category;
        private string method;
        private Dictionary<string, string> parameters;

        public MGDataLoader(string category, string methodName)
        {
            this.category = category;
            this.method = methodName;
            parameters = null;
        }

        public void AddParameter(string key, string value)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            parameters.Add(key, value);
        }

        public async Task LoadDataAsync(Action<T> onSuccess, Action<Error> onFail)
        {
            DataLoader loader = new DataLoader();
            await loader.LoadDataAsync(
                GetAPIUrl(),
                response =>
                {
                    T t = Activator.CreateInstance<T>();
                    if (t is ICustomizeJsonDeserializable)
                    {
                        var JsonData = JsonObject.Parse(response);
                        var errorCode = JsonData["err_code"].GetNumber().ToString();
                        if (!errorCode.Equals("200", StringComparison.OrdinalIgnoreCase))
                        {
                            string errorMessage = JsonData["err_msg"].GetString();
                            if(onFail != null)
                            {
                                onFail.Invoke(new Error() { ErrorCode = int.Parse(errorCode), Message = errorMessage });
                            }
                        }
                        else
                        {
                            (t as ICustomizeJsonDeserializable).Deserialize(JsonData["data"].GetArray().Stringify());
                            if(onSuccess != null)
                            {
                                onSuccess.Invoke(t);
                            }
                        }
                    }
                    else
                    {
                        var metadata = JsonSerializer.Deserialize<MetadataModel<T>>(response, true);

                        if (metadata.ErrorCode != 200)
                        {
                            if (onFail != null)
                            {
                                onFail.Invoke(new Error() { ErrorCode = metadata.ErrorCode, Message = metadata.ErrorMessage });
                            }
                        }
                        else
                        {
                            if (onSuccess != null)
                            {
                                onSuccess.Invoke(metadata.Data);
                            }
                        }
                    }

                }, error =>
                {
                    if (onFail != null)
                    {
                        onFail.Invoke(error);
                    }
                });
        }

        private string GetAPIUrl()
        {
            if (parameters == null || parameters.Count == 0)
            {
                return string.Format("{0}/{1}/{2}/{3}", HOST, VERSION, category, method);
            }
            else
            {
                return string.Format("{0}/{1}/{2}/{3}/?{4}", HOST, VERSION, category, method, CombineURLParameters(parameters));
            }
        }

        private string CombineURLParameters(Dictionary<string, string> parameters)
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
    }
}
