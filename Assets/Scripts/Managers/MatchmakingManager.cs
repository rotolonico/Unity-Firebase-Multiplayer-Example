using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using Serializables;
using UnityEngine;

namespace Managers
{
    public class MatchmakingManager : MonoBehaviour
    {
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> queueListener;

        public async Task<RequestState> JoinQueue(string playerName, Action<string> onGameFound, Action<AggregateException> fallback)
        {
            var joinQueueResult = await FunctionsAPI.CallJoinQueueFunction(playerName);

            if (joinQueueResult.isFaulted)
                return joinQueueResult;

            async void OnValueChanged(ValueChangedEventArgs args)
            {
                var gameId = StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as string;
                if (gameId == null) return;
                onGameFound(gameId);
            }

            queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{AuthAPI.GetUserId()}/gameId",
                OnValueChanged, fallback);

            return new RequestState();
        }

        public async Task<RequestState> LeaveQueue()
        {
            var leaveQueueResult = await FunctionsAPI.CallLeaveQueueFunction();

            if (leaveQueueResult.isFaulted)
                return leaveQueueResult;
            
            DatabaseAPI.StopListeningForValueChanged(queueListener);

            return new RequestState();
        }
    }
}