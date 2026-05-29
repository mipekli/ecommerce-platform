using MediatR;
using Identity.API.DTOs;

namespace Identity.API.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
