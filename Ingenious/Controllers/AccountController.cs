using Ingenious.Models.Account;
using Ingenious.Models.Helpers;
using Ingenious.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Ingenious.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            if (model == null)
                return Ok(new ApiResponse<string>("Please provide valid registration details.", 400));

            var user = await _accountRepository.RegisterUser(model);
            if (user == null)
            {
                return Ok(new ApiResponse<string>("Failed to register the user. Please try again.", 400));
            }

            return Ok(new ApiResponse<object>(user, "Your account has been successfully created!", 200));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (model == null)
                return Ok(new ApiResponse<string>("Please provide valid password details.", 400));

            var result = await _accountRepository.ChangePassword(model.User_Id, model.OldPassword, model.NewPassword);
            if (!result)
            {
                return Ok(new ApiResponse<string>("Unable to change the password. Please check your current password and try again.", 400));
            }

            return Ok(new ApiResponse<string>("Password changed successfully!", 200));
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _accountRepository.DeleteUser(userId);
            if (!result)
            {
                return NotFound(new ApiResponse<string>("The user does not exist or could not be found.", 404));
            }

            return Ok(new ApiResponse<string>("User deleted successfully.", 200));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            try
            {
                if (model == null)
                    return Ok(new ApiResponse<string>("Please provide valid login credentials.", 400));

                var loginResponse = await _accountRepository.Login(model);
                if (loginResponse == null)
                {
                    return Ok(new ApiResponse<string>("Invalid username or password. Please try again.", 401));
                }

                return Ok(new ApiResponse<object>(loginResponse, "Login successful!", 200));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("blocked"))
                {
                    return Ok(new ApiResponse<string>("Your account is blocked. Please contact support.", 403));
                }
                else if (ex.Message.Contains("deleted"))
                {
                    return Ok(new ApiResponse<string>("Your account has been deleted. Please contact support.", 410));
                }
                else if (ex.Message.Contains("Invalid username"))
                {
                    return Ok(new ApiResponse<string>("Your account has been deleted. Please contact support.", 401));
                }
                else if (ex.Message.Contains("Invalid password"))
                {
                    return Ok(new ApiResponse<string>("Your account has been deleted. Please contact support.", 401));
                }
                else
                {
                    return StatusCode(500, new ApiResponse<string>($"Internal server error: {ex.Message}", 500));
                }
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] AccessRequestResponseDto model)
        {
            if (model == null)
                return Ok(new ApiResponse<string>("Please provide a valid refresh token.", 400));

            var refreshTokenResponse = await _accountRepository.RefreshAccessToken(model);
            if (refreshTokenResponse == null)
            {
                return Ok(new ApiResponse<string>("Invalid refresh token. Please login again.", 401));
            }

            return Ok(new ApiResponse<object>(refreshTokenResponse, "Token refreshed successfully!", 200));
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _accountRepository.GetRoles();
            return Ok(new ApiResponse<object>(roles, "Roles fetched successfully!", 200));
        }

        //[HttpGet]
        //[Route("GetUsersById_Role")]
        //public async Task<object> GetUsersById_Role(string userId, int roleId)
        //{
        //    try
        //    {
        //        string roleName = roleId == 1 ? "Admin" :
        //           roleId == 2 ? "Doctor" :
        //           roleId == 3 ? "Pharmacist" : "LabAssistant";

        //        GetAllAspNetUsersDto aspNetUsers = new GetAllAspNetUsersDto();

        //        AspNetUsersDto user = new AspNetUsersDto();

        //        var results = await _accountRepository.GetAllAspNetUsers(roleId, userId);

        //        aspNetUsers = new GetAllAspNetUsersDto()
        //        {
        //            RoleId = roleId,
        //            Name = roleName,
        //            Users = results.ToList()

        //        };
        //        return Ok(new ApiResponse<GetAllAspNetUsersDto>(aspNetUsers, "User fetched successfully", 200));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>("An error occurred while fetching a user: " + ex.Message, 500));
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllAdminIds")]
        //public async Task<IActionResult> GetAllAdminIds()
        //{
        //    try
        //    {
        //        var adminUsers = await _accountRepository.GetAllAdminIds();

        //        var adminIds = adminUsers.Select(user => user.Id).ToList();

        //        return Ok(new ApiResponse<List<string>>(adminIds, "AdminIds are fetched successfully.", 200));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>("An error occurred while fetching AdminIds: " + ex.Message, 500));
        //    }
        //}

        [HttpPost("block-user/{aspNetUserId}")]
        public async Task<IActionResult> IsBlockUserAccountByIdAsync(string aspNetUserId, bool isBlocked)
        {
            if (string.IsNullOrEmpty(aspNetUserId))
                return Ok(new ApiResponse<string>("Please provide a valid user ID.", 400));

            try
            {
                var result = await _accountRepository.IsBlockUserAccountByIdAsync(aspNetUserId, isBlocked);
                return Ok(new ApiResponse<string>("The user account has been successfully blocked.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while blocking the user: {ex.Message}", 500));
            }
        }

        [HttpGet]
        [Route("GetUsersByRole")]
        public async Task<object> GetUsersById_Role(string role)
        {
            try
            {
                GetAllAspNetUsersDto aspNetUsers = new GetAllAspNetUsersDto();

                IEnumerable<AspNetUsersDto> users = await _accountRepository.GetAllAspNetUsersByRole(role);
                return Ok(new ApiResponse<IEnumerable<AspNetUsersDto>>(users, "User fetched successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while fetching a user: " + ex.Message, 500));
            }
        }
    }
}