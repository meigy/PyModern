using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace CSharp.Script
{ 
    public static class CSCompiler
    {
        private enum CSCRETURNCODE
        {
            NONE = 0,
            CODEERROR = 1,            
            CREATEERROR = 3,
            EXECUTEERROR = 5,
            UNKOWNERROR = -1,
        }

        public static List<string> ExtendRefs = new List<string>();
        public static string BaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static CompilerResults Compile(string csscript, string output, out int error, out string message)
        {
            error = (int)CSCRETURNCODE.UNKOWNERROR;
            message = "";
            //get the code to compile
            string strSourceCode = csscript;            
            if (File.Exists(csscript))
            {
                strSourceCode = File.ReadAllText(csscript);
            }

            // 1.Create a new CSharpCodePrivoder instance
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            /*
            objCompilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            objCompilerParameters.ReferencedAssemblies.Add(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PresentationCore.dll"));
            objCompilerParameters.ReferencedAssemblies.Add(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PresentationFramework.dll"));
            objCompilerParameters.ReferencedAssemblies.Add(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "System.Xaml.dll"));
            objCompilerParameters.ReferencedAssemblies.Add(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WindowsBase.dll"));
            objCompilerParameters.ReferencedAssemblies.Add("System.Xml.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            */
            foreach (var reflib in ExtendRefs) objCompilerParameters.ReferencedAssemblies.Add(reflib);
            objCompilerParameters.GenerateInMemory = string.IsNullOrEmpty(output);
            objCompilerParameters.OutputAssembly = output;

            // 3.CompilerResults: Complile the code snippet by calling a method from the provider
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, strSourceCode);

            if (cr.Errors.HasErrors)
            {
                string strErrorMsg = cr.Errors.Count.ToString() + " Errors:";

                for (int x = 0; x < cr.Errors.Count; x++)
                {
                    strErrorMsg = strErrorMsg + "/r/nLine: " +
                                 cr.Errors[x].Line.ToString() + " - " +
                                 cr.Errors[x].ErrorText;
                }

                error = (int)CSCRETURNCODE.CODEERROR;
                message = strErrorMsg;
                //MessageBox.Show("There were build erros, please modify your code.", "Compiling Error");
                return cr;
            }

            error = (int)CSCRETURNCODE.NONE;
            return cr;
        }

        public static int Compile(string csscript, string output, out string message)
        {
            int error = (int)CSCRETURNCODE.UNKOWNERROR;
            message = "";
            try
            {                
                Compile(csscript, output, out error, out message);
            }
            catch (Exception e)
            {
                message += e.Message;
            }

            return (int)error;
        }

        public static object Compile(string csscript, string objecttype, object[] args, out int error, out string message)
        {
            error = (int)CSCRETURNCODE.UNKOWNERROR;
            message = "";
            CompilerResults cr = null;
            try
            {
                cr = Compile(csscript, string.Empty, out error, out message);
                if (error != (int)CSCRETURNCODE.NONE)
                {
                    return error;
                }
            }
            catch (Exception e)
            {
                message += e.Message;
                return error;
            }            

            // 4. Invoke the method by using Reflection
            Assembly objAssembly = cr.CompiledAssembly;
            object objInstance = null;
            if (args == null)
            {
                objInstance = objAssembly.CreateInstance(objecttype);
            }
            else
            {
                objInstance = objAssembly.CreateInstance(objecttype, true, BindingFlags.Default, null, args, null, new object[0]);
            }

            if (objInstance == null)
            {
                error = (int)CSCRETURNCODE.CREATEERROR;
                message = "Error: " + "Couldn't load class.";
                return null;
            }
            return objInstance;
        }

        public static int Execute(string csscript, string mainclass, string[] args, out string message)
        {
            message = "";
            int myerror = (int)CSCRETURNCODE.NONE;
            CompilerResults cr = null;
            try
            {                
                cr = Compile(csscript, string.Empty, out myerror, out message);
            }
            catch (Exception e)
            {
                message += e.Message;
                return (int)myerror;
            }

            try
            {
                Assembly objAssembly = cr.CompiledAssembly;
                Type type = objAssembly.GetType(mainclass);
                object obj = type.InvokeMember("Main", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
            }
            catch (Exception e)
            {
                message += e.Message;
                return (int)CSCRETURNCODE.EXECUTEERROR;
            }

            return (int)CSCRETURNCODE.NONE;
        }

        public static object CreateObject(string assemblyfile, string objecttype, object[] args)
        {
            Assembly objAssembly = Assembly.LoadFrom(assemblyfile);
            object objInstance = null;
            if (args == null)
            {
                objInstance = objAssembly.CreateInstance(objecttype);
            }
            else
            {
                objInstance = objAssembly.CreateInstance(objecttype, true, BindingFlags.Default, null, args, null, new object[0]);
            }

            return objInstance;
        }

        public static object CreateObject(CompilerResults cpr, string objecttype, object[] args, int reserved)
        {
            Assembly objAssembly = cpr.CompiledAssembly;
            object objInstance = null;
            if (args == null)
            {
                objInstance = objAssembly.CreateInstance(objecttype);
            }
            else
            {
                objInstance = objAssembly.CreateInstance(objecttype, true, BindingFlags.Default, null, args, null, new object[0]);
            }

            return objInstance;
        }
    }
}
