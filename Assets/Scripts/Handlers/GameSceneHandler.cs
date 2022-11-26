using System;
using System.Linq;
using APIs;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Handlers
{
    public class GameSceneHandler : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject yourTurnText;

        private void Start()
        {
            var players = MainManager.Instance.gameManager.currentGameInfo.playersInfo;
            foreach (var player in players)
            {
                var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                var newPlayerHandler = newPlayer.GetComponent<PlayerHandler>();

                newPlayerHandler.playerInfo = player.Value;
                newPlayerHandler.localPlayer = player.Key == AuthAPI.GetUserId();
                newPlayerHandler.yourTurnText = yourTurnText;
            }
        }

        public void Leave() => SceneManager.LoadScene("MenuScene");

        private void OnDestroy() => MainManager.Instance.gameManager.LeaveGame();
    }
}