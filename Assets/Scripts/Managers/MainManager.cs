using Serializables;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;

        public MatchmakingManager matchmakingManager;
        public GameManager gameManager;

        public string currentLocalPlayerId; // You can use Firebase Auth to turn this into a userId. Just using the player name for a player id as an example for now!

        private void Awake() => Instance = this;

        private void Start()
        {
            matchmakingManager = GetComponent<MatchmakingManager>();
            gameManager = GetComponent<GameManager>();
        }
    }
}