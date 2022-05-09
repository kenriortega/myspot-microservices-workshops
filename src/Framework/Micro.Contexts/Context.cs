namespace Micro.Contexts;

public sealed class Context : IContext
{
    public string TraceId { get; }
    public string CorrelationId { get; }
    public string? MessageId { get; }
    public string? CausationId { get; }
    public string? ActivityId { get; }
    public string? UserId { get; }
    public string? Span { get; }

    public Context()
    {
        TraceId = string.Empty;
        CorrelationId = Guid.NewGuid().ToString("N");
    }

    public Context(string traceId, string correlationId, string? messageId = null, string? causationId = null,
        string? activityId = null, string? userId = null, string? span = null)
    {
        TraceId = traceId;
        CorrelationId = correlationId;
        MessageId = messageId;
        CausationId = causationId;
        ActivityId = activityId;
        UserId = userId;
        Span = span;
    }
}