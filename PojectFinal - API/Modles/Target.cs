using PojectFinal___API.Enums;
using PojectFinal___API.Services;
using System.ComponentModel.DataAnnotations;

namespace PojectFinal___API.Modles
{
    public class Target
    {
        [Key]
        public int Id { get; set; }

        public string name { get; set; }

        public string position { get; set; }

        public int x { get; set; } = new int();
        
        public int y { get; set; } = new int();

        public string photoUrl { get; set; }

        public string? Status { get; set; }
       
    }


}
