using System.Diagnostics;

namespace Pipelink.Insights.Helpers;

public static class ActivityHelper
{
    private static readonly ActivitySource Source = new("Pipelink.Insights", "1.0.0");

    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return Source.StartActivity(name, kind);
    }

    public static Activity? StartActivity(string name, ActivityKind kind, ActivityContext parentContext)
    {
        return Source.StartActivity(name, kind, parentContext);
    }
} 