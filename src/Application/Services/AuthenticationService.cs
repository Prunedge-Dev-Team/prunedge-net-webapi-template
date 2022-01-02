using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private User? _user;

    public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
    {
        var user = _mapper.Map<User>(userForRegistrationDto);
        var email = userForRegistrationDto.Email!.ToLowerInvariant().Trim();
        // TODO: Remove this for user verification
        user.Verified = true;
        user.Email = email;
        user.UserName = email;
        var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);

        if (result.Succeeded)
            await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);
        return result;
    }

    public async Task<bool> ValidateUser(UserForLoginDto userForLogin)
    {
        _user = await _userManager.FindByEmailAsync(userForLogin.Email!.ToLower().Trim());

        var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForLogin.Password));
        if(!result)
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed, wrong email or password");
        
        if(_user != null && !_user.Verified)
        {
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed, User is not verified");
            return false;
        }
        return result;
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokeOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var jwtSecret = _configuration.GetSection("JwtSettings")["secret"];
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user!.Email),
            new Claim("Email", _user.Email),
            new Claim("FirstName", _user.FirstName),
            new Claim("LastName", _user.LastName),
            new Claim("PhoneNumber", _user.PhoneNumber)
        };

        var roles = await _userManager.GetRolesAsync(_user);
        var userRoles = "";
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            userRoles = $"{userRoles},roles";
        }
        claims.Add(new Claim("Roles", userRoles));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expiresIn"])),
            signingCredentials: signingCredentials);
        return tokenOptions;
    }
}