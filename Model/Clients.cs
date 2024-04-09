namespace Practice.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clients
    {
        public long ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Full_Name { get; set; }

        public long? Course { get; set; }

        public virtual Services Services { get; set; }
    }
}
