using maze_text_game.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game
{
    public class AddAuthHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            IEnumerable<object> attributes = context.ApiDescription.CustomAttributes();
            
            //Check if the auth filter is applied to the element
            bool isAuthFiltered = attributes.Any(a => a.GetType() == typeof(AuthFilter));
            
            if (isAuthFiltered)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "Bearer <JWT>"
                });
            }
        }
    }
}
