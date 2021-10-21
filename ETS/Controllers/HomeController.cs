using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETS.Controllers
{
    public class HomeController : BaseController
    {

        [HttpGet]
        [Route("~/")]
        public IActionResult Index()
        {
            return Ok("The ETS API is Working smoothly!");
        }
    }
}
