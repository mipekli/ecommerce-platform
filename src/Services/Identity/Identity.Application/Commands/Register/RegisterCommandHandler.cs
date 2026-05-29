using MediatR;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;

namespace Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Bu e-posta adresi ile kayıtlı bir kullanıcı zaten var.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Email, request.FirstName, request.LastName, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var (token, expiresAt) = _jwtService.GenerateTokenPair(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, expiresAt);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }
}
