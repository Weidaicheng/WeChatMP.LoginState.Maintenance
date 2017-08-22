using NLog;
using System;
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
        private static int? expireDays;
        #endregion

        #region log
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region 属性
        /// <summary>
        /// AppId
        /// </summary>
        public static string AppId
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(appId))
                    {
                        appId = ConfigurationManager.AppSettings["AppId"];
                    }

                    return appId;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// AppSecret
        /// </summary>
        public static string AppSecret
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(appSecret))
                    {
                        appSecret = ConfigurationManager.AppSettings["AppSecret"];
                    }

                    return appSecret;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 微信API地址
        /// </summary>
        public static string WeChatApiAddr
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(weChatApiAddr))
                    {
                        weChatApiAddr = ConfigurationManager.AppSettings["WeChatApiAddr"];
                    }

                    return weChatApiAddr;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Redis服务器地址
        /// </summary>
        public static string RedisServerHost
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(redisServerHost))
                    {
                        redisServerHost = ConfigurationManager.AppSettings["RedisServerHost"];
                    }

                    return redisServerHost;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Redis服务器端口（默认：6379）
        /// </summary>
        public static int? RedisServerPort
        {
            get
            {
                try
                {
                    if (redisServerPort == null)
                    {
                        redisServerPort = int.Parse(ConfigurationManager.AppSettings["RedisServerPort"]);
                    }

                    return redisServerPort;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 过期天数
        /// </summary>
        public static int? ExpireDays
        {
            get
            {
                try
                {
                    if (expireDays == null)
                    {
                        expireDays = int.Parse(ConfigurationManager.AppSettings["ExpireDays"]);
                    }

                    return expireDays;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw ex;
                }
            }
        }
        #endregion
    }
}
