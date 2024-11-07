using Catalogo_Api.DTOs;
using Catalogo_Api.Models;
using Catalogo_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Catalogo_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ItolkenService _tolkenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ItolkenService tolkenService,
                     UserManager<ApplicationUser> userManager,
                     RoleManager<IdentityRole> roleManager,
                     IConfiguration configuration,
                     ILogger<AuthController> logger)
    {
        _tolkenService = tolkenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username!);
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var AuthClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id",user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var userRole in userRoles)
            {

                AuthClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = _tolkenService.GenerateAcessToken(AuthClaims, _configuration);
            var refreshToken = _tolkenService.GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                                out int refreshTokenValidityInMinutes);
            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime =
                                DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
            
           

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo


            });
        }
        return Unauthorized();
    }
    [HttpPost]
    [Route("Register")]

    public async Task<IActionResult> Resgister([FromBody] RegisterModel model)
    {
        var userExist = await _userManager.FindByNameAsync(model.Username!);
        if (userExist != null) return StatusCode(StatusCodes.Status500InternalServerError,
                                      new Response { Status = "Error", Message = "Use already exists" });

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError,
                                       new Response { Status = "Error", Message = "User creation failed" });

        return Ok(new Response { Status = "Success", Message = "User created successfuly" });




    }
    [HttpPost]
    [Route("refresh-token")]

    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel == null)
        {
            return BadRequest("Invalid client request");
        }
        string? accessToken = tokenModel.AcessToken
                              ?? throw new ArgumentNullException(nameof(tokenModel));

        string? refreshToken = tokenModel.RefreshToken
                              ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tolkenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);
        if (principal == null)
        {
            return BadRequest("Invalid token access/refresh token");
        }
        string UserName = principal.Identity!.Name!;
        var user = await _userManager.FindByNameAsync(UserName!);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token/refresh token");
        }
        var newAccessToken = _tolkenService.GenerateAcessToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tolkenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken

        });
    }
   
    [HttpPost]
    [Route("Revolke/{username}")]
    [Authorize(Policy = "ExclusiveOnly")]
    public async Task<IActionResult> Revolke(string username)
    {
        var user=await _userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("Invalid user name");
        user.RefreshToken=null;
        await _userManager.UpdateAsync(user);
        return NoContent();
    }
    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "SuperAdminOnly")]

    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var rouleResul=await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (rouleResul.Succeeded)
            {
                _logger.LogInformation(1, "Roles added");
                return StatusCode(StatusCodes.Status200OK,
                       new Response { Status = "Success",Message=$"Role {roleName} added successfully" }); 
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest,
                       new Response { Status = "Success", Message = $"Issue adding the new {roleName} role" });
            }
            
        }
        return StatusCode(StatusCodes.Status400BadRequest,
            new Response { Status = "Error", Message = "Role already exist" });
    }
    [HttpPost]
    [Route("RoleUser")]
    [Authorize(Policy = "SuperAdminOnly")]

    public async Task<IActionResult> AddRoler(string email,string roleName)
    {
        var user= await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result=await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Succeess", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogInformation(1,$"Error:Unable to the add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Error" ,Message=$"Unable to the add {user.Email} to the {roleName} role "});

            }
        }
        return BadRequest(new {error="Unable to find user"});
    }
    

}
