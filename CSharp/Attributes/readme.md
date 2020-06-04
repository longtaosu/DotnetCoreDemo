# Obsolete

将一段代码标记为不推荐使用，使用 obsolete 标记后会产生一个警告或者错误。

```c#
namespace AttributeExamples
{
    [Obsolete("use class B")]
    public class A
    {
        public void Method() { }
    }

    public class B
    {
        [Obsolete("use NewMethod", true)]
        public void OldMethod() { }

        public void NewMethod() { }
    }

    public static class ObsoleteProgram
    {
        public static void Main()
        {
            // Generates 2 warnings:
            A a = new A();

            // Generate no errors or warnings:
            B b = new B();
            b.NewMethod();

            // Generates an error, compilation fails.
            // b.OldMethod();
        }
    }
}
```

上面的代码中，将Obsolete属性应用到了 class A 和 B.OldMethod()，因为OldMethod方法的第二个参数设置了`true`，所以会产生一个变异错误，而类A只会产生一个警告。

调用 B.NewMethod()方法，不会产生警告或者错误。

![](Images\01warnings.png)



# AttributeUsage

AttributeUsage属性决定一个自定义的类如何使用，使用该属性可以控制以下属性：

- 可以使用到程序的哪些元素。如果没有限制使用，属性可以使用到程序的以下所有元素:

1. assembly
2. module
3. field
4. event
5. method
6. param
7. property
8. return
9. type

- 是否属性可以在程序的一个元素上多次使用。
- 属性是否可以在类的继承中传递







# 参考

<https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/general>