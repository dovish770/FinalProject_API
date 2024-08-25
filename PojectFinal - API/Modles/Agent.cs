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

        public string nickname { get; set; }
       
        public string? Status { get; set; }
       
        public int x { get; set; } = 0;
       
        public int y { get; set; } = 0;

        public string photoUrl {  get; set; }
        
    }
}
