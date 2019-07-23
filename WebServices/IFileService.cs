using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IFileService
    {

        /// <summary>
        /// Creates file in specified subfolder
        /// </summary>
        /// <param name="subfolder">for example "images"</param>
        /// <param name="filename">for example "test.jpg"</param>
        /// <returns></returns>
        string getFile(string subfolder, string filename);

        /// <summary>
        /// Creates file in specified subfolder
        /// </summary>
        /// <param name="subfolder">for example "images"</param>
        /// <param name="name">for example "test.jpg"</param>
        /// <returns></returns>
        string CreateFile(string subfolder, string name);
        /// <summary>
        /// creates file with random name with desired extension
        /// </summary>
        /// <param name="subfolder">For example "images"</param>
        /// <param name="extension">For example "jpg"</param>
        /// <returns></returns>
        string CreateUniqueFile(string subfolder, string extension);


    }
}
