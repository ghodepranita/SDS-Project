using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.BreakDown;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("Breakdown")]
    public class BreakDownController : BaseApiController
    {
        private decimal LogId = 0;

        #region Add Breakdown  Details
        [HttpPost]
        [Route("AddBreakdownDetails")]
        public IHttpActionResult AddBreakdownDetails([FromBody]BreakDownModel postModel)
        {
            BreakDownPostData model = new BreakDownPostData();
            if (postModel == null && !String.IsNullOrEmpty(postModel.DeliveryBoyId) && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                model.BreakdownId = _unitOfWork._breakdownRepository.AddUpdateBreakDownDetails(postModel);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddBreakdownDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Get  Break down Details
        [HttpPost]
        [Route("GetBreakdownDetails")]
        public IHttpActionResult GetBreakdownDetails([FromBody]BreakDownModel postModel)
        {
            BreakDownPostData model = new BreakDownPostData();
            if (postModel == null && !String.IsNullOrEmpty(postModel.DeliveryBoyId) && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                DateTime? BreakDownDateTime = null;
                if (!string.IsNullOrWhiteSpace(postModel.BreakDownDateTime))
                {
                    BreakDownDateTime = BusinessCont.StrConvertIntoDatetime(postModel.BreakDownDateTime);
                }

                model = _unitOfWork._breakdownRepository.GetBreakDownDetails(postModel.DistributorId,Convert.ToDecimal(postModel.DeliveryBoyId), BreakDownDateTime);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetBreakdownDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Get Break down History Details
        [HttpPost]
        [Route("GetBreakdownHistory")]
        public IHttpActionResult GetBreakdownHistory([FromBody]BreakDownModel postModel)
        {
            BreakDownPostData model = new BreakDownPostData();
            if (postModel == null && !String.IsNullOrEmpty(postModel.DeliveryBoyId) && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                DateTime? FromDate = null,ToDate=null;
                if (!string.IsNullOrWhiteSpace(postModel.FromDate))
                {
                    FromDate = BusinessCont.StrConvertIntoDatetime(postModel.FromDate);
                }
                if (!string.IsNullOrWhiteSpace(postModel.ToDate))
                {
                    ToDate = BusinessCont.StrConvertIntoDatetime(postModel.ToDate);
                }

                model = _unitOfWork._breakdownRepository.GetBreakDownHistory(postModel.DistributorId, Convert.ToDecimal(postModel.DeliveryBoyId), FromDate,ToDate);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetBreakdownHistory", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

    }
}
