using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.ConfigurationModels;
using Domain.Entities;
using Domain.Exceptions;
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
    private readonly JwtConfiguration _jwtConfiguration;

    public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _jwtConfiguration = new JwtConfiguration();
        _configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);
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

    public async Task<TokenDto> CreateToken(bool populateExp)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();
        if(populateExp)
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken, _user.RefreshTokenExpiryTime);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using(var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        //var jwtSettins = _configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret)),
            ValidateLifetime = false,
            ValidAudience = _jwtConfiguration.ValidAudience,
            ValidIssuer = _jwtConfiguration.ValidIssuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        
        return principal;
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
        //var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.ExpiresIn)),
            signingCredentials: signingCredentials);
        return tokenOptions;
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.RefreshToken);
        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if(user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.Now)
            throw new RefreshTokenBadRequest();
        
        _user = user;
        return await CreateToken(populateExp: false);
    }
}