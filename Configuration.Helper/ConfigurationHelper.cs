using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Helper
{
    public static class ConfigurationHelper
    {
        #region field
        private static string appId;
        private static string appSecret;
        private static string weChatApiAddr;
        private static string redisServerHost;
        private static int? redisServerPort = 6379;
        private static int? expireDays = 20;
        #endregion

        #region 属性
        /// <summary>
        /// AppId
        /// </summary>
        public static string AppId
        {
            get
            {
                if(string.IsNullOrEmpty(appId))
                {
                    appId = ConfigurationManager.AppSettings["AppId"];
                }

                return appId;
            }
        }

        /// <summary>
        /// AppSecret
        /// </summary>
        public static string AppSecret
        {
            get
            {
                if(string.IsNullOrEmpty(AppSecret))
                {
                    appSecret = ConfigurationManager.AppSettings["AppSecret"];
                }

                return appSecret;
            }
        }

        /// <summary>
        /// 微信API地址
        /// </summary>
        public static string WeChatApiAddr
        {
            get
            {
                if(string.IsNullOrEmpty(weChatApiAddr))
                {
                    weChatApiAddr = ConfigurationManager.AppSettings["WeChatApiAddr"];
                }

                return weChatApiAddr;
            }
        }

        /// <summary>
        /// Redis服务器地址
        /// </summary>
        public static string RedisServerHost
        {
            get
            {
                if(string.IsNullOrEmpty(redisServerHost))
                {
                    redisServerHost = ConfigurationManager.AppSettings["RedisServerHost"];
                }

                return redisServerHost;
            }
        }

        /// <summary>
        /// Redis服务器端口（默认：6379）
        /// </summary>
        public static int? RedisServerPort
        {
            get
            {
                if(redisServerPort == null)
                {
                    redisServerPort = int.Parse(ConfigurationManager.AppSettings["RedisServerPort"]);
                }

                return redisServerPort;
            }
        }

        /// <summary>
        /// 过期天数
        /// </summary>
        public static int? ExpireDays
        {
            get
            {
                if(expireDays == null)
                {
                    expireDays = long.Parse(ConfigurationManager.AppSettings["ExpireDays"]);
                }

                return expireDays;
            }
        }
        #endregion
    }
}
