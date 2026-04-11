using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.AutoMapper;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Intent.RoslynWeaver.Attributes;
using Shesha;
using Shesha.Authorization;
using Shesha.Modules;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Boxfusion.Modules.Domain.Module", Version = "1.0")]

namespace BoxFusion.TicketingSystem.Domain
{
    [IntentManaged(Mode.Ignore)]
    /// <summary>
    /// TicketingSystemCommon Module
    /// </summary>
    [DependsOn(
        typeof(SheshaCoreModule),
        typeof(SheshaApplicationModule)
    )]
    public class TicketingSystemModule : SheshaModule
    {
        public override SheshaModuleInfo ModuleInfo => new SheshaModuleInfo("BoxFusion.TicketingSystem")
        {
            FriendlyName = "TicketingSystem",
            Publisher = "BoxFusion",
        };

        public override void Initialize()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => cfg.AddMaps(thisAssembly)
            );
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
        }

        public override void PostInitialize()
        {
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(
                typeof(TicketingSystemModule).Assembly,
                moduleName: "TicketingSystemCommon",
                useConventionalHttpVerbs: true);
        }
    }
}