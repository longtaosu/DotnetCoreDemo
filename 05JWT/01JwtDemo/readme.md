# JWT

# 1.参考

[Implement JWT Authentication In ASP.NET Core APIs](http://binaryintellect.net/articles/1fdc8b3f-06a1-4f36-8c0b-7852bf850f52.aspx)

[完美实现 JWT 滑动授权刷新](https://www.cnblogs.com/laozhang-is-phi/p/10462316.html)



仅包含token分发以及验证：

https://github.com/DomTripodi93/ProductionDotNet









# 2.概念

JWT是 Json Web Tokens的缩写，是一个在客户端与服务端传递数据的开放标准。相对于传统的Cookie认证，JWT更安全，也可以用于非浏览器的客户端。

JWT包含3部分：header、payload、signature。

不同于cookies（自动的传递到服务端），JWT需要准确的传递到服务端，一个简化的流程如下：

1. 客户端将发送安全认证信息，比方说用户名和密码，到服务端进行验证
2. 服务端验证用户名和密码
3. 如果用户名和密码正确，向客户端颁发JWT token
4. 客户端接收token并存储在本地
5. 当向服务端请求资源或者接口时，客户端需要将JWT token添加到 Authorization header
6. 服务端通过Authorization header获取JWT token
7. 如果token有效，服务端执行客户端的请求



# 3.基于JWT的安全认证

通过以下步骤实现JWT认证：

1. 将JWT信息存储在配置文件
2. 在程序的startup启用 JWT authentication
3. 创建认证机制验证用户名和密码，颁发JWT
4. 创建一个API
5. 从客户端调用API

