using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sociatis.Models.DevIssues
{
    public class CreateIssueViewModel
    {
        [Required]
        [MaxLength(50)]
        [DisplayName("Problem's name")]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }
    }
}