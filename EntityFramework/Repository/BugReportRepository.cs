using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Entities.Repository
{
    public class BugReportRepository : RepositoryBase<BugReport, SociatisEntities>, IBugReportRepository
    {
        public BugReportRepository(SociatisEntities context) : base(context)
        {
        }

        public override void Add(BugReport entity)
        {
            entity.Content += string.Format("<br/><img src='{3}'/><br/>By {0} - {1}<br/>{2}", entity?.Citizen?.Entity?.Name, entity?.Citizen?.Email, DateTime.Now, entity.ImgUrl);

            base.Add(entity);
            try
            {
                SaveChanges();
            }
            catch (Exception) { throw; }

            string filename = string.Format("~/Bugs/Bug_{0}_{1:yyyy-MM-dd_HH-mm}.html", entity.ID, DateTime.Now);
            var path = HostingEnvironment.MapPath(filename);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllText(path, string.Format("<html><body>{0}</body></html>", entity.Content));
        }
    }
}
