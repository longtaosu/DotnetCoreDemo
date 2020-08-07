using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopShelfDemo
{
    public class JobFactory : IJobFactory
    {
        private IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
