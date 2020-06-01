# 安装依赖

```
//安装ssh
sudo yum install curl policycoreutils openssh-server openssh-clients policycoreutils-python
sudo systemctl enable sshd
sudo systemctl start sshd

//邮件通知服务
sudo yum install postfix
sudo systemctl enable postfix
sudo systemctl start postfix

```



# 配置yum加速

官方源地址：`https://about.gitlab.com/downloads/#centos6`

清华大学镜像源 : `https://mirror.tuna.tsinghua.edu.cn/help/gitlab-ce`

> vi /etc/yum.repos.d/gitlab_gitlab-ce.repo

```
[gitlab-ce]
name=Gitlab CE Repository
baseurl=https://mirrors.tuna.tsinghua.edu.cn/gitlab-ce/yum/el$releasever/
gpgcheck=0
enabled=1
```



# 安装

```
curl -sS https://packages.gitlab.com/install/repositories/gitlab/gitlab-ce/script.rpm.sh | sudo bash
yum install gitlab-ce
```



# 修改配置

修改gitlab配置信息

/etc/gitlab/

```
#修改配置项
external_url：http://本机ip
```

重启配置服务

```
gitlab-ctl reconfigure
```

重启gitlab

```
gitlab-ctl restart
```



# 登录

gitlab默认装在机器的80端口，直接根绝ip即可访问，第一次登陆需要设置root账号的密码



# 参考

<https://blog.csdn.net/gnail_oug/article/details/95956861>



# 常见问题

> Developer无法提交代码

【Settings】-->【Repository】

修改`Allowed to merge`、`Allowed to push`两个属性



> 用户组权限

| 角色      | 权限                                                         |
| --------- | ------------------------------------------------------------ |
| Guest     | 可以创建issue、发表评论，不能读写版本库                      |
| Reporter  | 可以克隆代码，不能提交，QA、PM可以赋予这个权限               |
| Developer | 可以克隆代码、开发、提交、push，RD可以赋予这个权限           |
| Master    | 可以创建项目、添加tag、保护分支、添加项目成员、编辑项目，核心RD负责人可以赋予这个权限 |
| Owner     | 可以设置项目访问权限 - Visibility Level、删除项目、迁移项目、管理组成员，开发组leader可以赋予这个权限 |



