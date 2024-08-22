using PojectFinal___API.Enums;
using System.ComponentModel.DataAnnotations;

namespace PojectFinal___API.Modles
{
    public class Mission
    {
        [Key]
        public int? Id { get; set; }

        public Agent Agent { get; set; }

        public Target Target { get; set; }
      
        public double? Duration { get; set; }
        
        public string? TimLeft { get; set; }

        public string? Status { get; set; }

        public Mission() { }
    }
}
