using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Core
{
    /// <summary>
    /// 餐桌信息
    /// </summary>
    public class Table : EntityCommon
    {
        /// <summary>
        /// 餐桌容量
        /// </summary>
        public int Size { get; set; }
    }
}
