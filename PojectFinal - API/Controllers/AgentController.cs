using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PojectFinal___API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        //חיבור לDB
        private readonly DbConnection _contection;        
        public AgentController(DbConnection dbConnection)
        {
            this._contection = dbConnection;
        }

        //--רשימת סוכנים--
        [HttpGet]
        public async Task<IActionResult> GetAgent()
        {
            
            var list = await _contection.agents.ToArrayAsync();  //שולף טבלה של סוכנים וממיר לרשימה

            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    success = true,
                    agents = list
                });
        }

        //יצירת מטרה
        [HttpPost]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {

            agent.Status = StatusAgent.statusAgent.UnderCover.ToString(); //מגדיר סטטוס של סוכן
            _contection.agents.Add(agent); //מסויף לDB

            await _contection.SaveChangesAsync();

            return StatusCode(
                StatusCodes.Status201Created,
                new { success = true, agent = agent });
        }

        //מגדיר מיקום של סוכן חדש
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> SetLocation(int id, [FromBody] Location location)
        {
            Agent agent = await _contection.agents.FirstOrDefaultAsync(x => x.Id == id); //מציאת הסוכן לפי id
           
            if (agent.Status != StatusAgent.statusAgent.UnderCover.ToString()) //מוודא שהסוכן רדום
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Erroe = "cannot set location for an Agent that is on a mission!" });
            }

            //מעדכן מיקום
            agent.x = location.x;
            agent.y = location.y;

            CreateMissions(agent);

            await _contection.SaveChangesAsync();//שמירה

            return StatusCode(
            StatusCodes.Status200OK,
            new { messege = "target has been located", agent = agent }
            );
        }

        //אחראי להזיז סוכן בהתאם לקריאה מהסימולטור
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveAgent(int id, [FromBody] string move)
        {
            Agent agent = await _contection.agents.FirstOrDefaultAsync(x => x.Id == id); //שליפה מהמסד נתונים

            if (!Move.Moves.ContainsKey(move)) //שולל מקרה של פקודה לא טובה
            {
                return BadRequest();
            }

            agent = MoveService.MoveAgent(move, agent); //מבצע את התזוזה של הסוכן בהתאם לפקודה

            CreateMissions(agent);

            await _contection.SaveChangesAsync(); //שמירה

            return StatusCode(
            StatusCodes.Status200OK);
        }

        private async Task CreateMissions(Agent agent)
        {
            var list = await _contection.targets.ToArrayAsync(); //שולף טבלה של מטרות וממיר לרשימה

            foreach (Target target in list)
            {
                if (target.Status == StatusTarget.statusTarget.Alive.ToString())//וידוא שהסטטוס מתאים
                {

                    if (MissionService.IsMission(agent, target) && await IsNotTargeted(target.Id))// וידוא שהמטרה קרובה מספיק ושהיא לא מצוותת למשימה פעילה אחרת
                    {
                        Mission mission = MissionService.CreateMission(agent, target);//השמה של משימה
                        _contection.missions.Add(mission);//הוספה לטבלה
                    }
                }
            }
        }

        //בדיקה האם המטרה משויכת למשימה פעילה
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
