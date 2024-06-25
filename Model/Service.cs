namespace Practice.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Diagnostics;
    using System.IO;

    [Table("Service")]
    public partial class Service
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Service()
        {
            ClientService = new HashSet<ClientService>();
            ServicePhoto = new HashSet<ServicePhoto>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Column(TypeName = "money")]
        public decimal Cost { get; set; }

        public int DurationInSeconds { get; set; }

        public string Description { get; set; }

        public double? Discount { get; set; }

        [StringLength(1000)]
        public string MainImagePath { get; set; }

        public bool HasDiscount => Discount > 0;
        public string Image => MainImagePath.StartsWith(" ") ? Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), MainImagePath.Remove(0, 1)) : Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), MainImagePath);
        public bool HaveDesc => !String.IsNullOrEmpty(Description);
        public int TimeInMin => DurationInSeconds / 60;
        public decimal? PriceDiscount => Cost - (Cost * (int)Discount / 100);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientService> ClientService { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServicePhoto> ServicePhoto { get; set; }
    }
}
