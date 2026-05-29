using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Commands.Register;

public record RegisterCommand(string Email, string FirstName, string LastName, string Password) : IRequest<AuthResponse>;
