# workflow-core

https://github.com/danielgerlag/workflow-core

<https://workflow-core.readthedocs.io/en/latest/>



# Smartflow-Sharp（含Demo）

https://github.com/chengderen/Smartflow-Sharp



# elsa-core

https://github.com/elsa-workflows/elsa-core



# microwf

https://github.com/thomasduft/microwf







```
dotnet skyapm config sample_app 192.168.11.167:44371
```

# skyapm配置

https://github.com/apache/skywalking/blob/5.x/docs/README_ZH.md

https://www.cnblogs.com/savorboard/p/asp-net-core-skywalking.html

https://www.cnblogs.com/weihanli/p/deploy-skywalking-via-docker.html

https://www.cnblogs.com/weiBlog/p/10427454.html

https://blog.csdn.net/sD7O95O/article/details/100135342

http://www.vnfan.com/wower/d/14fc511be066a860.html

dotnet sampleapp.dll --urls http://*:5000





sudo docker run --name skywalking -d -p 1234:1234 -p 11800:11800 -p 12800:12800 --restart always apache/skywalking-oap-server



sudo docker run --name skywalking-ui -d -p 8080:8080 --link skywalking:skywalking -e SW_OAP_ADDRESS=skywalking:12800 --restart always apache/skywalking-ui







https://www.cnblogs.com/SteveLee/p/10463200.html