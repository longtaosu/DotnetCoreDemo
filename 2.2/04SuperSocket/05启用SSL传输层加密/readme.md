# 在SuperSocket中启用TLS/SSL传输层加密

## 1.SuperSocket支持传输层加密（TLS/SSL）

SuperSocket有自动的对TLS/SSL的支持，可以无需增加或者修改任何代码，就能让服务器支持TLS/SSL。

## 2.授权证书

两种方式提供证书：

1. 一个带有私钥的 X509 证书文件
   - 可以通过CertificateCreator in SuperSocket（http://supersocket.codeplex.com/releases/view/59311）生成证书文件用于测试；
   - 在生产环境，应该向证书颁发机构购买证书
2. 一个在本地证书仓库的证书

## 3.通过证书文件启用 TLS/SSL

通过下面的步骤修改配置文件来使用准备好的证书文件：

- 在server节点设置security属性；
- 在server节点下增加certificate子节点；

最后的配置如下：

```xml
<server name="EchoServer"
        serverTypeName="EchoService"
        ip="Any" port="443"
        security="tls">
    <certificate filePath="localhost.pfx" password="supersocket"></certificate>
</server>
```

提示：certificate节点下的password属性的值是这个证书文件的私钥

还有一个可选的配置选项“**keyStorageFlags**”用于证书加载：

```xml
<certificate filePath="localhost.pfx"
             password="supersocket"
             keyStorageFlags="UserKeySet"></certificate>
```

可以通过阅读下面这篇MSDN文章了解关于这个选项的更多信息：<http://msdn.microsoft.com/zh-cn/library/system.security.cryptography.x509certificates.x509keystorageflags(v=vs.110).aspx>

## 4.通过本地证书仓库的证书来启用TLS/SSL

可以通过本地证书仓库的证书，而不是使用一个物理文件。只需要在配置中设置要使用的storeName和thumbprint：

```xml
<server name="EchoServer"
        serverTypeName="EchoService"
        ip="Any" port="443"
        security="tls">
    <certificate storeName="My" thumbprint="‎f42585bceed2cb049ef4a3c6d0ad572a6699f6f3"></certificate>
</server>
```

其他可选参数：

**storeLocation** - CurrentUser, LocalMachine

```xml
<certificate storeName="My"
             thumbprint="‎f42585bceed2cb049ef4a3c6d0ad572a6699f6f3">
             storeLocation="LocalMachine"
</certificate>
```

## 5.你也可以只为服务器实例的其中一个监听启用TLS/SSL，而其它监听仍然使用明文传输。

```xml
<server name="EchoServer" serverTypeName="EchoService" maxConnectionNumber="10000">
    <certificate storeName="My" thumbprint="‎f42585bceed2cb049ef4a3c6d0ad572a6699f6f3"></certificate>
    <listeners>
      <add ip="Any" port="80" />
      <add ip="Any" port="443" security="tls" />
    </listeners>
</server>
```

## 6.客户端安全证书验证

在TLS/SSL安全通信中，客户端的安全证书不是必须的，但是有些系统要更高级别的保障，此功能允许在服务器验证客户端证书。

首先，要启用客户端证书验证，需要在配置中的证书节点增加新的属性“clientCertificateRequired”：

```xml
<certificate storeName="My"
             storeLocation="LocalMachine"
             clientCertificateRequired="true"
             thumbprint="‎f42585bceed2cb049ef4a3c6d0ad572a6699f6f3"/>
```

然后你需要重写AppServer的方法“ValidateClientCertificate(...)”用于实现验证逻辑：

```c#
protected override bool ValidateClientCertificate(YourSession session, object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
   //Check sslPolicyErrors

   //Check certificate

   //Return checking result
   return true;
}
```

