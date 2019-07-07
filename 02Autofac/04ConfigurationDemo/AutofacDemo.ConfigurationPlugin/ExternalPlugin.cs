using AutofacDemo.ConfigurationInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.ConfigurationPlugin
{
    /// <summary>
    /// 插件接口的实现，可以从外部加载组件
    /// </summary>
    public class ExternalPlugin : IPlugin
    {
        public string Name
        {
            get { return "ExternalPlugin"; }
        }
    }
}
