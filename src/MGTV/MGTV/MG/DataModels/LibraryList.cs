using System.Runtime.Serialization;

namespace MGTV.MG.DataModels
{
    [DataContract]
    public class LibraryList
    {
        [DataMember(Name = "channeName")]
        public string ChannelName { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "items")]
        public Video[] Videos { get; set; }

        public LibraryList()
        {
            ChannelName = string.Empty;
            Count = 0;
            Page = 0;
        }
    }

}
