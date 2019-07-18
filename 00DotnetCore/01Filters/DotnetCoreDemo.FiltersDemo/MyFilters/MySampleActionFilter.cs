using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetCoreDemo.FiltersDemo.MyFilters
{
    public class MySampleActionFilter : IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
