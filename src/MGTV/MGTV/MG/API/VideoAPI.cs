using MGTV.MG.DataModels;
using SharedFx.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedFx.Data;

namespace MGTV.MG.API
{
    public class VideoAPI
    {
        private const string CATEGORY = "video";

        public static async Task GetById(int id, Action<Video> onSuccess = null, Action<Error> onFail = null)
        {
            string methodName = "getById";
            MGDataLoader<Video> loader = new MGDataLoader<Video>(CATEGORY, methodName);
            loader.AddParameter("vid", id.ToString());
            await loader.LoadDataAsync(onSuccess, onFail);
        }

        public static async Task<string> GetRealVideoAddress(string url, Action<Error> onFail = null)
        {
            string address = string.Empty;

            DataLoader loader = new DataLoader();
            await loader.LoadDataAsync(url, response => {
                RealVideoAddressData data = JsonSerializer.Deserialize<RealVideoAddressData>(response, true);
                if(data.Status.Equals("ok", StringComparison.OrdinalIgnoreCase))
                {
                    address = data.Info;
                }
                else
                {
                    if(onFail != null)
                    {
                        onFail.Invoke(new Error() { Message = data.Info });
                    }
                }
            }, error => {
                if(onFail != null)
                {
                    onFail.Invoke(error);
                }
            });

            return address;
        }
    }
}
