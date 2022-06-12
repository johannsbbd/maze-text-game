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
            this.playerLimit = playerLimit;
            this.SessionId = Guid.NewGuid().ToString();
            this.map = new Map(mapSize.Height, mapSize.Width);
            this.players = new Dictionary<string, Player>();
            this.playerPositions = new Dictionary<string, Point>();
        }

        public void MovePlayer(Direction direction, string playerSessionId) {
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
