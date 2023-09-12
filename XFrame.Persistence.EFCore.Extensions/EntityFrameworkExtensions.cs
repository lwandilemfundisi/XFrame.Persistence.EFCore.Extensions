using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFrame.DomainContainers;

namespace XFrame.Persistence.EFCore.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IDomainContainer RegisterServices(
            this IDomainContainer domainContainer,
            Action<IServiceCollection> registerServices)
        {
            registerServices(domainContainer.ServiceCollection);
            return domainContainer;
        }

        public static IDomainContainer ConfigurePersistence<TDbContext, TContextProvider>(
            this IDomainContainer domainContainer)
             where TContextProvider : class, IDbContextProvider<TDbContext>
            where TDbContext : DbContext
        {
            return domainContainer
                .ConfigureEntityFramework(EntityFrameworkConfiguration.New)
                .AddDbContextProvider<TDbContext, TContextProvider>()
                .RegisterServices(sr =>
                {
                    sr.AddTransient<IPersistenceFactory, EntityFrameworkPersistenceFactory<TDbContext>>();
                });
        }

        public static IDomainContainer ConfigureEntityFramework(
            this IDomainContainer domainContainer,
            IEntityFrameworkConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            configuration.Apply(domainContainer.ServiceCollection);
            return domainContainer;
        }

        public static IDomainContainer AddDbContextProvider<TDbContext, TContextProvider>(
            this IDomainContainer domainContainer)
            where TContextProvider : class, IDbContextProvider<TDbContext>
            where TDbContext : DbContext
        {
            domainContainer
                .ServiceCollection
                .AddTransient<IDbContextProvider<TDbContext>, TContextProvider>();

            return domainContainer;
        }
    }
}
