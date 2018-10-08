# SmartCode([Chinese Document](./README.md))

> SmartCode = IDataSource -> IBuildTask -> IOutput => Build Everything

## Introduction

![SmartCode](./doc/SmartCode-EN.png)

## SmartCode.Db (Code generator)

### Demo

![SmartCode](./doc/SmartCode-Db.gif)

### Getting Started

1. Install from .NET Core Global Tool  

  ``` shell
  dotnet tool install --global SmartCode.CLI
  ```

2. edit build configuration file (default: SmartCode.yml)
3. the command line executes the SmartCode command.
  - SmartCode
  - wait for prompt to enter the configuration file path (optional: SmartCode.yml file in the default program root directory)
  - carriage return execution command
4. wait for the end of the task execution.
5. View output directory results

### Building configuration files

``` yml
Module: SmartSql.Starter
Author: Ahoo Wang
DataSource:
  Name: Db
  Paramters:
    DbName: SmartSqlStarterDB
    DbProvider: SqlServer
    ConnectionString: Data Source=.;Initial Catalog=SmartSqlStarterDB;Integrated Security=True
Language: CSharp
TemplateEngine: Razor 
Output: 
  Type: File
  Path: 'E://SmartSql-Starter'

Build:
  ClearDir:
    Type: Clear
    Paramters:
      Dirs: '.'
  Solution:
    Type: Project
    Template: Sln.cshtml
    Output:
      Path: '.'
      Name: '{{Project.Module}}'
      Extension: '.sln'
  SmartSqlConfig:
    Type: Project
    Template: SqlMapConfig.cshtml
    Output:
      Path: '{{Project.Module}}.API'
      Name: 'SmartSqlMapConfig'
      Extension: '.xml'
  Entity_Project:
    Type: Project
    Template: Proj.cshtml
    Output:
      Path: '{{Project.Module}}.Entity'
      Name: '{{Project.Module}}.Entity'
      Extension: '.csproj'
  Entity:
    Type: Table
    Module: Entity
    Template: Entity.cshtml
    Output:
      Path: '{{Project.Module}}.{{Build.Module}}'
      Extension: '.cs'
    NamingConverter:
      Table:
        Tokenizer:
          Type: Default
          Paramters:
            IgnorePrefix: 'T_'
            Delimiter: '_'
        Converter:
          Type: Default
          Paramters: { }
      View:
        Tokenizer:
          Type: Default
          Paramters:
            IgnorePrefix: 'V_'
            Delimiter: '_'
        Converter:
          Type: Pascal
      Column:
        Tokenizer:
          Type: Default
          Paramters: 
            Delimiter: '_'
        Converter:
          Type: Pascal
  Repository_Project:
    Type: Project
    Template: Proj-Repository.cshtml
    Output:
      Path: '{{Project.Module}}.Repository'
      Name: '{{Project.Module}}.Repository'
      Extension: '.csproj'
  Repository:
    Type: Table
    Module: Repository
    Template: Repository.cshtml
    Output:
      Path: '{{Project.Module}}.{{Build.Module}}'
      Name: 'I{{OutputName}}Repository'
      Extension: .cs
    NamingConverter:
      Table:
        Tokenizer:
          Type: Default
          Paramters:
            IgnorePrefix: 'T_'
            Delimiter: '_'
        Converter:
          Type: Default
      View:
        Tokenizer:
          Type: Default
          Paramters: 
            IgnorePrefix: 'V_'
            Delimiter: '_'
        Converter:
          Type: Default

  SqlMap:
    Type: Table
    Template: SqlMap-SqlServer.cshtml
    Output:
      Path: '{{Project.Module}}.API/Maps'
      Extension: .xml
    IgnoreTables: null
    NamingConverter:
      Table:
        Tokenizer:
          Type: Default
          Paramters: 
            IgnorePrefix: 'T_'
            Delimiter: '_'
        Converter:
          Type: Default
      View:
        Tokenizer:
          Type: Default
          Paramters: 
            IgnorePrefix: 'V_'
            Delimiter: '_'
        Converter:
          Type: Default
      Column:
        Tokenizer:
          Type: Default
          Paramters: 
            IgnorePrefix: 'T_'
            Delimiter: '_'
        Converter:
          Type: Default
```

| Parameter Name | Description |
| :--------- | --------:|
| Module | Root Module Name |
| Author | Author |
| DataSource | Data Source |
| Language | Language: CSharp/Java/.... |
| TemplateEngine | Template Engine: Currently Built: Razor/Handlebars |
| Output | Output |
| Build | Task Build |

#### DataSource Data Source, Name: Db

> Property Name: Db, using the DbSource plugin as a data source

DbSource.Paramters accepts the following three parameters:

| Parameter Name | Description |
| :--------- | --------:|
| DbName | Database Name |
| DbProvider | Data Drivers: MySql, MariaDB, PostgreSql, SqlServer, Oracle, SQLite |
| ConnectionString | Connection String |

#### Build Task Build

| Parameter Name | Description |
| :--------- | --------:|
| Type | Build type, Clear: used to clean up the directory s / file s, Project: used to build a single file, such as: solution file / project file, Table: used to build a data table-based file, such as: Entity , Repository file|
| Module | Building Module Name |
| TemplateEngine | Template Engine, optional, default to root module engine |
| Template | Template File |
| Output | Output |
| IncludeTables | Include table name s |
| IgnoreTables | Ignore table name s |
| NamingConverter | Named Converter |
| Paramters | Custom Build Parameters |

#### NamingConverter Name Conversion

| Attribute | Description |
| :--------- | --------:|
| Type | Table/View/Column |
| Tokenizer | Word Segmenter |
| Converter | Converter: Camel/Pascal/None |

##### NamingConverter.Tokenizer Word Segmenter

| Attribute | Description |
| :--------- | --------:|
| Type | Default |
| Paramters.IgnorePrefix | Ignore prefix characters |
| Paramters.Delimiter | Separator |