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

        [Range(1, 1001)]
        public int x { get; set; }

        [Range(1, 1001)]
        public int y { get; set; }

        public string? Image {  get; set; }
        
    }
}
