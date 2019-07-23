using Entities;
using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Companies
{
    public class CompanyEmployeeViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ImageViewModel Avatar { get; set; }
        public double Salary { get; set; }
        public double? TodaySalary { get; set; }
        public double? Production { get; set; }
        public double Skill { get; set; }
        public int? HP { get; set; }
        public double MinHP { get; set; }
        public int? JobContractID { get; set; }
        public bool CanWork { get; set; } = false;

        public CompanyEmployeeViewModel()
        { }

        public CompanyEmployeeViewModel(CompanyEmployee employee)
        {
            Avatar = new ImageViewModel(employee.Citizen.Entity.ImgUrl);
            ID = employee.CitizenID;
            Name = employee.Citizen.Entity.Name;
            Production = (double?)employee.TodayProduction;
            Skill = employee.GetWorkSkill();
            HP = employee.TodayHP;
            MinHP = employee.MinHP;
            JobContractID = employee.JobContractID;
            Salary = (double)employee.Salary;
            TodaySalary = (double?)employee.TodaySalary;
        }
    }
}
