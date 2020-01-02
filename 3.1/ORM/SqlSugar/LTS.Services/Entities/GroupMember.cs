using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTS.Services.Entities
{
    [SugarTable("sys_workgroupmember")]
    public class GroupMember
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int UserID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Tel { get; set; }
        public int GroupID { get; set; }
        public int Sort { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
        public int IsEnabled { get; set; }
    }
}
