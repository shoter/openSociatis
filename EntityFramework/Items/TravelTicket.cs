using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;

namespace Entities.Items
{
    public class MovingTicket : ItemBase
    {
        public MovingTicket(EquipmentItem item) : base(item)
        {
        }

        public MovingTicket(int quantity, int quality) : base(quantity, quality)
        { }

        public override ProductTypeEnum ProductType {  get { return ProductTypeEnum.MovingTicket; } }

        public int HpLoss
        {
            get
            {
                return 6 - Quality;
            }
        }

        public double TravelDistance
        {
            get
            {
                return Math.Pow(Quality, 2) * 600;
            }
        }
    }
}
