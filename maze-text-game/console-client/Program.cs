using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace console_client
{
    internal class Program
    {

        static string currentGameId = null;
        static bool hasVoted = false;
        static bool isListeningCommands = false;
        static bool ended = false;

        static Thread renderThread = new Thread(() => {
            while (currentGameId != null && !ended) {
                Thread.Sleep(1000);
                RenderActiveGame();
            }
        });

        static void RenderActiveGame() {
            Console.Clear();
            dynamic game = Api.GetGame(currentGameId);

            if (!isListeningCommands && hasVoted) {
                Console.WriteLine("Press enter to proceed");
            }

            Console.WriteLine(currentGameId);
            Console.WriteLine(game.GameState);

            if (game.WinningPlayer != "") {
                ended = true;
                Console.WriteLine($"Player {game.WinningPlayer} has won!");
            }            
            Console.WriteLine(game.RenderedMap);
        }

        static void Main(string[] args)
        {            

            while (!ended) {
                string input = Console.ReadLine();

                if (input == "") {
                    break;
                }

                if (Regex.IsMatch(input, @"create \d+\s\d+\s\d+")) {
                    if (currentGameId != null) {
                        Console.WriteLine("You are already in a game");
                        continue;
                    }

                    string[] tokens = input.Split(' ');

                    string guid = Api.CreateGame(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]), Convert.ToInt32(tokens[3]))?.GameGuid;
                    if (guid == null) {
                        Console.WriteLine("Failed to create game.");
                        continue;
                    }
                    
                    currentGameId = guid;
                    RenderActiveGame();
                    Console.WriteLine($"Game with Id: '{guid}' created.\nType 'vote' to start the game.");
                    continue;
                }

                if (Regex.IsMatch(input, @"join .+")) {
                    if (currentGameId != null) {
                        Console.WriteLine("You are already in a game");
                        continue;
                    }

                    string[] tokens = input.Split(' ');

                    string gameId = tokens[1];

                    if (!Api.JoinGame(gameId)) {
                        Console.WriteLine("Could not join game");
                        continue;
                    }

                    currentGameId = gameId;
                    RenderActiveGame();
                    Console.WriteLine("Game joined. Type 'vote' to start the game.");
                    continue;
                }

                if (Regex.IsMatch(input, @"register .+")) {
                    string[] tokens = input.Split(' ');

                    if (Api.Auth(tokens[1]))
                    {
                        currentGameId = null;
                        Console.WriteLine("Successfully registered");
                    }
                    else {
                        Console.WriteLine("Failed to process registration");
                    }

                    continue;
                }

                if (input == "vote") {

                    if (currentGameId == null)
                    {
                        Console.WriteLine("You are not in a game");
                        continue;
                    }

                    if (Api.VoteStart(currentGameId))
                    {
                        hasVoted = true;
                        Console.WriteLine("Vote successful");
                    }
                    else {
                        Console.WriteLine("Vote failed");
                        continue;
                    }

                    RenderActiveGame();
                    renderThread.Start();
                    continue;
                }

                if (input == "get") {

                    if (currentGameId == null) {
                        Console.WriteLine("You are not in a game");
                        continue;
                    }

                    RenderActiveGame();

                    continue;
                }                
            }

            isListeningCommands = true;

            while (!ended) {

                var key = Console.ReadKey(true);
                string command = "";
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        command = "north";
                        break;
                    case ConsoleKey.RightArrow:
                        command = "east";
                        break;
                    case ConsoleKey.DownArrow:
                        command = "south";
                        break;
                    case ConsoleKey.LeftArrow:
                        command = "west";
                        break;
                }

                if (command != "") {
                    Api.RunCommand(currentGameId, command);
                }
            }
        }
    }
}
