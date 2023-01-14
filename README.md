# Unity-Firebase-Multiplayer-Example
## What is this?
This is an example of a simple turn game created in Unity using the Firebase SDK. It contains a matchmaking backend system that allows two players to get into the same match and a ready-check and move submitting frontend system that allows the players to make their moves and sync their position with other players.

## How do I implement this into my App?
In order to implement this for your Firebase project follow these steps:
- Download the google-services file for your Unity project (if you need to build your App for Android or MacOS). More info [here](https://firebase.google.com/docs/unity/setup)
- Set up cloud functions for your Firebase project (firebase init) and then copy the contents of the index.js file in this repository's assets folder and paste it in your own project index.js file. Finally deploy the changes (firebase deploy). More info [here](https://firebase.google.com/docs/functions)

## v2 branch
- The v2 branch is an improvement of the Multiplayer system showcased in the [old tutorial video](youtu.be/wBvWaTTxfmo), which allows for a secure multiplayer example. This is done by disallowing the client from writing to the database completely and instead validating all requests through cloud functions.
- Additionally, Realtime Rules have been added to make it so the client can only read their own matchmaking and game data (database.rules.json) 
- You can watch the updated video that showcases the v2 branch [here](https://youtu.be/pjOlGwxYNXs)
