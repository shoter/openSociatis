using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json.Battle
{
    public class JsonFightModel :JsonSuccessModel
    {
        public double Damage { get; set; }
        public double HP { get; set; }
        public JsonFightModel(double damage, int citizenHP)
        {
            Message = string.Format("You dealt {0} points of damage", damage);
            Damage = damage;
            HP = citizenHP;

        }
    }
}