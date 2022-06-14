using System.Collections.Concurrent;

namespace maze_text_game.Games
{
    public class GamesSingleton
    {
        private static ConcurrentDictionary<string, Game> _games = null;
        private static object checkLock = new object();
        private GamesSingleton()
        { }

        public static ConcurrentDictionary<string, Game> Instance
        {
            get
            {
                lock (checkLock)
                {
                    if (_games == null)
                        _games = new ConcurrentDictionary<string, Game>();
                    return _games;
                }
            }
        }
    }
}
