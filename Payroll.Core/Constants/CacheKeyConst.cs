namespace Payroll.Core.Constants
{
    public static class CacheKeyConst
    {
        public const string Quartz_MachineName = "Quartz-MachineName";
        public const string Quartz_Scheduler = "QuartzScheduler";
    }

    public static class CacheSchedulerJobConst
    {
        #region Seperation Job
        public const string SchedulerJob_GetCanceledAndDeleteInvoice = "SchedulerJob_GetCanceledAndDeleteInvoice";
        public const int ExpiredTime = 60;
        #endregion
    }
}
