using Common.EntityFramework;
using Common.Transactions;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebServices
{
    public class BaseService
    {
        protected readonly ITransactionScopeProvider transactionScopeProvider;

        public BaseService()
        {
            transactionScopeProvider = DependencyResolver.Current.GetService<ITransactionScopeProvider>();
        }


        private bool _SaveChanges = true;
        private List<BaseService> attachedServices = new List<BaseService>();
        public bool SaveChanges
        {
            get
            {
                return _SaveChanges;
            }
            private set
            {
                _SaveChanges = value;
                foreach (var service in attachedServices)
                    service.SaveChanges = value;
            }
        }

        public void ConditionalSaveChanges(IRepository repository)
        {
            if (SaveChanges)
                repository.SaveChanges();
        }

        public T Attach<T>(T service)
        {
            if (service is BaseService)
            {
                if(attachedServices.Contains(service as BaseService) == false)
                    attachedServices.Add(service as BaseService);
            }
            return service;
        }

        protected BaseServiceNoSaveChanges NoSaveChanges
        {
            get
            {
                return new BaseServiceNoSaveChanges(this);
            }
        }



        protected class BaseServiceNoSaveChanges : IDisposable
        {
            private BaseService parent;
            public bool OriginalSaveChanges { get; set; }

            public BaseServiceNoSaveChanges(BaseService parent)
            {
                this.parent = parent;
                OriginalSaveChanges = parent.SaveChanges;
                parent.SaveChanges = false;
            }

            public void Dispose()
            {
                parent.SaveChanges = OriginalSaveChanges;
            }
        }
    }

    
}
