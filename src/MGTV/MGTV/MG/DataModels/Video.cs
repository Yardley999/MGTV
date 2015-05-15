using System.Runtime.Serialization;

namespace MGTV.MG.DataModels
{
    [DataContract]
    public class Video
    {
        [DataMember(Name = "videoId")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "imgUrl")]
        public string ImageUrl { get; set; }

        [DataMember(Name = "videoSources")]
        public VideoDefinition[] VideoSources { get; set; }

        [DataMember(Name = "relatedVideos")]
        public Video[] RelatedVideos { get; set; }

        public Video()
        {
            Id = -1;
            Title = string.Empty;
            ImageUrl = string.Empty;
        }
    }

    [DataContract]
    public class VideoDefinition
    {
        [DataMember(Name = "definition")]
        public int Definition { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        public VideoDefinition()
        {
            Definition = -1;
            Name = string.Empty;
            Url = string.Empty;
        }
    }


    [DataContract]
    public class RealVideoAddressData
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "info")]
        public string Info { get; set; }

        [DataMember(Name = "isothercdn")]
        public string IsOtherCDN { get; set; }

        public RealVideoAddressData()
        {
            Status = string.Empty;
            Info = string.Empty;
            IsOtherCDN = string.Empty;
        }
    }


}
