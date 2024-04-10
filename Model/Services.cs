namespace Practice.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Services
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Services()
        {
            Clients = new HashSet<Clients>();
        }

        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Service_Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public decimal? PriceDiscount => Price - (Price * Discount / 100);

        public byte? Discount { get; set; }
        public bool HasDiscount => Discount > 0;

        public int Time { get; set; }

        public int TimeInMin => Time / 60;

        public bool HaveDesc => Description.Length > 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Clients> Clients { get; set; }
    }
}
