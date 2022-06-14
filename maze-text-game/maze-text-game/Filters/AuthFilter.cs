using maze_text_game.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game.Filters
{
    public class AuthFilter : Attribute, IAuthorizationFilter
    {
        public AuthFilter()
        {

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string tokenHeader = context.HttpContext.Request.Headers["Authorization"];

            if (tokenHeader == null)
            {
                context.ModelState.AddModelError("Authorization", "Invalid Authorization Header. Please specify Authorization: Bearer <JWT>");
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            if (!tokenHeader.StartsWith("Bearer "))
            {
                context.ModelState.AddModelError("Authorization", "Invalid Authorization Header. Please specify Authorization: Bearer <JWT>");
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            //Get the JWT token - we skip "Bearer " (7 chars) in the tokenHeader string
            string token = tokenHeader.Substring(7);

            if (!JWTUtils.VerifyToken(token))
            {
                context.ModelState.AddModelError("Authorization", "Invalid JWT Token.");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
                return;
            }

            //Attach the PlayerGuid to the headers
            context.HttpContext.Request.Headers.Add("PlayerGuid", JWTUtils.GetPlayerGuid(token));

            //Attch the PlayerName to the headers
            context.HttpContext.Request.Headers.Add("PlayerName", JWTUtils.GetPlayerName(token));
        }
    }
}
