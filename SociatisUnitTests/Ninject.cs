using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisUnitTests
{
    public class Ninject
    {

        private static IKernel _instance;
        public static IKernel Current
        {
            get
            {
                if (_instance == null)
                    Init();
                return _instance;
            }
        }

        public static void Init()
        {
            _instance = new StandardKernel();
            Sociatis.App_Start.NinjectWebCommon.RegisterServices(_instance);
        }
    }
}
