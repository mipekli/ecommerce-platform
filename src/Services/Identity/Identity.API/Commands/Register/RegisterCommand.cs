using MediatR;
using Identity.API.DTOs;

namespace Identity.API.Commands.Register;

public record RegisterCommand(string Email, string FirstName, string LastName, string Password) : IRequest<AuthResponse>;
