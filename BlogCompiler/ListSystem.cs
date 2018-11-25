using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BlogCompiler.Entity;

namespace BlogCompiler
{
    class ListSystem : CommonSystem
    {
        public void Run()
        {
            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();

            foreach (var category in categoryList)
            {
                if (category.ISHOME)
                {
                    continue;
                }
                CreateList(categoryList, category);
            }
        }

        private void CreateList(List<Category> list, Category category)
        {
            String savePath = ConfigurationManager.AppSettings["SavePath"];
            String templateList;
            if (category.Post.Where(x => !x.ISDELETED).Count() > 0)
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

            templateListStringBuilder.Replace("#####KEYWORDS#####", ConfigurationManager.AppSettings["Keywords"]);
            templateListStringBuilder.Replace("#####DESCRIPTION#####", ConfigurationManager.AppSettings["Description"]);
            templateListStringBuilder.Replace("#####CANONICAL#####", ConfigurationManager.AppSettings["SiteRoot"]);
            templateListStringBuilder.Replace("#####AUTHOR#####", ConfigurationManager.AppSettings["Author"]);
            templateListStringBuilder.Replace("#####ALTERNATE#####", ConfigurationManager.AppSettings["SiteRoot"] + "/" + ConfigurationManager.AppSettings["Rss"]);



            StringBuilder buffer = new StringBuilder();
            StringBuilder templateBuffer = null;
            int MAXPOST = Int32.Parse(ConfigurationManager.AppSettings["MaxPosting"]);
            int i = 0;

            foreach (var item in category.Post.Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED))
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
                data.Replace("#####URL#####", ConfigurationManager.AppSettings["SiteRoot"] + category.URL);
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
                        data.Replace("#####URL#####", ConfigurationManager.AppSettings["SiteRoot"] + category.URL);
                        WriteFile(savePath + category.URL.Replace("/", "\\"), data);
                    }
                    else
                    {
                        data.Replace("#####URL#####", ConfigurationManager.AppSettings["SiteRoot"] + categoryUrlBuffer + i.ToString() + ext);
                        WriteFile(savePath + categoryUrlBuffer.Replace("/", "\\") + i.ToString() + ext, data);
                    }
                }
            }
        }
    }
}
