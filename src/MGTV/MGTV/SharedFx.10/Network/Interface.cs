using System;
using System.Threading.Tasks;

namespace SharedFx.Interface
{
    public interface HttpPOSTRequestExecuter<TSucceess, TError>
    {
        Task RunAsync(Action<TSucceess> onSuccess, Action<TError> onFail);
    }

    public interface ICustomizeJsonDeserializable
    {
         void Deserialize(string json);
    }
}
