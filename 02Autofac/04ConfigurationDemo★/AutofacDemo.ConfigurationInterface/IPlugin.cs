using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.ConfigurationInterface
{
    /// <summary>
    /// 用于通过配置实现组件加载、插件处理的插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 获取插件的名字
        /// </summary>
        string Name { get; }
    }
}
