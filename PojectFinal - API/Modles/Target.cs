using PojectFinal___API.Enums;
using PojectFinal___API.Services;
using System.ComponentModel.DataAnnotations;

namespace PojectFinal___API.Modles
{
    public class Target
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }

        public string Occupation { get; set; }

        [Range(1, 1001)]
        public int x { get; set; }

        [Range(1, 1001)]
        public int y { get; set; }

        public string? Status { get; set; }         
    }
}
