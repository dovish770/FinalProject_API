using PojectFinal___API.Modles;
using PojectFinal___API.Enums;

namespace PojectFinal___API.Services
{
    public class MissionService
    {       
        public static Mission CreateMission(Agent agent, Target target)
        {
            Mission mission = new Mission();
            mission.Agent = agent;
            mission.Target = target;
            mission.Duration = 0;
            mission.TimLeft = TimeDistanceService.UpdateTimeLeft(mission);
            mission.Status = StatusMission.statusMission.Suggestion.ToString();

            return mission;
        }

        //בדיקת קירבה בין מטרה לסוכן - מחזיר
        //true אם קטן מ200
        public static bool IsMission(Agent agent, Target target)
        {
            var distance = TimeDistanceService.Distance(agent.x, agent.y, target.x, target.y);
            return distance < 200 && target.y>0 && target.x == 0;
        }

        //יוצר פקודה או הוראה לחיסול עבור סוכן במשימה
        public static string CreateCommeandForAgent(Agent agent, Target target)
        {                        
            //
            if (agent.x == target.x && agent.y < target.y)
            {
                return "n";
            }
            if (agent.x == target.x && agent.y > target.y)
            {
                return "s";
            }
            //
            if (agent.y == target.y && agent.x < target.x)
            {
                return "e";
            }
            if (agent.y == target.y && agent.x > target.x)
            {
                return "w";
            }
            //
            if (agent.x > target.x && agent.y > target.y)
            {
                return "sw";
            }
            if (agent.x < target.y && agent.x < target.y)
            {
                return "ne";
            }
            //
            if (agent.x < target.x && agent.y > target.y)
            {
                return "se";
            }
            if (agent.x > target.x && agent.y < target.y)
            {
                return "nw";
            }    
            
            return "";
        }        
    }
}
