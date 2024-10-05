using Aadyam.SDS.Business.Repositories.UnitOfWork;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        protected internal IUnitOfWork _unitOfWork;
        public BaseApiController()
        {
            _unitOfWork = new UnitOfWork();
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
