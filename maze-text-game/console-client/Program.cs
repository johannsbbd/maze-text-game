using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace console_client
{
    internal class Program
    {

        static string currentGameId = null;

        static Thread renderThread = new Thread(() => {
            while (currentGameId != null) {
                Thread.Sleep(5000);
                RenderActiveGame();
            }
        });

        static void RenderActiveGame() {
            Console.Clear();
            dynamic game = Api.GetGame(currentGameId);
            Console.WriteLine(game.GameState);
            Console.WriteLine(game.RenderedMap);
        }

        static void Main(string[] args)
        {
            bool ended = false;

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

                    Console.WriteLine(guid);
                    currentGameId = guid;
                    RenderActiveGame();
                    continue;
                }

                if (Regex.IsMatch(input, @"join .+")) {
                    if (currentGameId != null) {
                        Console.WriteLine("You are already in a game");
                        continue;
                    }

                    string[] tokens = input.Split(' ');

                    string gameId = tokens[1];

                    if (Api.JoinGame(gameId)) {
                        Console.WriteLine("Game joined");
                    } else {
                        Console.WriteLine("Could not join game");
                    }

                    currentGameId = gameId;
                    RenderActiveGame();
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
                        Console.WriteLine("Vote successful");
                    }
                    else {
                        Console.WriteLine("Vote failed");
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
                    RenderActiveGame();
                }
            }
        }
    }
}
