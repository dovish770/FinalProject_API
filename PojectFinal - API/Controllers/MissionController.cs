using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;
using System.Reflection;
using System.Collections.Generic;

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

        //כל המשימות
        [HttpGet]
        public async Task<IActionResult> GetAMissions()
        {
            var missions = await _contection.missions.Include(m => m.Agent).Include(m => m.Target).ToArrayAsync();//שליפה של כל המשימות
            return StatusCode(
                200, missions);                            
        }

        [HttpGet("summery")]
        public async Task<IActionResult> GetMissionsSummery()
        {
            var list = await _contection.missions.ToArrayAsync();
            var suggestion = list.Count(ag => ag.Status == StatusMission.statusMission.Suggestion.ToString());//רשימה שתכיל כל המשימות המוצעות

            var Actice = list.Count(ag => ag.Status == StatusMission.statusMission.Actice.ToString());//רשימה שתכיל כל המשימות הפעילות

            var Complited = list.Count(ag => ag.Status == StatusMission.statusMission.Comlpeted.ToString());//רשימה שתכיל כל המשימות שהושלמו

            return StatusCode(200, new { suggestion = suggestion, Actice = Actice, Complited = Complited });
        }


        [HttpGet("ratio")]
        public async Task<IActionResult> GetAvailableAgents()
        {
            var listAgents = _contection.agents.Where(ag => ag.Status == StatusAgent.statusAgent.UnderCover.ToString()).ToList();//מכיל סוכנים רדומים
            var list = await _contection.targets.ToArrayAsync(); //שולף טבלה של מטרות וממיר לרשימה

            List<Agent> availableAgents = new List<Agent>();

            for (int i = 0; i<listAgents.Count(); i++)
            {
                foreach (Target target in list)
                {
                    if (target.Status == StatusTarget.statusTarget.Alive.ToString())//וידוא שהסטטוס מתאים
                    {

                        if (MissionService.IsMission(listAgents[i], target) && await IsNotTargeted(target.Id))// וידוא שהמטרה קרובה מספיק ושהיא לא מצוותת למשימה פעילה אחרת
                        {
                            availableAgents.Add(listAgents[i]);
                            break;
                        }
                    }
                }
            }
            return StatusCode(200, new { availableAgents = availableAgents.Count() });
        }
     
        //מתחיל משימה
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var mission = await _contection.missions.Include(m => m.Agent).Include(m => m.Target).FirstOrDefaultAsync(x => x.Id == id);//שליפה של המשימה מהdb 
            
            mission.TimLeft = TimeDistanceService.UpdateTimeLeft(mission);//מעדכן את הזמן שנותר לחיסול

            if (!MissionService.IsMission(mission.Agent, mission.Target)) //?האם המטרה עדיין מספיק קרובה לסוכן?
            {
                _contection.missions.Remove(mission);//לא. מחיקה מהDB
                await _contection.SaveChangesAsync();

                return StatusCode(400);
            }

            mission.Status = StatusMission.statusMission.Actice.ToString();//מעדכן את המשימה לפעילה
           
            mission.Agent.Status = StatusAgent.statusAgent.OnAMission.ToString();//active" מעדכן סטטוס של סוכן ל"

            await _contection.SaveChangesAsync();

            var missions1 = await _contection.missions.Where(m => m.Agent == mission.Agent && m.Id != mission.Id || m.Target == mission.Target && m.Id != mission.Id).ToListAsync();            

            _contection.missions.RemoveRange(missions1);//הסרה של המשימות שהוצעו לסוכן ולמטרה שכרגע צוותו                                  
            await _contection.SaveChangesAsync();
            
            await _contection.SaveChangesAsync();

            return RedirectToAction("GetAMissions");
        }

        //עידכון סטטוס של כל המשימות הפעילות - הזזה של הסוכנים שלהם
        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissions()
        {
            var list = await _contection.missions.Include(m => m.Agent).Include(m => m.Target).ToArrayAsync();//שליפה של כל המשימות

            foreach (var mission in list)
            {
                if (mission.Status != StatusMission.statusMission.Actice.ToString())//מסנן את המשימות הלא פעילות
                {
                    continue;
                }

                var agent = mission.Agent;
                var target = mission.Target;

                var comand = MissionService.CreateCommeandForAgent(agent, target);//יצירת פקודה לתזוזה של הסוכן
                
                agent = MoveService.MoveAgent(comand, agent);//הזזה בפועל של הסוכן

                if (agent.x == target.x && agent.y == target.y)//ביצוע חיסול אם הסוכן השיג את המטרה והם כעת על אותה משבצת
                {
                    mission.Target.Status = StatusTarget.statusTarget.Eliminated.ToString();
                    mission.Agent.Status = StatusAgent.statusAgent.UnderCover.ToString();
                    mission.TimLeft = 0;
                    mission.Duration += 0.2;
                    mission.Status = StatusMission.statusMission.Comlpeted.ToString();                    
                    continue;
                }

                mission.TimLeft = TimeDistanceService.UpdateTimeLeft(mission);
                mission.Duration = TimeDistanceService.UpdateDuration(mission);
            }
            _contection.SaveChanges();

            return StatusCode(200);
        }


        //בדיקה האם המטרה המוצעת למשימה פעילה
        private async Task<bool> IsNotTargeted(int id)
        {
            Mission mission = await _contection.missions.FirstOrDefaultAsync(x => x.Target.Id == id); //שליפה מהמסד נתונים
            if (mission == null || mission.Status == StatusMission.statusMission.Suggestion.ToString())
            {
                return true;
            }
            return false;
        }
        }
}
