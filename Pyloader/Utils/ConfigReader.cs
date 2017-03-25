using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;

namespace Utils
{
    public class AppConfig
    {
        private Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public AppConfig()
        {

        }

        ~AppConfig()
        {
            //Save();
        }

        public T GetConfig<T>(string key, T defaultval) //where T : struct, new()
        {
            //T ta = new T();
            Type t = typeof(T);
            if (!config.AppSettings.Settings.AllKeys.Contains(key))
            {
                /*
                if (t.IsValueType)
                {
                    return new T();
                }
                return (T)t.Assembly.CreateInstance(t.Name);
                    * */
                SetConfig<T>(key, defaultval);
                return defaultval;
            }

            return (T)Convert.ChangeType(config.AppSettings.Settings[key].Value, t);
        }

        public void SetConfig<T>(string key, T value) //where T : struct, new()
        {
            //T ta = new T();
            if (config.AppSettings.Settings.AllKeys.Contains(key))
            {
                config.AppSettings.Settings[key].Value = value.ToString();
            }
            else
            {
                config.AppSettings.Settings.Add(key, value.ToString());
            }
        }

        /*
        public void Save()
        {
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        */
    }
}
