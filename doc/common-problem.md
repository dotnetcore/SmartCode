# SmartCode 常见问题

## SmartCode 能干什么？

> SmartCode = IDataSource -> IBuildTask -> IOutput => Build Everything

SmartCode的执行流是 数据源->构建任务->输出，也就是说应用场景非常广泛。

1. 从DB读取数据结构，最终生成整个解决方案=代码生成器（SmartCode.Generator）
2. ETL，其实很显然SmartCode执行流跟ETL很像，做一些相应的扩展便可支持ETL（SmartCode.ETL）
3. Mode First，从Model类结构解析出数据源，最终生成SQL脚本，执行生成DB结构，并生成整个解决方案
4. 静态文档生成器
5. 还有很多等待你去发掘

## SmartCode 扩展性如何

SmartCode 插件机制拥有非常灵活的扩展能力，SmartCode 中一切都是插件。只要继承IPlugin接口即可，然后配置到appsettings.json，然后通过IPluginManager获取插件实例。

``` json
{
  "Logging": {
    "IncludeScopes": false,
    "Console": {
      "LogLevel": {
        "Default": "Debug"
      }
    }
  },
  "SmartCode": {
    "Version": "v1.16.0",
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
        "Paramters": {
          "ConnectionString": "Server=localhost;Port=5432;User Id=postgres;Password=SmartSql; Database=smartcode_etl;"
        }
      }
    ]
  }
}
```

## 如何自定义模板

目前SmartCode支持模板引擎 **Razor**  。
Razor 模板引擎使用的是官方版本，这一点上.NETer同学可以很轻松的自定义SmartCode模板，需要注意的是Razor模板的Model为BuildContext,具体方法可以参考源代码中的模板。编写完成之后放到RazorTemplates，构建时指定好即可。

## 代码生成器支持多少种数据库

SmartCode获取数据源结构使用的是[SmartSql](https://github.com/Ahoo-Wang/SmartSql)，所以SmartCode支持所有ADO.NET驱动相关的数据库：MySql/PostgreSql/SqlServer/Oracle/SQLite 等
