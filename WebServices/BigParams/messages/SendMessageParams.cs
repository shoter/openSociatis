using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.messages
{
    public class SendMessageParams
    {
        public int AuthorID { get; set; }
        public int ThreadID { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Day { get; set; }
    }
}
