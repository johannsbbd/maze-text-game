using System.Linq;

namespace maze_text_game.Utils
{
    public class RenderUtils
    {
        private static char? getPlayerAt(int x, int y, Game g)
        {
            for (int i = 0; i < g.Players.Count; i++)
            {
                var point = g.PlayerPositions.Values.ElementAt(i);
                if (point.x == x && point.y == y)
                {
                    return (char)(i + 65);
                }
            }

            return null;
        }

        public static string renderMap(Game game)
        {
            int height = game.Map.MapSize.Height;
            int width = game.Map.MapSize.Width;

            string res = "";

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char? playerChar = getPlayerAt(x, y, game);
                    if (playerChar != null)
                    {
                        res += playerChar;
                        continue;
                    }

                    res += (char)game.Map.getMap()[y, x];
                }

                res += "\n";
            }

            return res;
        }
    }
}
