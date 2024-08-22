using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;

namespace PojectFinal___API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly DbConnection _contection;        
        public MissionController(DbConnection dbConnection)
        {
            this._contection = dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAMissions()
        {            
            var missions = await _contection.missions.ToArrayAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    missions = missions
                }
            );
        }
        
        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissions()
        {
            var list = await _contection.missions.ToArrayAsync();

            foreach (var mission in list)
            {
                var agent = mission.Agent;
                var target = mission.Target;

                var comand = MissionService.CreateCommeandForAgent(agent, target);

                if (agent.x == target.x && agent.y == target.y)
                {
                    mission.Target.Status = StatusTarget.statusTarget.Eliminated.ToString();
                    mission.Agent.Status = StatusAgent.statusAgent.UnderCover.ToString();
                    mission.TimLeft = "0";
                    mission.Duration += 0.2;
                    mission.Status = StatusMission.statusMission.Comlpeted.ToString();
                    break;
                }

                agent = MoveService.MoveAgent(comand, agent);
                mission.TimLeft = TimeDistanceService.UpdateTimeLeft(mission);
                mission .Duration = TimeDistanceService.UpdateDuration(mission);
            }

            return StatusCode(
                StatusCodes.Status200OK);
        }



        

    }
}
