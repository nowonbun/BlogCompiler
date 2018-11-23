using System;
using System.Collections.Generic;
using System.Text;
using BlogCompiler.Entity;
using System.IO;

namespace BlogCompiler
{
    abstract class CommonSystem
    {
        protected void WriteFile(String filepath, StringBuilder html)
        {
            byte[] data = Encoding.UTF8.GetBytes(html.ToString());
            FileInfo file = new FileInfo(filepath);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            using (FileStream stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            Console.WriteLine("Write - Destination:" + file.FullName);
        }

        protected StringBuilder ReadFile(String filepath)
        {
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, filepath));
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }
            byte[] data = new byte[file.Length];
            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                stream.Read(data, 0, data.Length);
            }
            return new StringBuilder(Encoding.UTF8.GetString(data));
        }

        protected String GetMenu(List<Category> list, String code)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var item in list)
            {
                buffer.Append(@"<li class='nav-item'><a class='nav-link text-uppercase waves-effect waves-light menu-nav-blog ");
                if (String.Equals(code, item.CATEGORY_CODE))
                {
                    buffer.Append(" active ");
                }
                buffer.Append("' href='");
                buffer.Append(item.URL);
                buffer.Append("'>");
                buffer.Append(item.CATEGORY_NAME);
                buffer.Append("</a></li>");
                buffer.AppendLine();
            }
            return buffer.ToString();
        }
    }
}
