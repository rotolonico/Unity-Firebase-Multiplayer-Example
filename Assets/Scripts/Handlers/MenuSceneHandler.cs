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
            MainManager.Instance.currentLocalPlayerId = playerNameIF.text;
            SceneManager.LoadScene("MatchmakingScene");
        }
    }
}