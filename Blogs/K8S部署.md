# 安装方式

- KubeOperator
- rancher
- kubeadm
- kubespray



# Kubeadm

## 环境配置

### 配置主机

3个主机

| 角色   | 主机名        | IP             |
| ------ | ------------- | -------------- |
| Master | centos_master | 192.168.11.58  |
| Node   | centos_node1  | 192.168.11.111 |
| Node   | centos_node2  | 192.168.11.173 |

修改hosts文件，添加主机名和IP映射关系

```shell
# 安装vim编辑器
yum -y install vim*

# 编辑hosts文件
vim /etc/hosts

# 在host文件添加下面的内容
192.168.11.58  master
192.168.11.111 node1
192.168.11.173 node2
```



### 关闭防火墙

```shell
systemctl stop firewalld
systemctl disable firewalld
```



### 校正时间

系统时间不一致，会导致node节点无法加入集群
查看系统时间

```shell
date
```

安装ntp

```shell
yum install -y ntp
```

同步时间

```shell
ntpdate cn.pool.ntp.org
```



### 关闭selinux

```shell
sed -i 's/enforcing/disabled/' /etc/selinux/config
setenforce 0
```



### 关闭swap

K8S中不支持swap分区，编辑etc/fstab将swap那一行注释掉或者删除掉

```shell
vim /etc/fstab
#/dev/mapper/centos-swap swap                    swap    defaults        0 0
```



### 桥接ipv4

创建/etc/sysctl.d/k8s.conf文件，添加如下内容

```shell
net.bridge.bridge-nf-call-ip6tables = 1
net.bridge.bridge-nf-call-iptables = 1
net.ipv4.ip_forward = 1
```



## 安装Docker&Kubeadm&Kubelet

> 以下操作请在所有的节点中进行

### 安装Docker

```shell
wget https://mirrors.aliyun.com/docker-ce/linux/centos/docker-ce.repo -O /etc/yum.repos.d/docker-ce.repo
yum -y install docker-ce-18.06.1.ce-3.el7
systemctl enable docker && systemctl start docker
docker --version
# Docker version 18.06.1-ce, build e68fc7a
```

如果有报错，请安装wget

```shell
yum -y install wget
```



### 添加阿里云Yum源

新建文件：/etc/yum.repos.d/kubernetes.repo，文件内容如下：

```ini
[kubernetes]
name=Kubernetes
baseurl=https://mirrors.aliyun.com/kubernetes/yum/repos/kubernetes-el7-x86_64
enabled=1
gpgcheck=1
repo_gpgcheck=1
gpgkey=https://mirrors.aliyun.com/kubernetes/yum/doc/yum-key.gpg
https://mirrors.aliyun.com/kubernetes/yum/doc/rpm-package-key.gpg
```



### 安装Kubeadm&Kubelet&Kubectl

注意，本次部署K8S版本号为1.13.3

```shell
yum install -y kubelet-1.13.3 kubeadm-1.13.3 kubectl-1.13.3
systemctl enable kubelet
```

### 常见问题

- 碰到需要kubernetes-cni的问题：

```shell
#####错误：软件包：kubelet-1.13.3-0.x86_64 (kubernetes)
需要：kubernetes-cni = 0.6.0
可用: kubernetes-cni-0.3.0.1-0.07a8a2.x86_64 (kubernetes)
kubernetes-cni = 0.3.0.1-0.07a8a2
可用: kubernetes-cni-0.5.1-0.x86_64 (kubernetes)
kubernetes-cni = 0.5.1-0
可用: kubernetes-cni-0.5.1-1.x86_64 (kubernetes)
kubernetes-cni = 0.5.1-1
可用: kubernetes-cni-0.6.0-0.x86_64 (kubernetes)
kubernetes-cni = 0.6.0-0
正在安装: kubernetes-cni-0.7.5-0.x86_64 (kubernetes)
kubernetes-cni = 0.7.5-0
您可以尝试添加 --skip-broken 选项来解决该问题
您可以尝试执行：rpm -Va --nofiles --nodigest
```

解决：手动安装kubernetes-cni对应的版本

```shell
yum install -y kubelet-1.13.3 kubeadm-1.13.3 kubectl-1.13.3 kubernetes-cni-0.6.0
```

- 使用yum安装程序时，提示xxx.rpm公钥尚未安装

```shell
从 https://mirrors.aliyun.com/kubernetes/yum/doc/yum-key.gpg 检索密钥
导入 GPG key 0xA7317B0F:
 用户ID     : "Google Cloud Packages Automatic Signing Key <gc-team@google.com>"
 指纹       : d0bc 747f d8ca f711 7500 d6fa 3746 c208 a731 7b0f
 来自       : https://mirrors.aliyun.com/kubernetes/yum/doc/yum-key.gpg

e3438a5f740b3a907758799c3be2512a4b5c64dbe30352b2428788775c6b359e-kubectl-1.13.3-0.x86_64.rpm 的公钥尚未安装

 失败的软件包是：kubectl-1.13.3-0.x86_64
 GPG  密钥配置为：https://mirrors.aliyun.com/kubernetes/yum/doc/yum-key.gpg
```

***解决：***使用 yum install xxx.rpm --nogpgcheck 命令格式跳过公钥检查，比如跳过kubectl和kubeadm的公钥检查如下命令：

```shell
yum install -y kubectl-1.13.3-0.x86_64 --nogpgcheck
yum install -y kubeadm-1.13.3-0.x86_64 --nogpgcheck
```

查看kubeadm、kubelet版本 

```shell
kubelet --version
kubeadm version
```



## 部署Kubernetes Master

> 以下步骤请在k8s-master节点上操作

```shell
kubeadm init \
--apiserver-advertise-address=192.168.11.58 \
--image-repository registry.aliyuncs.com/google_containers \
--kubernetes-version v1.13.3 \
--service-cidr=10.1.0.0/16 \
--pod-network-cidr=10.244.0.0/16
```

![10token.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/10token.png)

接下来，为了顺利使用kubectl命令，执行以下命令

```shell
mkdir -p $HOME/.kube
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config
# kubectl get nodes
```

这时你可以使用kubectl了，当你执行完kubectl get nodes之后，你会看到如下状态

```shell
kubectl get nodes
```

![02GetNodes.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/02GetNodes.png)

常见错误

> the kubelet version is higher than the control plane version.

![03KubeletVersion.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/03KubeletVersion.png)


```shell
yum -y remove kubelet
yum install -y kubelet-1.13.3 --nogpgcheck
yum install -y kubeadm-1.13.3-0.x86_64 --nogpgcheck
```

### 部署Kubernetes Pod

继续在k8s-master上操作

```shell
kubectl apply -f \
https://raw.githubusercontent.com/coreos/flannel/a70459be0084506e4ec919aa1c114638878db11b/Documentation/kube-flannel.yml
```

![04部署pod.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/04部署pod.png)

然后通过以下命令验证：全部为Running则OK，其中一个不为Running，比如：Pending、ImagePullBackOff都表明Pod没有就绪　　

```shell
kubectl get pod --all-namespaces
```

![05状态验证.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/05状态验证.png)


 如果其中有的Pod没有Running，可以通过以下命令查看具体错误原因，比如这里我想查看kube-flannel-ds-amd64-8bmbm这个pod的错误信息：

```shell
kubectl describe pod kube-flannel-ds-amd64-xpd82 -n kube-system
```

在此过程中可能会遇到无法从qury.io拉取flannel镜像从而导致无法正常Running，解决办法如下：

使用国内云服务商提供的镜像源然后通过修改tag的方式曲线救国

```shell
docker pull quay-mirror.qiniu.com/coreos/flannel:v0.11.0-amd64
docker tag quay-mirror.qiniu.com/coreos/flannel:v0.11.0-amd64 quay.io/coreos/flannel:v0.10.0-amd64
docker rmi quay-mirror.qiniu.com/coreos/flannell:v0.11.0-amd64
```

![06running.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/06running.png)

这时，我们再看看master节点的状态就会从NotReady变为Ready：

```shell
kubectl get nodes
```

![07GetNodes.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/07GetNodes.png)

那么，恭喜你，Master节点部署结束了。如果你只想要一个单节点的K8S，那么这里就完成了部署了。



## 加入Kubernetes Node

在两台Node节点上执行join命令：

> token和sha256的值，需要根据init后的命令获取

```shell
kubeadm join 192.168.11.58:6443 --token 2jn0ia.hl0w4szssitfjw1p --discovery-token-ca-cert-hash \sha256:ce259ac38db7831350528c8cb101bdd5c43ddc25881b6c82fa0d795df44acc77
```

这里需要注意的就是，带上在Master节点Init成功后输出的Token。如果找不到了，没关系，可以通过以下命令来查看：

```shell
kubeadm token list 
```

注：token默认有效期24小时，过期后使用该命令无法查看，可通过下面到方法修改。

```shell
kubeadm token create
```

获取ca证书sha256编码hash值

```shell
openssl x509 -pubkey -in /etc/kubernetes/pki/ca.crt | openssl rsa -pubin -outform der 2>/dev/null | openssl dgst -sha256 -hex | sed 's/^.* //'
```

Node节点上成功join之后会得到以下信息：

![09JoinK8s.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/09JoinK8s.png)

  这时，我们在master节点上执行以下命令可以看到集群各个节点的状态了：

![08集群.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/08集群.png)

###  常见问题

- **证书已存在**

![11证书已存在.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/11证书已存在.png)

  解决办法：删除相应目录下的证书文件，重新执行命令

- **Downloading configuration for the kubelet from the "kubelet-config-1.15" ConfigMap in the kube-system namespace**
  **configmaps "kubelet-config-1.15" is forbidden: User "system:bootstrap:6w889o" cannot get resource "configmaps" in API group "" in the namespace "kube-system"**

![12版本不一致.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/12版本不一致.png)

 这种提示一般是kubelet版本与kubeadm版本不一致导致，重新安装kubelet即可

![13查询版本.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/13查询版本.png)

```shell
yum remove kubelet

yum install -y kubelet-1.13.3 kubeadm-1.13.3 kubectl-1.13.3 kubernetes-cni-0.6.0
```

- **Node状态非Ready**


>kubectl get nodes
>
>如果看到两个Node状态不是Ready，那么可能需要检查哪些Pod没有正常运行：

```shell
kubectl get pod --all-namespaces
```

　　然后按照【部署Kubernetes Pod】中的检查方式进行检查并修复，最终kubectl get nodes效果应该状态都是Running。注意的是在检查时需要注意是哪个Node上的错误，然后在对应的Node进行修复，比如拉取flannel镜像。

- **Get https://192.168.198.111:6443/api/v1/namespaces/kube-public/configmaps/cluster-info: x509: certificate has expired or is not yet valid**

![14时间问题.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/14时间问题.png)

该问题出现，是因为节点之间时间不同步导致，重复最开始的时间同步即可

## 测试Kubernetes集群

这里为了快速地验证一下我们的K8S集群是否可用，创建一个示例Pod：

```
kubectl create deployment nginx --image=nginx
kubectl expose deployment nginx --port=80 --type=NodePort
kubectl get pod,svc
```

![15部署nginx.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/15部署nginx.png)

  如果想要看到更多的信息，比如pod被部署在了哪个Node上，可以通过 kubectl get pods,svc -o wide来查看。

```
kubectl get pods,svc -o wide
```

![17pod详细信息.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/17pod详细信息.png)

 因为是NodePort方式，因此其映射暴露出来的端口号会在30000-32767范围内随机取一个，我们可以直接通过浏览器输入IP地址访问，比如这时我们通过浏览器来访问一下任一Node的IP地址加端口号，例如http://192.168.11.58:32481/或http://192.168.11.173:32481/　

![16nginx测试.png](https://gitee.com/imstrive/ImageBed/raw/master/20200729/16nginx测试.png)

 如果能够成功看到，那么恭喜你，你的K8S集群能够成功运行了



# 参考

https://www.cnblogs.com/imstrive/p/11409008.html