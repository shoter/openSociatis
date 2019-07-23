using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class UploadRepository : RepositoryBase<Upload, SociatisEntities>, IUploadRepository
    {
        public UploadRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
