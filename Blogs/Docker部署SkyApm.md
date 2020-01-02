# 注意事项

主要保证8080、10800、11800、12800端口未被占用

浏览器最好选择chrome测试，如果没有数据，可选择清楚缓存后再测试

# 安装Docker-compose

```
sudo curl -L "https://github.com/docker/compose/releases/download/1.23.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
```

执行命令

```
sudo chmod +x /usr/local/bin/docker-compose
```

测试安装是否成功

```
docker-compose version
```

# 编写Docker-compose文件

```
version: '2'
services:
  elasticsearch:
    image: elasticsearch:6.8.0
    container_name: skywalking-es
    restart: always
    ports:
      - 9200:9200
      - 9300:9300
    environment:
      discovery.type: single-node
      TZ: Asia/Shanghai
  oap:
    image: apache/skywalking-oap-server:6.5.0
    container_name: skywalking-oap
    depends_on:
      - elasticsearch
    links:
      - elasticsearch
    restart: always
    #前边为外网端口号,后边为容器应用端口号
    ports:
      - 11800:11800
      - 12800:12800
    environment:
      # 设置时区
      TZ: Asia/Shanghai
  ui:
    image: apache/skywalking-ui:6.5.0
    container_name: skywalking-ui
    depends_on:
      - oap
    links:
      - oap
    restart: always
    ports:
      - 8080:8080
    #设置环境,配置覆盖yml的配置
    environment:
      collector.ribbon.listOfServers: oap:12800
      security.user.admin.password: 123456
```

# 安装SkyWalking

使用之前的docker-compose文件安装SkyWalking

```
docker-compose up -d
```

# 安装Dotnet

```
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
```

安装.Net Core SDK

```
sudo yum install dotnet-sdk-3.0
```

安装.Net Core runtime

```
sudo yum install dotnet-runtime-3.0
```

安装Asp.net core runtime

```
sudo yum install aspnetcore-runtime-3.0
```

# 生成程序并配置环境变量

```
dotnet new mvc -n sampleapp
cd sampleapp
dotnet add package SkyAPM.Agent.AspNetCore
export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=SkyAPM.Agent.AspNetCore
export SKYWALKING__SERVICENAME=sample_app

dotnet run --urls="http://*:5000"
```

# 配置SkyApm

安装Cli

```
dotnet tool install -g SkyAPM.DotNet.CLI
```

生成配置文件

```
dotnet skyapm config sample_app 127.0.0.1:11800
```

# 效果图

仪表盘

![仪表盘.png](https://gitee.com/imstrive/ImageBed/raw/master/20191205/%E4%BB%AA%E8%A1%A8%E7%9B%98.png)

拓扑图

![拓扑图.png](https://gitee.com/imstrive/ImageBed/raw/master/20191205/%E6%8B%93%E6%89%91%E5%9B%BE.png)