using Newtonsoft.Json;
using System.Reflection;
namespace VkStorage.Application.Common.BaseRequests
{
    public class BaseResponse
    {
        [JsonIgnore]
        private Exception? _exception = null;
        [JsonIgnore]
        public string? Message { get; set; } = null;
        public void SetException(Exception? exception) { _exception = exception; }
        public Exception? GetException() => _exception;

        public object this[string propertyName]
        {
            get
            {
                System.Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                System.Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
    }
}
