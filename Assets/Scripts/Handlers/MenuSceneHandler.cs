using System;
using APIs;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Handlers
{
    public class MenuSceneHandler : MonoBehaviour
    {
        public TMP_InputField playerNameIF;


        public void Play()
        {
            MainManager.Instance.currentLocalPlayerName = playerNameIF.text;
            SceneManager.LoadScene("MatchmakingScene");
        }

        public void SignOut()
        {
            AuthAPI.SignOut();
            SceneManager.LoadScene("LoginScene");
        }
    }
}