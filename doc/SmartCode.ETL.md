# SmartCode.ETL 这不是先有鸡还是蛋的问题

> 继国庆节 SmartCode 正式版（SmartCode.Generator）发布之后，SmartCode 迎来了新的能力 SmartCode.ETL !

> SmartCode 正式版从开始发布就从未说过自己仅仅是个代码生成器，这点上从我第一次宣布SmartCode正式开源的文章就可以说明：《SmartCode 不只是代码生成器》，这不仅仅是一句推广语！

## SmartCode.Generator

相信不少同学都用过各种代码生成器，这里我就不做详细介绍了，如果想体验 SmartCode.Generator 请至 <https://www.cnblogs.com/Ahoo-Wang/p/SmartCode-intro.html> 配置好数据库连接，一键生成解决方案。

## Why SmartCode.ETL

相信不少已经落地微服务架构方案的同学都会遇到同样的问题：

1. 业务方的查询需求似乎总是跨微服务DB的
2. 领导层需要查看的报表数据总是全局的（需要聚合跨微服务DB的）

## So SmartCode.ETL

1. 从多个微服务DB 同步业务聚合查询数据到 all_biz DB （解决：微服务架构一定会遇到的业务方需要跨微服务DB查询的问题）
2. 从 all_biz DB 同步聚合分析数据到 report DB (解决：领导层查看的报表数据聚合问题)

## How SmartCode.ETL

1. 安装 SmartCode from dotnet-cli

   ``` powershell
   dotnet tool install --global SmartCode.CLI
   ```

2. 使用 SmartCode.Generator 生成 同步Sql表结构脚本，以及 SmartCode.ETL 构建配置
3. 执行Sql同步脚本初始化表结构
4. 使用任务调度（crontab） + SmartCode.ETL 同步分析数据
5. 通过持久化 etl_task 监控 etl执行情况（目前支持PostgreSql）

>简单来说就是SmartCode生成SmartCode，任务调度执行SmartCode命令行。（这真的不是先有鸡还是蛋的问题.....）

## SmartCode 插件概览

``` json
{
  "SmartCode": {
    "Version": "v1.16.15",
    "Plugins": [
      {
        "Type": "SmartCode.IDataSource,SmartCode",
        "ImplType": "SmartCode.NoneDataSource,SmartCode"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.App.BuildTasks.ClearBuildTask,SmartCode.App"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.App.BuildTasks.ProjectBuildTask,SmartCode.App"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.App.BuildTasks.MultiTemplateBuildTask,SmartCode.App"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.App.BuildTasks.ProcessBuildTask,SmartCode.App"
      },
      {
        "Type": "SmartCode.IOutput,SmartCode",
        "ImplType": "SmartCode.App.Outputs.FileOutput,SmartCode.App"
      },
      {
        "Type": "SmartCode.IDataSource,SmartCode",
        "ImplType": "SmartCode.Generator.DbTableSource,SmartCode.Generator"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.Generator.BuildTasks.TableBuildTask,SmartCode.Generator"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.Generator.BuildTasks.SingleBuildTask,SmartCode.Generator"
      },
      {
        "Type": "SmartCode.INamingConverter,SmartCode",
        "ImplType": "SmartCode.Generator.TableNamingConverter,SmartCode.Generator"
      },
      {
        "Type": "SmartCode.TemplateEngine.ITemplateEngine,SmartCode.TemplateEngine",
        "ImplType": "SmartCode.TemplateEngine.Impl.RazorCoreTemplateEngine,SmartCode.TemplateEngine"
      },
      {
        "Type": "SmartCode.Generator.IDbTypeConverter,SmartCode.Generator",
        "ImplType": "SmartCode.Generator.DbTypeConverter.DefaultDbTypeConverter,SmartCode.Generator"
      },
      {
        "Type": "SmartCode.IDataSource,SmartCode",
        "ImplType": "SmartCode.ETL.ExtractDataSource,SmartCode.ETL"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.ETL.BuildTasks.TransformBuildTask,SmartCode.ETL"
      },
      {
        "Type": "SmartCode.ETL.ITransformEngine,SmartCode.ETL",
        "ImplType": "SmartCode.ETL.TransformEngine.RazorTransformEngine,SmartCode.ETL"
      },
      {
        "Type": "SmartCode.IBuildTask,SmartCode",
        "ImplType": "SmartCode.ETL.BuildTasks.LoadBuildTask,SmartCode.ETL"
      },
      {
        "Type": "SmartCode.ETL.IETLRepository,SmartCode.ETL",
        "ImplType": "SmartCode.ETL.NoneETLRepository,SmartCode.ETL"
      },
      {
        "Type": "SmartCode.ETL.IETLRepository,SmartCode.ETL",
        "ImplType": "SmartCode.ETL.PostgreSql.PGETLRepository,SmartCode.ETL.PostgreSql",
        "Parameters": {
          "ConnectionString": "Server=localhost;Port=5432;User Id=postgres;Password=SmartSql; Database=smartcode_etl;"
        }
      }
    ]
  }
}

```

## ETL 构建配置

``` yml
Author: Ahoo Wang
DataSource:
  Name: Extract
  Parameters:
    DbProvider: SqlServer
    ConnectionString: Data Source=.;Initial Catalog=SmartSqlDB;Integrated Security=True
    Query: SELECT [Id],[UserName],[Status],[LastLoginTime],[CreationTime],[ModifyTime],[Deleted] FROM [T_User] With(NoLock) Where ModifyTime>@LastMaxModifyTime
    PKColumn: Id
    AutoIncrement: true
    ModifyTime: ModifyTime
Parameters:
  ETLCode: SmartCode.ETL.Test
  ETLRepository: PG
Build:
  Transform:
    Type: Transform
    Parameters:
      Script: 
  Load2PostgreSql: 
    Type: Load
    Parameters:
      DbProvider: PostgreSql
      ConnectionString: Server=localhost;Port=5432;User Id=postgres;Password=SmartSql; Database=smartsql_db;
      Table: t_user__temp
      PreCommand: CREATE TABLE t_user__temp( LIKE t_user );
      PostCommand: "Delete From t_user as source Where EXISTS(select * from t_user__temp temp where temp.id=source.id);
      Insert Into t_user  SELECT * From t_user__temp;
      Drop Table t_user__temp;
      "
      ColumnMapping: [{Column: Id,Mapping: id}
      ,{Column: UserName,Mapping: user_name}
      ,{Column: Status,Mapping: status}
      ,{Column: LastLoginTime,Mapping: last_login_time}
      ,{Column: CreationTime,Mapping: creation_time}
      ,{Column: ModifyTime,Mapping: modify_time}
      ,{Column: Deleted,Mapping: deleted}]
```

### 根 Parameters

| 参数名        | 说明                           |
| :------------ | -----------------------------: |
| ETLCode       | ETL任务Code,区分任务类型，唯一 |
| ETLRepository | ETL任务持久化仓储，None/PG/SQLite|

### DataSource 参数说明

> 属性 Name:Extract,使用 ExtractDataSource 插件作为数据源

#### ExtractDataSource.Parameters

| 参数名           | 说明                                                                                                    |
| :--------------- | ------------------------------------------------------------------------------------------------------: |
| DbProvider       | 数据驱动提供者:MySql,MariaDB,PostgreSql,SqlServer,Oracle,SQLite                                         |
| ConnectionString | 连接字符串                                                                                              |
| Query            | 查询命令，需要抽取的数据。默认会自动注入三个参数 LastMaxId,LastMaxModifyTime,LastQueryTime 作为查询条件 |
| PKColumn         | 主键列名                                                                                                |
| AutoIncrement    | 是否为自增主键，true 自动计算抽取的最大主键值(MaxId)                                                    |
| ModifyTime       | 最近一次修改时间列名，设置后自定计算抽取的最大修改时间列(MaxModifyTime)                                 |

### Build.Load 参数说明

> 属性 Type:Load,使用 LoadBuildTask 插件作为构建任务

#### Build.Load.Parameters

| 参数名           | 说明                                                            |
| :--------------- | --------------------------------------------------------------: |
| DbProvider       | 数据驱动提供者:MySql,MariaDB,PostgreSql,SqlServer,Oracle,SQLite |
| ConnectionString | 连接字符串                                                      |
| Table            | 目标表名                                                        |
| PreCommand       | 执行批量插入任务之前执行的命令                                  |
| PostCommand      | 执行批量插入任务之后执行的命令                                  |
| ColumnMapping    | 列映射                                                          |

## 同步策略

### LastMaxId

LastMaxId 即上一次抽取的数据最大Id值(第一次抽取时LastMaxId为-1)，该模式使用于数据插入后不再变更的数据表。

### LastMaxModifyTime

LastMaxModifyTime 即上一次抽取的数据最大ModifyTime值(第一次抽取时LastMaxModifyTime为1970-01-01 08:00:00)，适用于插入数据后还会变更的数据表。

### 并发任务同步

1. 对 Id 取模,分拆不同任务，同时并发执行

### 大数据量同步

1. 使用 Top/Limit 限制数据抽取数量，分多次同步执行完成整个数据同步。

### ETL_Task 任务监控

![ETL_Task](./SmartCode-ETL-Task.png)

#### 性能监控

##### 运行环境

1. 源抽取库：Windows Server 2012 , 8 vCPU 16 GB + SSD + SqlServer-2014
2. 目标分析库：CentOS-7 , 8 vCPU 16 GB + SSD + PostgreSql-11 + SmartCode

##### ETL_Task.Extract

以下是数据抽取性能，抽取数量为 1434678，耗时 41267 毫秒。

``` json
{
    "MaxId": 1755822,
    "PKColumn": "Id",
    "QuerySize": 1434678,
    "QueryTime": "2018-11-01T11:31:53.6191084+08:00",
    "QueryCommand": {
        "Taken": 41267,
        "Command": "Select * From T_ProductSearchLog  With(NoLock) Where Id>@LastMaxId",
        "Parameters": {
            "LastMaxId": -1,
            "LastQueryTime": "1970-01-01T08:00:00",
            "LastMaxModifyTime": "1970-01-01T08:00:00"
        }
    }
}
```

##### ETL_Task.Load

以下是数据加载性能，批量插入数据量为 1434678，耗时 21817 毫秒，平均每秒插入 65759.6 条数据。

``` json
{
    "Size": 1434678,
    "Table": "t_product_search_log",
    "Taken": 21817,
    "PreCommand": null,
    "PostCommand": null
}
```

------

目前 SmartCode.ETL 已经落地到我们的生产环境了（11-01上线截至 2018-11-16 16：50 执行了 65520 次同步任务，暂无error日志抛出）

> PS： 虽然 SmartCode.ETL 只花了周末俩天时间完成扩展，但已经可以满足我们至少90%的应用场景。这足以见得 SmartCode 扩展能力是多么令人意外了。当然SmartCode的其他能力还得后续等各位一起发掘！！！
