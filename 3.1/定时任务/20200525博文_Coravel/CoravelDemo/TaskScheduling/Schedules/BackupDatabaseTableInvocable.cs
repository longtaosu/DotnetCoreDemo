using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduling.Schedules
{
    public class BackupDatabaseTableInvocable : IInvocable
    {
        private ILogger _logger;
        private string _tableName;

        public BackupDatabaseTableInvocable(ILogger<BackupDatabaseTableInvocable> logger,string tableName)
        {
            this._logger = logger;
            this._tableName = tableName;
        }

        public Task Invoke()
        {
            Console.WriteLine($"任务执行   开始时间：{DateTime.Now}");
            Thread.Sleep(TimeSpan.FromSeconds(15));
            Console.WriteLine($"任务执行结束时间：{DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
