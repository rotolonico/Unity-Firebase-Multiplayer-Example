/* eslint-disable max-len */
/* eslint-disable require-jsdoc */

const functions = require("firebase-functions");
const admin = require("firebase-admin");
admin.initializeApp(functions.config().firebase);

const database = admin.database();

exports.joinQueue = functions.https.onCall((data, context) => {
  try {
    assertAuth(context);

    const playerName = data;
    const playerId = context.auth.uid;

    const queueRef = database.ref("matchmaking/" + playerId);
    return queueRef.set({
      name: playerName,
    }).then(() => {
      return successfulRequest(null);
    }).catch((error) => {
      return failedRequest(error);
    });
  } catch (error) {
    console.log("ERROR:" + error);
    return failedRequest(error);
  }
});

exports.leaveQueue = functions.https.onCall((data, context) => {
  try {
    assertAuth(context);

    const playerId = context.auth.uid;

    const queueRef = database.ref("matchmaking/" + playerId);
    return queueRef.remove().then(() => {
      return successfulRequest(null);
    }).catch((error) => {
      return failedRequest(error);
    });
  } catch (error) {
    console.log("ERROR:" + error);
    return failedRequest(error);
  }
});

exports.matchmaker = functions.database.ref("matchmaking/{playerId}")
    .onCreate((snap, context) => {
      const gameId = generateGameId();

      database.ref("matchmaking").once("value").then((players) => {
        let secondPlayer = null;
        players.forEach((player) => {
          if (!player.child("gameId").exists() && player.key !== context.params.playerId) {
            secondPlayer = player;
          }
        });

        if (secondPlayer === null) return null;

        database.ref("matchmaking").transaction(function(matchmaking) {
          // If any of the players gets into another game during the transaction, abort the operation
          if (matchmaking === null || matchmaking[context.params.playerId]["gameId"] !== undefined || matchmaking[secondPlayer.key]["gameId"] !== undefined) return matchmaking;

          matchmaking[context.params.playerId]["gameId"] = gameId;
          matchmaking[secondPlayer.key]["gameId"] = gameId;
          return matchmaking;
        }).then((result) => {
          if (result.snapshot.child(context.params.playerId + "/gameId").val() !== gameId) return;

          const game = {
            gameInfo: {
              gameId: gameId,
              playersInfo: {
                [context.params.playerId]: {
                  id: context.params.playerId,
                  name: result.snapshot.child(context.params.playerId + "/name").val(),
                },
                [secondPlayer.key]: {
                  id: secondPlayer.key,
                  name: result.snapshot.child(secondPlayer.key + "/name").val(),
                },
              },
            },
            turn: context.params.playerId,
          };

          database.ref("games/" + gameId).set(game).then((snapshot) => {
            console.log("Game created successfully!");
            return null;
          }).catch((error) => {
            console.log(error);
          });

          return null;
        }).catch((error) => {
          console.log(error);
        });

        return null;
      }).catch((error) => {
        console.log(error);
      });
    });

exports.setReady = functions.https.onCall((data, context) => {
  try {
    assertAuth(context);

    const playerId = context.auth.uid;

    return database.ref("matchmaking/" + playerId + "/gameId").once("value").then((gameId) => {
      if (gameId.val() === null) return failedRequest("Player is not in a game");
      const readyRef = database.ref("games/" + gameId.val() + "/ready/" + playerId);
      return readyRef.set(true).then(() => {
        return successfulRequest(null);
      }).catch((error) => {
        return failedRequest(error);
      });
    }).catch((error) => {
      return failedRequest(error);
    });
  } catch (error) {
    console.log("ERROR:" + error);
    return failedRequest(error);
  }
});

exports.makeMove = functions.https.onCall((data, context) => {
  try {
    assertAuth(context);

    const playerId = context.auth.uid;
    const move = data;

    assertValidMove(move);

    return database.ref("matchmaking/" + playerId + "/gameId").once("value").then((gameId) => {
      if (gameId.val() === null) return failedRequest("Player is not in a game");
      const gameRef = database.ref("games/" + gameId.val());
      const movesRef = gameRef.child(playerId + "/moves");
      const turnRef = gameRef.child("turn");

      return gameRef.once("value").then((game) => {
        if (game.child("turn").val() !== playerId) return failedRequest("It's not your turn");

        return movesRef.push().set(move).then(() => {
          turnRef.set(Object.keys(game.child("gameInfo/playersInfo").val()).filter((id) => id !== playerId)[0]);
          return successfulRequest(null);
        }).catch((error) => {
          throw error;
        });
      }).catch((error) => {
        throw error;
      });
    }).catch((error) => {
      throw error;
    });
  } catch (error) {
    console.log("ERROR:" + error);
    return failedRequest(error.message);
  }
});

function generateGameId() {
  const possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  let gameId = "";
  for (let j = 0; j < 20; j++) gameId += possibleChars.charAt(Math.floor(Math.random() * possibleChars.length));
  return gameId;
}

function successfulRequest(data) {
  return {
    isFaulted: false,
    data: data,
  };
}

function failedRequest(errorMessage) {
  return {
    isFaulted: true,
    error: errorMessage,
  };
}

function assertAuth(context) {
  const auth = context.auth;

  if (!auth) {
    throw new functions.https.HttpsError("unauthenticated", "User is not authenticated");
  }
}

function assertValidMove(move) {
  if (move === undefined ||
    !Array.isArray(move) ||
    move.length !== 2 ||
    move[0] == 0 && move[1] == 0 ||
    Math.abs(move[0]) != 1 && move[0] != 0 ||
    Math.abs(move[1]) != 1 && move[1] != 0) {
    throw new functions.https.HttpsError("invalid-argument", "Move is not valid");
  }
}
