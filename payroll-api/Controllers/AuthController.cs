using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Core.Helpers;
using Payroll.Core.Logger;
using Payroll.Logic.Model.User;
using Payroll.Logic.Services.Auth;
using Payroll.SharedModel.Request.Auth;
using payroll_api.Common.Settings;
using payroll_api.Helpers.Jwt;
using System.Net;
using System.Security.Claims;

namespace payroll_api.Controllers
{
    public class AuthController : BaseAuthInApiController
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly Lazy<IAuthService> _authService;
        public AppSettings _appSettings;

        public AuthController(IJwtHelper jwtHelper, AppSettings appSettings, IAuthService authService)
        {
            _jwtHelper = jwtHelper ?? throw new ArgumentException(nameof(jwtHelper));
            _appSettings = appSettings;
            _authService = new Lazy<IAuthService>(() => authService);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest entry)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { Status = "Not OK", Message = "The input was not valid", Code = (int)HttpStatusCode.BadRequest });
            }
            try
            {
                UserInfoModel findUser = await _authService.Value.GetByUsername(entry.UserName);

                if (findUser == null)
                {
                    return Ok(new { Status = "Not Found", Message = $"Not found user with {entry.UserName}", Code = (int)HttpStatusCode.OK });
                }

                if (entry.Password.CheckPassword(findUser.Password))
                {
                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.NameIdentifier,entry.UserName),
                        new Claim(ClaimTypes.Sid,findUser.Id.ToString())
                    };
                    if (findUser.RoleNames.Length > 0)
                    {
                        foreach (var role in findUser.RoleNames)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                    DateTime expiresTime = DateTime.Now.AddMinutes(findUser.TokenLifeTimeMinutes ?? 15);

                    string token = _jwtHelper.CreateSecurityToken(expiresTime, claimsIdentity.Claims);
                    //// <Insert User Logging>
                    //await _authUserService.Value.InsertUserLogging(new CreateUserLoggingModel()
                    //{
                    //    UserId = findUser.Id,
                    //    Username = findUser.Username,
                    //    Token = token,
                    //    ExpirationTime = expiresTime
                    //});
                    return await Task.FromResult(Ok(new { Type = JwtBearerDefaults.AuthenticationScheme, token, ExpiresAt = findUser.TokenLifeTimeMinutes }));
                }
                else
                {
                    return Ok(new { Status = "Not OK", Message = $"Username or password was not valid", Code = (int)HttpStatusCode.BadRequest });
                }
            }
            catch (Exception ex)
            {
                await LogHelper.LoggerError("AuthController.SignIn", entry, ex);

                return Ok(new { Status = "Not OK", Message = ex.Message, Code = (int)HttpStatusCode.InternalServerError });
            }
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
