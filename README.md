# SmartCode

> SmartCode = IDataSource -> IBuildTask -> IOutput => Build Everything

## Introduction

![SmartCode](./doc/SmartCode.png)

## SmartCode.Db (代码生成器)

### Demo

![SmartCode](./doc/SmartCode-Db.gif)

### Getting Started

1. 下载
2. 解压
3. 设置环境变量
4. 编辑构建配置文件（默认：SmartCode.yml）
5. 命令行执行SmartCode命令
    - SmartCdoe
    - 等待提示输入配置文件路径（可选：默认程序根目录下SmartCode.yml文件）
    - 回车执行命令
6. 等待任务执行结束
7. 查看输出目录结果

### 构建配置文件

| 参数名 | 说明 |
| :--------- | --------:|
| Module | 根模块名 |
| Author | 作者 |
| DataSource | 数据源 |
| Language | 语言：CSharp/Java/.... |
| TemplateEngine | 模板引擎：目前内置：Razor/Handlebars |
| Output | 输出 |
| Build | 任务构建s |

#### DataSource 数据源，Name:Db

> 属性 Name:Db,使用DbSource插件作为数据源

DbSource.Paramters 接受以下三个参数：

| 参数名 | 说明 |
| :--------- | --------:|
| DbName | 数据库名称 |
| DbProvider | 数据驱动提供者:MySql,MariaDB,PostgreSql,SqlServer,Oracle,SQLite |
| ConnectionString | 连接字符串 |

#### Build 任务构建

| 构建类型 | 说明 |
| :--------- | --------:|
| Clear | 用于清理目录s/文件s |
| Project | 用于构建单文件，如：解决方案文件/项目文件.... |
| Table | 用于构建以数据表为基础的文件，如：Entity,Repository文件... |

#### NamingConverter 命名转换

| 属性 | 说明 |
| :--------- | --------:|
| 类型 | Table/View/Column |
| Tokenizer | 分词器 |
| Converter | 转换器：Camel/Pascal/None |

##### NamingConverter.Tokenizer 分词器

| 属性 | 说明 |
| :--------- | --------:|
| Type | Default |
| Paramters.IgnorePrefix | 忽略前缀字符 |
| Paramters.Delimiter | 分隔符 |