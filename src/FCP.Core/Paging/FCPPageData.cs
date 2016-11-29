using System;
using System.Collections.Generic;

namespace FCP.Core
{
    /// <summary>
    /// FCP分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FCPPageData<T> where T : class
    {
        /// <summary>
        /// 页号
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount
        {
            get
            {
                if (pageSize <= 0) return 0;

                return (int)Math.Ceiling((double)total / pageSize);
            }
        }

        /// <summary>
        /// 列表数据
        /// </summary>
        public IList<T> data { get; set; }
    }
}
