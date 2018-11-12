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

            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();
            foreach (var category in categoryList)
            {
                CreatePost(categoryList, category);
                if (category.ISHOME)
                {
                    continue;
                }
                CreateList(categoryList, category);
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
            Console.WriteLine("Write - Destination:" + file.FullName);
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
            List<StringBuilder> listTemplate = new List<StringBuilder>();
            StringBuilder templateListStringBuilder = ReadFile(templateList);
            templateListStringBuilder.Replace("#####MAINTITLE#####", ConfigurationManager.AppSettings["MainTitle"]);
            templateListStringBuilder.Replace("#####MENU#####", GetMenu(list, category.CATEGORY_CODE));
            templateListStringBuilder.Replace("#####TITLE#####", category.CATEGORY_NAME + " " + ConfigurationManager.AppSettings["ListTitleWord"]);
            StringBuilder buffer = new StringBuilder();
            StringBuilder templateBuffer = null;
            int MAXPOST = Int32.Parse(ConfigurationManager.AppSettings["MaxPosting"]);
            int i = 0;
            foreach (var item in category.Post.OrderByDescending(x => x.CREATEDATED))
            {
                if (i >= MAXPOST)
                {
                    i = 0;
                    templateBuffer.Replace("#####List#####", buffer.ToString());
                    listTemplate.Add(templateBuffer);
                }
                if (i == 0)
                {
                    buffer.Clear();
                    templateBuffer = new StringBuilder();
                    templateBuffer.Append(templateListStringBuilder.ToString());
                }
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
                buffer.Append("<p class='my-list-date date-type-1'>");
                buffer.Append(item.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                buffer.Append("에 작성된 글...</p>");
                buffer.Append("<p class='my-list-date date-type-2'>");
                buffer.Append(item.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                buffer.Append(" 작성 </p>");
                buffer.Append("<a href='.");
                buffer.Append(item.LOCATION);
                buffer.Append("' class='btn btn-primary btn-sm'>Go read");
                buffer.Append("<i class='fa fa-play ml-2'></i>");
                buffer.Append("</a>");
                buffer.Append("</div>");
                buffer.Append("</div>");
                buffer.Append("</div>");
                i++;
            }
            if (templateBuffer != null)
            {
                templateBuffer.Replace("#####List#####", buffer.ToString());
                listTemplate.Add(templateBuffer);
            }
            else
            {
                listTemplate.Add(templateListStringBuilder);
            }

            //PAGING
            int MAXPAGING = Int32.Parse(ConfigurationManager.AppSettings["MaxPaging"]);
            if (listTemplate.Count < 2)
            {
                StringBuilder data = listTemplate[0];
                data.Replace("#####PAGING#####", "");
                WriteFile(savePath + category.URL.Replace("/", "\\"), data);
            }
            else
            {
                String categoryUrl = category.URL;
                int pos = categoryUrl.LastIndexOf(".");
                String categoryUrlBuffer = categoryUrl.Substring(0, pos);
                String ext = categoryUrl.Substring(pos, categoryUrl.Length - pos);
                for (i = 0; i < listTemplate.Count; i++)
                {
                    StringBuilder data = listTemplate[i];
                    buffer.Clear();
                    buffer.Append("<nav class='d-flex justify-content-center wow fadeIn'>");
                    buffer.Append("<ul class='pagination pg-blue'>");
                    buffer.Append("<li class='page-item'><a class='page-link' href='");
                    int z = i / MAXPAGING;
                    z = z - 1;
                    if (z < 1)
                    {
                        buffer.Append(category.URL);
                    }
                    else
                    {
                        z = z * MAXPAGING;
                        buffer.Append(categoryUrlBuffer + z.ToString() + ext);
                    }
                    buffer.Append("' aria-label='Previous'> <span aria-hidden='true'>&laquo;</span> <span class='sr-only'>Previous</span>");
                    buffer.Append("</a></li>");
                    for (var j = MAXPAGING * (i / MAXPAGING); j < (MAXPAGING * (i / MAXPAGING)) + MAXPAGING && j < listTemplate.Count; j++)
                    {
                        if (i == j)
                        {
                            buffer.Append("<li class='page-item active'><p class='page-link'>");
                            buffer.Append(j + 1);
                            buffer.Append("<span class='sr-only'>(current)</span></p></li>");
                        }
                        else
                        {
                            buffer.Append("<li class='page-item'><a class='page-link' href='");
                            if (j == 0)
                            {
                                buffer.Append(category.URL);
                            }
                            else
                            {
                                buffer.Append(categoryUrlBuffer + j.ToString() + ext);
                            }
                            buffer.Append("'>");
                            buffer.Append(j + 1);
                            buffer.Append("</a></li>");
                        }
                    }
                    buffer.Append("<li class='page-item'><a class='page-link' href='");
                    z = i / MAXPAGING;
                    z = z + 1;
                    if (z > (listTemplate.Count - 1) / MAXPAGING)
                    {
                        z = listTemplate.Count - 1;
                        buffer.Append(categoryUrlBuffer + z.ToString() + ext);
                    }
                    else
                    {
                        z = z * MAXPAGING;
                        buffer.Append(categoryUrlBuffer + z.ToString() + ext);
                    }
                    buffer.Append("' aria-label='Next'> <span aria-hidden='true'>&raquo;</span> <span class='sr-only'>Next</span>");
                    buffer.Append("</a></li>");
                    buffer.Append("</ul>");
                    buffer.Append("</nav>");
                    data.Replace("#####PAGING#####", buffer.ToString());
                    if (i == 0)
                    {
                        WriteFile(savePath + category.URL.Replace("/", "\\"), data);
                    }
                    else
                    {
                        WriteFile(savePath + categoryUrlBuffer.Replace("/", "\\") + i.ToString() + ext, data);
                    }
                }
            }
        }

        private void CreatePost(List<Category> list, Category category)
        {
            String templatePost = ConfigurationManager.AppSettings["TemplatePost"];
            String savePath = ConfigurationManager.AppSettings["SavePath"];
            List<Post> recently = FactoryDao.GetDao<PostDao>().GetRecentlyAll(6);

            foreach (var post in category.Post)
            {
                StringBuilder templatePostStringBuilder = ReadFile(templatePost);

                templatePostStringBuilder.Replace("#####MAINTITLE#####", ConfigurationManager.AppSettings["MainTitle"]);
                templatePostStringBuilder.Replace("#####MENU#####", GetMenu(list, category.CATEGORY_CODE));
                templatePostStringBuilder.Replace("#####CATEGORY#####", category.CATEGORY_NAME);
                templatePostStringBuilder.Replace("#####TITLE#####", post.TITLE);
                templatePostStringBuilder.Replace("#####CREATEDDATE#####", post.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                templatePostStringBuilder.Replace("#####LASTUPDATEDDATE#####", post.LAST_UPDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                templatePostStringBuilder.Replace("#####CREATEDDATE2#####", post.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                templatePostStringBuilder.Replace("#####LASTUPDATEDDATE2#####", post.LAST_UPDATED.ToString("yyyy-MM-dd HH:mm"));

                StringBuilder buffer = new StringBuilder();
                buffer.Append(Encoding.UTF8.GetString(post.IMAGE));
                templatePostStringBuilder.Replace("#####IMAGE#####", buffer.ToString());
                buffer.Clear();
                templatePostStringBuilder.Replace("#####IMAGECOMMNET#####", post.IMAGE_COMMENT);

                StringBuilder postBuffer = ReadFile(post.FILEPATH);
                templatePostStringBuilder.Replace("#####CONTENTS#####", postBuffer.ToString());
                int state = 0;
                Post pre = null;
                Post next = null;
                foreach (var check in category.Post)
                {
                    if (check == post || check.IDX == post.IDX)
                    {
                        state = 1;
                        continue;
                    }
                    if (state == 0)
                    {
                        pre = check;
                    }
                    if (state == 1)
                    {
                        next = check;
                        break;
                    }
                }
                buffer.Clear();
                if (pre != null || next != null)
                {
                    buffer.Append("<div class='row mt-3 mb-3'>");
                    buffer.Append("<div class='col-12 d-flex align-items-stretch'>");
                    buffer.Append("<div class='card my-style-custom'>");
                    buffer.Append("<div class='my-pre-post-nav'>");
                    buffer.Append("<div class='row'>");
                    buffer.Append("#####PREPOST##### ");
                    buffer.Append("#####PRENEXTLINE##### ");
                    buffer.Append("#####NEXTPOST#####");
                    buffer.Append("</div>");
                    buffer.Append("</div>");
                    buffer.Append("</div>");
                    buffer.Append("</div>");
                    buffer.Append("</div>");
                    templatePostStringBuilder.Replace("#####PRENEXT#####", buffer.ToString());
                }
                else
                {
                    templatePostStringBuilder.Replace("#####PRENEXT#####", "");
                }

                buffer.Clear();
                if (pre != null)
                {
                    buffer.Append("<div class='col-12 mb-1'>");
                    buffer.Append("<p class='my-pre-list-title' style='margin-bottom:0px;'><a href='");
                    buffer.Append(pre.LOCATION);
                    buffer.Append("'> ");
                    buffer.Append("<span class='my-pre-next-icon'><i class='fa fa-chevron-up'></i>이전글</span>");
                    buffer.Append(pre.TITLE);
                    buffer.Append("</a>");
                    buffer.Append("<span class='my-list-date float-right date-type-1 font-7'>");
                    buffer.Append(pre.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                    buffer.Append("에 작성된 글...</span>");
                    buffer.Append("<span class='my-list-date float-right date-type-2 font-7'>");
                    buffer.Append(pre.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                    buffer.Append(" 작성</span>");
                    buffer.Append("</p>");
                    buffer.Append("</div>");
                }
                templatePostStringBuilder.Replace("#####PREPOST#####", buffer.ToString());
                buffer.Clear();

                if (next != null)
                {
                    buffer.Append("<div class='col-12 mt-1'>");
                    buffer.Append("<p class='my-pre-list-title' style='margin-bottom:0px;'><a href='");
                    buffer.Append(next.LOCATION);
                    buffer.Append("'> ");
                    buffer.Append("<span class='my-pre-next-icon'><i class='fa fa-chevron-down'></i>다음글</span>");
                    buffer.Append(next.TITLE);
                    buffer.Append("</a>");
                    buffer.Append("<span class='my-list-date float-right date-type-1 font-7'>");
                    buffer.Append(next.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                    buffer.Append("에 작성된 글...</span>");
                    buffer.Append("<span class='my-list-date float-right date-type-2 font-7'>");
                    buffer.Append(next.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                    buffer.Append(" 작성</span>");
                    buffer.Append("</p>");
                    buffer.Append("</div>");
                }
                templatePostStringBuilder.Replace("#####NEXTPOST#####", buffer.ToString());
                buffer.Clear();

                if (pre != null && next != null)
                {
                    templatePostStringBuilder.Replace("#####PRENEXTLINE#####", "<div class='col-12 my-blog-line'></div>");
                }
                else
                {
                    templatePostStringBuilder.Replace("#####PRENEXTLINE#####", "");
                }
                buffer.Clear();
                foreach (var p in recently)
                {
                    if (p == post || p.IDX == post.IDX)
                    {
                        continue;
                    }
                    buffer.Append("<li>");
                    buffer.Append("<div class='row'>");
                    buffer.Append("<div class='col-12 mb-0'>");
                    buffer.Append("<a href='");
                    buffer.Append(p.LOCATION);
                    buffer.Append("'>[");
                    buffer.Append(p.Category.CATEGORY_NAME);
                    buffer.Append("] ");
                    buffer.Append(p.TITLE);
                    buffer.Append("</a>");
                    buffer.Append("<span class='my-list-date float-right date-type-1 font-7'>");
                    buffer.Append(p.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                    buffer.Append("에 작성된 글...</span>");
                    buffer.Append("<span class='my-list-date float-right date-type-2 font-7'>");
                    buffer.Append(p.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                    buffer.Append(" 작성</span>");
                    buffer.Append("</div>");
                    buffer.Append("</div>");
                    buffer.Append("</li>");
                }
                templatePostStringBuilder.Replace("#####RECENTLYPOST#####", buffer.ToString());

                WriteFile(savePath + post.LOCATION, templatePostStringBuilder);
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
