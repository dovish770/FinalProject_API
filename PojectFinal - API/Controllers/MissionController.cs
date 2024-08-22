using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        

        //[HttpPost("update")]
        //public IActionResult Update()
        //{
        //    return
        //}
    }
}
