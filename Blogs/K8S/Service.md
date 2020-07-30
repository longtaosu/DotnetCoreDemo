# 声明

本文转载自：[ASP.NET Core on K8S深入学习（4）你必须知道的Service](https://mp.weixin.qq.com/s?__biz=MzA4NzQzNTg4Ng==&mid=2651729479&idx=1&sn=1349a06335a1a8d6f1aa5e95a16f81fd&chksm=8bc3f146bcb47850edcca95b980e8f356e781591512f5713f9cd867c4e75fc323d8ec50f5383&scene=158#rd)

# 概念

Service用于发现后端Pod服务，为一组具有相同功能的容器应用提供一个统一的入口地址。将请求进行负载分发到后端的各个容器应用上的控制器。

## Service类型

### ClusterIP

ClusterIP服务是Kubernetes的**默认服务**。它提供一个集群内的服务，集群内的其他应用都可以访问该服务，但是集群外部无法访问。

这种服务常用于内部程序互相的访问，且不需要外部访问，那么这个时候用ClusterIP就比较合适。

```ini
apiVersion: v1
kind: Service
metadata:
name: my-internal-service
selector:
app: my-app
spec:
type: ClusterIP
ports:
- name: http
port: 80
targetPort: 80
protocol: TCP
```



### NodePort

除了只在内部访问的服务，我们还有很多需要暴露出来公开访问的服务。在ClusterIP基础上为Service在每台机器上绑定一个端口，就可以通过 <NodeIP>:NodePort 来访问这些服务。

> 这种方式需要一个额外的端口来进行暴露，且端口范围只能是30000-32767，如果节点/VM的IP发生变化，需要考虑

```ini
apiVersion: v1
kind: Service
metadata:
name: my-nodeport-service
selector:
app: my-app
spec:
type: NodePort
ports:
- name: http
port: 80
targetPort: 80
nodePort: 30036
protocol: TCP
```



### LoadBalancer

LoadBalancer服务是暴露服务到internet的标准方式，它借助Cloud Provider创建一个外部的负载均衡器，并将请求转发到<NodeIP>:NodePort（向节点导流）。

> 每一个用 LoadBalancer 暴露的服务都会有它自己的IP地址，每个用到的 LoadBalancer都需要付费。

```ini
kind: Service
apiVersion: v1
metadata:
  name: my-service
spec:
  selector:
    app: MyApp
  ports:
  - protocol: TCP
    port: 80
    targetPort: 9376
  clusterIP: 10.0.171.239
  loadBalancerIP: 78.11.24.19
  type: LoadBalancer
status:
  loadBalancer:
    ingress:
    - ip: 146.148.47.155
```



## Service的创建与运行

准备一个名为 deployment.yaml 的文件。

```ini
apiVersion: apps/v1
kind: Deployment
metadata:
  name: edc-webapi-deployment
  namespace: aspnetcore
spec:
  replicas: 2
  selector:
    matchLabels:
      name: edc-webapi
  template:
    metadata:
      labels:
        name: edc-webapi
    spec:
      containers:
      - name: edc-webapi-container
        image: edisonsaonian/k8s-demo
        ports:
        - containerPort: 80
        imagePullPolicy: IfNotPresent
```

部署服务

```shell
kubectl apply -f edc-api.yaml
```

查看集群的IP

![02ClusterIP.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/02ClusterIP.png)

通过curl指令测试接口服务

![01Curl.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/01Curl.png)



## 创建Service

为上面创建的两个Pod创建一个Service

```ini
apiVersion: v1
kind: Service
metadata:
  name: edc-webapi-service
  namespace: aspnetcore
spec:
  ports:
    - port: 8080
      targetPort: 80
  selector:
    name: edc-webapi
```

部署Service

```shell
kubectl apply -f deployservice.yaml
```

这里需要注意的几个点：

1. port : 8080 => 指将Service的8080端口映射到Pod的对应端口上，这里Pod的对应端口由 targetPort 指定。
2. selector => 指将具有 name: edc-webapi 这个label的Pod作为我们这个Service的后端，为这些Pod提供统一IP和端口。

```shell
kubectl get service -n aspnetcore
curl 10.1.25.186:8080/api/values
```

查看Service Type

![04ClusterIPService.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/04ClusterIPService.png)

通过curl测试接口

![03CurlService.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/03CurlService.png)

在默认情况下，Service的类型是ClusterIP，只能提供集群内部的服务访问。如果想要为外部提供访问，那么需要改为NodePort。



### 使用NodePort

下面为Service增加NodePort访问方式：

```ini
apiVersion: v1
kind: Service
metadata:
  name: edc-webapi-service
  namespace: aspnetcore
spec:
  type: NodePort
  ports:
    - port: 8080
      targetPort: 80
  selector:
    name: edc-webapi
```

再次进行创建，会覆盖已有配置

```shell
kubectl apply -f deployservice.yml
```

验证部署方式

```shell
kubectl get service -n aspnetcore
```

![05NodePortService.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/05NodePortService.png)

这里的Port已经变为了 8080:30177，即将Service中的8080端口映射到Node节点的30177端口，我们可以访问Node节点的30177端口获取返回数据。

![06NodePort.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/06NodePort.png)

### 指定特定端口

刚刚的NodePort默认情况下是随机选择一个端口（30000-32767范围内），可以使用NodePort属性指定一个特定端口

```ini
apiVersion: v1
kind: Service
metadata:
  name: edc-webapi-service
  namespace: aspnetcore
spec:
  type: NodePort
  ports:
    - nodePort: 31000 
      port: 8080
      targetPort: 80
  selector:
    name: edc-webapi
```

这里我们指定一个外部访问端口：31000，通过kubectl覆盖后，我们再次验证

![07NodePort指定端口.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/07NodePort指定端口.png)

最后，再次总结下三个端口配置：

- nodePort => Node节点上监听的端口，也就是外部访问的Service端口
- port => ClusterIP上监听的端口，即内部访问的Service端口
- targetPort => Pod上监听的端口，即容器内部的端口



### DNS访问Service

Kubernetes默认安装了一个dns组件coredns，位于kube-system命名空间中

![08coredns.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/08coredns.png)

每当有新的Service被创建时，coredns会添加该Service的DNS记录，于是Cluster中的Pod便可以通过servicename.namespacename来访问Service了，从而实现服务发现的效果

通过临时创建一个busybox Pod来访问edc-webapi-service.aspnetcore:8080

```shell
kubectl run busybox --rm -ti --image=busybox /bin/sh
```

![09通过Name访问服务.png](https://gitee.com/imstrive/ImageBed/raw/master/20200730/09通过Name访问服务.png)





# 参考

https://kubernetes.io/zh/docs/concepts/services-networking/service/

https://mp.weixin.qq.com/s?__biz=MzA4NzQzNTg4Ng==&mid=2651729479&idx=1&sn=1349a06335a1a8d6f1aa5e95a16f81fd&chksm=8bc3f146bcb47850edcca95b980e8f356e781591512f5713f9cd867c4e75fc323d8ec50f5383&scene=158#rd