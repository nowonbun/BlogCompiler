using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCompiler.Entity
{
    public class PostDao : Dao<Post>
    {
        public List<Post> GetAll()
        {
            return Transaction((context) =>
            {
                return context.Post.Where(x => !x.ISDELETED).ToList();
            });
        }
    }
}
