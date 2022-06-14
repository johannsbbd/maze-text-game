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

            if (!tokenHeader.StartsWith("Bearer "))
            {
                context.ModelState.AddModelError("Authorization", "Invalid Authorization Header. Please specify Authorization: Bearer <JWT>");
                context.Result = new BadRequestResult();
                return;
            }

            //Get the JWT token - we skip "Bearer " (8 chars) in the tokenHeader string
            string token = tokenHeader.Substring(8);

            if (!JWTUtils.VerifyToken(token))
            {
                context.ModelState.AddModelError("Authorization", "Invalid JWT Token.");
                context.Result = new UnauthorizedResult();
                return;
            }

            //Attach the PlayerGuid to the headers
            context.HttpContext.Request.Headers.Add("PlayerGuid", JWTUtils.GetPlayerGuid(token));

            //Attch the PlayerName to the headers
            context.HttpContext.Request.Headers.Add("PlayerName", JWTUtils.GetPlayerName(token));
        }
    }
}
