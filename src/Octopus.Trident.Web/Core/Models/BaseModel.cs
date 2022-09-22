using System.ComponentModel.DataAnnotations;

namespace Octopus.Trident.Web.Core.Models
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}
