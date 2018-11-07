using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace BlogCompiler.Entity
{
    public class CategoryDao : Dao<Category>
    {
        public List<Category> GetAll()
        {
            return Transaction((context) =>
            {
                return context.Category.ToList();
            });
        }
        public List<Category> GetAllIncludePost()
        {
            return Transaction((context) =>
            {
                return context.Category.Include(t => t.Post).OrderBy(x => x.SEQUENCE).ToList();
            });
        }
    }
}
