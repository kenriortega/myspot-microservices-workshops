using Micro.Handlers;
using MySpot.Services.Reservations.Core.DomainServices;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class MakeReservationHandler : ICommandHandler<MakeReservation>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IWeeklyReservationsService _weeklyReservationsService;
    private readonly IDispatcher _dispatcher;

    public MakeReservationHandler(IWeeklyReservationsRepository weeklyReservationsRepository,
        IWeeklyReservationsService weeklyReservationsService, IDispatcher dispatcher)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
        _weeklyReservationsService = weeklyReservationsService;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(MakeReservation command, CancellationToken cancellationToken = default)
    {
        var (userId, parkingSpotId, capacity, licensePlate, date, note) = command;
        _ = new LicensePlate(licensePlate);
        _ = new Capacity(capacity);
        _ = new Date(date);

        await Task.CompletedTask;
    }
}