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
        [StringLength(200)]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string Patronymic { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        public long? Course { get; set; }

        public virtual Services Services { get; set; }
    }
}
