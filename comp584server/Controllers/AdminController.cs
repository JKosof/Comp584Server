using comp584server.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WorldModel;

namespace comp584server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<WorldModelUser> userManager, JwtHandler jwthandler) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            WorldModelUser? worldUser = await userManager.FindByNameAsync(loginRequest.Username);
            //if (worlduser is null || !await userManager.CheckPasswordAsync(worlduser, loginRequest.Password))
            //{
            //    Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    return;
            //}
            if (worldUser == null)
            {
                return Unauthorized("invalid username");
            }

            bool passwordIsValid = await userManager.CheckPasswordAsync(worldUser, loginRequest.Password);
            if (!passwordIsValid)
            {
                return Unauthorized("invalid password");
            }
            JwtSecurityToken token = await jwthandler.GenerateTokenAsync(worldUser);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse { 
                Success = true,
                Message = "login successful",
                Token = tokenString});

        }
    }
}
