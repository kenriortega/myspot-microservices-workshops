namespace Micro.Contexts;

public interface IContext
{
    string TraceId { get; }
    string CorrelationId { get; }
    string? MessageId { get; }
    string? CausationId { get; }
    string? ActivityId { get; }
    string? UserId { get; }
    string? Span { get; }
}