namespace Payroll.Logic.Quartz.Keys
{
    internal sealed class DataKeys
    {
        public static readonly string NameKey = "NameKey";
        public static readonly string GroupKey = "GroupKey";
        public static readonly string TriggerType = "TriggerType";
        public static readonly string RepeatCount = "RepeatCount";
        public static readonly string Interval = "Interval";
        public static readonly string IntervalType = "IntervalType";
        public static readonly string Cron = "Cron";
        public static readonly string RequestBody = "RequestBody";
        public static readonly string CreateTime = "CreateTime";
        public static readonly string StartTime = "StartTime";
        public static readonly string EndTime = "EndTime";

        public static readonly string LastException = "LastException";
        public static readonly string LogList = "LogList";
    }

    public class JobGroupKey
    {
        #region Separation
        public const string SampleJob = "SampleJob";
        public const string InsertSyncInvoiceSchedulerJob = "InsertSyncInvoiceSchedulerJob";
        #endregion

    }
}
