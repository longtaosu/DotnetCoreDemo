using System;
using System.Collections.Generic;
using System.Text;

namespace TopShelfDemo
{
    public class AppSettings
    {
        public List<CronJob> Jobs { get; set; }

        public DBOption DBConfig { get; set; }

        public Api Api { get; set; }
    }

    /// <summary>
    /// 定时任务
    /// </summary>
    public class CronJob
    {
        /// <summary>
        /// Job的名称（类名）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Job状态，1：可用，0：不可用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Job执行的规则
        /// </summary>
        public string Cron { get; set; }
    }

    /// <summary>
    /// 客服数据库配置
    /// </summary>
    public class DBOption
    {
        public int DbType { get; set; }
        public string Coon { get; set; }
    }

    public class Api
    {
        public string ServiceUrl { get; set; }

        public string api_url { get; set; }
    }
}
