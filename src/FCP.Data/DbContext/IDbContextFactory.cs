using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCP.Data
{
    /// <summary>
    /// DbContext工厂 接口
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        string connectionString { get; }

        /// <summary>
        /// 数据提供程序名称
        /// </summary>
        string providerName { get; }

        /// <summary>
        /// 数据库provider
        /// </summary>
        IDbProvider dbProvider { get; }

        /// <summary>
        /// 新建数据库会话
        /// </summary>
        /// <returns></returns>
        IDbContext openDbContext();
    }
}
