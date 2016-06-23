using System.Configuration;

namespace FCP.Util
{
    /// <summary>
    /// 应用程序配置
    /// </summary>
    public static class AppConfigUtil
    {
        /// <summary>
        /// 根据配置节key，获取相应值value
        /// </summary>
        /// <param name="keyName">配置节Key</param>
        /// <returns></returns>
        public static string getConfigValue(string keyName)
        {
            string configValue = ConfigurationManager.AppSettings[keyName];
            return configValue;
        }

        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="keyName">健</param>
        /// <param name="keyValue">值</param>
        /// <returns></returns>
        public static bool updateConfig(string keyName, string keyValue)
        {
            #region
            bool IsOK = true;

            //修改状态
            bool isModified = false;

            //遍历所有节点，有则设置修改状态为真
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == keyName)
                {
                    isModified = true;
                }
            }

            //将当前应用程序的配置文件作为 Configuration 对象打开。 
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (isModified)
            {
                try
                {
                    //修改状态为真，则删除节点
                    config.AppSettings.Settings.Remove(keyName);
                }
                catch
                {
                    IsOK = false;
                }
            }

            if (IsOK)
            {
                try
                {
                    //增加节点
                    config.AppSettings.Settings.Add(keyName, keyValue);
                    //保存 Configuration 对象
                    config.Save(ConfigurationSaveMode.Modified);
                    //刷新命名节，这样在下次检索它时将从磁盘重新读取。
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch
                {
                    IsOK = false;
                }
            }

            return IsOK;
            #endregion
        }
    }
}
