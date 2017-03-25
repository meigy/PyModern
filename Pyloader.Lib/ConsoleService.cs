using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Hosting.Shell;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

using Share.Utils;

namespace IronPythonLoader
{
    public class ConsoleService: ConsoleHost
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        public delegate void delegateOnExecute(ConsoleService host, ScriptEngine engine, PythonContext context);
        private delegateOnExecute OnExecute = null;

        public bool isConsole = false;

        //private PythonCommandLine Pycommandline = new PythonCommandLine();
        //private PythonContext Context { get { return Pycommandline.GetPrivateProperty<PythonContext>("PythonContext"); } }

        public class ConsoleParams
        {
            public delegateOnExecute OnExecute { get; set; }
        }

        public ConsoleService(ConsoleParams param) : base()
        {
            OnExecute = param.OnExecute;
            Init();

            if (isConsole)
            {
                AllocConsole();
            }
        }      

        ~ConsoleService()
        {
            if (isConsole)
            {
                try
                {
                    //FreeConsole();
                }
                catch (Exception)
                {                    
                }                
            }
        }

        protected virtual void Init()
        { }
        
        /*
        ~ConsoleService()
        {
            Pycommandline = null;
        }
        */
        protected override CommandLine CreateCommandLine()
        {
            return new NewPythonCommandLine();
            /*
            if (Pycommandline == null)
            {
                Pycommandline = new PythonCommandLine();
            }
            return Pycommandline;
            */
        }


        protected override void ExecuteInternal()
        {
            if (OnExecute != null)
            {
                OnExecute(this, Engine, HostingHelpers.GetLanguageContext(Engine) as PythonContext);
            }         

            base.ExecuteInternal();

            if (isConsole && (this.CommandLine.ExitCode != 0))
            {
                Console.ReadKey();
            }
        }

        /*
        public void LoadAssemblys(string[] assemblyfiles)
        {
            foreach (string module in assemblyfiles)
            {
                Assembly a = Assembly.LoadFrom(module);
                Context.DomainManager.LoadAssembly(a);
            }            
        }
                
        public void LoadPackages(string[] assemblyfiles)
        {
            foreach (string module in assemblyfiles)
            {
                try
                {
                    ClrModule.AddReference(Context.GetPrivateProperty<CodeContext>("SharedContext"), new FileInfo(module).Name);
                }
                catch
                {
                }
            }
        }
        */
    }

}
