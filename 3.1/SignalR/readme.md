# 后端

新建Webapi项目

创建 `ChatHub` 类，继承于 `Hub` 

```c#
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
```

启用ChatHub，将ChartHub映射到路径 `/chathub` 下（需要前后端约定）

```c#
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
});
```

后端启用CORS

> 8080是前端的端口，前后端分离需要设置跨域，否则无法正常访问

```c#
//ConfigureServices
services.AddSignalR();

//Configure
app.UseCors(builder => {
    builder.WithOrigins("http://localhost:8080")
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader();
});
```

通过刚才的代码，我们将 ChatHub 下的方法映射到路径 /chathub 下



# 前端

前端引入 `@microsoft/signalr`，使用 npm install进行安装

```json
  "dependencies": {
    "@microsoft/signalr": "3.1.2",
    "core-js": "^3.6.4",
    "element-ui": "^2.13.0",
    "vue": "^2.6.11",
    "vue-class-component": "^7.2.2",
    "vue-property-decorator": "^8.3.0",
    "vue-router": "^3.1.5",
    "vuex": "^3.1.2"
  },
```

具体Element-ui以及路由不再赘述，可以看之前的文章。

新建Chat.vue，设置路由，具体代码如下

```html
<template>
  <div class="hello">
    <!-- <img alt="Vue logo" src="../assets/logo.png">     -->
    <el-input type="textarea" :rows="10" v-model="textarea1" style="width:45%">
    </el-input>
    <div>
        <el-input v-model="name" placeholder="姓名" style="width:200px"></el-input>
        <el-input v-model="message" placeholder="信息" style="width:200px"></el-input>
            <el-button type="primary" @click="send">发送</el-button>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import * as signalR from '@microsoft/signalr';

@Component
export default class Chat extends Vue {
  @Prop() private msg!: string;
    // var divMessage: HTMLDivElement = document.querySelector(".hello");
    private textarea1 = '1231';
    private name= '';
    private message= '';
    private hubConnection: signalR.HubConnection  = new signalR.HubConnectionBuilder()
                        .withUrl("https://localhost:44342/chatHub")
                        .withAutomaticReconnect()
                        //.configureLogging(signalR.LogLevel.Information)
                        .build();


    mounted() {
        this.start();

        this.hubConnection.on("ReceiveMessage",(name,message)=>{
            this.textarea1 = this.textarea1.concat( '\r\n' + name + '：' 
            + message);

        });
    }
               
    start(): void {
        this.hubConnection.start()
            .then(a=>{
                if(this.hubConnection.connectionId){                    //this.hubConnection.invoke("SendMessage",this.name,this.message).catch(err=>console.log(err.toString()));
                }
            });
        
    }  
                
    send(){
        this.hubConnection.send("SendMessage",this.name,this.message);        
    }
}
</script>
```

前端显示如下

![01前端视图.png](https://gitee.com/imstrive/ImageBed/raw/master/20200316/01前端视图.png)

> import * as signalR from '@microsoft/signalr';

将SignalR引入到当前页，定义hubConnection继承于signalR.HubConnection。

通过withUrl("https://localhost:44342/chatHub")将前端跟后端提供的接口绑定（后端启动端口是44342）。

```javascript
this.hubConnection.on("ReceiveMessage",(name,message)=>{
    this.textarea1 = this.textarea1.concat( '\r\n' + name + '：' 
    + message);
```

前端监听 `ReceiveMessage` 方法，该方法需要和后端提供一样的签名，作用是将接收到的name和message属性打印到页面上。

定义Send方法，将在点击按钮的时候出发Click事件，将信息发送

```javascript
    send(){
        this.hubConnection.send("SendMessage",this.name,this.message);        
    }
```

具体效果如下

![聊天动图.gif](https://gitee.com/imstrive/ImageBed/raw/master/20200316/聊天动图.gif)


# 参考

[配合使用 ASP.NET Core SignalR 和 TypeScript 以及 Webpack](https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/signalr-typescript-webpack?view=aspnetcore-3.1&tabs=visual-studio)

[NetCore + SignalR 实现日志消息推送](https://www.cnblogs.com/laozhang-is-phi/p/netcore-vue-signalr.html)