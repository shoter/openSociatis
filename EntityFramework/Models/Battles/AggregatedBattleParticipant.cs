using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Battles
{
    public class AggregatedBattleParticipant
    {
        public string ImgUrl { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Damage { get; set; }
        public bool IsAttacker { get; set; }
    }
}
