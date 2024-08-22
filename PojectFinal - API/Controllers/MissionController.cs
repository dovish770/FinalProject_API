using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PojectFinal___API.Services;

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



        [HttpPost("update")]
        public async Task<IActionResult> Update()
        {
            var list = await _contection.missions.ToArrayAsync();

            foreach (var mission in list) {
                var agent = mission.Agent;

                var comand = MissionService.CreateCommeandForAgent(agent, mission.Target);

                if (comand == "kill") {
                    //Kill(mission);
                    break;
                }

                agent.

                




            return null;

    }
}
