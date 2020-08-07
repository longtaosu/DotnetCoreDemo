# Yaml安装

下载Yaml文件

```shell
wget https://raw.githubusercontent.com/kubernetes/dashboard/v1.10.1/src/deploy/recommended/kubernetes-dashboard.yaml
```

通过vim编辑Yaml文件中的默认的镜像源地址（此处替换为李振良老师的地址）

```ini
containers:
      - name: kubernetes-dashboard
        #image: k8s.gcr.io/kubernetes-dashboard-amd64:v1.10.1
        image: lizhenliang/kubernetes-dashboard-amd64:v1.10.1
```

由于默认Dashboard只能集群内访问，因此修改Service为NodePort类型，暴露到外部可以访问

```ini
kind: Service
apiVersion: v1
metadata:
  labels:
    k8s-app: kubernetes-dashboard
  name: kubernetes-dashboard
  namespace: kube-system
spec:
  type: NodePort
  ports:
    - port: 443
      targetPort: 8443
      nodePort: 30000
  selector:
    k8s-app: kubernetes-dashboard
```

![01NodePort.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/01NodePort.png)

使用此Yaml文件创建Dashboard

```shell
kubectl apply -f kubernetes-dashboard.yaml
```

看到提示“service/kubernetes-dashboard created”代表Dashboard创建成功了，这时我们通过浏览器来访问一下：

![03输入令牌.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/03输入令牌.png)

看到了登录界面，需要我们配置kubeconfig或输入token，这里我们选择后者，通过以下命令获取输出的token：

```shell
kubectl create serviceaccount dashboard-admin -n kube-system
kubectl create clusterrolebinding dashboard-admin --clusterrole=cluster-admin --serviceaccount=kube-system:dashboard-admin
kubectl describe secrets -n kube-system $(kubectl -n kube-system get secret | awk '/dashboard-admin/{print $1}')
```

执行上面的命令后，会得到Token

![02Token.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/02Token.png)


拿到token在登录界面的令牌区域输入，然后点击登录

![img](https://img2018.cnblogs.com/blog/687946/201909/687946-20190907135012373-551835086.png)

如果忘记Token，使用下面的命令 

```shell
kubectl -n kube-system describe secret $(kubectl -n kube-system get secret | grep admin-user | awk '{print $1}')
```

即可进入下图所示的主界面了

![04主页面.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/04主页面.png)


# 重装Dashboard

在kubernetes-dashboard.yaml所在路径下

```shell
kubectl delete -f kubernetes-dashboard.yaml
kubectl create -f kubernetes-dashboard.yaml
```



# 常用命令

查看所有的pod运行状态

```shell
kubectl get pod --all-namespaces
```

查看dashboard映射的端口

```shell
kubectl -n kube-system get service kubernetes-dashboard
```



# 常见错误

- 主页面无法显示

只有火狐可以直接打开，其他360（两种模式）、chrome、Edge都不行。

> 1.设置浏览器安全策略
>
> 2.将证书设置成系统信任

- nodePort: Invalid value

DashBoard有效范围是：30000-32767，范围以外的数值无效



# 参考

https://www.cnblogs.com/imstrive/p/11480424.html

https://mp.weixin.qq.com/s/a7nAVPtkLkuSpCRdyz9q0A