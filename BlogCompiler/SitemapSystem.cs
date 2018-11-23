using System;
using System.Text;
using System.Configuration;
using BlogCompiler.Entity;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BlogCompiler
{
    class SitemapSystem : CommonSystem
    {
        public void Run()
        {
            String savePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["SavePath"])).FullName;
            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();
            CreateSitemap(savePath, categoryList);
        }
        private void CreateSitemap(String savePath, List<Category> categorylist)
        {
            String root = ConfigurationManager.AppSettings["SiteRoot"];
            StringBuilder sitemap = new StringBuilder();
            sitemap.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            var postlist = categorylist.SelectMany(x => x.Post);

            foreach (var post in postlist.Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED))
            {
                sitemap.Append("<url>");
                sitemap.Append("<loc>");
                sitemap.Append(root + post.LOCATION);
                sitemap.Append("</loc>");
                sitemap.Append("<lastmod>");
                sitemap.Append(post.LAST_UPDATED.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz"));
                sitemap.Append("</lastmod>");
                sitemap.Append("<changefreq>");
                switch (post.CHANGEFREG)
                {
                    case 1: sitemap.Append("never"); break;
                    case 2: sitemap.Append("yearly"); break;
                    case 3: sitemap.Append("monthly"); break;
                    case 4: sitemap.Append("weekly"); break;
                    case 5: sitemap.Append("daily"); break;
                    case 6: sitemap.Append("hourly"); break;
                    default: sitemap.Append("always"); break;
                }

                sitemap.Append("</changefreq>");
                sitemap.Append("<priority>");
                switch (post.CHANGEFREG)
                {
                    case 1: sitemap.Append("0.1"); break;
                    case 2: sitemap.Append("0.2"); break;
                    case 3: sitemap.Append("0.3"); break;
                    case 4: sitemap.Append("0.4"); break;
                    case 5: sitemap.Append("0.5"); break;
                    case 6: sitemap.Append("0.6"); break;
                    case 7: sitemap.Append("0.7"); break;
                    case 8: sitemap.Append("0.8"); break;
                    case 9: sitemap.Append("0.9"); break;
                    case 10: sitemap.Append("1.0"); break;
                    default: sitemap.Append("0.0"); break;
                }

                sitemap.Append("</priority>");
                sitemap.Append("</url>");
            }
            sitemap.Append("</urlset>");
            WriteFile(new FileInfo(savePath + "\\" + ConfigurationManager.AppSettings["SiteMap"]).FullName, sitemap);
        }
    }
}
