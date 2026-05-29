using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
