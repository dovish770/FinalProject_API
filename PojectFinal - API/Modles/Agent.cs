using PojectFinal___API.Enums;
using System.ComponentModel.DataAnnotations;

using PojectFinal___API.Services;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PojectFinal___API.Modles
{
    public class Agent
    {
        [Key]
        public int? Id { get; set; }

        public string name { get; set; }
       
        public string? Status { get; set; }

        [Range(0, 1001)]
        public int x { get; set; } = 0;

        [Range(0, 1001)]
        public int y { get; set; } = 0;

        public string Image {  get; set; }
        
    }
}
