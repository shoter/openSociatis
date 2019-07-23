using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Messages
{
    public class MessageRecipientViewModel
    {
        public int EntityID { get; set; }
        public string Name { get; set; }

        public MessageRecipientViewModel() { }

        public MessageRecipientViewModel(Entity entity)
        {
            EntityID = entity.EntityID;
            Name = entity.Name;
        }
    }
}
