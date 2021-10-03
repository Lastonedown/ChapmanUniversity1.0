using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChapmanUniversity1._0.Models
{
    public enum Season{
        Spring,
        Summer,
        Fall,
        Winter
    }
    public class Semester
    {
        [Required]
        public int Id { get; set;}

        [Required]
        public Season Season { get; set;}

    }
}