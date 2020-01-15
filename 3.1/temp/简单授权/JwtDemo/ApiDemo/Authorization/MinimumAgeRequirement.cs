using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }

        public MinimumAgeRequirement(int age) { Age = age; }
    }
}
