using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Barron_Amanda_HW5.Models
{
    public class Repository
    {
        public Int32 RepositoryID { get; set; }

        [Display(Name = "User Name")]
        public String UserName { get; set; }
      
        [Display(Name = "Repo Name")]
        public String RepositoryName { get; set; }

        public String Description { get; set; }

        public String Tags { get; set; }

        public String URL { get; set; }

        [Display(Name = "Last Update")]
        [DataType(DataType.Date)]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "Stars (in Thousands)")]
        public Decimal StarCount { get; set; }

        //navigational property for language
        public Language Language { get; set; }

    }
}