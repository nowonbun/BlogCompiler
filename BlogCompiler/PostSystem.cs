using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BlogCompiler.Entity;
using System.Text.RegularExpressions;

namespace BlogCompiler
{
    class PostSystem : CommonSystem
    {
        public void Run()
        {
            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();

            foreach (var category in categoryList)
            {
                CreatePost(categoryList, category);
            }
        }

        private String CreateDescription(String contents)
        {
            contents = contents.ToLower();
            int pos = contents.IndexOf("<pre");
            while (pos > -1)
            {
                int epos = contents.IndexOf("</pre>", pos);
                if (epos < 0)
                {
                    break;
                }
                contents = contents.Remove(pos, epos - pos);
                pos = contents.IndexOf("<pre");
            }
            String ret = Regex.Replace(contents, @"<[^>]*>", "").Replace("&nbsp;", "");
            int maxlength = Int32.Parse(ConfigurationManager.AppSettings["DescriptMaxLength"]);
            if (ret.Length > maxlength)
            {
                return ret.Substring(0, maxlength - 2) + "..";
            }
            return ret;
        }

        private void CreatePost(List<Category> list, Category category)
        {
            String templatePost = ConfigurationManager.AppSettings["TemplatePost"];
            String savePath = ConfigurationManager.AppSettings["SavePath"];
            List<Post> recently = FactoryDao.GetDao<PostDao>().GetRecentlyAll(6);

            foreach (var post in category.Post.Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED))
            {
                StringBuilder templatePostStringBuilder = ReadFile(templatePost);
                String contents = ReadFile(post.FILEPATH).ToString();

                templatePostStringBuilder.Replace("#####MAINTITLE#####", ConfigurationManager.AppSettings["MainTitle"]);
                templatePostStringBuilder.Replace("#####MENU#####", GetMenu(list, category.CATEGORY_CODE));
                templatePostStringBuilder.Replace("#####CATEGORY#####", category.CATEGORY_NAME);
                templatePostStringBuilder.Replace("#####TITLE#####", post.TITLE);
                templatePostStringBuilder.Replace("#####CREATEDDATE#####", post.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                templatePostStringBuilder.Replace("#####LASTUPDATEDDATE#####", post.LAST_UPDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                templatePostStringBuilder.Replace("#####CREATEDDATE2#####", post.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                templatePostStringBuilder.Replace("#####LASTUPDATEDDATE2#####", post.LAST_UPDATED.ToString("yyyy-MM-dd HH:mm"));

                templatePostStringBuilder.Replace("#####KEYWORDS#####", ConfigurationManager.AppSettings["Keywords"]);
                //templatePostStringBuilder.Replace("#####DESCRIPTION#####", ConfigurationManager.AppSettings["Description"]);
                templatePostStringBuilder.Replace("#####DESCRIPTION#####", CreateDescription(contents));
                templatePostStringBuilder.Replace("#####CANONICAL#####", ConfigurationManager.AppSettings["SiteRoot"]);
                templatePostStringBuilder.Replace("#####AUTHOR#####", ConfigurationManager.AppSettings["Author"]);
                templatePostStringBuilder.Replace("#####ALTERNATE#####", ConfigurationManager.AppSettings["SiteRoot"] + "/" + ConfigurationManager.AppSettings["Rss"]);

                //templatePostStringBuilder.Replace("#####SUMMARY#####", Regex.Replace(contents, @"<[^>]*>", "").Replace("&nbsp;", ""));
                //templatePostStringBuilder.Replace("#####SUMMARY#####", ConfigurationManager.AppSettings["Description"]);
                templatePostStringBuilder.Replace("#####SUMMARY#####", CreateDescription(contents));
                templatePostStringBuilder.Replace("#####URL#####", ConfigurationManager.AppSettings["SiteRoot"] + post.LOCATION);

                StringBuilder buffer = new StringBuilder();
                //buffer.Append(Encoding.UTF8.GetString(post.IMAGE));
                //templatePostStringBuilder.Replace("#####IMAGE#####", buffer.ToString());
                //buffer.Clear();
                //templatePostStringBuilder.Replace("#####IMAGECOMMNET#####", post.IMAGE_COMMENT);
                templatePostStringBuilder.Replace("#####CONTENTS#####", contents);
                int state = 0;
                Post pre = null;
                Post next = null;
                foreach (var check in category.Post.Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED))
                {
                    if (check == post || check.IDX == post.IDX)
                    {
                        state = 1;
                        continue;
                    }
                    if (state == 0)
                    {
                        next = check;
                    }
                    if (state == 1)
                    {
                        pre = check;
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
                    buffer.Append("<p class='my-pre-list-title' style='margin-bottom:0px;'><a class='prenext-link' href='");
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
                    buffer.Append("<p class='my-pre-list-title' style='margin-bottom:0px;'><a class='prenext-link' href='");
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
                    buffer.Append("<li class='row'>");
                    buffer.Append("<div class='col-12 mb-0'>");
                    buffer.Append("<i class='fa fa-caret-right mr-2'></i>");
                    buffer.Append("<a href='");
                    buffer.Append(p.LOCATION);
                    buffer.Append("'>[");
                    buffer.Append(p.Category.CATEGORY_NAME);
                    buffer.Append("] ");
                    buffer.Append(p.TITLE);
                    buffer.Append("<span class='my-list-date float-right date-type-1 font-7'>");
                    buffer.Append(p.CREATEDATED.ToString("yyyy년 MM월 dd일 HH시 mm분"));
                    buffer.Append("에 작성된 글...</span>");
                    buffer.Append("<span class='my-list-date float-right date-type-2 font-7'>");
                    buffer.Append(p.CREATEDATED.ToString("yyyy-MM-dd HH:mm"));
                    buffer.Append(" 작성</span>");
                    buffer.Append("</a>");
                    buffer.Append("</div>");
                    buffer.Append("</li>");
                }
                templatePostStringBuilder.Replace("#####RECENTLYPOST#####", buffer.ToString());

                WriteFile(savePath + post.LOCATION, templatePostStringBuilder);
            }
        }

    }
}
