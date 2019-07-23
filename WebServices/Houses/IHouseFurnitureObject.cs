using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Houses
{
    public interface IHouseFurnitureObject
    {
        void Process(int newDay);
        decimal CalculateDecay();
        bool CanUpgrade();

        void Upgrade();
        void AfterUpgrade(int newQuality);
        int GetUpgradeCost();

        string ToHumanReadable();
    }
}
