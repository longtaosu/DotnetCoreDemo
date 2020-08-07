using Chloe;
using System;
using System.Collections.Generic;
using System.Text;
using TopShelfDemo.Entities;

namespace TopShelfDemo.Services
{
    public interface ITestService
    {
        List<WorkBill> DataAccess();
    }

    public class TestService : ITestService
    {
        private IDbContext _dbContext;
        public TestService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<WorkBill> DataAccess()
        {
            var data = _dbContext.Query<WorkBill>().ToList();

            return data;
        }
    }
}
