using SharedFx.Interface;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Windows.Data.Json;
using SharedFx.Data;

namespace MGTV.MG.DataModels
{
    public class ChannelList : ICustomizeJsonDeserializable
    {
        public Recommendation Recommendation { get; set; }

        public List<Channel> Channels { get; set; }

        public ChannelList()
        {
            Channels = new List<Channel>();
            Recommendation = new Recommendation();
        }

        public void Deserialize(string json)
        {
            JsonArray array = JsonArray.Parse(json);
            foreach (var item in array)
            {
                try
                {
                    Channel channel = JsonSerializer.Deserialize<Channel>(item.Stringify(), true);
                    Channels.Add(channel);
                }
                catch
                {
                    Recommendation recom = JsonSerializer.Deserialize<Recommendation>(item.Stringify());
                    Recommendation = recom;
                }
            }
        }
    }

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
        public Video[] Children { get; set; }

        public Channel()
        {
            Id = -1;
            Name = string.Empty;
            IconUrl = string.Empty;
        }
    }


    [DataContract]
    public class Recommendation
    {
        [DataMember(Name = "channelId")]
        public int Id { get; set; }

        [DataMember(Name = "channelName")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "children")]
        public RecommendationDetail Children { get; set; }

        public Recommendation()
        {
            Id = -1;
            Name = string.Empty;
            IconUrl = string.Empty;
            Children = new RecommendationDetail();
        }
    }

    [DataContract]
    public class RecommendationDetail
    {
        [DataMember(Name = "flash")]
        public Video[] FlashItems { get; set; }

        [DataMember(Name = "recommend")]
        public Video[] Recommendations { get; set; }

        public RecommendationDetail()
        {

        }
    }
}
