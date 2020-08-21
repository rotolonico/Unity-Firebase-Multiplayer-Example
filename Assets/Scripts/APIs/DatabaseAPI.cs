using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

namespace APIs
{
    public static class DatabaseAPI
    {
        private static DatabaseReference reference;

        public static void InitializeDatabase()
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://[PROJECT-ID].firebaseio.com/");
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public static void PostJSON(string path, string json, Action callback, Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            customReference.SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("PostJSON was canceled.");
                    fallback(task.Exception);
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("PostJSON encountered an error: " + task.Exception);
                    fallback(task.Exception);
                    return;
                }
                
                callback();
            });
        }

        public static void PostObject<T>(string path, T obj, Action callback,
            Action<AggregateException> fallback) =>
            PostJSON(path, StringSerializationAPI.Serialize(typeof(T), obj), callback, fallback);

        public static void PushJSON(string path, string json, Action callback, Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            customReference.Push().SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("PostJSON was canceled.");
                    fallback(task.Exception);
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("PostJSON encountered an error: " + task.Exception);
                    fallback(task.Exception);
                    return;
                }

                Debug.Log("JSON posted successfully");
                callback();
            });
        }
        
        public static void PushObject<T>(string path, T obj, Action callback,
            Action<AggregateException> fallback) =>
            PushJSON(path, StringSerializationAPI.Serialize(typeof(T), obj), callback, fallback);

        public static void GetObject<T>(string path, Action<T> callback,
            Action<AggregateException> fallback) =>
            GetJSON(path,
                json => { callback((T) StringSerializationAPI.Deserialize(typeof(T), json.GetRawJsonValue())); },
                fallback);

        public static void GetJSON(string path, Action<DataSnapshot> callback, Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            customReference.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("PostJSON was canceled.");
                    fallback(task.Exception);
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("PostJSON encountered an error: " + task.Exception);
                    fallback(task.Exception);
                    return;
                }
                
                callback(task.Result);
            });
        }

        public static KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildAdded(string path, Action<ChildChangedEventArgs> onChildAdded,
            Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args)
            {
                if (args.DatabaseError != null)
                {
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                onChildAdded(args);
            }

            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(customReference,
                CurrentListener);
            customReference.ChildAdded += CurrentListener;

            return listenerPair;
        }

        public static void StopListeningForChildAdded(
            KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener) =>
            listener.Key.ChildAdded -= listener.Value;

        public static KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildRemoved(string path, Action<ChildChangedEventArgs> onChildAdded,
            Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args)
            {
                if (args.DatabaseError != null)
                {
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                onChildAdded(args);
            }
            
            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(customReference,
                CurrentListener);
            customReference.ChildRemoved += CurrentListener;

            return listenerPair;
        }

        public static void StopListeningForChildRemoved(
            KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener) =>
            listener.Key.ChildRemoved -= listener.Value;

        public static KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> ListenForChildChanged(string path, Action<ChildChangedEventArgs> onChildAdded,
            Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ChildChangedEventArgs args)
            {
                if (args.DatabaseError != null)
                {
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                onChildAdded(args);
            }
            
            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>(customReference,
                CurrentListener);
            customReference.ChildChanged += CurrentListener;

            return listenerPair;
        }

        public static void StopListeningForChildChanged(
            KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> listener) =>
            listener.Key.ChildChanged -= listener.Value;

        public static KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> ListenForValueChanged(string path, Action<ValueChangedEventArgs> onChildAdded,
            Action<AggregateException> fallback)
        {
            var customReference = GetReferenceFromPath(path);

            void CurrentListener(object o, ValueChangedEventArgs args)
            {
                if (args.DatabaseError != null)
                {
                    fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                onChildAdded(args);
            }
            
            var listenerPair = new KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>>(customReference,
                CurrentListener);
            customReference.ValueChanged += CurrentListener;

            return listenerPair;
        }

        public static void StopListeningForValueChanged(
            KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> listener) =>
            listener.Key.ValueChanged -= listener.Value;

        public static void
            CheckIfNodeExists(string path, Action<bool> callback, Action<AggregateException> fallback) =>
            GetJSON(path, snapshot => callback(snapshot.Exists), fallback);

        public static DatabaseReference GetReferenceFromPath(string path)
        {
            var splitPath = path.Split('/');
            return splitPath.Aggregate(reference, (current, child) => current.Child(child));
        }
    }
}