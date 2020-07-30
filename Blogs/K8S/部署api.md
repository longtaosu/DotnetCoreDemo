# Yaml

创建namespaces

```
kubectl create namespace [命名空间]
```

编写服务部署的Yaml文件，可以起名为deploy.yaml

```json
apiVersion: apps/v1
kind: Deployment
metadata:
  name: aspnetcore-demo1
  namespace: aspnetcore
  labels:
    name: aspnetcore-demo1
spec:
  replicas: 2
  selector:
    matchLabels:
      name: aspnetcore-demo1
  template:
    metadata:
      labels:
        name: aspnetcore-demo1
    spec:
      containers:
      - name: k8s-demo
        image: edisonsaonian/k8s-demo
        ports:
        - containerPort: 80
        imagePullPolicy: Always

---

kind: Service
apiVersion: v1
metadata:
  name: aspnetcore-demo1
  namespace: aspnetcore
spec:
  type: NodePort
  ports:
    - port: 80
      targetPort: 80
  selector:
    name: aspnetcore-demo1
```

这里这个deploy.yaml就会告诉K8S关于你的API的所有信息，以及通过什么样的方式暴露出来让外部访问。

需要注意的是，这里我们提前为要部署的ASP.NET Core WebAPI项目创建了一个namespace，叫做aspnetcore，因此这里写的namespace : aspnetcore。

K8S中通过标签来区分不同的服务，因此这里统一name写成了aspnetcore-demo1。

在多实例的配置上，通过replicas : 2这个设置告诉K8S给我启动2个实例起来，当然你可以写更大的一个数量值。

最后，在spec中告诉K8S我要通过NodePort的方式暴露出来公开访问，因此端口范围从上一篇可以知道，应该是 30000-32767这个范围之内。

# Kubectl部署

在部署的Yaml文件编辑好之后

```shell
kubectl create -f deploy.yaml
```

提示如下：

![01创建namespace并部署service.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/01创建namespace并部署service.png)

看到上面的提示"service created"，就可以知道已经创建好了，这里我们再通过下面这个命令来验证一下：

```
kubectl get svc -n aspnetcore
```

 可以看到，在命名空间aspnetcore下，就有了一个aspnetcore-demo的服务运行起来了，并通过端口号30751向外部提供访问。

![02查看部署的服务.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/02查看部署的服务.png)

 

# 验证WebApi

首先，我们可以通过浏览器来访问一下这个API接口，看看是否能正常访问到。

- **/api/values**

![03接口测试.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/03接口测试.png)

- **/api/values/1000**

![04接口测试.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/04接口测试.png)

 

# Dashboard

## 状态查看

通过Dashboard查看状态（需要切换命名空间）

![05DashBoard.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/05DashBoard.png)

 

## Dashboard动态伸缩

![06动态伸缩.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/06动态伸缩.png)

  将弹窗中需要的容器数由2改为1

![08伸缩.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/08伸缩.png)

 查看状态

![07伸缩后.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/07伸缩后.png)

 

## Kubectl动态伸缩

除了在Dashboard中可视化地操作进行伸缩，也可以通过kubectl来进行，例如下面这句命令，将容器实例扩展到3个。需要注意的是，由于我们的k8s-demo1所在的命名空间是在aspnetcore下，因此也需要指明--namespace=aspnetcore。

```
kubectl scale deployment aspnetcore-demo1 --replicas=2 --namespace=aspnetcore
```

![09kubectl进行伸缩.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/09kubectl进行伸缩.png)

再次查看dashboard

![10伸缩后.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/10伸缩后.png)

 

## 自动伸缩

在K8S中，提供了一个autoscale接口来实现服务的自动伸缩，它会采用默认的自动伸缩策略（例如根据CPU的负载情况）来帮助我们实现弹性伸缩的功能。例如下面这句命令可以实现我们的k8s-demo可以伸缩的范围是1~3个，根据负载情况自己伸缩，在没有多少请求量压力很小时收缩为一个，在压力较大时启动另一个实例来降低负载。

```
kubectl autoscale deployment aspnetcore-demo1 --min=1 --max=3 --namespace=aspnetcore
```

 

# 参考

https://www.cnblogs.com/imstrive/p/11479726.html

https://mp.weixin.qq.com/s?__biz=MzA4NzQzNTg4Ng==&mid=2651729355&idx=1&sn=c737a6831d69ec8bc86c13ba2a4be45e&chksm=8bc3f2cabcb47bdc4f710f0b2452c6fa336dfc0ebb58a8d2666e469e5203ffbb98336f457f80&scene=21#wechat_redirect