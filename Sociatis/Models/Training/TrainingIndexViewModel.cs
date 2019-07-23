using Entities;
using Sociatis.Models.Citizens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Training
{
    public class TrainingIndexViewModel
    {
        public bool Trained { get; set; }
        public double CurrentStrength { get; set; }

        public TrainingIndexViewModel(Citizen citizen)
        {
            Trained = citizen.Trained;
        }
    }
}