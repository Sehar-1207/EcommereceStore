using System.ComponentModel.DataAnnotations;

namespace Products.Catalogue.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
