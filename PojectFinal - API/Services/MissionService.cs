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

        public static bool IsMission(Agent agent, Target target)
        {
            var distance = TimeDistanceService.Distance(agent.x, agent.y, target.x, target.y);
            return distance < 200;
        }


    }
}
