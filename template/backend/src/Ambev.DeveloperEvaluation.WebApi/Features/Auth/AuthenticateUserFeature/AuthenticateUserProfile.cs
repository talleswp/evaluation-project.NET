using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

/// <summary>
/// AutoMapper profile for authentication-related mappings
/// </summary>
public sealed class AuthenticateUserProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateUserProfile"/> class
    /// </summary>
    public AuthenticateUserProfile()
    {
        CreateMap<User, AuthenticateUserResponse>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // Add the missing mapping
        CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();

        // Add the missing mapping
        CreateMap<Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser.AuthenticateUserResult, AuthenticateUserResponse>()
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }
}
