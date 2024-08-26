using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;

namespace PojectFinal___API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        //חיבור לDB
        private readonly DbConnection _contection;        
        public AgentsController(DbConnection dbConnection)
        {
            this._contection = dbConnection;
        }

        //--רשימת סוכנים--
        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {           
            var list = await _contection.agents.ToArrayAsync();  //שולף טבלה של סוכנים וממיר לרשימה
            Console.WriteLine(list);
            return StatusCode(200, list);                
        }        

        [HttpGet("summery")] 
        public async Task<IActionResult> GetAgentsSummery()
        {
            var list = await _contection.agents.ToArrayAsync();
            var undetCover = list.Count(ag => ag.Status == StatusAgent.statusAgent.UnderCover.ToString());//רשימה שתכיל כל הסוכנים הרדומים

            var onAmission = list.Count(ag => ag.Status == StatusAgent.statusAgent.OnAMission.ToString());//רשימה שתכיל כל הסוכנים הפעילים

            return StatusCode(200, new {UnderCover = undetCover, OnAmission = onAmission});
        }
        
        //יצירת מטרה
        [HttpPost]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {

            agent.Status = StatusAgent.statusAgent.UnderCover.ToString(); //מגדיר סטטוס של סוכן
            _contection.agents.Add(agent); //מסויף לDB

            await _contection.SaveChangesAsync();
            agent = await _contection.agents.FirstOrDefaultAsync(ag => ag.Id == agent.Id);//מושך את הסוכן כדי שיהיה לו id

            return StatusCode(201, new { success = true, id = agent.Id });
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

            await CreateMissions(agent);

            await _contection.SaveChangesAsync();//שמירה

            return StatusCode(
            StatusCodes.Status200OK,
            new { messege = "target has been located", agent = agent }
            );
        }

        //אחראי להזיז סוכן בהתאם לקריאה מהסימולטור
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveAgent(int id, [FromBody] Diraction dir)
        {
            Agent agent = await _contection.agents.FirstOrDefaultAsync(x => x.Id == id); //שליפה מהמסד נתונים

            var move = dir.direction;

            if (agent.Status ==  StatusAgent.statusAgent.OnAMission.ToString())//בדיקה שהסטטוס מתאים
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { messege = "cannot move an agent on a mission!" });
            }
           
            if (!MoveService.IsMoveOutOfRange(agent.x, agent.y, move))//שגיאה אם התזוזה היא מחוץ לגבולות
            {
            return StatusCode(400, new {messege = "cannot move outside of boarders!"}); 
            }


            agent = MoveService.MoveAgent(move, agent); //מבצע את התזוזה של הסוכן בהתאם לפקודה

            await CreateMissions(agent);

            await _contection.SaveChangesAsync(); //שמירה

            return StatusCode(
            StatusCodes.Status200OK);
        }

        //יוצר משימות
        private async Task CreateMissions(Agent agent)
        {
            var list = await _contection.targets.ToArrayAsync(); //שולף טבלה של מטרות וממיר לרשימה

            foreach (Target target in list)
            {
                if (target.Status == StatusTarget.statusTarget.Alive.ToString() && MissionService.IsMission(agent, target) && await IsNotTargeted(target.Id))//וידוא שהסטטוס מתאים ושהמטרה קרובה מספיק ושהיא לא מצוותת למשימה פעילה אחרת
                {                   
                    var listMissions = await _contection.missions.Where(m => m.Agent.Id == agent.Id && m.Target.Id == target.Id).ToArrayAsync(); //מוציא משימות שמוצעות כבר לסוכן ולמטרה הזאת

                    var tempmission = MissionService.CreateMission(agent, target);// השמה של משימה זמנית
                        
                    var mission = new Mission();//השמת המשימה שתחזור -היעילה מביניהם
                        
                    if (listMissions != null)
                    {
                        mission = MissionService.BestSugestion(listMissions, tempmission);//פונקציה מחזירה את ההצעה הטובה ביותר
                    }
                    _contection.missions.Add(mission);//הוספה לטבלה
                    
                }
            }
            await _contection.SaveChangesAsync();
        }

        //בדיקה האם המטרה המוצעת למשימה פעילה
        private async Task<bool> IsNotTargeted(int id)
        {
            List<Mission> missions = await _contection.missions.Where(x => x.Target.Id == id).ToListAsync(); //שליפה מהמסד נתונים רשימה של משימות על המטרה
            foreach (var mission in missions)
            {
                if (mission.Status == StatusMission.statusMission.Actice.ToString())//כל משימה נבדקת ואם היא פעילה יוחזר false
                {
                    return false;
                }
            }               
            return true;
        }
    }
}
