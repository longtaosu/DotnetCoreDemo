# 参考

https://github.com/autofac/Examples

# 注意

1. 可以直接从 Container 解析组件，但是这种方法并不推荐

   ```c#
   container.Resolve<IDateWriter>().WriteDate()
   ```

   

2. 从容器中创建一个 子生命周期 并从中解析，当完成了解析组件，释放掉生命周期，其他所有也就随之清理
   服务定位器模式是一种反模式，在代码中四处人为地创建生命周期而少量的使用容器并不是最佳的方式

   ```c#
   using (var scope = container.BeginLifetimeScope())
   {
       var writer = scope.Resolve<IDateWriter>();
       writer.WriteDate();
   }
   ```

