using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code
{
    public class Colors
    {
        //low->medium->high
        public static Dictionary<ResourceTypeEnum, List<string>> ResourceColors = new Dictionary<ResourceTypeEnum, List<string>>
        {
            {
                ResourceTypeEnum.Wood, new List<string>()
                {
                    "#bae4b3",
                    "#74c476",
                    "#31a354",
                    "#006d2c"
                }
            },
            {
                ResourceTypeEnum.Grain, new List<string>()
                {
                    "#fed98e",
                    "#fe9929",
                    "#d95f0e",
                    "#993404"
                }
            } ,
            {
                ResourceTypeEnum.IronOre, new List<string>()
                {
                    "#fcae91",
                    "#fb6a4a",
                    "#de2d26",
                    "#a50f15"
                }
            },
            {
                ResourceTypeEnum.TeaLeaf, new List<string>()
                {
                    "#c2e699",
                    "#78c679",
                    "#31a354",
                    "#006837"
                }
            },
            {
                ResourceTypeEnum.Oil, new List<string>()
                {
                    "#cccccc",
                    "#969696",
                    "#636363",
                    "#252525"
                }
            }
        };
    }
}