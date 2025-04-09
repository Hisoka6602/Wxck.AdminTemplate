using System.ComponentModel;

namespace Wxck.AdminTemplate.CommsCore.Enums.User {

    public enum LoginMethod {

        /// <summary>
        /// 用户名登录
        /// </summary>
        [Description("用户名/用户代码")]
        LoginCode,

        /// <summary>
        /// 手机号登录
        /// </summary>
        [Description("手机号")]
        Phone,

        /// <summary>
        /// 微信扫码登录
        /// </summary>
        [Description("微信扫码")]
        WeChatQrCode,

        /// <summary>
        /// QQ扫码登录
        /// </summary>
        [Description("QQ扫码")]
        QqQrCode,

        /// <summary>
        /// 支付宝扫码登录
        /// </summary>
        [Description("支付宝扫码")]
        AlipayQrCode
    }
}