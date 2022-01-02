using Application.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace Application.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
}