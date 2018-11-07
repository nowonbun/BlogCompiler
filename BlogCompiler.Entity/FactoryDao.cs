using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCompiler.Entity
{
    public class FactoryDao
    {
        private static FactoryDao instance = null;
        private Dictionary<Type, IDao> flyweight = new Dictionary<Type, IDao>();
        public static T GetDao<T>()
        {
            if (instance == null)
            {
                instance = new FactoryDao();
            }
            return (T)instance.GetEntity(typeof(T));
        }
        private FactoryDao()
        {

        }
        private IDao GetEntity(Type key)
        {
            if (!flyweight.ContainsKey(key))
            {
                flyweight.Add(key, (IDao)Activator.CreateInstance(key));
            }
            return flyweight[key];
        }
    }
}
