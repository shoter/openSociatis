using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Classes.Constructions
{
    public abstract class ConstructionBase : IConstruction
    {
        public ConstructionBase(Construction construction)
        {
            Progress = construction.Progress;
        }
        public int Progress { get; private set; }

        public abstract int GetProgressNeededToBuild();
    }
}
