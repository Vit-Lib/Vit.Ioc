# Vit.Ioc 
��ASP.NET Core�У�����һ����õ��Դ���IOC����������ʹ�õ�����IOC��������Autofac�����Դ�������֧�������ļ�ע�룬���������ֹ����Ӵ󣬽��������ܵ�Vit.Ioc��һ�׿�Դ������ע��⣬������ͨ�������ļ�ʵ������ע�룬����Ҫ���ǣ���ʹ�������ǳ��򵥣��򵥣��򵥣�    
[Դ���ַ](https://github.com/serset/Vit.Ioc)    
������һ������˵��Vit.Ioc�������÷���    

# (x.1)һ����ʹ��Vit.Ioc������
���ȣ����Ǵ���һ����ͨ��Web��Ŀ�����ǿ�һ������3��cs�ļ�

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

�������ع�עһ������������Ҫע�����ĵط����ֱ��� Program.cs�ļ��е�15�У�Startup.cs�ļ��е�14�к�19�С�Startup.cs�ļ��е����д����߼�д��Logical.cs�ļ��У����������ļ��ֱ�������ע���õ���User���ļ���Controller�ļ�
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

�����Ŀ��Ҫ�ṩ����api�ӿ� /info �� /api/Values,�ֱ𷵻� �ַ���"hello world!",��ͨ������ע���User�����б�    
��������[����](https://github.com/serset/Vit.Ioc/tree/main/Demo/Vit.Ioc.Demo.Ori)�鿴���еĴ��롣    
����һ���Ƚϼ򵥵���Ŀ��������ע��ʹ�õķ�ʽҲû��ʲô���ף���Ҫ��������ǣ�����ע����ͨ������ʵ�ֵģ��ڱ����ʱ���ȷ���˴���ע���߼���    


# (x.2)ʹ��Vit.Ioc������
�����ǲ�ʹ��Vit.Ioc�����ӣ��������ǿ�һ��ʹ��Vit.Ioc����ʲô���ӡ�

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
        /* �ڴ�Assembly�в�����(�� Vit.Core)(assemblyFile��assemblyName ָ����һ����) */
        "assemblyName": "Microsoft.AspNetCore.Hosting.Abstractions",
        /* ��̬���ص����� */
        "className": "Microsoft.AspNetCore.Hosting.HostingAbstractionsWebHostBuilderExtensions",
        /* ��̬���صľ�̬���� */
        "methodName": "UseUrls",
        /* �����������ɲ�ָ�������Զ��ڲ���ǰ���һ������ */
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
        /* �������ڡ���Ϊ Scoped��Singleton��Transient��Ĭ��Scoped */
        "Lifetime": "Scoped",
        "Service": "Demo.Controllers.IUser",
        "Implementation": "Demo.Controllers.UserA"
      },
      {
        /* �������ڡ���Ϊ Scoped��Singleton��Transient��Ĭ��Scoped */
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
[�����ַ](https://github.com/serset/Vit.Ioc/tree/main/Demo/Vit.Ioc.Demo)    
���Կ���������ע��������ط� (ori.1)��(ori.2)��(ori.3) ��ͨ�������ļ�������ʵ�֣�����Program.cs�ļ��ж���һ��
```csharp
                .Populate()//(demo.2)
```
�������д��봥�����Զ��������ļ��н��д���ע��Ĺ��ܡ���ע��Ĳ����������࣬Ҳ�����Ǿ�̬������    
����������ط������Խ���ע�룬������ƽ���õ��ľ��󲿷ֳ�������ע�룬�����������˴��������ԡ�    