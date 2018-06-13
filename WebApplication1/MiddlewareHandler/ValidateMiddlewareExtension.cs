using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiddlewareHandler
{
    public static class ValidateMiddlewareExtension
    {
        public static IApplicationBuilder UseVaidateMiddleware(this IApplicationBuilder builder, IConfiguration configuration) 
            => builder.UseMiddleware<ValidateMiddleware>(configuration);
    }
}
