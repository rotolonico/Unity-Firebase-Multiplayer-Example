using System;
using Managers;
using Serializables;
using TMPro;
using UnityEngine;

namespace Handlers
{
    public class PlayerHandler : MonoBehaviour
    {
        public string playerId;

        public bool localPlayer;
        public bool isLocalPlayerTurn;

        public GameObject yourTurnText;

        private bool executeMove;
        private Move moveToExecute;

        private void Start()
        {
            if (localPlayer)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
                MainManager.Instance.gameManager.ListenForLocalPlayerTurn(() =>
                {
                    Debug.Log("It's your turn!");
                    isLocalPlayerTurn = true;
                }, Debug.Log);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
                MainManager.Instance.gameManager.ListenForMoves(playerId, ExecuteMove, Debug.Log);
            }
        }

        private void Update()
        {
            if (executeMove)
            {
                transform.position += (Vector3) moveToExecute.direction;
                executeMove = false;
            }
            
            if (!localPlayer) return;
            yourTurnText.SetActive(isLocalPlayerTurn);
            if (!isLocalPlayerTurn) return;

            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            if (Math.Abs(x) < 0.01f && Math.Abs(y) < 0.01f) return;

            var move = new Move(new Vector2(x, y));
            MainManager.Instance.gameManager.SendMove(move,
                () => MainManager.Instance.gameManager.SetTurnToOtherPlayer(playerId, () => ExecuteMove(move),
                    Debug.Log), error =>
                {
                    Debug.Log(error);
                    isLocalPlayerTurn = true;
                });

            isLocalPlayerTurn = false;
        }

        private void ExecuteMove(Move move)
        {
            moveToExecute = move;
            executeMove = true;
        }
    }
}