using Ingenious.Data;
using Ingenious.Models.Entities;
using Ingenious.Models.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Ingenious.Extensions
{
    public static class JwtExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 5)) // Adjust the version as per your MySQL version
                )
            );
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 5;
            });

            services.AddHttpContextAccessor();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // JWT Config
            services.Configure<JWTModel>(configuration.GetSection("JWT"));
            var section = configuration.GetSection("JWTConfig");

            // Clear default inbound claims mapping
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SymmetricSecurityKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = section["Audience"],
                    ValidIssuer = section["Issuer"],
                    ClockSkew = TimeSpan.Zero
                };

                // Handle SignalR Tokens
                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");


                        var path = context.HttpContext.Request.Path;

                        // Check if the request is for SignalR hub
                        // Log for debugging
                        //Console.WriteLine($"Extracted Token: {accessToken}");

                        // Set token for notification SignalR hub requests
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token successfully validated.");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
