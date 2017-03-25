using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using IronPython.Compiler;
using IronPython.Modules;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System.IO;

using Share.Utils;

namespace IronPython.Hosting
{
    /// <summary>
    /// A simple Python command-line should mimic the standard python.exe
    /// </summary>
    public partial class NewPythonCommandLine : CommandLine
    {
        private static string PythonContext_GetVersionString = string.Empty;

        private CodeContext PythonContext_SharedContext { get { return PythonContext.GetPrivateProperty<CodeContext>("SharedContext"); } }

        private PythonTuple PythonTuple_EMPTY { get { return new PythonTuple(); } }

        private PythonOptions PythonContext_PythonOptions { get { return PythonContext.GetPrivateProperty<PythonOptions>("PythonOptions"); } }

        private string PythonContext_InitialPrefix { get { return PythonContext.GetPrivateProperty<string>("InitialPrefix"); } }
        private void PythonContext_InsertIntoPath(int index, string directory)
        {
            PythonContext.CallPrivateMethod("InsertIntoPath", index, directory);
        }

        private void modCtx_InitializeBuiltins(ModuleContext modCtx, bool moduleBuiltins)
        {
            modCtx.CallPrivateMethod("InitializeBuiltins", moduleBuiltins);
        }
        private void PythonContext_AddToPath(string directory)
        {
            PythonContext.CallPrivateMethod_Overload("AddToPath", new Type[] {typeof(string)}, directory);
        }

        private object PythonContext_GetSystemStateValue(string name)
        {
            return PythonContext.CallPrivateMethod<object>("GetSystemStateValue", name);
        }

        private void PythonContext_SetSystemStateValue(string name, object value)
        {
            PythonContext.CallPrivateMethod("SetSystemStateValue", name, value);
        }

        private void PythonContext_SetHostVariables(string prefix, string executable, string versionString)
        {
            PythonContext.CallPrivateMethod_Overload("SetHostVariables", new Type[] {typeof(string), typeof(string), typeof(string)}, prefix, executable, versionString);
        }

        internal bool Importer_TryImportMainFromZip(CodeContext/*!*/ context, string/*!*/ path, out object importer)
        {
            //return (bool)Reflectionutils.CallPrivateStaticMethod(typeof(Importer), "TryImportMainFromZip", context, path, out importer);
            //object tempimporter = null;
            object[] _params = new object[] { context, path, new object() };
            bool ret = Reflectionutils.CallPrivateStaticMethod<bool>(typeof(Importer), "TryImportMainFromZip", _params) ;
            importer = _params[2];
            return ret;
        }

        private PythonDictionary PythonContext_BuiltinModuleInstance___dict__   {
            get { return PythonContext.BuiltinModuleInstance.GetPrivateProperty<PythonDictionary>("__dict__"); }
        }

        public string Runtime_Path { get {
                return System.Environment.GetEnvironmentVariable("IRONPYTHON_RUNTIME");
            }
        }
        public string Package_Path { get {
                return System.Environment.GetEnvironmentVariable("IRONPYTHON_PACKAGE");
            }  }
        public string Lib_Path { get {
                return System.Environment.GetEnvironmentVariable("IRONPYTHON_LIB");
            }
        }
        public string Home_Path { get {
                return System.Environment.GetEnvironmentVariable("IRONPYTHON_HOME");
            }
        }
        public void initializeRunTime()
        {
            string dir = Runtime_Path;
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (file.ToLower().EndsWith(".dll"))
                    {
                        try
                        {
                            Language.DomainManager.LoadAssembly(Assembly.LoadFrom(file));
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void InitializeHome()
        {
            //string dir = Path.Combine(PythonContext_InitialPrefix, "DLLs");
            string dir = Home_Path;
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (file.ToLower().EndsWith(".dll"))
                    {
                        try
                        {
                            ClrModule.AddReference(PythonContext_SharedContext, new FileInfo(file).Name);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public void InitializeLib()
        {
            if (!string.IsNullOrEmpty(Lib_Path))
            {
                foreach (string lib in Lib_Path.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    PythonContext_AddToPath(lib);
                }                
            }
        }
    }
}

namespace IronPython.Runtime
{
    internal partial class NewOutputWriter : TextWriter
    {
        private CodeContext _context_SharedContext { get { return _context.GetPrivateProperty<CodeContext>("SharedContext"); } }

        private object _context_SystemStandardError { get { return _context.GetPrivateProperty<object>("SystemStandardError"); } }

        private object _context_SystemStandardOut { get { return _context.GetPrivateProperty<object>("SystemStandardOut"); } }

        private Encoding file_Encoding { get {
                PythonFile file = Sink as PythonFile;
                return (file != null) ? file.GetPrivateProperty<Encoding>("Encoding") : null;
            }
        }
    }
}
