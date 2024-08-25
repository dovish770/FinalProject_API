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

        public static bool IsMoveOutOfRange(int x, int y, string move)
        {           

            switch (move)
            {
                case "n":
                    return y > 1;
                case "s":
                    return y < 1000;
                case "e":
                    return x < 1000;
                case "w":
                    return x > 1;
                default:
                    return false;
            }        
        }
    }
}
