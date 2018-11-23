using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace BlogCompiler
{
    class FileSystem
    {
        public void Run()
        {
            String savePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["SavePath"])).FullName;
            String templatePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["TemplatePath"])).FullName;
            List<String> excep = new List<string>()
            {
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplateList"])).FullName,
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplateEmpty"])).FullName,
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplatePost"])).FullName
            };
            DirectoryInfo dir = new DirectoryInfo(templatePath);
            foreach (var d in dir.GetFiles())
            {
                if (excep.Contains(d.FullName))
                {
                    continue;
                }
                CopyDirectory(savePath, Environment.CurrentDirectory, d.FullName);
            }
            foreach (var d in dir.GetDirectories())
            {
                CopyDirectory(savePath, dir.FullName, d.FullName);
            }
        }
        private void CopyDirectory(String savePath, String root, String path)
        {
            if (Directory.Exists(path))
            {
                String rel = path.Replace(root, "");
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (var d in dir.GetDirectories())
                {
                    CopyDirectory(savePath, root, d.FullName);
                }
                foreach (var f in dir.GetFiles())
                {
                    FileInfo des = new FileInfo(savePath + "\\" + rel + "\\" + f.Name);
                    if (!des.Directory.Exists)
                    {
                        des.Directory.Create();
                    }
                    f.CopyTo(des.FullName, true);
                    Console.WriteLine("COPY - Source:" + f.FullName + " Destination:" + des.FullName);
                }
            }
            else
            {
                FileInfo file = new FileInfo(path);
                FileInfo des = new FileInfo(savePath + "\\" + file.Name);
                if (!des.Directory.Exists)
                {
                    des.Directory.Create();
                }
                file.CopyTo(des.FullName, true);
                Console.WriteLine("COPY - Source:" + file.FullName + " Destination:" + des.FullName);
            }

        }
    }
}
