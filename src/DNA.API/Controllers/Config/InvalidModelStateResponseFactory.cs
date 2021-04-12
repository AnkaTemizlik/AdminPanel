using DNA.API.Extensions;
using DNA.API.Services.Communication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Controllers.Config {
    public static class InvalidModelStateResponseFactory {
        public static IActionResult ProduceErrorResponse(ActionContext context) {
            var errors = context.ModelState.GetErrorMessages();
            var response = new ErrorResource(messages: errors);

            return new BadRequestObjectResult(response);
        }
    }
}
