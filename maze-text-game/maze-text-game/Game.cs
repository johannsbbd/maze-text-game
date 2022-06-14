using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace maze_text_game
{
    public class Game
    {
        public string SessionId { get; private set; }
        public Map Map { get; private set; }
        public int PlayerLimit { get; private set; }
        public Dictionary<string, Player> Players { get; private set; }
        public Player VictoriousPlayer { get; private set; } = null;
        public Dictionary<string, Point> PlayerPositions { get; private set; }
        public State GameState { get; private set; }

        public Game(int playerLimit, Size mapSize) {
            if (playerLimit <= 0) {
                throw new GameException("Player limit must be a positive integer.");
            }

            if (mapSize.Width <= 0 || mapSize.Height <= 0) {
                throw new GameException("Map Dimensions must be positive integers.");
            }

            this.PlayerLimit = playerLimit;
            this.SessionId = Guid.NewGuid().ToString();
            this.Map = new Map(mapSize.Height, mapSize.Width);
            this.Players = new Dictionary<string, Player>();
            this.PlayerPositions = new Dictionary<string, Point>();
            this.GameState = State.Waiting;
        }

        public void AddPlayer(string playerSessionId, Player player) {
            if (Players.ContainsKey(playerSessionId)) {
                throw new GameException("Player is already in the game.");
            }

            if (Players.Count == PlayerLimit) {
                throw new GameException("Game is full.");
            }

            if (this.GameState == State.InProgress) {
                throw new GameException("Game is already in progress.");
            }

            List<Point> startPoints = this.Map.getStartPoints();
            if (startPoints.Count == 0) {
                throw new GameException("Map contains no starting points.");
            }
            
            PlayerPositions.Add(playerSessionId, startPoints[Players.Count % startPoints.Count]);
            Players.Add(playerSessionId, player);
        }

        public void VoteStart(string playerSessionId) {
            if (Players[playerSessionId].HasVoted) {
                throw new GameException("Player has already voted.");
            }

            Players[playerSessionId].HasVoted = true;

            if (Players.Count == PlayerLimit) {
                bool ready = true;
                foreach (Player player in Players.Values) {
                    ready &= player.HasVoted;
                }

                if (ready) {
                    this.GameState = State.InProgress;
                }
            }
        }

        public void RemovePlayer(string playerSessionId) {
            if (!Players.ContainsKey(playerSessionId)) {
                throw new GameException("Player is not in the game.");
            }

            Players.Remove(playerSessionId);
            PlayerPositions.Remove(playerSessionId);

            if (Players.Count == 1) {
                this.VictoriousPlayer = Players.Values.First();
                this.GameState = State.Ended;
            }
        }

        public void MovePlayer(Direction direction, string playerSessionId) {
            if (this.VictoriousPlayer != null) {
                throw new GameException("Game has ended.");
            }

            Point movement;
            switch (direction) {
                case Direction.North: movement = new Point(0, -1); break;
                case Direction.South: movement = new Point(0, 1); break;
                case Direction.East: movement = new Point(1, 0); break;
                case Direction.West: movement = new Point(-1, 0); break;
                default: movement = new Point(0, 0); break;
            }

            var playerPosition = this.PlayerPositions[playerSessionId];
            var newPosition = new Point(playerPosition.x + movement.x, playerPosition.y + movement.y);

            if (newPosition.x >= this.Map.MapSize.Width || newPosition.y >= this.Map.MapSize.Height) {
                throw new PlayerMoveException("Player cannot move outside of map bounds.");
            }

            if (this.Map.getMap()[newPosition.x, newPosition.y] == BlockType.wall) {
                throw new PlayerMoveException("Player cannot walk through wall");
            }

            if (this.Map.getMap()[newPosition.x, newPosition.y] == BlockType.flag) {
                this.VictoriousPlayer = this.Players[playerSessionId];
                this.GameState = State.Ended;
            }

            this.PlayerPositions[playerSessionId] = newPosition;
        }
    }

    public enum Direction {
        North,
        East,
        South,
        West,
    }

    public enum State {
        Waiting,
        InProgress,
        Ended,
    }

    public class PlayerMoveException : Exception {
        public PlayerMoveException(string message) : base(message) { }
    }

    public class GameException : Exception {
        public GameException(string msg) : base(msg) { }
    }
}
