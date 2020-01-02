# 参考

https://segmentfault.com/a/1190000018494267

https://blog.csdn.net/qq_32448349/article/details/82428696



# 安装

## 1.下载rpm包

```shell
wget https://dev.mysql.com/get/mysql80-community-release-el7-2.noarch.rpm
```

![在这里插入图片描述](https://gitee.com/imstrive/ImageBed/raw/master/20191201/下载mysql.png)

​    安装rpm包

```shell
sudo chmod 755 mysql80-community-release-el7-2.noarch.rpm
sudo yum install mysql80-community-release-el7-2.noarch.rpm
sudo yum update
```

## 2.安装MySQL

```shell
sudo yum install mysql-community-server
```

## 3.启动MySQL,并设置开机自动启动

```shell
sudo systemctl start mysqld
sudo systemctl enable mysqld
```

查看Mysql服务状态

```mysql
service mysqld status
```

## 4.修改密码

### 4.1查看临时密码

与安装MySQL5.7不同,MySQL8.0安装过程中没有设置密码操作,MySQL自带root用户,root用户密码在MySQL启动时会写入日志文件中,可以使用一下命令查看:

```
grep "A temporary password" /var/log/mysqld.log
```

或者（两者执行一个即可）

```shell
cat /var/log/mysqld.log | grep password
```

### 4.2修改密码

使用日志文件中的密码后需要修改root密码才能对数据库进行操作.

```shell
mysql -u root -p  # 然后输入日志文件中的密码
ALTER USER 'root'@'localhost' IDENTIFIED BY '新密码';    # 新密码必须符合MySQL8.0 
密码策略,需要有一定的强度,否则会失败
```

重启Mysql服务

```shell
systemctl restart mysqld
```

## 5.设置远程主机可以访问数据库

```shell
USE mysql
select host, user, authentication_string, plugin from user;
UPDATE user SET host='%' WHERE user='root'  #  修改root用户可以远程登录
FLUSH PRIVILEGES   # 刷新权限
```



# 常见错误

## 1.caching_sha2_password

修复Authentication plugin 'caching_sha2_password' cannot be loaded

```shell
ALTER USER 'root'@'%' IDENTIFIED BY '新密码' PASSWORD EXPIRE NEVER;
ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY '新密码';
```





ALTER USER 'root'@'localhost' IDENTIFIED BY 'Sulongtao@123';



ALTER USER 'root'@'%' IDENTIFIED BY 'Sulongtao@123' PASSWORD EXPIRE NEVER; 

ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY 'Sulongtao@123';