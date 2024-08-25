using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;
using System.Reflection;

namespace PojectFinal___API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly DbConnection _contection;
        public MissionsController(DbConnection dbConnection)
        {
            this._contection = dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAMissions()
        {
            var missions = await _contection.missions.Include(m => m.Agent).Include(m => m.Target).ToArrayAsync();
            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    missions = missions
                }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var mission = await _contection.missions.FirstOrDefaultAsync(x => x.Id == id);
            if (mission == null) return StatusCode(400);

            mission.Status = StatusMission.statusMission.Actice.ToString();//מעדכן את המשימה לפעילה

            var missions1 = await _contection.missions.Include(m => m.Agent == mission.Agent).ToArrayAsync();
            var missions2 = await _contection.missions.Include(m => m.Target == mission.Target).ToArrayAsync();
            _contection.missions.RemoveRange(missions1);//הסרה של המסימות שהוצעו לסוכן ולמטרה שכרגע צוותו
            _contection.missions.RemoveRange(missions2);

            _contection.SaveChanges();
            return StatusCode(200);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissions()
        {
            var list = await _contection.missions.Include(m => m.Agent).Include(m => m.Target).ToArrayAsync();

            foreach (var mission in list)
            {
                if (mission.Status != StatusMission.statusMission.Actice.ToString())
                {
                    continue;
                }
                var agent = mission.Agent;
                var target = mission.Target;

                var comand = MissionService.CreateCommeandForAgent(agent, target);
                
                agent = MoveService.MoveAgent(comand, agent);

                if (agent.x == target.x && agent.y == target.y)
                {
                    mission.Target.Status = StatusTarget.statusTarget.Eliminated.ToString();
                    mission.Agent.Status = StatusAgent.statusAgent.UnderCover.ToString();
                    mission.TimLeft = 0;
                    mission.Duration += 0.2;
                    mission.Status = StatusMission.statusMission.Comlpeted.ToString();
                }

                mission.TimLeft = TimeDistanceService.UpdateTimeLeft(mission);
                mission.Duration = TimeDistanceService.UpdateDuration(mission);
            }
            _contection.SaveChanges();

            return StatusCode(
                StatusCodes.Status200OK);
        }



        

    }
}
