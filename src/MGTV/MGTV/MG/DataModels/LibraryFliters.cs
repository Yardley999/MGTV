using System.Runtime.Serialization;

namespace MGTV.MG.DataModels
{
    [DataContract]
    public class LibraryFilter
    {
        [DataMember(Name = "filterType")]
        public string Type { get; set; }

        [DataMember(Name = "filterName")]
        public string Name { get; set; }
        
        [DataMember(Name = "filterItems")]
        public FilterItem[] Items { get; set; }

        public LibraryFilter()
        {
            Type = string.Empty;
            Name = string.Empty;
        }
    }

    [DataContract]
    public class FilterItem
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public FilterItem()
        {
            Id = string.Empty;
            Name = string.Empty;
        }
    }
}
