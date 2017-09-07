using Autofac;
using Autofac.Integration.WebApi;
using Cache.Redis;
using Configuration.Helper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebAPI.Controllers;
using WeChat.Core;

namespace WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Autofac
            HttpConfiguration config = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<WeChatLoginController>();
            builder.RegisterType<WeChatServiceHandler>();

            builder.RegisterType<RedisHandler>().As<RedisHandler>();
            builder.RegisterType<WeChatServiceHandler>().As<WeChatServiceHandler>();
            builder.Register(c => new RestClient(ConfigurationHelper.WeChatApiAddr)).As<IRestClient>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
