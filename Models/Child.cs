using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class Child
    {
        public long ChildId { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; } = 0.00;
        
        [ForeignKey("User")]
        public long UserId { get; set; }
    }
}