using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Queries.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<UserDto>;
