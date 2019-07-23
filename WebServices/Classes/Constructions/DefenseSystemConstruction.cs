using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebServices.Classes.Constructions
{
    public class DefenseSystemConstruction : ConstructionBase
    {
        public int Quality { get; set; }
        public DefenseSystemConstruction(Construction construction) : base(construction)
        {
            Quality = construction.Company.Quality;
        }

        public override int GetProgressNeededToBuild()
        {
            switch (Quality)
            {
                case 1:
                    return 100;
                case 2:
                    return 750;
                case 3:
                    return 3000;
                case 4:
                    return 8000;
                case 5:
                    return 25000;
            }
            throw new ArgumentException("Quality out of bounds!");
        }
    }
}
