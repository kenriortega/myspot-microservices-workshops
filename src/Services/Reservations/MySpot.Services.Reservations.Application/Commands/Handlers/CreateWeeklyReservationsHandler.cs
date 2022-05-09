using Micro.Handlers;
using Micro.Time;
using MySpot.Services.Reservations.Application.Exceptions;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class CreateWeeklyReservationsHandler : ICommandHandler<CreateWeeklyReservations>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;

    public CreateWeeklyReservationsHandler(IWeeklyReservationsRepository weeklyReservationsRepository,
        IUserRepository userRepository, IClock clock)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
        _userRepository = userRepository;
        _clock = clock;
    }

    public async Task HandleAsync(CreateWeeklyReservations command, CancellationToken cancellationToken = default)
    {
        var userId = command.UserId;
        var week = new Week(_clock.Current());
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        var weeklyReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (weeklyReservations is not null)
        {
            return;
        }
        
        weeklyReservations = new WeeklyReservations(AggregateId.Create(), user, week);
        await _weeklyReservationsRepository.AddAsync(weeklyReservations, cancellationToken);
    }
}