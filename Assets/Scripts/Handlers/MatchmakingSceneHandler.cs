using System;
using System.Collections;
using APIs;
using Managers;
using Serializables;
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

        private void JoinQueue() => StartCoroutine(JoinQueueCoroutine());

        private IEnumerator JoinQueueCoroutine()
        {
            var playerName = MainManager.Instance.currentLocalPlayerName;
            var joinQueueTask = MainManager.Instance.matchmakingManager.JoinQueue(playerName, gameId =>
            {
                this.gameId = gameId;
                gameFound = true;
            }, Debug.Log);

            yield return joinQueueTask.WaitForTask();
            
            if (joinQueueTask.Result.isFaulted) Debug.LogError(joinQueueTask.Result.error.readableMessage);
        }

        private void Update()
        {
            if (!gameFound || readyingUp) return;
            readyingUp = true;
            GameFound();
        }

        private void GameFound()
        {
            MainManager.Instance.gameManager.GetCurrentGameInfo(gameId, AuthAPI.GetUserId(),
                gameInfo =>
                {
                    Debug.Log("Game found. Ready-up!");
                    gameFound = true;
                    MainManager.Instance.gameManager.ListenForAllPlayersReady(gameInfo.playersInfo.Keys,
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
            MainManager.Instance.matchmakingManager.LeaveQueue();
            SceneManager.LoadScene("MenuScene");
        }

        public void Ready() => StartCoroutine(ReadyCoroutine());

        private IEnumerator ReadyCoroutine()
        {
            var readyUpTask = MainManager.Instance.gameManager.SetLocalPlayerReady();
            yield return readyUpTask.WaitForTask();
            
            if (readyUpTask.Result.isFaulted) Debug.LogError(readyUpTask.Result.error.readableMessage);
            else Debug.Log("You are now ready!");
        }
    }
}