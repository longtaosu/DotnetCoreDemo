# 安装

## 安装环境

下载VMWare

https://www.vmware.com/products/workstation-pro/workstation-pro-evaluation.html

下载CentOS

http://isoredirect.centos.org/centos/7/isos/x86_64/



下载完成后，安装虚拟机。



## 安装系统

打开VM，点击【创建新的虚拟机】。

![01新建虚拟机.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/01新建虚拟机.png)

选择【自定义（高级）】选项，点击【下一步】

![02自定义.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/02自定义.png)

选择【稍后安装操作系统】，点击【下一步】。

![03稍后安装操作系统.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/03稍后安装操作系统.png)

选择【Linux】、【CentOS 7 64位】后，点击【下一步】。

![04系统选择.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/04系统选择.png)

配置虚拟机的名称、虚拟机安装位置。点击【下一步】

![05配置安装位置.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/05配置安装位置.png)

设置【处理器数量】，根据自己的电脑配置设置即可

![06配置处理器.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/06配置处理器.png)

配置内存，建议在2G以上

![07配置内存.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/07配置内存.png)

配置网络，选择【使用桥连接网络】

![08配置网络.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/08配置网络.png)

选择IO控制器，使用默认的即可

![09选择IO控制器.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/09选择IO控制器.png)

选择磁盘类型，使用默认即可

![10选择磁盘类型.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/10选择磁盘类型.png)

选择【创建新虚拟磁盘】

![11创建新虚拟磁盘.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/11创建新虚拟磁盘.png)

设置【磁盘占用】，使用默认20G即可

![12设置磁盘占用.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/12设置磁盘占用.png)

磁盘文件，使用默认即可

![13磁盘文件.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/13磁盘文件.png)

### 自定义硬件

> 打印机此处默认是存在，选择移除即可

![14选择镜像.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/14选择镜像.png)

点击【完成】，即完成系统安装的配置过程



## 配置系统

开启虚拟机

![15简体中文.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/15简体中文.png)

**选择【install centos 7】**

选择【简体中文】

![15开启虚拟机.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/15开启虚拟机.png)

![18配置.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/18配置.png)

> 默认情况下，安装源、软件源会出现感叹号，点进去退出即可，无需单独配置
>
> 网络一定要选择打开的状态

![16选择网络.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/16选择网络.png)

选择安装位置

![17选择安装位置.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/17选择安装位置.png)

完成上面四项的配置后，点击【开始安装】

配置root用户的密码

![19配置密码.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/19配置密码.png)

安装完成后，点击【重启】即可

![20完成安装.png](https://gitee.com/imstrive/ImageBed/raw/master/20200728/20完成安装.png)



# 指令

>  查看ip地址，安装后，系统采用固定ip的方式，具体的ip可见ens33 的 inet字段

```shell
ip addr
```



# 参考

https://blog.csdn.net/BryantJamesHua/article/details/101480034