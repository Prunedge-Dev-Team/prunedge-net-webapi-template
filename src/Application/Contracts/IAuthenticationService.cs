using Application.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace Application.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
    Task<bool> ValidateUser(UserForLoginDto userForLogin);
    Task<TokenDto> CreateToken(bool populateExp);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}