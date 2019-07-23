using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace WebServices
{
    public class FileService : IFileService
    {

        public string getFile(string subfolder, string filename)
        {
            return HostingEnvironment.MapPath(
                string.Format(@"~\Images\{0}\{1}", subfolder, filename));
        }

        /// <summary>
        /// Creates file in specified subfolder
        /// </summary>
        /// <param name="subfolder">for example "images"</param>
        /// <param name="name">for example "test.jpg"</param>
        /// <returns></returns>
        public string CreateFile(string subfolder, string name)
        {
            var file = getFile(subfolder, name);
            if (File.Exists(file))
                throw new Exception(string.Format("File {0} exists", name));

            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.Create(file).Dispose();

            return file;

        }

        /// <summary>
        /// creates file with random name with desired extension
        /// </summary>
        /// <param name="subfolder">For example "images"</param>
        /// <param name="extension">For example "jpg"</param>
        /// <returns></returns>
        public string CreateUniqueFile(string subfolder, string extension)
        {
            string filename = "";
            string file = "";
            do
            {
                filename = Guid.NewGuid().ToString() + "." + extension;
                file = getFile(subfolder, filename);

            } while (File.Exists(file));


            return CreateFile(subfolder, filename);
        } 
    }
}
