using DAS.DataAccess.Helpers;
using Google.Protobuf.WellKnownTypes;
using Ingenious.Models.Account;
using Ingenious.Models.EmailDtos;
using Ingenious.Models.Entities;
using Ingenious.Models.Helpers;
using Ingenious.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ingenious.Repositories
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> RegisterUser(RegisterUserDto model);
        Task<bool> ChangePassword(string userId, string oldPassword, string newPassword);
        Task<bool> DeleteUser(string userId);
        Task<AccessRequestResponseDto> Login(UserLoginDto model);
        Task<AccessRequestResponseDto> RefreshAccessToken(AccessRequestResponseDto model);
        Task<IEnumerable<IdentityRole>> GetRoles();
        Task<IEnumerable<AspNetUsersDto>> GetAllAspNetUsers(int roleId, string userId);
        Task<IEnumerable<AspNetUsersDto>> GetAllAspNetUsersByRole(string role);
        Task<List<AdminEmailDto>> GetAllEmails();
        Task<IList<ApplicationUser>> GetAllAdminIds();
        Task<bool> IsBlockUserAccountByIdAsync(string aspNetUserId, bool isBlocked);
    }
    public class AccountRepository : IAccountRepository
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTModel _jwtSetting;
        private readonly IConfiguration _configuration;
        private readonly IEmailRepository _emailRepository;
        private readonly IAddressRepository _addressRepository;

        public AccountRepository(
            ConnectionStrings connectionStrings,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JWTModel> jwtSetting,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IEmailRepository emailRepository,
            IAddressRepository addressRepository)
        {
            _connectionStrings = connectionStrings;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSetting = jwtSetting.Value;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailRepository = emailRepository;
            _addressRepository = addressRepository;
        }

        public async Task<ApplicationUser> RegisterUser(RegisterUserDto model)
        {
            string connStr = await _connectionStrings.Get();

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                await con.OpenAsync();
                using (MySqlTransaction transaction = await con.BeginTransactionAsync())
                {
                    try
                    {
                        var user = new ApplicationUser
                        {
                            UserName = model.Username,
                            Email = model.Username,
                            PhoneNumber = model.ContactNumber,
                            FullName = model.FullName,
                            isBlocked = model.isBlocked,
                            IsDeleted = model.IsDeleted
                        };

                        var result = await _userManager.CreateAsync(user, model.Password);

                        if (!result.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        var role = await _roleManager.FindByIdAsync(model.RoleId);
                        var isAddedToRole = await _userManager.AddToRoleAsync(user, role.Name);

                        if (!isAddedToRole.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        if (model.Address != null)
                        {
                            model.Address.AspNetUserId = user.Id;
                            int addressId = await _addressRepository.AddAddressWithTransactionAsync(model.Address, transaction);

                            if (addressId <= 0)
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }
                        }

                        await transaction.CommitAsync();
                        return user;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"Error registering user: {ex.Message}", ex);
                    }
                }
            }
        }

        public async Task<bool> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<AccessRequestResponseDto> Login(UserLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                throw new Exception("Invalid username.");
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);

            if (!result.Succeeded)
            {
                throw new Exception("Invalid password.");
            }

            if (user.isBlocked)
            {
                throw new Exception("Your account is blocked. Please contact support.");
            }

            if (user.IsDeleted)
            {
                throw new Exception("Your account has been deleted. Please contact support.");
            }

            return await GetLoginResponse(model);
        }

        // Refresh Access Token
        public async Task<AccessRequestResponseDto> RefreshAccessToken(AccessRequestResponseDto model)
        {
            string accessToken = model.AccessToken;
            string refreshToken = model.RefreshToken;
            var getprinicipalModel = GetPrincipalFromExpiredToken(accessToken);

            var user = await _userManager.FindByNameAsync(getprinicipalModel.Name);

            if (user == null || model.RefreshToken != refreshToken || model.RefreshTokeExpirationTime <= DateTime.Now)
            {
                return null;
            }



            AccessRequestResponseDto accessRequestResponse = GenerateAccessToken(getprinicipalModel.Claims);

            return accessRequestResponse;
        }

        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        ////Helper Method to check roles are present in DB for register method 
        //private async Task EnsureRolesExistAsync()
        //{
        //    var roles = new[] { UserRoleConstants.Doctor, UserRoleConstants.Patient, UserRoleConstants.Admin };

        //    foreach (var role in roles)
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(role));
        //        }
        //    }
        //}
        // Login 
        private async Task<AccessRequestResponseDto> GetLoginResponse(UserLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);

            if (user != null && result.Succeeded)
            {
                var getRole = await _userManager.GetRolesAsync(user);

                IdentityOptions _options = new IdentityOptions();

                var section = _configuration.GetSection("JWTConfig");
                var authClaims = new List<Claim>
                        {
                            new Claim("AspNetUserId", user.Id.ToString()),
                            new Claim("audience", section["Audience"]),
                            new Claim("issuer", section["Audience"]),
                            new Claim("UserName", user.UserName.ToString()),
                            new Claim("FullName", user.FullName.ToString()),
                            new Claim("IsBlocked", user.isBlocked.ToString()),
                            new Claim("IsDeleted", user.IsDeleted.ToString()),
                            new Claim(_options.ClaimsIdentity.RoleClaimType,getRole.FirstOrDefault())
                        };
                var createAccessAndRefreshToken = CreateAccessAndRefreshToken(authClaims, user);
                return await Task.FromResult(createAccessAndRefreshToken);
            }
            return null;
        }

        private AccessRequestResponseDto GenerateAccessToken(List<Claim> authClaims)
        {
            var section = _configuration.GetSection("JWTConfig");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SymmetricSecurityKey"]));
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Issuer = section["Issuer"],
            //    Audience = section["Audience"],
            //    Subject = new ClaimsIdentity(authClaims),
            //    Expires = DateTime.Now.AddMinutes(Convert.ToInt32(section["Expires"])),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.secret)), SecurityAlgorithms.HmacSha384Signature)
            //};
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = section["Issuer"],
                Audience = section["Audience"],
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.Now.AddMinutes(Convert.ToInt32(section["Expires"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SymmetricSecurityKey"])), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            var securityHandler = tokenhandler.CreateToken(tokenDescriptor);
            var token = tokenhandler.WriteToken(securityHandler);

            AccessRequestResponseDto accessRequestResponse = new AccessRequestResponseDto()
            {
                AccessToken = token,
                //AccessTokeExpirationTime = token.ValidTo,
                AccessTokeExpirationTime = Convert.ToDateTime(tokenDescriptor.Expires),
                RefreshToken = GenerateRefreshToken(),
                RefreshTokeExpirationTime = DateTime.Now.AddDays(1)
            };

            return accessRequestResponse;
        }

        // GenerateRefreshToken
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        // Principal Expired Token
        private Principal GetPrincipalFromExpiredToken(string token)
        {
            var section = _configuration.GetSection("JWTConfig");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = section["Audience"],
                ValidateIssuer = true,
                ValidIssuer = section["Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SymmetricSecurityKey"])),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            ////if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha384Signature, StringComparison.InvariantCultureIgnoreCase))
            ////    throw new SecurityTokenException("Invalid token");
            //var userName = principal.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(x => x.Value).FirstOrDefault();
            //Principal prinicipalModel = new Principal()
            //{
            //    Name = userName,
            //    Claims = principal.Claims.ToList()
            //};
            var userNameClaim = principal.Claims
                                .FirstOrDefault(x => x.Type == "UserName");

            if (userNameClaim != null)
            {
                var userName = userNameClaim.Value;
                Principal principalModel = new Principal()
                {
                    Name = userName,
                    Claims = principal.Claims.ToList()
                };
                return principalModel;
            }
            else
            {
                // Log details about missing claim
                Console.WriteLine("Claim 'nameidentifier' not found in token.");
                throw new SecurityTokenException("Invalid token");
            }
            //return prinicipalModel;
        }


        // Creating and Refreshing the AccessToken
        private AccessRequestResponseDto CreateAccessAndRefreshToken(List<Claim> Claims, ApplicationUser User)
        {
            AccessRequestResponseDto accessRequestResponse = GenerateAccessToken(Claims);

            return accessRequestResponse;
        }

        private bool IsDtoEmpty<T>(T dto) where T : class
        {
            return dto.GetType().GetProperties().All(prop => prop.GetValue(dto) == null);
        }

        public async Task<IEnumerable<AspNetUsersDto>> GetAllAspNetUsers(int roleId, string userId)
        {
            var parameters = new MySqlParameter[]
                {

                   new MySqlParameter("@p_RoleId", roleId),
                   new MySqlParameter("@p_UserId", userId)
                };

            return await DbHelper.GetList<AspNetUsersDto>("sp_get_All_users", parameters, _connectionStrings);
        }

        public async Task<IEnumerable<AspNetUsersDto>> GetAllAspNetUsersByRole(string role)
        {
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_Role", role)
            };

            return await DbHelper.GetList<AspNetUsersDto>("GetAllUsersByRole", parameters, _connectionStrings);
        }

        public async Task<List<AdminEmailDto>> GetAllEmails()
        {
            var parameters = new MySqlParameter[] { };

            return await DbHelper.GetList<AdminEmailDto>("get_Emails", parameters, _connectionStrings);
        }

        public async Task<IList<ApplicationUser>> GetAllAdminIds()
        {
            return await _userManager.GetUsersInRoleAsync("Admin");
        }

        public async Task<bool> IsBlockUserAccountByIdAsync(string aspNetUserId, bool isBlocked)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(aspNetUserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.isBlocked = isBlocked;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to block the user.");
            }

            if (isBlocked)
            {
                EmailSettingDto emailSetting = await _emailRepository.GetEmailSetting(type: "SMTPInfo");
                EmailTemplateDto emailTemplate = await _emailRepository.GetEmailTemplate(type: "BlockUser");
                string emailMsg = emailTemplate.Template;
                emailMsg = emailMsg.Replace("|*name*|", user.FullName);
                bool isSend = await _emailRepository.SendEmail(emailSetting, user.UserName, "Your Account Has Been Blocked", emailTemplate.Template);
            }

            return true;
        }
    }
}
