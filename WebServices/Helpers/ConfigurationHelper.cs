using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServices.Helpers
{
    public static class ConfigurationHelper
    {
        private static Mutex Mutex = new Mutex();
        private static ConfigurationTable _configuration;
        public static ConfigurationTable Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                lock(Mutex)
                {
                    _configuration = value;
                }
            }
        }

        public static void Init(ConfigurationTable configuration)
        {
            Configuration = configuration;
        }
    }
}
