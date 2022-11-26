using System;
using System.Collections.Generic;

namespace Serializables
{
    [Serializable]
    public class GameInfo
    {
        public string gameId;
        public Dictionary<string, PlayerInfo> playersInfo;
        public string localPlayerId;
    }
}
