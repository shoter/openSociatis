using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis_Test_Suite.Mocks
{
    public class MockDependencyResolver : IDependencyResolver
    {
        Dictionary<Type, object> mockObjects = new Dictionary<Type, object>();
        Dictionary<Type, object> mocks  = new Dictionary<Type, object>();

        public Mock<T> GetMock<T>()
            where T:class
        {
            return (Mock<T>)mocks[typeof(T)];
        }


        public Mock<T> AddMock<T>(params object[] args)
            where T:class
        {
            var mock = new Mock<T>(args);
            AddMock(mock);

            return mock;
        }

        public MockDependencyResolver AddMock<T>(Mock<T> mock)
            where T:class
        {
            mockObjects.Add(typeof(T), mock.Object);
            mocks.Add(typeof(T), mock);
            return this;
        }


        public object GetService(Type serviceType)
        {
            return mockObjects[serviceType];
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
