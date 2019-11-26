using Autofac.Multitenant;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.MultitenantDemo
{
    public class ManualTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        /// <summary>
        /// 获取/设置 当前租户的ID，默认值为0
        /// </summary>
        public object CurrentTenantId { get; set; }

        public bool TryIdentifyTenant(out object tenantId)
        {
            if(this.CurrentTenantId.ToString() == "0")
            {
                tenantId = null;
            }
            else
            {
                tenantId = this.CurrentTenantId;
            }
            return true;
        }
    }
}
