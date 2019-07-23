using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices;
using Ninject;

namespace SociatisUnitTests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void CreateCitizen()
        {
            IAuthService service = Ninject.Current.Get<IAuthService>();
            
        }
    }
}
