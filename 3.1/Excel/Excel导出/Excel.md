# 问题

- File下载文件，中文名乱码

- Excel在CentOS下文件输出问题

```shell
sudo yum install libgdiplus
sudo yum install libgdiplus-devel
sudo yum install glibc
sudo yum install libc6-dev 
ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
```



```shell
yum install -y libgdiplus glibc glibc-devel
```



- 
- NLog中文乱码（服务器）

```shell
vi /etc/locale.conf
```









# Magicode.IE

> https://github.com/dotnetcore/Magicodes.IE
>
> 基于 Epplus，需要添加引用：Magicodes.IE.Excel
>
> 在Linux下使用，需要安装libgdiplus
>
> ```shell
> sudo yum install libgdiplus
> ```
> 查看安装状态
>
> ```shell
> rpm -qa |grep libgdiplus
> ```
>



## 导入、导出特性

**ExporterAttribute**

| 属性                 | 作用                                           |
| -------------------- | ---------------------------------------------- |
| Name                 | 名称（当前Sheet名称）                          |
| HeaderFontSize       | 头部字体大小                                   |
| FontSize             | 正文字体大小                                   |
| MaxRowNumberOnSheet  | Sheet最大允许的行数，设置了之后将输出多个Sheet |
| TableStyle           | 表格样式风格                                   |
| AutoFitAllColumn     | 自适应所有列                                   |
| Author               | 作者                                           |
| ExporterHeaderFilter | 头部筛选器                                     |
| AutoCenter           | 设置后可将整个表都进行居中                     |

**ExporterHeaderAttribute**

| 属性             | 作用       |
| ---------------- | ---------- |
| DisplayName      | 显示名称   |
| FontSize         | 字体大小   |
| IsBold           | 是否加粗   |
| Format           | 格式化     |
| IsAutoFit        | 是否自适应 |
| IsIgnore         | 是否忽略   |
| AutoCenterColumn | 设置列居中 |



## DTO导出

DTO数据导出，直接将数据导出即可。

```c#
public IActionResult ExportSimple()
{
    IExporter exporter = new ExcelExporter();

    var data = GetData();

    //web文件下载
    var file = exporter.ExportAsByteArray(data).Result;
    return File(file, "application/vnd.ms-excel", "文件下载.xlsx");

    //导出到本地
    //await exporter.Export("文件下载.xlsx", data);
}
```



## 模板导出

Magicodes.IE.Excel模板导出支持单元格渲染和表格渲染。

- 单元格渲染

```
{{Company}}
```

> 双大括号是必须的
>
> 支持子对象属性

- 表格渲染

| 序号                   | 书号   | 书 名    | 主编              | 出版社              | 定价      | 采购数量             | 备注                |
| ---------------------- | ------ | -------- | ----------------- | ------------------- | --------- | -------------------- | ------------------- |
| {{Table>>Data\|RowNo}} | {{No}} | {{Name}} | {{EditorInChief}} | {{PublishingHouse}} | {{Price}} | {{PurchaseQuantity}} | {{Remark\|>>Table}} |

> 渲染语法以 "Table>>Data|"为开始，其中"Data"为列表属性
>
> "RowNo"、"No"等均为列表字段
>
> 必须以"|>>Table"结尾

```c#
public IActionResult ExportByTemplate()
{
    IExportFileByTemplate exporter = new ExcelExporter();

    var data = GetData();
    var result = data.Obj2ExcelResult<Student>();

    //web文件下载
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");
    var file = exporter.ExportBytesByTemplate(result, path).Result;
    return File(file, "application/vnd.ms-excel", $"模板导出.xlsx");

    //导出到本地
    //await exporter.ExportByTemplate("模板导出.xlsx", data,Directory.GetCurrentDirectory()+@"\Templates\template.xlsx");
}
```



## 数据导入

```c#
public IActionResult ImportData()
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory() ,"c.xlsx");

    IExcelImporter importer = new ExcelImporter();
    var import = importer.Import<StudentImport>(filePath).Result;
    var data = import.Data;

    return Ok();
}
```



## 参考

[https://github.com/dotnetcore/Magicodes.IE/blob/master/docs/2.%E5%9F%BA%E7%A1%80%E6%95%99%E7%A8%8B%E4%B9%8B%E5%AF%BC%E5%87%BAExcel.md](https://github.com/dotnetcore/Magicodes.IE/blob/master/docs/2.基础教程之导出Excel.md)



# Weihanli.Npoi

> 基于 NPOI
>
> https://github.com/WeihanLi/WeihanLi.Npoi

## 列属性

**ColumnAttribute**

| 属性      | 作用     |
| --------- | -------- |
| Index     | 排序     |
| Title     | 标题     |
| Formatter |          |
| IsIgnored | 是否忽略 |
| Width     | 列宽     |



## DTO导出

需要注意，如果是要生成excel数据的byte数组，**需要指定xls格式还是Xlsx格式**。

```c#
public IActionResult TestExport()
{
    var data = GetData();

    //输出到本地
    //var path = Path.Combine(Directory.GetCurrentDirectory(), "exportTest.xlsx");
    //data.ToExcelFile(path);
    //return Ok("本地文件生成完成");

    //web下载
    var file = data.ToExcelBytes(ExcelFormat.Xlsx);
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");
    return File(file, "application/vnd.ms-excel", "文件下载.xlsx");
}
```



## 模板导出

模板可以有3种数据：

- Global：导出时可以指定一些参数，作为Global参数，默认参数格式：$(Gloabl:PropName)
- Header：配置的对应属性的显示名称，默认是属性名称，默认参数格式：$(Header:PropName)
- Data：对应数据的属性值，默认参数格式：$(Data:PropName)

默认模板参数格式（从1.8.2版本开始支持通过 TemplateHelper.ConfigurationTemplateOptions方法来自定义）

- Global：$(Global:{0})
- Header：$(Header:{0})
- Data：$(Data:{0})
- Data Begin：<Data>
- Data End：</Data>

**模板**

|                   | $(Global:Title) |                        |
| ----------------- | --------------- | ---------------------- |
| $(Header:Age)     | $(Header:Name)  | $(Header:Sex)          |
| <Data>$(Data:Age) | $(Data:Name)    | $(Data:Sex)</Data>     |
|                   |                 | 作者：$(Global:Author) |

```c#
public IActionResult TestExportByTemplate()
{
    var data = GetData();

    //输出到本地
    //var path = Path.Combine(Directory.GetCurrentDirectory(), "Tempaltes", "Template.xlsx");
    //data.ToExcelFileByTemplate(
    //    path,
    //    Directory.GetCurrentDirectory() + "test.xlsx",
    //    extraData: new
    //    {
    //        Author = "lts",
    //        Title = "Template export test"
    //    }
    //);
    //return Ok("本地文件生成完成");

    //web下载
    var template = Path.Combine(Directory.GetCurrentDirectory(), "Tempaltes", "Template.xlsx");
    var file = data.ToExcelBytesByTemplate(template, 
                                           extraData: new
    {
        Author = "lts",
        Title = "Template export test"
    });
    return File(file, "application/vnd.ms-excel", "文件下载.xlsx");
}
```

> 注：模板中Global中的数据，需要使用匿名类传递到参数：extraData



## 数据导入

```c#
public void TestImport()
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "exportTest.xlsx");

    var data = ExcelHelper.ToEntityList<Test>(path);
}
```



## 参考

https://github.com/WeihanLi/WeihanLi.Npoi/blob/dev/docs/articles/zh/GetStarted.md

https://github.com/WeihanLi/WeihanLi.Npoi/blob/dev/docs/articles/zh/TemplateExport.md

