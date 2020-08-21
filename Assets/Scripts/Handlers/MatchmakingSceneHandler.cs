using System;
using APIs;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Handlers
{
    public class MatchmakingSceneHandler : MonoBehaviour
    {
        public GameObject searchingPanel;
        public GameObject foundPanel;

        private bool gameFound;
        private bool readyingUp;
        private string gameId;

        private void Start() => JoinQueue();

        private void JoinQueue() =>
            MainManager.Instance.matchmakingManager.JoinQueue(MainManager.Instance.currentLocalPlayerId, gameId =>
                {
                    this.gameId = gameId;
                    gameFound = true;
                },
                Debug.Log);

        private void Update()
        {
            if (!gameFound || readyingUp) return;
            readyingUp = true;
            GameFound();
        }

        private void GameFound()
        {
            MainManager.Instance.gameManager.GetCurrentGameInfo(gameId, MainManager.Instance.currentLocalPlayerId,
                gameInfo =>
                {
                    Debug.Log("Game found. Ready-up!");
                    gameFound = true;
                    MainManager.Instance.gameManager.ListenForAllPlayersReady(gameInfo.playersIds,
                        playerId => Debug.Log(playerId + " is ready!"), () =>
                        {
                            Debug.Log("All players are ready!");
                            SceneManager.LoadScene("GameScene");
                        }, Debug.Log);
                }, Debug.Log);

            searchingPanel.SetActive(false);
            foundPanel.SetActive(true);
        }

        public void LeaveQueue()
        {
            if (gameFound) MainManager.Instance.gameManager.StopListeningForAllPlayersReady();
            else
                MainManager.Instance.matchmakingManager.LeaveQueue(MainManager.Instance.currentLocalPlayerId,
                    () => Debug.Log("Left queue successfully"), Debug.Log);
            SceneManager.LoadScene("MenuScene");
        }

        public void Ready() =>
            MainManager.Instance.gameManager.SetLocalPlayerReady(() => Debug.Log("You are now ready!"), Debug.Log);
    }
}