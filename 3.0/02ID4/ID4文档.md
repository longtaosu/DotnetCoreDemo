# 文档

https://identityserver4.readthedocs.io/en/latest/

http://www.identityserver.com.cn/Home/Detail/shuyu

https://github.com/IdentityServer/IdentityServer4/tree/master/samples



# 专业术语

## 身份认证服务器

IdentityServer是基于OpenID Connect协议标准的身份认证和授权程序，它实现了OpenID Connect 和 OAuth 2.0 协议。

## 功能

保护你的资源
使用本地帐户或通过外部身份提供程序对用户进行身份验证
提供会话管理和单点登录
管理和验证客户机
向客户发出标识和访问令牌
验证令牌

## 术语解释

### 用户（User）

用户是使用已注册的客户端（指在id4中已经注册）访问资源的人。

### 客户端（Client）

客户端不仅可以是Web应用程序，app或桌面应用程序（你就理解为pc端的软件即可），SPA，服务器进程等。

### 资源（Resources）

资源就是你想用identityserver保护的东东，可以是用户的身份数据或者api资源。

用户的身份信息实际由一组claim组成，例如姓名或者邮件都会包含在身份信息中（将来通过identityserver校验后都会返回给被调用的客户端）。

### **身份令牌**

至少要标识某个用户（Called the sub aka subject claim）的主身份信息，和该用户的认证时间和认证方式。

### 访问令牌

访问令牌允许客户端访问某个 API 资源。客户端请求到访问令牌，然后使用这个令牌来访问 API资源。访问令牌包含了客户端和用户（如果有的话，这取决于业务是否需要，但通常不必要）的相关信息，API通过这些令牌信息来授予客户端的数据访问权限。



# 已支持的规范

OpenID Connect

OAuth2.0



# 打包和构建

## IdentityServer4

https://github.com/identityserver/IdentityServer4

https://www.nuget.org/packages/IdentityServer4/

## Quickstart UI

https://github.com/IdentityServer/IdentityServer4.Quickstart.UI

## 令牌校验功能

https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation

https://www.nuget.org/packages/IdentityServer4.AccessTokenValidation

## ASP.NET Core Identity

https://github.com/IdentityServer/IdentityServer4.AspNetIdentity

https://www.nuget.org/packages/IdentityServer4.AspNetIdentity