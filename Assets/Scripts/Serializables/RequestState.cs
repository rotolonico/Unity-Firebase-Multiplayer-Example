using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;

namespace Serializables
{
    [Serializable]
    public class RequestResponse<T> : RequestState
    {
        public T data;

        public static bool operator ==(RequestResponse<T> wrapper, T type) =>
            wrapper != null && EqualityComparer<T>.Default.Equals(wrapper.data, type);

        public static bool operator !=(RequestResponse<T> wrapper, T type) => !(wrapper == type);

        public RequestResponse(T data) => this.data = data;

        protected bool Equals(RequestResponse<T> other)
        {
            return isFaulted == other.isFaulted && error == other.error &&
                   EqualityComparer<T>.Default.Equals(data, other.data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RequestResponse<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = isFaulted.GetHashCode();
                hashCode = (hashCode * 397) ^ (error != null ? error.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(data);
                return hashCode;
            }
        }

        public static RequestResponse<T> FromDictionary<T>(Dictionary<object, object> dictionary)
        {
            var isFaulted = false;
            if (dictionary.ContainsKey("isFaulted")) isFaulted = (bool) dictionary["isFaulted"];

            if (isFaulted)
            {
                var error = "";
                if (dictionary.ContainsKey("error")) error = (string) dictionary["error"];
                return new RequestResponse<T>(default).MakeFaulted(error);
            }

            T data = default;
            if (dictionary.ContainsKey("data")) data = (T) dictionary["data"];
            return new RequestResponse<T>(data);
        }
    }

    [Serializable]
    public class RequestState
    {
        public bool isFaulted;
        public RequestError error;
    }

    [Serializable]
    public class RequestError
    {
        public enum ErrorType
        {
            Exception,
            Message
        }

        public Exception rawException;
        public string readableMessage;
        public ErrorType errorType;

        public void SetReadableMessage(string message) => readableMessage = message;
    }

    public static class RequestExtensions
    {
        public static T MakeFaulted<T>(this T authObject, string error) where T : RequestState
        {
            authObject.isFaulted = true;
            authObject.error = new RequestError
            {
                rawException = new Exception(error),
                readableMessage = error,
                errorType = RequestError.ErrorType.Message
            };
            return authObject;
        }

        public static T MakeFaulted<T>(this T authObject, Exception error) where T : RequestState
        {
            authObject.isFaulted = true;
            authObject.error = new RequestError
            {
                rawException = error,
                readableMessage = error.Message,
                errorType = RequestError.ErrorType.Exception
            };
            return authObject;
        }

        public static T MakeFaulted<T>(this T authObject, RequestError error) where T : RequestState
        {
            authObject.isFaulted = true;
            authObject.error = error;
            return authObject;
        }

        public static IEnumerator WaitForTask(this Task task) => MainManager.Instance.WaitForTask(task);

        public static async Task<Task> WaitForTaskAsync(this Task task) =>
            await MainManager.Instance.WaitForTaskAsync(task);

        public static void ExecuteCoroutineGlobally(this IEnumerator coroutine)
        {
            if (MainManager.Instance != null) MainManager.Instance.StartCoroutine(coroutine);
        }
    }
}