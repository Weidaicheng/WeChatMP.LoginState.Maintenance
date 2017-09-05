using System.Configuration;

namespace Configuration.Helper
{
    public static class ConfigurationHelper
    {
        #region field
        private static string appId;
        private static string appSecret;
        private static string weChatApiAddr;
        private static string redisServerHost;
        private static int? redisServerPort;
        private static string redisPassword;
        private static int? expireDays;
        #endregion

        #region 属性
        /// <summary>
        /// AppId
        /// </summary>
        public static string AppId
        {
            get
            {
                if (string.IsNullOrEmpty(appId))
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
                if (string.IsNullOrEmpty(appSecret))
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
                if (string.IsNullOrEmpty(weChatApiAddr))
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
                if (string.IsNullOrEmpty(redisServerHost))
                {
                    redisServerHost = ConfigurationManager.AppSettings["RedisServerHost"];
                }

                return redisServerHost;
            }
        }

        /// <summary>
        /// Redis服务器端口
        /// </summary>
        public static int? RedisServerPort
        {
            get
            {
                if (redisServerPort == null)
                {
                    redisServerPort = int.Parse(ConfigurationManager.AppSettings["RedisServerPort"]);
                }

                return redisServerPort;
            }
        }

        /// <summary>
        /// Redis密码
        /// </summary>
        public static string RedisPassword
        {
            get
            {
                if (string.IsNullOrEmpty(redisPassword))
                {
                    redisPassword = ConfigurationManager.AppSettings["RedisPassword"];
                }

                return redisPassword;
            }
        }

        /// <summary>
        /// 过期天数
        /// </summary>
        public static int? ExpireDays
        {
            get
            {
                if (expireDays == null)
                {
                    expireDays = int.Parse(ConfigurationManager.AppSettings["ExpireDays"]);
                }

                return expireDays;
            }
        }
        #endregion
    }
}
