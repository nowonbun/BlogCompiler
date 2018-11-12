using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace BlogCompiler.Entity
{
    public class PostDao : Dao<Post>
    {
        public List<Post> GetRecentlyAll(int count)
        {
            return Transaction((context) =>
            {
                return context.Post.Include(t => t.Category).Where(x => !x.ISDELETED).OrderByDescending(x => x.CREATEDATED).Take(count).ToList();
            });
        }
    }
}
