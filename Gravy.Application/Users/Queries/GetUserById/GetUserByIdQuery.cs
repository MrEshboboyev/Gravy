using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;