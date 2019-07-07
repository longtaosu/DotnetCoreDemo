# 说明

包含3个项目：ConfigurationDemo、ConfigurationInterface、ConfigurationPlugin



**ConfigurationInterface**提供插件接口 **IPlugin**

**ConfigurationPlugin**提供了对 **IPlugin** 的一种实现：**ExternalPlugin**

**ConfigurationDemo** 提供了对 **IPlugin** 的默认实现：**InternalPlugin**



**ConfigurationDemo** 通过**autofac.json**文件实现对外部dll的加载（需要将**ConfigurationPlugin.dll**放在bin目录下），根据配置文件完成加载以及依赖注入