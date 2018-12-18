using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BlogCompiler.Entity;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlogCompiler
{
    class RssSystem : CommonSystem
    {
        public void Run()
        {
            String savePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["SavePath"])).FullName;
            var categoryList = FactoryDao.GetDao<CategoryDao>().GetAllIncludePost();
            CreateRss(savePath, categoryList);
        }
        private void CreateRss(String savePath, List<Category> categorylist)
        {
            var postlist = categorylist.SelectMany(x => x.Post);
            String root = ConfigurationManager.AppSettings["SiteRoot"];
            StringBuilder rss = new StringBuilder();
            rss.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            rss.Append("<rss version=\"2.0\" xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:taxo=\"http://purl.org/rss/1.0/modules/taxonomy/\" xmlns:activity=\"http://activitystrea.ms/spec/1.0/\" >");
            rss.Append("<channel>");
            rss.Append("<title>");
            rss.Append(ConfigurationManager.AppSettings["RssTitle"]);
            rss.Append("</title>");
            rss.Append("<link>");
            rss.Append(root);
            rss.Append("</link>");
            rss.Append("<description>");
            rss.Append(ConfigurationManager.AppSettings["RssDescription"]);
            rss.Append("</description>");
            rss.Append("<language>");
            rss.Append(ConfigurationManager.AppSettings["RssLanguage"]);
            rss.Append("</language>");
            DateTime dt = DateTime.Now;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            rss.Append("<pubDate>");
            rss.Append(dt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", dtfi));
            rss.Append("</pubDate>");
            rss.Append("<generator>");
            rss.Append(ConfigurationManager.AppSettings["RssGenerator"]);
            rss.Append("</generator>");
            rss.Append("<managingEditor>");
            rss.Append(ConfigurationManager.AppSettings["RssEditor"]);
            rss.Append("</managingEditor>");
            rss.Append("<webMaster>");
            rss.Append(ConfigurationManager.AppSettings["RssEditor"]);
            rss.Append("</webMaster>");
            foreach (var post in postlist.Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED))
            {
                rss.Append("<item>");
                rss.Append("<title>");
                rss.Append(post.TITLE);
                rss.Append("</title>");
                rss.Append("<link>");
                rss.Append(root + post.LOCATION);
                rss.Append("</link>");
                rss.Append("<description>");
                String contents = ReadFile(post.FILEPATH).ToString();
                //rss.Append(Regex.Replace(contents, @"<(/)?([a-zA-Z]*)(\\s[a-zA-Z]*=[^>]*)?(\\s)*(/)?>", ""));
                rss.Append(CreateDescription(ReadFile(post.FILEPATH).ToString()));
                //rss.Append(Regex.Replace(contents, @"<(/)?([a-zA-Z]*)(\\s[a-zA-Z]*=[^>]*)?(\\s)*(/)?>", ""));
                rss.Append("</description>");
                rss.Append("<category>");
                rss.Append(post.Category.CATEGORY_NAME);
                rss.Append("</category>");
                rss.Append("<author>");
                rss.Append(ConfigurationManager.AppSettings["RssEditor"]);
                rss.Append("</author>");
                rss.Append("<guid>");
                rss.Append(root + post.LOCATION);
                rss.Append("</guid>");
                rss.Append("<pubDate>");
                rss.Append(post.CREATEDATED.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", dtfi));
                rss.Append("</pubDate>");
                rss.Append("</item>");
            }
            rss.Append("</channel>");
            rss.Append("</rss>");
            WriteFile(new FileInfo(savePath + "\\" + ConfigurationManager.AppSettings["Rss"]).FullName, rss);
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
            return "<![CDATA[" + Regex.Replace(contents, @"<[^>]*>", "").Replace("&nbsp;", "") + "]]>";
        }
    }
}
