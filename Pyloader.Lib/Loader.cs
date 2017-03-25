using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Hosting.Shell;

using IronPython.Hosting;
using IronPython.Runtime;

namespace IronPythonLoader
{
    public class Loader
    {
        private const string CONSOLE = "console";
        private const string RUN = "run";
        private const string LIB = "library";
        //private const string PATH = "path";
        private const string OTHER = "other";
        private const string ASSEMBLY = "assembly";

        private const string IRONPYTHONPATH = "IRONPYTHONPATH";
        private const string IRONPYTHON_RUNTIME = "IRONPYTHON_RUNTIME";
        private const string IRONPYTHON_PACKAGE = "IRONPYTHON_PACKAGE";
        private const string IRONPYTHON_ROOT = "IRONPYTHON_ROOT";
        private const string IRONPYTHON_HOME = "IRONPYTHON_HOME";
        private const string IRONPYTHON_LIB = "IRONPYTHON_LIB";

        private static string PyLoader_Root = string.Empty;
        private static string PyLoader_RunTime = string.Empty;
        private static string PyLoader_Package = string.Empty;
        private static string PyLoader_Home = string.Empty;
        private static string PyLoader_Lib = string.Empty;

        //private static bool PyLoader_Protable = false;

        private static ConsoleService.ConsoleParams Consoleparams = new ConsoleService.ConsoleParams();

        //[STAThread]
        public static int Main(string[] args)
        {
            InitializeEnvirment();                 

            if (args.Length == 0)
            {                
                InitlizeParams(ref args);
                InitlizeModules();
            }

            Consoleparams.OnExecute = OnExecute;
            
            return OpenConsole(args, Consoleparams);
        }

        public static void InitializeEnvirment()
        {
            /// PyLoader根目录，默认当前路径
            PyLoader_Root = Tools.FixPathFormat(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            //PyLoader_Root = Tools.FixPathFormat(Environment.CurrentDirectory);
            PyLoader_RunTime = Tools.FixPathFormat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //PyLoader_RunTime = Tools.FixPathFormat(Environment.CurrentDirectory);
            System.Environment.SetEnvironmentVariable(IRONPYTHON_ROOT, PyLoader_Root);

            /*
            /// PyLoader运行环境，默认当前路径，检测到zip文件时指定到临时目录
            string pythonruntime = Tools.FixPathFormat(Environment.CurrentDirectory) + Properties.Settings.Default.runtime;
            if (File.Exists(pythonruntime))
            {
                PyLoader_RunTime = Tools.FixPathFormat(Tools.CreateTempDirectory());
            }
            */
            System.Environment.SetEnvironmentVariable(IRONPYTHON_RUNTIME, PyLoader_RunTime);

            /// 执行脚本目录，PyLoader运行环境/home
            PyLoader_Home = Tools.FixPathFormat(PyLoader_RunTime + "home");
            System.Environment.SetEnvironmentVariable(IRONPYTHON_HOME, PyLoader_Home);

            /// PyLoader扩展，PyLoader运行环境/DLLs
            PyLoader_Package = Tools.FixPathFormat(PyLoader_RunTime + "DLLs");
            System.Environment.SetEnvironmentVariable(IRONPYTHON_PACKAGE, PyLoader_Package);

            /// Lib路径，PyLoader运行环境/Lib，如果有指定参数，以参数为准
            PyLoader_Lib = Path.Combine(PyLoader_RunTime, "Lib");
            AppConfig config = new AppConfig();
            if (!string.IsNullOrEmpty(config.GetConfig<string>(LIB, string.Empty)))
            {
                string[] libs = config.GetConfig<string>(LIB, string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < libs.Length; i++) libs[i] = Path.Combine(PyLoader_RunTime, libs[i]);
                PyLoader_Lib = string.Join(";", libs);
            }
            System.Environment.SetEnvironmentVariable(IRONPYTHON_LIB, PyLoader_Lib);

            /// 设置IRONPYTHONPATH
            string pythonpath = System.Environment.GetEnvironmentVariable(IRONPYTHONPATH);

            List<string> pythonpathes = new List<string>();
            pythonpathes.Add(PyLoader_RunTime);
            pythonpathes.Add(PyLoader_Package);
            pythonpathes.Add(PyLoader_Home);

            string addpath = string.Join(";", pythonpathes);

            pythonpath = (string.IsNullOrEmpty(pythonpath) ? string.Empty : pythonpath) + ";" + addpath;
            if (!string.IsNullOrEmpty(pythonpath))
            {
                System.Environment.SetEnvironmentVariable(IRONPYTHONPATH, pythonpath);
            }

            /*
            /// 解压环境文件
            if (File.Exists(pythonruntime))
            {
                string error = "";
                if (!Tools.UnZipFile(pythonruntime, PyLoader_RunTime, out error))
                {
                    throw new Exception("unzip exception: ! \r\n" + error);
                }
            }
            */
        }

        public static void InitlizeModules()
        {
            AppConfig appconfig = new AppConfig();
            string[] assemblys = appconfig.GetConfig<string>(ASSEMBLY, "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string assembly in assemblys)
            {
                Tools.LoadAssembly(Path.Combine(PyLoader_RunTime, assembly));
            }

            System.Environment.CurrentDirectory = PyLoader_Home;
        }

        public static void InitlizeParams(ref string[] args)
        {
            //begin
            List<string> newargs = new List<string>();
            AppConfig config = new AppConfig();
            //run
            if (!string.IsNullOrEmpty(config.GetConfig<string>(RUN, string.Empty)))
            {
                //newargs.Add(Path.GetFullPath(config.GetConfig<string>(RUN, string.Empty) + "\\runbuilder.py"));
                newargs.Add(PyLoader_Home + "main.py");
            }

            //others
            if (!string.IsNullOrEmpty(config.GetConfig<string>(OTHER, string.Empty)))
            {
                string[] otherargs = config.GetConfig<string>(OTHER, string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                newargs.AddRange(otherargs);
            }

            /*
            //path
            if (!string.IsNullOrEmpty(config.GetConfig<string>(PATH, string.Empty)))
            {
                string searchpath = Path.GetFullPath(config.GetConfig<string>(PATH, string.Empty));
                string[] searchpathes = searchpath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string path in searchpathes) pythonpathes.Add(Path.Combine(PyLoader_RunTime, path));
            }
            */
            args = newargs.ToArray();
        }

        public static int OpenConsole(string[] args, ConsoleService.ConsoleParams _params)
        {
            if (args.Length == 0)
            {
                new PythonWindowsConsoleHost(_params).ShowHelp();
                return 1;
            }

            bool RunInConsole = false;
            List<string> newargs = new List<string>();
            AppConfig config = new AppConfig();

            if (!string.IsNullOrEmpty(config.GetConfig<string>(CONSOLE, string.Empty)))
            {
                RunInConsole = config.GetConfig<bool>(CONSOLE, false);
            }

            Process p = Process.GetCurrentProcess();
            Process pparent = p.Parent();
            if (Path.GetFileName(pparent.ProcessName.ToLower()) == "cmd")
            {
                RunInConsole = true;
            }

            if (args.Length == 1 && ((args[0] == "-c") || (args[0] == "/c")))
            {
                RunInConsole = true;
                args = new string[0];
            }

            if (RunInConsole && (Environment.GetEnvironmentVariable("TERM") == null))
            {
                Environment.SetEnvironmentVariable("TERM", "dumb");
            }

            return RunInConsole ? new PythonConsoleHost(_params).Run(args) : new PythonWindowsConsoleHost(_params).Run(args);
        }

        public static void OnExecute(ConsoleService host, ScriptEngine engine, PythonContext context)
        {

        }
    }

    #region TOOLS
    public static class Tools
    {
        public static Func<string, string> ResolvePaths = delegate (string paths)
        {
            if (string.IsNullOrEmpty(paths)) return string.Empty;
            string[] pathitems = paths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathitems.Length; i++)
            {
                pathitems[i] = Path.GetFullPath(pathitems[i]);
            }
            return string.Join(";", pathitems);
        };

        public static Func<string, string> ResolveNullString = x => { return string.IsNullOrEmpty(x) ? string.Empty : x; };

        public static Func<string, string> FixPathFormat = x => { return x.EndsWith(@"\") ? x : x + "\\"; };

        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFile">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="error">出错信息</param>
        /// <returns>解压是否成功</returns>
 
        public static string CreateTempDirectory()
        {
            string tempdir = Tools.FixPathFormat(Path.GetTempFileName() + "1");
            if (!System.IO.Directory.Exists(tempdir))
                System.IO.Directory.CreateDirectory(tempdir);
            return tempdir;
        }

        public static Assembly LoadAssembly(string assembly_library)
        {
            Assembly assembly = Assembly.LoadFrom(assembly_library);
            return assembly;
        }

        public static Process Parent(this Process process)
        {
            Func<int, string> FindIndexedProcessName = delegate(int pid)
            {
                var processName = Process.GetProcessById(pid).ProcessName;
                var processesByName = Process.GetProcessesByName(processName);
                string processIndexdName = null;
                for (var index = 0; index < processesByName.Length; index++)
                {
                    processIndexdName = index == 0 ? processName : processName + "#" + index;
                    var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                    if ((int)processId.NextValue() == pid)
                    {
                        return processIndexdName;
                    }
                }
                return processIndexdName;
            };

            Func<string, Process> FindPidFromIndexedProcessName = delegate(string indexedProcessName) 
            {
                var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
                return Process.GetProcessById((int)parentId.NextValue());
            };

            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }

    }

    #endregion
}
