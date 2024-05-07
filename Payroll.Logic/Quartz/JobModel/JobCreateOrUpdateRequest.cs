namespace Payroll.Logic.Quartz.JobModel
{
    public class JobCreateOrUpdateRequest
    {

        public string Name { get; set; }

        public string Group { get; set; }

        public int HttpMethod { get; set; }

        public string RequestUrl { get; set; }

        public int TriggerType { get; set; }

        public int RepeatCount { get; set; }
        public int Interval { get; set; }

        public int IntervalType { get; set; }

        public string Cron { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string RequestBody { get; set; }

        public string Description { get; set; }

        public bool IsUpdate { get; set; }

        public int Id { get; set; }
        public string ContractChild { get; set; }
        public int NumberRemind { get; set; }
    }
}
