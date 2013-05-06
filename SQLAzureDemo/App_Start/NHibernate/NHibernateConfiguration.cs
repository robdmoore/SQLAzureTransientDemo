using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Driver;

namespace SQLAzureDemo.App_Start.NHibernate
{
    public class NHibernateConfiguration<TDriver>
        where TDriver : IDriver
    {
        private readonly IPersistenceConfigurer _databaseConfig;

        public NHibernateConfiguration(string connectionString)
        {
            _databaseConfig = MsSqlConfiguration.MsSql2008
                .ConnectionString(connectionString)
                .Driver<TDriver>();
        }

        public ISessionFactory GetSessionFactory()
        {
            var config = Fluently.Configure()
                .Database(_databaseConfig)
                .Mappings(x => x.AutoMappings.Add(
                    AutoMap.Assembly(GetType().Assembly)
                    .Where(type => type.Namespace != null && type.Namespace.EndsWith("Database.Models"))
                ));

            return config.BuildSessionFactory();
        }
    }
}