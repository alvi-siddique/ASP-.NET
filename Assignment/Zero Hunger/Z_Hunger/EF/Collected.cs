//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Z_Hunger.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Collected
    {
        [Required]
        public int CollectedID { get; set; }

        [Required]
        public int CollectionRequestID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public string Details { get; set; }

        public virtual CollectionRequest CollectionRequest { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}

