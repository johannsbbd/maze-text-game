using maze_text_game.DTO;
using maze_text_game.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using maze_text_game.Games;
using maze_text_game.Filters;

namespace maze_text_game.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AuthFilter]
    public class CommandController : Controller
    {

        private ConcurrentDictionary<string, Game> _games = GamesSingleton.Instance;

        private readonly ILogger<CommandController> _logger;

        public CommandController(ILogger<CommandController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult ActionCommand(CommandReqDTO dto)
        {
            try
            {
                Game game = null;

                //Find game
                bool foundGame = _games.TryGetValue(dto.GameGuid, out game);

                //We could not find the game
                if (!foundGame)
                {
                    ModelState.AddModelError("GameGuid", "Could not find game with id: " + dto.GameGuid);
                    return BadRequest(ModelState); ;
                }

                //Get player details from headers, attached by AuthFilter
                string playerGuid = Request.Headers["PlayerGuid"];

                //Move player
                if (!string.IsNullOrEmpty(dto.Command))
                {
                    string direction = dto.Command;
                    Direction directionEnumValue;
                    switch (direction.ToLower())
                    {
                        case "north":
                            directionEnumValue = Direction.North;
                            break;
                        case "west":
                            directionEnumValue = Direction.West;
                            break;
                        case "south":
                            directionEnumValue = Direction.South;
                            break;
                        case "east":
                            directionEnumValue = Direction.East;
                            break;
                        default:
                            ModelState.AddModelError("error", "Invalid command");
                            return BadRequest(ModelState);
                    }

                    game.MovePlayer(directionEnumValue, playerGuid);
                }
                else 
                {
                    ModelState.AddModelError("error", "Invalid command");
                    return BadRequest(ModelState);
                }
                
                //Return Ok
                return Ok();
            }
            catch (GameException ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                string errorId = LogUtils.LogError(_logger, ex);
                ModelState.AddModelError("error", "Internal server error in Command: ErrorId=" + errorId);
                ModelState.AddModelError("real_error", ex.Message);
                ModelState.AddModelError("stack_trace", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }
    }
}
