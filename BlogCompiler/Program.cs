using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using BlogCompiler.Entity;

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
            foreach (var category in FactoryDao.GetDao<CategoryDao>().GetAllIncludePost())
            {
                Console.WriteLine(category.CATEGORY_CODE);
                foreach (var post in category.Post.Where(x => !x.ISDELETED))
                {
                    Console.WriteLine(post.TITLE);
                }
            }
        }

    }
}
