using FluentData;
using System.Diagnostics;
using FCP.Util;
using System;
using System.Configuration;

namespace FCP.Data
{
    /// <summary>
    /// DbContext工厂
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        #region 私有变量
        private string _connectionString;  //数据库连接串
        private string _providerName;   //数据库提供程序名
        private IDbProvider _dbProvider;  //数据库提供程序
        #endregion               

        #region 构造函数
        public DbContextFactory()
            : this("defaultConnection")
        {            
        }

        public DbContextFactory(string connectionName)
            : this(getConfigConnectionString(connectionName))
        {
        }

        public DbContextFactory(ConnectionStringSettings connectionSettings)
            : this(connectionSettings.ConnectionString, connectionSettings.ProviderName)
        {            
        }

        public DbContextFactory(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
        }
        #endregion

        #region 属性
        public string connectionString
        {
            get { return _connectionString; }
        }

        public string providerName
        {
            get { return _providerName; }
        }

        public IDbProvider dbProvider
        {
            get
            {
                _dbProvider = _dbProvider ?? getDbProvider(providerName);
                return _dbProvider;               
            }
        }
        #endregion        

        #region 私有静态方法

        #region 获取连接串配置
        private static ConnectionStringSettings getConfigConnectionString(string connectionName)
        {
            var connectionObject = ConfigurationManager.ConnectionStrings[connectionName];
            if (connectionObject == null)
            {
                throw new ArgumentNullException("找不到数据库连接配置：" + connectionName);
            }
            return connectionObject;
        }
        #endregion        

        #region 获取数据库provider
        private static DbProviderFactory dbProviderFactory = new DbProviderFactory();

        /// <summary>
        /// 获取数据库provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        private static IDbProvider getDbProvider(string providerName)
        {
            return dbProviderFactory.getDbProvider(providerName);
        }
        #endregion
        
        #endregion        

        public IDbContext openDbContext()
        {
            return new DbContext().ConnectionString(connectionString, dbProvider)
                .OnError(traceDbError);
        }

        public IDbContext openDbContext(System.Data.Common.DbProviderFactory adoNetProviderFactory)
        {
            return new DbContext().ConnectionString(connectionString, dbProvider, adoNetProviderFactory)
                .OnError(traceDbError);
        }

        /// <summary>
        /// 记录数据库执行异常
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void traceDbError(ErrorEventArgs eventArgs)
        {
            string errorMessage = string.Format("数据库执行异常：{0}", eventArgs.Exception.FormatLogMessage());
            Trace.TraceError(errorMessage);
        }
    }
}
