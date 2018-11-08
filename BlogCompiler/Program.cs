using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using BlogCompiler.Entity;
using System.IO;

namespace BlogCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
            Console.WriteLine("Press Any Key...");
            Console.ReadLine();
        }
        public Program()
        {

        }
        public void Run()
        {
            String templateList = ConfigurationManager.AppSettings["TemplateList"];
            String templateEmpty = ConfigurationManager.AppSettings["TemplateEmpty"];
            String emplatePost = ConfigurationManager.AppSettings["TemplatePost"];
            String savePath = ConfigurationManager.AppSettings["SavePath"];

            StringBuilder templateListStringBuilder = ReadFile(templateList);


            CreateHeader(templateListStringBuilder, "01");
            WriteFile(Path.Combine(savePath, "test.html"), templateListStringBuilder);

            foreach (var category in FactoryDao.GetDao<CategoryDao>().GetAllIncludePost())
            {
                Console.WriteLine(category.CATEGORY_CODE);
                foreach (var post in category.Post.Where(x => !x.ISDELETED))
                {
                    Console.WriteLine(post.TITLE);
                }
            }
        }

        private void WriteFile(String filepath, StringBuilder html)
        {
            byte[] data = Encoding.UTF8.GetBytes(html.ToString());
            using (FileStream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        private StringBuilder ReadFile(String filepath)
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

        private void CreateHeader(StringBuilder html, String code)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var category in FactoryDao.GetDao<CategoryDao>().GetAll())
            {
                buffer.Append(@"<li class='nav-item'><a class='nav-link text-uppercase waves-effect waves-light menu-nav-blog ");
                if (String.Equals(code, category.CATEGORY_CODE))
                {
                    buffer.Append(" active ");
                }
                buffer.Append("' href='");
                buffer.Append(category.URL);
                buffer.Append("'>");
                buffer.Append(category.CATEGORY_NAME);
                buffer.Append("</a></li>");
                buffer.AppendLine();
            }
            html.Replace("#####MENU#####", buffer.ToString());
        }

    }
}
