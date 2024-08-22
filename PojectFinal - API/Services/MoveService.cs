using PojectFinal___API.Modles;
using System.Linq.Expressions;

namespace PojectFinal___API.Services
{
    public class MoveService
    {     
        public static Agent MoveAgent(string move, Agent agent) 
        {
            var commands = Move.Moves[move];

            agent.x += commands.x;
            agent.y += commands.y;

            return agent;
        }
    }
}
