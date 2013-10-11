using System.Web.Http;
using Microsoft.Practices.Unity;
using Repository;
using Cache;
using Operations;

namespace WebAPI
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // e.g. container.RegisterType<ITestService, TestService>(); 
            container.RegisterType<IBlobRepository, BlobRepository>();
            container.RegisterType<IAzureCache, AzureCache>();

            container.RegisterType<IOperations, Operations.Operations>(
                new InjectionConstructor(
                    container.Resolve<IBlobRepository>(), 
                    container.Resolve<IAzureCache>()));

            return container;
        }
    }
}