using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{   
    [RoutePrefix("Logger")]    
    public class LoggerController : BaseApiController
    {
        [Route("GetLogDetails")]
        [HttpPost]      
        public IHttpActionResult GetLogDetails()
        {
            var result = _unitOfWork.GetLoggerRepositoryInstance.GetLoggerDetails();
            if(result !=null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
