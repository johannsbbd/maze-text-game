
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

namespace maze_text_game.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        public AuthController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/auth")]
        public ActionResult<AuthTokenResDTO> AuthUser(AuthTokenReqDTO dto)
        {
            try
            {
                string playerGuid = Guid.NewGuid().ToString();
                string jwtToken = JWTUtils.generateJWT(dto.PlayerName, playerGuid);
                AuthTokenResDTO tokenResDTO = new AuthTokenResDTO(jwtToken);
                return tokenResDTO;
            }
            catch (Exception ex)
            {
                string errorId = LogUtils.LogError(_logger, ex);
                ModelState.AddModelError("error", "Internal server error in AuthUser: ErrorId=" + errorId);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }       
    }
}
