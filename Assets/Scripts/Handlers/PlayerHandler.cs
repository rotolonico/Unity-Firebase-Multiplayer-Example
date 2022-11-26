using System;
using System.Collections;
using Managers;
using Serializables;
using TMPro;
using UnityEngine;

namespace Handlers
{
    public class PlayerHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshPro nameText;
        
        public PlayerInfo playerInfo;

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
                GetComponent<SpriteRenderer>().color = Color.blue;

            nameText.text = playerInfo.name;
            
            MainManager.Instance.gameManager.ListenForMoves(playerInfo.id, ExecuteMove, Debug.Log);
        }

        private void Update()
        {
            if (executeMove)
            {
                transform.position += (Vector3)moveToExecute.direction;
                executeMove = false;
            }

            if (!localPlayer) return;
            yourTurnText.SetActive(isLocalPlayerTurn);
            if (!isLocalPlayerTurn) return;

            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            if (Math.Abs(x) < 0.01f && Math.Abs(y) < 0.01f) return;

            var move = new Move(new Vector2(x, y));
            StartCoroutine(SendMoveCoroutine(move));

            isLocalPlayerTurn = false;
        }

        private IEnumerator SendMoveCoroutine(Move move)
        {
            var sendMoveTask = MainManager.Instance.gameManager.SendMove(move);
            yield return sendMoveTask.WaitForTask();

            if (sendMoveTask.Result.isFaulted)
            {
                Debug.Log(sendMoveTask.Result.error.readableMessage);
                isLocalPlayerTurn = true;
            }
            else Debug.Log("Move sent!");
        }

        private void ExecuteMove(Move move)
        {
            moveToExecute = move;
            executeMove = true;
        }
    }
}