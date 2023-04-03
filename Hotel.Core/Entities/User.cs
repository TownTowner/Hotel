using System.ComponentModel.DataAnnotations;

namespace Hotel.Core
{
    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 客人
        /// </summary>
        Guest = 0,
        /// <summary>
        /// 管理员
        /// </summary>
        Employee = 1
    }
    /// <summary>
    /// 用户
    /// </summary>
    public class User : EntityCommon
    {
        [StringLength(200)]
        public new string Name { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(200)]
        public string Tel { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(500)]
        public string Address { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; } = UserType.Guest;
    }
}
