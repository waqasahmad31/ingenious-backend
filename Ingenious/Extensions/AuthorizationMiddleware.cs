using Ingenious.Models.Helpers;
using Newtonsoft.Json;

namespace Ingenious.Extensions
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);

            if (httpContext.Response.StatusCode == 401)
            {
                var apiResponse = new ApiResponse<string>("You must be logged in to access this resource.", 401);
                await WriteResponse(httpContext, apiResponse);
            }
            else if (httpContext.Response.StatusCode == 403)
            {
                var apiResponse = new ApiResponse<string>("You do not have permission to access this resource. Please ensure you have the correct role.", 403);
                await WriteResponse(httpContext, apiResponse);
            }
        }

        private async Task WriteResponse(HttpContext httpContext, ApiResponse<string> apiResponse)
        {
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
        }
    }
}
