using System.Runtime.Serialization;

namespace MGTV.MG.DataModels
{

    [DataContract]
    public class MetadataModel<T>
    {
        [DataMember(Name = "err_code")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "err_msg")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "data")]
        public T Data { get; set; }

        public MetadataModel()
        {
            ErrorCode = -1;
            ErrorMessage = string.Empty;
        }
    }

}
