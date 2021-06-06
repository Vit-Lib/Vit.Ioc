using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ValuesController : ControllerBase
    {  
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IEnumerable<IUser> userList)
        {
            return userList.Select(g=>g.GetInfo()).ToList() ;
        }
         
    }
}
