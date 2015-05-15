using System.Runtime.Serialization;

namespace MGTV.MG.DataModels
{

    [DataContract]
    public class Channel
    {
        [DataMember(Name = "channelId")]
        public int Id { get; set; }

        [DataMember(Name = "channelName")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "children")]
        public ChannelDetail Children { get; set; }

        public Channel()
        {
            Id = -1;
            Name = string.Empty;
            IconUrl = string.Empty;
        }
    }


    [DataContract]
    public class ChannelDetail
    {
        [DataMember(Name = "flash")]
        public Video[] FlashItems { get; set; }

        [DataMember(Name = "recommend")]
        public Video[] Recommendations { get; set; }

        public ChannelDetail()
        {

        }
    }
}
