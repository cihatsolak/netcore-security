using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataProtection.Web.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Color { get; set; }

        /// <summary>
        /// Product Id 'nin şifrelenmiş hali.
        /// </summary>
        [NotMapped]
        public string EncryptedId { get; set; }
    }
}
