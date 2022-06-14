using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using maze_text_game.DTO;
using System.Collections.Concurrent;
using maze_text_game.Utils;
using maze_text_game.Filters;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace maze_text_game.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AuthFilter]
    public class GameController : ControllerBase
    {
        private static ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();

        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public ActionResult<GameCreateResDTO> CreateGame(GameCreateReqDTO dto)
        {
            try
            {
                Game game = null;
                bool gameAddedToDictionary = false;

                //Try adding the game to dictionary
                while (!gameAddedToDictionary)
                {
                    //Create game
                    game = new Game(dto.PlayerLimit, new Size(dto.MapWidth, dto.MapHeight));

                    //Attempt to add game to dictionary
                    gameAddedToDictionary = _games.TryAdd(game.SessionId, game);
                }

                //Get player details from headers, attached by AuthFilter
                string playerGuid = Request.Headers["PlayerGuid"];
                string playerName = Request.Headers["PlayerName"];

                //Join game as player
                game.AddPlayer(playerGuid, new Player(playerName));

                //Return created Game
                GameCreateResDTO responseDTO = new GameCreateResDTO(game.SessionId);
                return responseDTO;

            } catch (GameException ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return BadRequest(ModelState);

            } catch (Exception ex)
            {
                string errorId = LogUtils.LogError(_logger, ex);
                ModelState.AddModelError("error", "Internal server error in CreateGame: ErrorId=" + errorId);
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }

        [HttpPost]
        [Route("join")]
        public ActionResult JoinGame(GameReqDTO dto)
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
                    return BadRequest(ModelState);
                }

                //Get player details from headers, attached by AuthFilter
                string playerGuid = Request.Headers["PlayerGuid"];
                string playerName = Request.Headers["PlayerName"];

                //Join game as player
                game.AddPlayer(playerGuid, new Player(playerName));

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
                ModelState.AddModelError("error", "Internal server error in JoinGame: ErrorId=" + errorId);
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }

        [HttpPost]
        [Route("leave")]
        public ActionResult LeaveGame(GameReqDTO dto)
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
                    return BadRequest(ModelState);
                }

                //Get player details from headers, attached by AuthFilter
                string playerGuid = Request.Headers["PlayerGuid"];

                //Join game as player
                game.RemovePlayer(playerGuid);

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
                ModelState.AddModelError("error", "Internal server error in LeaveGame: ErrorId=" + errorId);
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }

        [HttpGet]
        public ActionResult<GameResDTO> GetGame(string gameId)
        {
            try
            {
                Game game = null;

                //Find game
                bool foundGame = _games.TryGetValue(gameId, out game);

                //We could not find the game
                if (!foundGame)
                {
                    ModelState.AddModelError("GameGuid", "Could not find game with id: " + gameId);
                    return BadRequest(ModelState);
                }

                //Prepare response
                GameResDTO responseDTO = new GameResDTO()
                {
                    RenderedMap = RenderUtils.RenderMap(game),
                    GameState = GameStateMapper.GameStateEnumMap(game.GameState),
                    WinningPlayer = game.VictoriousPlayer?.Name ?? ""
                };

                //Return response
                return responseDTO;
            }
            catch (GameException ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                string errorId = LogUtils.LogError(_logger, ex);
                ModelState.AddModelError("error", "Internal server error in GetGame: ErrorId=" + errorId);
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
        }
    }
}
