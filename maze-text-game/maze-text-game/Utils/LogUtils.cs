using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game.Utils
{
    public static class LogUtils
    {

        public static string LogError(ILogger logger, Exception ex)
        {
            Guid errorId = Guid.NewGuid();

            logger.LogError("ErrorId: " + errorId);
            logger.LogError("ErrorMessage: " + ex.Message);
            logger.LogError("ErrorStackTrace: " + ex.StackTrace);

            return errorId.ToString();
        }
    }
}
