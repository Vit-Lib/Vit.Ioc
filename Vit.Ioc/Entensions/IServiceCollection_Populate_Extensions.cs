using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using Vit.Core.Module.Log;
using Vit.Core.Util.Reflection;

namespace Vit.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class IServiceCollection_Populate_Extensions
    {


        /// <summary>
        /// 从配置文件自动注册服务
        /// //appsettings.json demo
        ///{"Ioc": {
        ///    "Services": [
        ///      {
        ///        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        ///        "Lifetime": "Scoped",
        ///        "Service": "Cy.NetCore.Common.Interfaces.IService,Cy.NetCore.Common.Interfaces"
        ///      },
        ///      {
        ///        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        ///        "Lifetime": "Scoped",
        ///        "Service": "Cy.NetCore.Common.Interfaces.IService,Cy.NetCore.Common.Interfaces",
        ///        "Implementation": "Cy.NetCore.Common.DataBase.ServiceImpl.ServiceA,Cy.NetCore.Common.DataBase"
        ///      },
        ///      {
        ///        /* 生命周期。可为 Scoped、Singleton、Transient。默认Scoped */
        ///        "Lifetime": "Scoped",
        ///        "Service": {
        ///
        ///          /* 在此Assembly文件中查找类(如 Vit.Core.dll)(assemblyFile、assemblyName 指定任一即可) */
        ///          "assemblyFile": "Did.SersLoader.Demo.dll",
        ///
        ///          /* 在此Assembly中查找类(如 Vit.Core)(assemblyFile、assemblyName 指定任一即可) */
        ///          //"assemblyName": "Did.SersLoader.Demo",
        ///
        ///          /* 动态加载的类名 */
        ///          "className": "Bearer"
        ///
        ///        },
        ///        "Implementation": {
        ///
        ///          /* 在此Assembly文件中查找类(如 Vit.Core.dll)(assemblyFile、assemblyName 指定任一即可) */
        ///          "assemblyFile": "Did.SersLoader.Demo.dll",
        ///
        ///          /* 在此Assembly中查找类(如 Vit.Core)(assemblyFile、assemblyName 指定任一即可) */
        ///          //"assemblyName": "Did.SersLoader.Demo",
        ///
        ///          /* 动态加载的类名 */
        ///          "className": "Bearer",
        ///
        ///          "Invoke": [
        ///            {
        ///              "Name": "fieldName",
        ///              "Value": "lith"
        ///            },
        ///            {
        ///              "Name": "prpertyName",
        ///              "Value": 12
        ///            },
        ///            {
        ///              "Name": "methodName",
        ///              "Params": [ 1, "12" ]
        ///            }
        ///          ]
        ///        }
        ///      }
        ///    ]
        ///  }
        ///}
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configPath">默认：Ioc.Services</param>
        public static IServiceCollection Populate(this IServiceCollection serviceCollection, string configPath = "Ioc.Services")
        {
            var serviceMaps = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<JArray>(configPath);
            if (serviceMaps != null)
                Populate(serviceCollection, serviceMaps);

            return serviceCollection;
        }


        /// <summary>
        /// 从配置文件自动注册服务
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="serviceMaps"></param>
        public static IServiceCollection Populate(this IServiceCollection serviceCollection, JArray serviceMaps)
        {  
            if (serviceMaps == null) return serviceCollection;

            foreach (var item in serviceMaps)
            {
                JObject serviceMap = item as JObject;
                if (serviceMap == null) continue;
                try
                {
                    //(x.1)
                    //生命周期。可为 Scoped、Singleton、Transient。默认Scoped 
                    string Lifetime = (serviceMap["Lifetime"]?.ConvertToString()) ?? "Scoped";

                    //(x.2)
                    Type Service = GetType(serviceMap["Service"], out _);
                    if (Service == null) continue;

                    //(x.3)
                    Type Implementation = GetType(serviceMap["Implementation"], out var invoke);


                    #region (x.4) implementationFactory                   
                    Func<IServiceProvider, object> implementationFactory = null;
                    if (invoke != null)
                    {
                        implementationFactory = (serviceProvider) =>
                        {
                            var constructorInfo = Implementation.GetConstructors().AsQueryable().Where(m => m.IsPublic)
                            .OrderByDescending(m => m.GetParameters().Count()).FirstOrDefault();

                            if (constructorInfo == null) return null;
                            var args = constructorInfo.GetParameters().Select(m => serviceProvider.GetService(m.ParameterType)).ToArray();

                            var obj = constructorInfo.Invoke(args);

                            Invoke(Implementation, obj, invoke);

                            return obj;

                        //return Activator.CreateInstance(Implementation, args);
                        //return null;
                    };
                    }
                    #endregion


                    #region (x.5)注入服务                    
                    switch (Lifetime)
                    {
                        case "Singleton":

                            if (Implementation == null)
                            {
                                serviceCollection.AddSingleton(Service);
                                break;
                            }

                            if (invoke == null)
                            {
                                serviceCollection.AddSingleton(Service, Implementation);
                                break;
                            }

                            serviceCollection.AddSingleton(Service, implementationFactory);
                            break;
                        case "Transient":
                            if (Implementation == null)
                            {
                                serviceCollection.AddTransient(Service);
                                break;
                            }

                            if (invoke == null)
                            {
                                serviceCollection.AddTransient(Service, Implementation);
                                break;
                            }

                            serviceCollection.AddTransient(Service, implementationFactory);
                            break;


                        case "Scoped":
                            if (Implementation == null)
                            {
                                serviceCollection.AddScoped(Service);
                                break;
                            }

                            if (invoke == null)
                            {
                                serviceCollection.AddScoped(Service, Implementation);
                                break;
                            }

                            serviceCollection.AddScoped(Service, implementationFactory);
                            break;
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return serviceCollection;
        }



        static Type GetType(JToken item,out JArray invoke) 
        {
            invoke = null;

            if (item == null) return null;

            string className;
            string assemblyFile = null;
            string assemblyName = null;
         

            if (item.TypeMatch(JTokenType.String))
            {
                className = item.ConvertToString();
            }
            else
            {
                className = item["className"]?.ConvertToString();
                assemblyFile = item["assemblyFile"]?.ConvertToString();
                assemblyName = item["assemblyName"]?.ConvertToString();

                invoke = item["Invoke"] as JArray;
            }
            return ObjectLoader.GetType(className, assemblyFile: assemblyFile, assemblyName: assemblyName);
        }


        static void Invoke(Type type, object obj, JArray invoke)
        {
            /*
               "Invoke": [
                {
                  "Name": "fieldName",
                  "Value": "lith"
                },
                {
                 "Name": "prpertyName",
                  "Value": 12
                },
                {
                  "Name": "methodName",
                  "Params": [1,"12"]
                }
              ]           
             */
            foreach (var json in invoke)
            {
                var name = json["Name"].ConvertToString();


                //(x.1)Field
                {
                    var info = type.GetField(name);
                    if (info != null)
                    {
                        info.SetValue(obj, json["Value"].Deserialize(info.FieldType));
                        continue;
                    }
                }

                //(x.2)Member
                {
                    var info = type.GetProperty(name);
                    if (info != null)
                    {
                        info.SetValue(obj, json["Value"].Deserialize(info.PropertyType));
                        continue;
                    }
                }


                //(x.3)Method
                {
                    var oriParams = json["Params"] as JArray;

                    #region (x.x.1)get method
                    var paramsCount = oriParams?.Count ?? 0;
                    var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name == name && m.GetParameters().Length == paramsCount);
                    #endregion

                    if (method != null)
                    {                     
                        //(x.x.2)build params
                        var paramTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
                        object[] paramArray = new object[paramTypes.Length];
                        if (oriParams != null)
                        {
                            for (var index = 0; index < paramTypes.Length; index++)
                            {
                                paramArray[index] = oriParams[index].Deserialize(paramTypes[index]);
                            }
                        }

                        //(x.x.3)invoke
                        method.Invoke(obj, paramArray);

                        continue;
                    }
                }


            }
        }

    }
}
