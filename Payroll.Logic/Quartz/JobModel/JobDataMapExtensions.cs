using Payroll.Logic.Quartz.Keys;
using Quartz;

namespace Payroll.Logic.Quartz.JobModel
{
    public static class JobDataMapExtensions
    {
        public static List<string> GetLogList(this JobDataMap map)
        {
            return (map[DataKeys.LogList] as List<string>) ?? new List<string>();
        }
    }
}
