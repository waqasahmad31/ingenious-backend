
namespace Ingenious.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomMiddleware(this WebApplication app)
        {

            app.UseSwagger();
            app.UseSwaggerUI();

            // Serve static files if wwwroot exists
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
            {
                app.UseDefaultFiles();
                app.UseStaticFiles();
            }
            else
            {
                app.Logger.LogWarning("The WebRootPath was not found: wwwroot directory is missing.");
            }

            // Middleware configuration
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<AuthorizationMiddleware>();

            // Enable HTTPS redirection if necessary
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseHttpsRedirection();
            }

            // Map controllers
            app.MapControllers();
        }
    }
}
