using Micro.Abstractions;
using MySpot.Services.Availability.Application.DTO;

namespace MySpot.Services.Availability.Application.Queries;

public class GetResource : IQuery<ResourceDto>
{
    public Guid ResourceId { get; set; }
}