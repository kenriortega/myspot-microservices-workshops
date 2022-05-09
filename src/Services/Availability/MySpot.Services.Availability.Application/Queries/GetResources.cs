using Micro.Abstractions;
using MySpot.Services.Availability.Application.DTO;

namespace MySpot.Services.Availability.Application.Queries;

public class GetResources : IQuery<IEnumerable<ResourceDto>>
{
    public IEnumerable<string>? Tags { get; set; }
    public bool MatchAllTags { get; set; }
}