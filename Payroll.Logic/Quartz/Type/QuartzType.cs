using System.ComponentModel;

namespace Payroll.Logic.Quartz.Type
{
    public enum HttpMethod
    {
        [Description("GET")]
        Get = 1,
        [Description("POST")]
        Post = 2
    }

    public enum TriggerType
    {
        [Description("Simple")]
        Simple = 1,
        [Description("Cron")]
        Cron = 2
    }

    public enum IntervalType
    {
        [Description("Second")]
        Second = 1,
        [Description("Minute")]
        Minute = 2,
        [Description("")]
        Hour = 3,
        [Description("Day")]
        Day = 4
    }
}
