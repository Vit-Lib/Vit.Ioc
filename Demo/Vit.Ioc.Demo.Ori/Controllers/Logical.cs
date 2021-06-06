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
