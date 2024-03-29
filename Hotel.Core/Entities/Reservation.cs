﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Core
{
    /// <summary>
    /// 预定状态
    /// </summary>
    public enum ReservationStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        UnHandled = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 1,
        /// <summary>
        /// 取消
        /// </summary>
        Canceled = 2
    }
    /// <summary>
    /// 预定信息
    /// </summary>
    public class Reservation : EntityBase
    {
        /// <summary>
        /// 客户 Id
        /// </summary>
        public int? GuestId { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        [ForeignKey("GuestId")]
        public virtual User Guest { get; set; }
        /// <summary>
        /// 桌号 Id
        /// </summary>
        public int? TableId { get; set; }
        /// <summary>
        /// 桌号
        /// </summary>
        [ForeignKey("TableId")]
        public virtual Table Table { get; set; }
        /// <summary>
        /// 预定时间
        /// </summary>
        public DateTime ReservationTime { get; set; }
        /// <summary>
        /// 预定状态
        /// </summary>
        public ReservationStatus? ReservationStatus { get; set; }
    }
}
