namespace FT.Utility.Helper
{
    using System;
    using System.IO;
    using System.Web;
    using System.Linq;
    public class IoHelper
    {
        public static void CopyFile(string fileFullPath, string destination, bool isDeleteSourceFile = false, string fileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
            {
                throw new ArgumentNullException("fileFullPath", "源文件全路径不能为空");
            }
            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException("找不到源文件", fileFullPath);
            }
            if (!Directory.Exists(destination))
            {
                throw new DirectoryNotFoundException("找不到目标目录 " + destination);
            }
            fileName = string.IsNullOrWhiteSpace(fileName) ? Path.GetFileName(fileFullPath) : fileName;
            File.Copy(fileFullPath, Path.Combine(destination, fileName), true);
            if (isDeleteSourceFile)
            {
                File.Delete(fileFullPath);
            }
        }

        public static long GetDirectoryLength(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return 0L;
            }
            var info = new DirectoryInfo(dirPath);
            var num = info.GetFiles().Sum(info2 => info2.Length);
            var directories = info.GetDirectories();
            if (directories.Length > 0)
            {
                num += directories.Sum(t => GetDirectoryLength(t.FullName));
            }
            return num;
        }

        public static string GetMapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            var applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (string.IsNullOrWhiteSpace(path)) return (applicationBase + path);
            path = path.Replace("/", @"\");
            if (!path.StartsWith(@"\"))
                path = @"\" + path;
            path = path.Substring(path.IndexOf('\\') + (applicationBase.EndsWith(@"\") ? 1 : 0));
            return (applicationBase + path);
        }
    }
}

