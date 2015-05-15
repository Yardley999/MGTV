using MGTV.MG.DataModels;
using SharedFx.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGTV.MG.API
{
    public class ChannelAPI
    {
        private const string CATEGORY = "channel";

        public static async Task GetMajorList(Action<Channel[]> onSuccess, Action<Error> onFail)
        {
            string methodName = "getMajorList";
            MGDataLoader<Channel[]> loader = new MGDataLoader<Channel[]>(CATEGORY, methodName);

            await loader.LoadDataAsync(onSuccess, onFail);
        }

        public static async Task GetList(int size, Action<Channel[]> onSuccess, Action<Error> onFail)
        {
            string methodName = "getList";
            MGDataLoader<Channel[]> loader = new MGDataLoader<Channel[]>(CATEGORY, methodName);
            loader.AddParameter("size", size.ToString());

            await loader.LoadDataAsync(onSuccess, onFail);
        }
    }
}
