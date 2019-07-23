using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSociatisWorld.Creators
{
    class CreatorBase<T>
        where T : class, new()
    {
        public CreatorBase()
        {
           
        }
    }
}
