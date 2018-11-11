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
                }
            }

        }

        public void Run()
        {
            String savePath = ConfigurationManager.AppSettings["SavePath"];
            String templatePath = ConfigurationManager.AppSettings["TemplatePath"];
            List<String> excep = new List<string>()
            {
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplateList"])).FullName,
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplateEmpty"])).FullName,
                new FileInfo(Path.Combine(Environment.CurrentDirectory,ConfigurationManager.AppSettings["TemplatePost"])).FullName
            };
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, templatePath));
            foreach (var d in dir.GetFiles())
            {
                if (excep.Contains(d.FullName))
                {
                    continue;
                }
                Console.WriteLine(d.FullName);
            }
            foreach (var d in dir.GetDirectories())
            {
                CopyDirectory(savePath, dir.FullName, d.FullName);
            }

            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();
            foreach (var category in categoryList)
            {
                if (category.ISHOME)
                {
                    continue;
                }
                CreateList(categoryList, category);
                CreatePost(categoryList, category);
            }
        }

        private void WriteFile(String filepath, StringBuilder html)
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

        private String GetMenu(List<Category> list, String code)
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

        private void CreateList(List<Category> list, Category category)
        {
            String savePath = ConfigurationManager.AppSettings["SavePath"];
            String templateList;
            if (category.Post.Count > 0)
            {
                templateList = ConfigurationManager.AppSettings["TemplateList"];
            }
            else
            {
                templateList = ConfigurationManager.AppSettings["TemplateEmpty"];
            }
            StringBuilder templateListStringBuilder = ReadFile(templateList);
            templateListStringBuilder.Replace("#####MAINTITLE#####", ConfigurationManager.AppSettings["MainTitle"]);
            templateListStringBuilder.Replace("#####MENU#####", GetMenu(list, category.CATEGORY_CODE));
            templateListStringBuilder.Replace("#####TITLE#####", category.CATEGORY_NAME + " " + ConfigurationManager.AppSettings["ListTitleWord"]);
            StringBuilder buffer = new StringBuilder();
            foreach (var item in category.Post)
            {
                buffer.Append("<hr class='mb-3 mt-3'>");
                buffer.Append("<div class='row mt-3 wow fadeIn'>");
                buffer.Append("<div class='col-lg-5 col-xl-4 mb-4'>");
                buffer.Append("<div class='view overlay rounded z-depth-1'>");
                buffer.Append("<img src='");
                buffer.Append(Encoding.UTF8.GetString(item.IMAGE));
                buffer.Append("' class='img-fluid summary-image' alt='Image_");
                buffer.Append(item.TITLE);
                buffer.Append("'> ");
                buffer.Append("<a href='.");
                buffer.Append(item.LOCATION);
                buffer.Append("'>");
                buffer.Append("<div class='mask rgba-white-slight'></div>");
                buffer.Append("</a>");
                buffer.Append("</div>");
                buffer.Append("</div>");
                buffer.Append("<div class='col-lg-7 col-xl-7 ml-xl-4 mb-4'>");
                buffer.Append("<h3 class='mb-3 font-weight-bold dark-grey-text'>");
                buffer.Append("<p class='my-list-title'><a href='.");
                buffer.Append(item.LOCATION);
                buffer.Append("'>");
                buffer.Append(item.TITLE);
                buffer.Append("</a></p>");
                buffer.Append("</h3>");
                buffer.Append("<p class='grey-text my-list-summary'>");
                buffer.Append(item.SUMMARY);
                buffer.Append("</p>");
                buffer.Append("<div style='text-align:right;'>");
                buffer.Append("<p class='my-list-date'>");
                buffer.Append(item.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                buffer.Append("에 작성된 글...</p>");
                buffer.Append("<a href='.");
                buffer.Append(item.LOCATION);
                buffer.Append("' class='btn btn-primary btn-sm'>Go read");
                buffer.Append("<i class='fa fa-play ml-2'></i>");
                buffer.Append("</a>");
                buffer.Append("</div>");
                buffer.Append("</div>");
                buffer.Append("</div>");
            }
            templateListStringBuilder.Replace("#####List#####", buffer.ToString());
            WriteFile(savePath + category.URL.Replace("/", "\\"), templateListStringBuilder);
        }

        private void CreatePost(List<Category> list, Category category)
        {
            String templatePost = ConfigurationManager.AppSettings["TemplatePost"];
            String savePath = ConfigurationManager.AppSettings["SavePath"];

            foreach (var post in category.Post)
            {
                StringBuilder templatePostStringBuilder = ReadFile(templatePost);

                templatePostStringBuilder.Replace("#####MAINTITLE#####", ConfigurationManager.AppSettings["MainTitle"]);
                templatePostStringBuilder.Replace("#####MENU#####", GetMenu(list, category.CATEGORY_CODE));
                templatePostStringBuilder.Replace("#####TITLE#####", category.CATEGORY_NAME);
                templatePostStringBuilder.Replace("#####CREATEDDATE#####", post.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                templatePostStringBuilder.Replace("#####LASTUPDATEDDATE#####", post.LAST_UPDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));

                StringBuilder postBuffer = ReadFile(post.FILEPATH);
                templatePostStringBuilder.Replace("#####CONTENTS#####", postBuffer.ToString());


                WriteFile(savePath + post.LOCATION, templatePostStringBuilder);
            }

        }
    }
}
