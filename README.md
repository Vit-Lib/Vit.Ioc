# Vit.Ioc 
在ASP.NET Core中，我们一般会用到自带的IOC容器，或者使用第三方IOC容器（如Autofac）。自带容器不支持配置文件注入，而第三方又过于庞大，接下来介绍的Vit.Ioc是一套开源的依赖注入库，它可以通过配置文件实现依赖注入，最重要的是，它使用起来非常简单！简单！简单！    
[源码地址](https://github.com/serset/Vit.Ioc)    
我们用一个例子说明Vit.Ioc的所有用法。    

# (x.1)一个不使用Vit.Ioc的例子
首先，我们创建一个普通的Web项目，我们看一下它的3个cs文件

```csharp
//Program.cs	
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(new string[] { "http://*:5123" }) //(ori.1)
                .UseStartup<Startup>();
    }
}
```

```csharp
//Startup.cs	
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Vit.Ioc.Demo.Controllers;

namespace App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.UseUser(16);//(ori.2)
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseInfo(); //(ori.3)
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
```


```csharp
//Logical.cs
using Demo.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Vit.Ioc.Demo.Controllers
{
    public static class Logical
    {
        public static void UseUser(this IServiceCollection services,int age)
        {
            services.AddScoped<IUser, UserA>();
            services.AddScoped<IUser>(provider =>
            {
                var user = new UserB();
                user.fieldValue = "field-test";
                user.propertyValue = "property-test";
                user.SetAge(age);
                return user;
            });
        }

        public static void UseInfo(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/info")
                {
                    await context.Response.WriteAsync("hello world!");
                    return;
                }
                await next();
            });
        }
    }
}
```

我们着重关注一下三个可能需要注入代码的地方，分别是 Program.cs文件中的15行，Startup.cs文件中的14行和19行。Startup.cs文件中的两行代码逻辑写在Logical.cs文件中，还有两个文件分别是依赖注入用到的User类文件和Controller文件
```csharp
//User.cs
namespace Demo.Controllers
{
    public interface IUser
    {
        string GetInfo();
    }
    public class UserA : IUser
    {
        public string GetInfo()
        {
            return "UserA";
        }
    }

    public class UserB : IUser
    {
        public string fieldValue;
        public string propertyValue { get; set; } = "default";
        public int age = 0;
        public void SetAge(int age)
        {
            this.age = age;
        }
        public string GetInfo()
        {
            return $"UserB fieldValue[{ fieldValue }]  propertyValue[{propertyValue}]  age[{age}]";
        }
    }
}
```

```csharp
//ValuesController.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ValuesController : ControllerBase
    {  
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IEnumerable<IUser> userList)
        {
            return userList.Select(g=>g.GetInfo()).ToList() ;
        }
         
    }
}

```

这个项目主要提供两个api接口 /info 和 /api/Values,分别返回 字符串"hello world!",和通过依赖注入的User对象列表。    
您可以在[这里](https://github.com/serset/Vit.Ioc/tree/main/Demo/Vit.Ioc.Demo.Ori)查看所有的代码。    
这是一个比较简单的项目，对依赖注入使用的方式也没有什么不妥，主要的问题就是，依赖注入是通过代码实现的，在编译的时候就确定了代码注入逻辑。    


# (x.2)使用Vit.Ioc的例子
上面是不使用Vit.Ioc的例子，现在我们看一下使用Vit.Ioc后是什么样子。

```csharp
//Program.cs
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Vit.Extensions;//(demo.1)

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .Populate()//(demo.2)
                //.UseUrls(new string[] { "http://*:5123" }) //(ori.1)
                .UseStartup<Startup>();
    }
}

```

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Vit.Ioc.Demo.Controllers;

namespace App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.UseUser(16);//(ori.2)
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseInfo(); //(ori.3)
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
```

```csharp
//appsettings.json
{
  "Ioc": {

    "IWebHostBuilder": [
      {
        /* 在此Assembly中查找类(如 Vit.Core)(assemblyFile、assemblyName 指定任一即可) */
        "assemblyName": "Microsoft.AspNetCore.Hosting.Abstractions",
        /* 动态加载的类名 */
        "className": "Microsoft.AspNetCore.Hosting.HostingAbstractionsWebHostBuilderExtensions",
        /* 动态加载的静态函数 */
        "methodName": "UseUrls",
        /* 函数参数，可不指定，会自动在参数前面加一个参数 */
        "params": [ [ "http://*:5123" ] ]
      }
    ],
    "IServiceCollection": [
      {
        "className": "Vit.Ioc.Demo.Controllers.Logical",    
        "methodName": "UseUser",
        "params": [ 16 ]
      }
    ],
    "IApplicationBuilder": [
      {
        "className": "Vit.Ioc.Demo.Controllers.Logical",
        "methodName": "UseInfo"
      }
    ],
    
    "Services": [
      {
        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        "Lifetime": "Scoped",
        "Service": "Demo.Controllers.IUser",
        "Implementation": "Demo.Controllers.UserA"
      },
      {
        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        "Lifetime": "Scoped",
        "Service": "Demo.Controllers.IUser",
        "Implementation": {
          "className": "Demo.Controllers.UserB",
          "Invoke": [
            {
              "Name": "fieldValue",
              "Value": "field-test"
            },
            {
              "Name": "propertyValue",
              "Value": "property-test"
            },
            {
              "Name": "SetAge",
              "Params": [ 16 ]
            }
          ]
        }
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
[代码地址](https://github.com/serset/Vit.Ioc/tree/main/Demo/Vit.Ioc.Demo)    
可以看到，代码注入的三处地方 (ori.1)、(ori.2)、(ori.3) 都通过配置文件进行了实现，而在Program.cs文件中多了一行
```csharp
                .Populate()//(demo.2)
```
就是这行代码触发了自动从配置文件中进行代码注入的功能。而注入的不仅可以是类，也可以是静态函数。    
如果这三个地方都可以进行注入，那我们平常用到的绝大部分场景都能注入，这无疑增加了代码的灵活性。    