using LTS.Services.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTS.Services.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IOptions<SqlSugarSet> _sqlSugarSet;
        private readonly ISqlSugarClient _dbContext;
        public GroupMemberService(IOptions<SqlSugarSet> sqlSugarSet,ISqlSugarClient dbContext)
        {
            _sqlSugarSet = sqlSugarSet;
            _dbContext = dbContext;
        }
        public List<GroupMember> GetUsers()
        {
            //SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            //{
            //    ConnectionString = _sqlSugarSet.Value.ConnectionString,
            //     IsAutoCloseConnection = true,
            //      DbType = DbType.MySql
            //});
            return _dbContext.Queryable<GroupMember>().ToList();
        }
    }


}
