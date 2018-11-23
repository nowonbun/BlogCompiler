using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using BlogCompiler.Entity;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

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

        private readonly FileSystem _fileSystem = new FileSystem();
        private readonly ListSystem _listSystem = new ListSystem();
        private readonly PostSystem _postSystem = new PostSystem();
        private readonly SitemapSystem _sitemapSystem = new SitemapSystem();
        private readonly RssSystem _rssSystem = new RssSystem();

        public void Run()
        {
            _fileSystem.Run();
            _listSystem.Run();
            _postSystem.Run();
            _sitemapSystem.Run();
            _rssSystem.Run();
        }
    }
}
