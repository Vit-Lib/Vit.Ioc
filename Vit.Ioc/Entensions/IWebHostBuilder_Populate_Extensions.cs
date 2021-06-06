
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using Vit.Core.Util.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Vit.Extensions
{
    public static partial class IWebHostBuilder_Populate_Extensions
    {

        public static IWebHostBuilder Populate(this IWebHostBuilder hostBuilder, string configPath = "Ioc")
        {
            var config = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<JObject>(configPath);

            if (null == config)
            {
                return hostBuilder;
            }

            #region Method Invoke
            void Invoke(JArray invokes,object firstParam) 
            {
                foreach (var item in invokes)
                {
                    JObject invoke = item as JObject;
                    if (invoke == null) continue;

                    #region (x.x.1)get type
                    var className = invoke["className"]?.ConvertToString();
                    var assemblyFile = invoke["assemblyFile"]?.ConvertToString();
                    var assemblyName = invoke["assemblyName"]?.ConvertToString();

                    Type type = ObjectLoader.GetType(className, assemblyFile: assemblyFile, assemblyName: assemblyName);
                    if (type == null) continue;
                    #endregion


                    #region (x.x.2)get method
                    var methodName = invoke["methodName"]?.ConvertToString();
                    if (methodName == null) continue;

                    var oriParams = invoke["params"] as JArray;
                    var paramsCount = oriParams == null ? 1 : oriParams.Count + 1;
                    var method = type.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == paramsCount);
                    if (method == null) continue;
                    #endregion

                    #region (x.x.3)build params
                    object[] paramArray = new object[paramsCount];
                    var paramTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
                    paramArray[0] = firstParam;
                    for (var i = 1; i < paramTypes.Length; i++)
                    {
                        paramArray[i] = oriParams[i - 1].Deserialize(paramTypes[i]);
                    }
                    #endregion

                    //(x.x.4)invoke
                    method.Invoke(null, paramArray);
                }
            }
            #endregion



            #region (x.1)Injection IWebHostBuilder
            {
                if (config["IWebHostBuilder"] is JArray invokes && invokes.Count > 0)
                {
                    Invoke(invokes, hostBuilder);
                }
            }
            #endregion

            #region (x.2)Injection IServiceCollection
            {
                if (config["IServiceCollection"] is JArray invokes && invokes.Count > 0)
                {
                    hostBuilder.ConfigureServices(services =>
                    {
                        Invoke(invokes, services);
                    });
                }
            }
            #endregion

            #region (x.3)Injection IApplicationBuilder
            {
                if (config["IApplicationBuilder"] is JArray invokes && invokes.Count > 0)
                {
                    hostBuilder.ConfigureServices((IServiceCollection services) =>
                    {
                        services.AddTransient<IStartupFilter>(m =>
                        {
                            return new AutoRequestServicesStartupFilter
                            {
                                applicationBuilder = app =>
                                {
                                    Invoke(invokes, app);
                                }
                            };
                        });
                    });
                }
            }            
            #endregion


            #region (x.4)Populate Services
            {
                if (config["Services"] is JArray ja)
                {
                    hostBuilder.ConfigureServices(services =>
                    {
                        services.Populate(ja);
                    });
                }
            }
            #endregion


            return hostBuilder;
        }



        #region AutoRequestServicesStartupFilter
        private class AutoRequestServicesStartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> applicationBuilder;
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    applicationBuilder.Invoke(builder);
                    next(builder);
                };
            }
        }
        #endregion

    }
}
