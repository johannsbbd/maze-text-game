using System;
using System.Collections.Generic;
using System.Drawing;

namespace maze_text_game
{
    public class Game
    {
        public string SessionId { get; private set; }
        private Map map;
        private int playerLimit;
        private Dictionary<string, Player> players;
        private Player victoriousPlayer = null;
        private Dictionary<string, Point> playerPositions;

        public Game(int playerLimit, Size mapSize) {
            if (playerLimit <= 0) {
                throw new GameException("Player limit must be a positive integer.");
            }

            if (mapSize.Width <= 0 || mapSize.Height <= 0) {
                throw new GameException("Map Dimensions must be positive integers.");
            }

            this.playerLimit = playerLimit;
            this.SessionId = Guid.NewGuid().ToString();
            this.map = new Map(mapSize.Height, mapSize.Width);
            this.players = new Dictionary<string, Player>();
            this.playerPositions = new Dictionary<string, Point>();
        }

        public void AddPlayer(string playerSessionId, Player player) {
            if (players.ContainsKey(playerSessionId)) {
                throw new GameException("Player is already in the game.");
            }

            if (players.Count == playerLimit) {
                throw new GameException("Game is full.");
            }

            List<Point> startPoints = this.map.getStartPoints();
            if (startPoints.Count == 0) {
                throw new GameException("Map contains no starting points.");
            }
            
            playerPositions.Add(playerSessionId, startPoints[players.Count % startPoints.Count]);
            players.Add(playerSessionId, player);
        }

        public void RemovePlayer(string playerSessionId) {
            if (!players.ContainsKey(playerSessionId)) {
                throw new GameException("Player is not in the game.");
            }

            players.Remove(playerSessionId);
            playerPositions.Remove(playerSessionId);
        }

        public void MovePlayer(Direction direction, string playerSessionId) {
            if (this.victoriousPlayer != null) {
                throw new GameException("Game has ended.");
            }

            Point movement;
            switch (direction) {
                case Direction.North: movement = new Point(0, 1); break;
                case Direction.South: movement = new Point(0, -1); break;
                case Direction.East: movement = new Point(1, 0); break;
                case Direction.West: movement = new Point(-1, 0); break;
                default: movement = new Point(0, 0); break;
            }

            var playerPosition = this.playerPositions[playerSessionId];
            var newPosition = new Point(playerPosition.x + movement.x, playerPosition.y + movement.y);

            if (newPosition.x >= this.map.MapSize.Width || newPosition.y >= this.map.MapSize.Height) {
                throw new PlayerMoveException("Player cannot move outside of map bounds.");
            }

            if (this.map.getMap()[newPosition.x, newPosition.y] == BlockType.wall) {
                throw new PlayerMoveException("Player cannot walk through wall");
            }

            if (this.map.getMap()[newPosition.x, newPosition.y] == BlockType.flag) {
                this.victoriousPlayer = this.players[playerSessionId];
            }

            this.playerPositions[playerSessionId] = newPosition;
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
