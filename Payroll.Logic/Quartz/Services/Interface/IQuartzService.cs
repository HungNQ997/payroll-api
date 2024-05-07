using Payroll.Logic.Quartz.JobModel;
using Quartz;

namespace Payroll.Logic.Quartz.Services.Interface
{
    public interface IQuartzService
    {
        Task CreateOrUpdateFor<T>(JobCreateOrUpdateRequest request) where T : IJob;
        Task ReCreateOrUpdateFor<T>(JobCreateOrUpdateRequest request) where T : IJob;
        public Task<bool> Delete(string name, string group);

        public Task Pause(string name, string group);

        public Task Resume(string name, string group);

        public Task Trigger(string name, string group);

        public Task<List<string>> Log(string name, string group);
    }
}
