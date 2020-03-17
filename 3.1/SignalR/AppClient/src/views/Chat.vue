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

    <!-- <el-button>默认按钮</el-button> -->

    <!-- <el-button type="success">成功按钮</el-button> -->
    <!-- <el-button type="info">信息按钮</el-button> -->
    <!-- <el-button type="text">文字按钮</el-button> -->
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
                if(this.hubConnection.connectionId){
                    //this.hubConnection.invoke("SendMessage",this.name,this.message).catch(err=>console.log(err.toString()));
                }
            });
        
    }  
                
    send(){
        console.log("发送信息")
        this.hubConnection.send("SendMessage",this.name,this.message);        
    }
}
</script>
// https://www.cnblogs.com/laozhang-is-phi/p/netcore-vue-signalr.html