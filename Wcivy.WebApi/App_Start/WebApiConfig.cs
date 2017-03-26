using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Wcivy.Core.Http;
using Wcivy.Core.Http.Filters;

namespace Wcivy.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // 签名验证BasicAuthFilter
            config.Filters.Add(new SignatureAttribute());
            // 截获并处理Action执行过程中发生的异常
            config.Filters.Add(new ActionExceptionAttribute());
            // 截获并处理Action执行过程之外发生的异常
            config.Services.Replace(typeof(IExceptionHandler), new UnhandledExceptionHandler());
            // 异常日志
            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
            // 返回统一对象
            config.MessageHandlers.Add(new ApiDelegatingHandler());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 使用json格式，删除默认的xml格式
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
