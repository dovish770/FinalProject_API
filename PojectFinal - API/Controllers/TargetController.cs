using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PojectFinal___API.Modles;
using PojectFinal___API.Services;
using PojectFinal___API.Enums;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;

namespace PojectFinal___API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        //חיבור למסד נתונים
        private readonly DbConnection _contection;        
        public TargetsController(DbConnection dbConnection)
        {
            this._contection = dbConnection;
        }

        //--רשימת מטרות--
        [HttpGet]
        public async Task<IActionResult> GetTargets()
        {
            var list = await _contection.targets.ToArrayAsync(); //שליפה של טבלת מטרות והמרה למערך

            return StatusCode(
                StatusCodes.Status200OK,
                new
                {
                    success = true,
                    targets = list
                });
        }

        [HttpGet("summery")]
        public async Task<IActionResult> GetTargetsSummery()
        {
            var list = await _contection.targets.ToArrayAsync();
            var alive = list.Count(ag => ag.Status == StatusTarget.statusTarget.Alive.ToString());//רשימה שתכיל כל המטרות החיות

            var eliminated = list.Count(ag => ag.Status == StatusTarget.statusTarget.Eliminated.ToString());//רשימה שתכיל כל המטרות המחוסלות

            return StatusCode(200, new { Alive = alive, Eliminated = eliminated });
        }

        //יצירת מטרה
        [HttpPost]
        public async Task<IActionResult> CreateTarget(Target target)
        {
            target.Status = StatusTarget.statusTarget.Alive.ToString(); //הגדרת סטטוס          
            _contection.targets.Add(target); //הוספה לטבלה
            await _contection.SaveChangesAsync(); //שמירה
            target = await _contection.targets.FirstOrDefaultAsync(ta => ta.Id == target.Id);
            return StatusCode(
                StatusCodes.Status201Created,
                new { success = true, id = target.Id });
        }

        //הגדרת מיקום
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> SetLocation(int id, [FromBody] Location location)
        {
            Target target = await _contection.targets.FirstOrDefaultAsync(x => x.Id == id); //שליפת מטרה מטבלת מסד נתונים

            if (target.Status != StatusTarget.statusTarget.Alive.ToString()) //בדיקה אם הסטוס מתאים
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Erroe = "cannot setlocation for a target that has already been eliminted!" });
            }            

            //החלה של המיקום במטרה
            target.x = location.x;
            target.y = location.y;

            await CreateMissions(target); //יצירת משימות

            await _contection.SaveChangesAsync(); //שמירה

            return StatusCode(
            StatusCodes.Status200OK,
            new { messege = "target has been located", targate = target }
            );
        }

        //הזז מטרה
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveTarget(int id, [FromBody] Diraction Dir)
        {
            Target target = await _contection.targets.FirstOrDefaultAsync(x => x.Id == id); //שליפת מטרה מטבלת מסד נתונים
            var move = Dir.direction;

            if (target.Status ==  StatusTarget.statusTarget.Eliminated.ToString())//בדיקה שהסטטוס מתאים
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { messege = "cannot move a target that has been eliminated!" });
            }

            if (!MoveService.IsMoveOutOfRange(target.x, target.y, move))//שגיאה אם התזוזה היא מחוץ לגבולות
            {
                return StatusCode(400, new { messege = "cannot move outside of boarders!" });
            }
                      
            var commands = Move.Moves[move]; //שליפת ממילון של קאודינטות בהתאם לפקודה

            //החלה של המיקום במטרה
            target.x += commands.x;
            target.y += commands.y;

            await CreateMissions(target); //יצירת משימות

            await _contection.SaveChangesAsync(); //שמירה

            return Ok();
        }

        //יצירת משימות
        private async Task CreateMissions(Target target)
        {
            var list = await _contection.agents.ToArrayAsync(); //שליפה של טבלת מטרות והמרה למערך

            //בדיקה אם ליצור משימה
            foreach (Agent agent in list) 
            {
                if (agent.Status == StatusAgent.statusAgent.UnderCover.ToString()) //בדיקה שהסטטוס מתאים
                {
                    if (MissionService.IsMission(agent, target)) //בדיקה אם הקירבה מספיקה כדי ליצור משימה
                    {
                        var listMissions = await _contection.missions.Where(m => m.Agent.Id == agent.Id && m.Target.Id == target.Id).ToArrayAsync(); //מוציא משימות שמוצעות כבר לסוכן ולמטרה הזאת

                        var tempmission = MissionService.CreateMission(agent, target);// השמה של משימה זמנית
                        var mission = new Mission();//השמת המשימה שתחזור -היעילה מביניהם

                        if (listMissions != null)
                        {
                            mission = MissionService.BestSugestion(listMissions, tempmission);//פונקציה מחזירה את ההצעה הטובה ביותר
                            _contection.missions.Add(mission); //הוספה למסד נתונים                        
                        }
                    }
                }
            }
            await _contection.SaveChangesAsync();
            return;
        }

    }
}
