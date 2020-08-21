# Unity-Firebase-Multiplayer-Example
## What is this?
This is an example of a simple turn game created in Unity using the Firebase SDK. It contains a matchmaking backend system that allows two players to get into the same match and a ready-check and move submitting frontend system that allows the players to make their moves and sync their position with other players.

## How do I implement this into my App?
In order to implement this for your Firebase project follow these steps:
- Download the google-services file for your Unity project (if you need to build your App for Android or MacOS). More info [here](https://firebase.google.com/docs/unity/setup)
- Update the EditorURL at DatabaseAPI.cs on line 17 with your Firebase project id
- Set up cloud functions for your Firebase project (firebase init) and then copy the contents of the index.js file in this repository's assets folder and paste it in your own project index.js file. Finally deploy the changes (firebase deploy). More info [here](https://firebase.google.com/docs/functions)
