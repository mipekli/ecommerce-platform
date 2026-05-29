using MediatR;
using Identity.API.DTOs;

namespace Identity.API.Queries.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<UserDto>;
