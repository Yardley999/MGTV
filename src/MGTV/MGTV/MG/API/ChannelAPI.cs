using MGTV.MG.DataModels;
using SharedFx.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGTV.MG.API
{
    public enum OrderType
    {
        HOT,
        LASTEST
    }

    public class ChannelAPI
    {
        private const string CATEGORY = "channel";

        public static async Task GetMajorList(Action<List<Channel>> onSuccess, Action<Error> onFail)
        {
            string methodName = "getMajorList";
            MGDataLoader<List<Channel>> loader = new MGDataLoader<List<Channel>>(CATEGORY, methodName);

            await loader.LoadDataAsync(onSuccess, onFail);
        }

        public static async Task GetList(int size, Action<ChannelList> onSuccess, Action<Error> onFail)
        {
            string methodName = "getList";
            MGDataLoader<ChannelList> loader = new MGDataLoader<ChannelList>(CATEGORY, methodName);
            loader.AddParameter("size", size.ToString());

            await loader.LoadDataAsync(onSuccess, onFail);
        }

        public static async Task GetLibraryFilters(int channelId, Action<LibraryFilter[]> onSuccess, Action<Error> onFail)
        {
            string methodName = "getLibraryFilters";
            MGDataLoader<LibraryFilter[]> loader = new MGDataLoader<LibraryFilter[]>(CATEGORY, methodName);
            loader.AddParameter("channelId", channelId.ToString());

            await loader.LoadDataAsync(onSuccess, onFail);
        }

        public static async Task GetLibraryList(
            Action<LibraryList> onSuccess, 
            Action<Error> onFail,
            int channelId, 
            OrderType orderType = OrderType.LASTEST, 
            Dictionary<string, string> filters = null,
            int pageCount = 1,
            int pageSize = 20)
        {
            string methodName = "getLibraryList";

            MGDataLoader<LibraryList> loader = new MGDataLoader<LibraryList>(CATEGORY, methodName);
            loader.AddParameter("channelId", channelId.ToString());
            loader.AddParameter("pageCount", pageCount.ToString());
            loader.AddParameter("pageSize", pageSize.ToString());
            loader.AddParameter("orderType", orderType.ToString());
           
            if(filters != null)
            {
                foreach (KeyValuePair<string, string> kv in filters)
                {
                    loader.AddParameter(kv.Key, kv.Value);
                }
            }

            await loader.LoadDataAsync(onSuccess, onFail);

        }
    }
}
