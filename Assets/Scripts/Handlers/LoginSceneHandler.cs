using System;
using System.Collections;
using APIs;
using Managers;
using Serializables;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Handlers
{
    public class LoginSceneHandler : MonoBehaviour
    {
        public TMP_InputField emailIF;
        public TMP_InputField passwordIF;
        public TextMeshProUGUI errorText;

        public void SignUp() => StartCoroutine(SignUpCoroutine());

        public void SignIn() => StartCoroutine(SignInCoroutine());

        private IEnumerator SignUpCoroutine()
        {
            var authTask = AuthAPI.SignUp(emailIF.text, passwordIF.text);
            yield return authTask.WaitForTask();

            var authResult = authTask.Result;

            if (authResult.isFaulted) HandleError(authResult.error);
            else HandleSuccess();
        }

        private IEnumerator SignInCoroutine()
        {
            var authTask = AuthAPI.SignIn(emailIF.text, passwordIF.text);
            yield return authTask.WaitForTask();

            var authResult = authTask.Result;

            if (authResult.isFaulted) HandleError(authResult.error);
            else HandleSuccess();
        }

        private void HandleError(RequestError error) => errorText.text = error.readableMessage;

        private void HandleSuccess()
        {
            errorText.text = "Success!";
            SceneManager.LoadScene("MenuScene");
        }
    }
}