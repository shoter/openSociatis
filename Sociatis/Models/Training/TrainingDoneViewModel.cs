using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Training
{
    public class TrainingDoneViewModel : TrainingIndexViewModel
    {
        public double StrengthGain { get; set; }
        public TrainingDoneViewModel(Citizen citizen, double strengthGain) : base(citizen)
        {
            StrengthGain = strengthGain;
        }
    }
}