const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp(functions.config().firebase);

var database = admin.database();

exports.matchmaker = functions.database.ref('matchmaking/{playerId}')
    .onCreate((snap, context) => {

        var gameId = generateGameId();

        database.ref('matchmaking').once('value').then(players => {
            var secondPlayer = null;
            players.forEach(player => {
                if (player.val() === "placeholder" && player.key !== context.params.playerId) {
                    secondPlayer = player;
                }
            });

            if (secondPlayer === null) return null;

            var game = {
                gameInfo: {
                    gameId: gameId,
                    playersIds: [context.params.playerId, secondPlayer.key]
                },
                turn: context.params.playerId
            }

            database.ref("games/" + gameId).set(game).then(snapshot => {

                database.ref("matchmaking/" + context.params.playerId).set(gameId);
                secondPlayer.ref.set(gameId);

                return null;
            }).catch(error => {
                console.log(error);
            });

            return null;
        }).catch(error => {
            console.log(error);
        });
    });

function generateGameId() {
    var possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var gameId = "";
    for (var j = 0; j < 20; j++) gameId += possibleChars.charAt(Math.floor(Math.random() * possibleChars.length));
    return gameId;
}