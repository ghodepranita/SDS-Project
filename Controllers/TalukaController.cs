using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Taluka;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("Taluka")]
    public class TalukaController : BaseApiController
    {
        /// <summary>
        /// Get Taluka and District Master Details  
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTalukaDistrictMaster")]
        public IHttpActionResult GetTalukaDistrictMaster([FromBody]MasterPostModel postModel)
        {
            if (postModel == null && !string.IsNullOrEmpty(postModel.tblName) && !string.IsNullOrEmpty(postModel.code))
                return BadRequest(BusinessCont.InvalidClientRqst);

            MasterPostModel model = new MasterPostModel();
            model = _unitOfWork._TalukaRepository.GetMasterDetailsStatusWise(postModel.DistributorId, postModel.code,postModel.StateCode, postModel.tblName);
            return Ok(model);
        }

        /// <summary>
        /// Get Taluka and District Master Details  
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDistributorTalukaList")]
        public IHttpActionResult GetDistributorTalukaList([FromBody]MasterPostModel postModel)
        {
            if (postModel == null && !string.IsNullOrEmpty(postModel.tblName) && !string.IsNullOrEmpty(postModel.code))
                return BadRequest(BusinessCont.InvalidClientRqst);

            MasterPostModel model = new MasterPostModel();
            model = _unitOfWork._TalukaRepository.GetDistributorTaluka(postModel.DistributorId);
            return Ok(model);
        }

        [HttpPost]
        [Route("GetStateMaster")]
        public IHttpActionResult GetStateMaster([FromBody]StateMasterModel postModel)
        {
            if (postModel == null && !string.IsNullOrEmpty(postModel.IsActive))
                return BadRequest(BusinessCont.InvalidClientRqst);

            StateMasterModel model = new StateMasterModel();
            model = _unitOfWork._TalukaRepository.GetStateMaster(postModel.IsActive);
            return Ok(model);
        }


        /// <summary>
        /// Add Update Distributor Taluka
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUpdateDistributorTaluka")]
        public IHttpActionResult AddUpdateDistributorTaluka([FromBody]DistTalukaModel postModel)
        {
            if (postModel == null && postModel.DistributorId > 0 && !string.IsNullOrEmpty(postModel.DistrictCode))
                return BadRequest(BusinessCont.InvalidClientRqst);

            MasterPostModel model = new MasterPostModel();
            model = _unitOfWork._TalukaRepository.AddUpdateDistributorTaluka(postModel);
            return Ok(model);
        }

    }
}
