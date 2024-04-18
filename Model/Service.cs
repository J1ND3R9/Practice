namespace Practice.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.IO;

    [Table("Service")]
    public partial class Service
    {
        public int ID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name_s { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public int Duration_in_sec { get; set; }

        [StringLength(500)]
        public string Descript { get; set; }

        public int? Discount { get; set; }

        [StringLength(1000)]
        public string Image_path { get; set; }

        public bool HasDiscount => Discount > 0;
        public string Image => Image_path.StartsWith(" ") ? Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Image_path.Remove(0, 1)) : Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Image_path);
        public bool HaveDesc => !String.IsNullOrEmpty(Descript);
        public int TimeInMin => Duration_in_sec / 60;
        public decimal? PriceDiscount => Price - (Price * Discount / 100);
    }
}
