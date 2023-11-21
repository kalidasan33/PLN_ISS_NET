using LoggingUtil;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Mvc;
//using ISS.BusinessService.Contract;
//using ISS.BusinessService.Implementation;
using System.Web.Mvc;
using ISS.BusinessRules.Contract.Order;
using ISS.BusinessService.Implementation.Order;
using ISS.BusinessRules.Contract.Common;
using ISS.BusinessService.Implementation.Common;
using ISS.BusinessRules.Contract.Information;
using ISS.BusinessRules.Contract.Capacity;
using ISS.BusinessRules.Contract.Textiles;
 

using ISS.BusinessService.Implementation.Information;
using ISS.BusinessService.Implementation.Capacity;
using ISS.BusinessService.Implementation.Textiles;

using KA.BusinessRules.Contract.BulkOrder;
using KA.BusinessService.Implementation.BulkOrder;
using KA.BusinessRules.Contract.AttributionOrder;
using KA.BusinessService.Implementation.AttributionOrder;
using KA.BusinessRules.Contract.MaterialSupply;
using KA.BusinessService.Implementation.MaterialSupply;


namespace Bootstrapper
{
    public static class UnityBootstrapper
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer Initialize()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
           
            return container;
        }

        /// <summary>
        /// Builds the unity container.
        /// </summary>
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            RegisterTypes(container);
            return container;
        }

        /// <summary>
        /// Registers types.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ILogger, Logger>();            

            container.RegisterType<IInformationService, InformationService>();

            container.RegisterType<IOrderService, OrderService>();  
            container.RegisterType<IApplicationService, ApplicationService>();
            container.RegisterType<ICapacityService, CapacityService>();
            container.RegisterType<ITextilesService, TextilesService>();
            container.RegisterType<IBulkOrderService, BulkOrderService>();
            container.RegisterType<IAttributionOrderService, AttributionOrderService>();
            container.RegisterType<IMaterialSupplyService, MaterialSupplyService>();
        }
    }
}