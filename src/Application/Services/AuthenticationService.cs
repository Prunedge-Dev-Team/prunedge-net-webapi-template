using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

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
}