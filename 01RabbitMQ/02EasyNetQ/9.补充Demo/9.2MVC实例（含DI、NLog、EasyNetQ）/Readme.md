# 1.Autofac

使用Autofac替换自带的DI，修改 **ConfigureServices** 的返回类型为 **IServiceProvider** 

注册服务：

```c#

    //注册消息队列的服务
    var builder = new ContainerBuilder();
    builder.RegisterType<MQService>().As<IMQService>().SingleInstance();
    builder.Populate(services);

    this.ApplicationContainer = builder.Build();

    //初始化服务
    this.ApplicationContainer.Resolve<IMQService>().InitMQ();
```

需要注意消息队列服务需要声明为单例模式，并需要调用 **InitMQ()** 进行初始化



# 2.日志工具NLog

添加依赖 NLog以及 NLog.Config

修改NLog.config内容为：

```bash
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">

  <variable name="myvar" value="myvalue"/>

  <!--
  See https://github.com/nlog/nlog/wiki/Configuration-file
  for information on customizing logging rules and outputs.
   -->
  <targets>
    <target xsi:type="NLogViewer"
            name="DebugInfo" layout="
            ${newline}时间：${longdate}
            ${newline}信息：${message}"/>
    <target xsi:type="File"
            name="ErrorInfo"
            fileName="${basedir}/Logs/${date:format=yyyyMMdd}_ErrorInfo.txt"
            layout="
              ${newline}时间： ${longdate}
              ${newline}来源： ${callsite}
              ${newline}等级： ${level}
              ${newline}堆栈： ${event-context:item=exception} ${stacktrace}
              ${newline}错误： ${exception:format=tostring}
              ${newline}信息： ${message}
              ${newline}
              ${newline}-----------------------------------------------------------"/>
    <target xsi:type="File"
            name="Info"
            fileName="${basedir}/Logs/${date:format=yyyyMMdd}_Info.txt"
            layout="
              ${newline}时间： ${longdate}
              ${newline}信息： ${message}
              ${newline}-----------------------------------------------------------"/>
  </targets>

  <rules>
    <logger name="*" level="Error" writeTo="ErrorInfo"></logger>
    <logger name="*" level="Info"  writeTo="Info"></logger>
    <logger name="*" minlevel="Trace" writeTo="DebugInfo"></logger>
  </rules>
</nlog>

```

添加 **NLogHelper** 辅助工具类：

```c#
    public class NLogHelper
    {
        private static Logger logHelper;
        static NLogHelper()
        {
            logHelper = LogManager.GetCurrentClassLogger();
        }
        #region Debug
        public static void Debug(string msg)
        {
            logHelper.Debug(msg);
        }

        public static void Debug(string msg, Exception err)
        {
            logHelper.Debug(err, msg);
        }
        #endregion

        #region Info
        public static void Info(string msg)
        {
            logHelper.Info(msg);
        }

        public static void Info(string msg, Exception err)
        {
            logHelper.Info(err, msg);
        }
        #endregion

        #region Warn
        public static void Warn(string msg)
        {
            logHelper.Warn(msg);
        }

        public static void Warn(string msg, Exception err)
        {
            logHelper.Warn(err, msg);
        }
        #endregion

        #region Trace
        public static void Trace(string msg)
        {
            logHelper.Trace(msg);
        }

        public static void Trace(string msg, Exception err)
        {
            logHelper.Trace(err, msg);
        }
        #endregion

        #region Error
        public static void Error(string msg)
        {
            logHelper.Error(msg);
        }

        public static void Error(string msg, Exception err)
        {
            logHelper.Error(err, msg);
        }
        #endregion

        #region Fatal
        public static void Fatal(string msg)
        {
            logHelper.Fatal(msg);
        }

        public static void Fatal(string msg, Exception err)
        {
            logHelper.Fatal(err, msg);
        }
        #endregion
    }
```



# 3.消息队列处理

添加依赖 **EasyNetQ**

## 3.1添加消息队列操作的接口：

```c#
    public interface IMQService
    {
        /// <summary>
        /// 
        /// </summary>
        void InitMQ();
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void PublishMessage<T>(T message)
            where T : class;
        /// <summary>
        /// 订阅消息
        /// </summary>
        void SubscribeMessage();
    }
```

## 3.2添加消息队列接口的实现：

```c#
    public class MQService : IMQService
    {
        #region 构造函数
        IBus bus;
        public MQService()
        {

        }
        #endregion

        #region MQ的发布、订阅及初始化
        public void InitMQ()
        {
            //声明队列并指定异常处理程序
            bus = RabbitHutch.CreateBus("host=localhost", x => x.Register<IConsumerErrorStrategy>(_ => new AlwaysRequeueErrorStrategy()));

            //订阅消息
            SubscribeMessage();
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void PublishMessage<T>(T message)
            where T : class
        {
            bus.Publish<T>(message);
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        public void SubscribeMessage()
        {
            //订阅 Question 类型的消息
            bus.SubscribeAsync<Question>("subscribe_question", x => HandleMessageAsync(x).Invoke(1));
        } 
        #endregion

        #region 业务程序
        private Func<int, Task> HandleMessageAsync(Question question)
        {
            return async (id) =>
            {
                if (new Random().Next(0, 2) == 0)
                {
                    Console.WriteLine("Exception Happened!!!!");
                    NLogHelper.Info("Exception Happened!!!!" + "   " + question.Text);
                    throw new Exception("Error Hanppened!" + "   " + question.Text);
                }
                else
                {
                    NLogHelper.Info("BEGIN");
                    Thread.Sleep(10000);
                    Console.WriteLine(string.Format("worker：{0}，content：{1}", id, question.Text));
                    NLogHelper.Info(string.Format("worker：{0}，content：{1}", id, question.Text));
                }
            };
        } 
        #endregion
    }
```

程序中对接口中的消息发布和订阅进行了实现，中间的业务处理中，写明了针对 类型为 Question 的消息的处理方法。

## 3.3添加消息队列异常处理

该方法同9.1的Demo，只需要添加 **AlwaysRequeueErrorStrategy** 即可。



# 4.测试

打开浏览器，输入地址：<https://localhost:44330/TestMQ/PublishMessage>

![微信截图_20190624160307](C:\Users\Gdky\Desktop\DotnetCoreDemo\01RabbitMQ\02EasyNetQ\9.补充Demo\9.2MVC实例（含DI、NLog、EasyNetQ）\Images\微信截图_20190624160307.png)

页面返回了操作时间，消息进行了两次重试并在第三次成功，成功的这一次执行了耗时10s的操作。



# 5.补充

1. 经过测试，消息异常时会重试
2. 消息处理过程中中断，重新启动程序后会重新执行