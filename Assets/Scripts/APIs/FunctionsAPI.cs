using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Functions;
using Serializables;

namespace APIs
{
    public static class FunctionsAPI
    {
        private static readonly FirebaseFunctions Functions = FirebaseFunctions.DefaultInstance;

        public static async Task<RequestState> CallJoinQueueFunction(string playerName) =>
            await CallFunction<object>("joinQueue", playerName);

        public static async Task<RequestState> CallLeaveQueueFunction() =>
            await CallFunction<object>("leaveQueue");

        public static async Task<RequestState> CallSetReadyFunction() =>
            await CallFunction<object>("setReady");

        public static async Task<RequestState> CallMakeMoveFunction(Move move) =>
            await CallFunction<object>("makeMove", move.ConvertToArray());

        private static async Task<RequestResponse<T>> CallFunction<T>(string name, object data = null)
        {
            var function = Functions.GetHttpsCallable(name);
            return await function.CallAsync(data).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return new RequestResponse<T>(default).MakeFaulted("Function call was canceled.");

                if (task.IsFaulted)
                    return new RequestResponse<T>(default).MakeFaulted(task.Exception?.GetBaseException().Message);

                return RequestResponse<T>.FromDictionary<T>(task.Result.Data as Dictionary<object, object>);
            });
        }
    }
}