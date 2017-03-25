using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Utils;

namespace Pyloader
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string pythonruntime = Tools.FixPathFormat(Environment.CurrentDirectory) + Properties.Settings.Default.runtime;
            if (!File.Exists(pythonruntime))
            {
                throw new Exception("can't load runtime");
            }

            string PyLoader_RunTime = Tools.FixPathFormat(Tools.CreateTempDirectory());
            string error = "";
            if (!Tools.UnZipFile(pythonruntime, PyLoader_RunTime, out error))
            {
                throw new Exception("unzip exception: ! \r\n" + error);
            }

            AppConfig config = new AppConfig();
            string PyLoader_Root = Tools.FixPathFormat(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            string PyLoader_Home = Tools.FixPathFormat(Path.Combine(PyLoader_RunTime, "home"));
            string Runmodule = PyLoader_Root + config.GetConfig<string>("run", string.Empty);

            if (File.Exists(Runmodule))
            {
                if (!Tools.UnZipFile(Runmodule, PyLoader_Home, out error))
                {
                    throw new Exception("unzip exception: ! \r\n" + error);
                }
            }

            //System.Environment.CurrentDirectory = PyLoader_RunTime;
            string envpath = Environment.GetEnvironmentVariable("path");
            envpath += ";" + PyLoader_RunTime;
            Environment.SetEnvironmentVariable("path", envpath);

            Assembly assembly = Tools.LoadAssembly(Path.Combine(PyLoader_RunTime, "Pyloader.lib.dll"));
            Type type = assembly.GetType("IronPythonLoader.Loader");

            try
            {
                //type.InvokeMember("Main", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
                MethodInfo method = type.GetMethod("Main", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, new object[] { args });
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.GetFullPath("./Pyloader.log"), e.Message);
            }

            DirectoryInfo di = new DirectoryInfo(PyLoader_RunTime);
            FileInfo[] fs = di.GetFiles();
            foreach (FileInfo fi in fs)
            {
                try
                {
                    File.Delete(fi.FullName);
                }
                catch (Exception)
                {
                    //donothing;
                }
            }

            return;
        }
    }
}
