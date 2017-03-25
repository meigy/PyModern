using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using UnzipTools;

namespace Utils
{
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
        public static bool UnZipFile(string zipFile, string unZipDir, out string error)
        {
            error = "";
            try
            {
                Unzip.UnzipFile(zipFile, unZipDir);
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }

            return true;
            /*
            if (!File.Exists(zipFile))
            {
                error = "file not exsist";
                return false;
            }

            if (unZipDir.Length == 0)
                unZipDir = Path.GetFullPath(zipFile) + Path.GetFileNameWithoutExtension(zipFile);
            if (!unZipDir.EndsWith(@"\"))
                unZipDir += @"\";
            if (!System.IO.Directory.Exists(unZipDir))
                System.IO.Directory.CreateDirectory(unZipDir);
            try
            {
                Shell32.ShellClass sc = new Shell32.ShellClass();
                Shell32.Folder SrcFolder = sc.NameSpace(zipFile);
                Shell32.Folder DestFolder = sc.NameSpace(unZipDir);
                Shell32.FolderItems items = SrcFolder.Items();

                DestFolder.CopyHere(items, 20);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
            return true;
            */
        }

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
    }

    #endregion
}
