using Micro.Exceptions;

namespace MySpot.Services.Reservations.Application.Exceptions;

internal sealed class UserNotFoundException : CustomException
{
    public Guid UserId { get; }

    public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
    {
        UserId = userId;
    }
}