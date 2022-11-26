using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Serializables;

namespace APIs
{
    public static class AuthAPI
    {
        private static readonly FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        
        public static async Task<RequestState> SignUp(string email, string password) =>
            await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return new RequestState().MakeFaulted("New user creation was canceled.");

                if (task.IsFaulted)
                    return new RequestState().MakeFaulted(task.Exception?.GetBaseException().Message);

                return new RequestState();
            });

        public static async Task<RequestState> SignIn(string email, string password) =>
            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return new RequestState().MakeFaulted("Sign in was canceled.");

                if (task.IsFaulted)
                    return new RequestState().MakeFaulted(task.Exception?.GetBaseException().Message);

                return new RequestState();
            });

        public static bool IsSignedIn() => auth.CurrentUser != null;

        public static string GetUserId() => auth.CurrentUser.UserId;
        
        public static void SignOut() => auth.SignOut();
        
    }
}