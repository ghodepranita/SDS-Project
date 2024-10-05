using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Distributor;
using Aadyam.SDS.Business.Model.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class SAController : BaseApiController
    {
        decimal LogId = 0;
        [HttpGet]
        [Route("SA/GetSADashboardCounts/{SACode}")]
        public DistributorCount GetSADashboardCounts(string SACode)
        {
            DistributorCount modelCount = new DistributorCount();
            try
            {
                modelCount = _unitOfWork.SARepository.GetSADashboardCounts(SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetSADashboardCounts", "SaCode" + SACode, BusinessCont.FailStatus, ex.Message);
            }
            return modelCount;
        }

        #region
        [HttpPost]
        [Route("SA/UpdateFeedbackSystemStatus")]
        public IHttpActionResult UpdateFeedbackSystemStatus([FromBody]UpdateFeedbackSystem postModel)
        {
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            UpdateFeedbackSystem model = new UpdateFeedbackSystem();
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                model.Id = _unitOfWork.UserRepository.UpdateFeedbackSystemStatus(postModel);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateFeedbackSystemStatus", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        /// <summary>
        /// SA dashboard stock details.
        [HttpPost]
        [Route("SA/DateWiseOrderCountDetailsForSA")]
        public DistributorDashboard DateWiseOrderCountDetailsForSA([FromBody]DistributorDashboard DistDetails)
        {
            DistributorDashboard distributorDashboard = new DistributorDashboard();
            distributorDashboard.lineGraphModel = new LineGraphModel();
            distributorDashboard.Status = BusinessCont.FailStatus;
            DateTime FromDate = DateTime.Now, ToDate = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(DistDetails.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(DistDetails.FromDate);
                if (!string.IsNullOrEmpty(DistDetails.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(DistDetails.ToDate);

                distributorDashboard.lineGraphModel = _unitOfWork.SARepository.GetDateWiseOrderCountForSA(DistDetails.SACode, FromDate, ToDate);
                distributorDashboard.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                distributorDashboard.Status = BusinessCont.FailStatus;
                distributorDashboard.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DateWiseOrderCountDetailsForSA", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorDashboard;
        }

        /// <summary>
        /// get distributor wise Step Status 
        /// </summary>
        /// <param name="SACode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SA/GetDistributorOnBoarding")]
        public IHttpActionResult GetDistributorOnBoarding([FromBody]DistirbutorOnBoarding postModel)
        {
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorOnBoardingModel model = new DistributorOnBoardingModel();
            model = _unitOfWork.SARepository.GetDistributorWiseOnBoardingStatus(postModel.SAcode,postModel.DistributorId);
            return Ok(model);
        }

        [HttpPost]
        [Route("SA/GetDistributorOnBoardingACL")]
        public IHttpActionResult GetDistributorOnBoardingACL([FromBody]DistirbutorOnBoarding postModel)
        {
            DistributorOnBoardingModel model = new DistributorOnBoardingModel();
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            model = _unitOfWork.SARepository.GetDistributorWiseOnBoardingStatusFromACL(postModel.DistributorIds);
            return Ok(model);
        }

        /// <summary>
        /// get distributor wise Step Status 
        /// </summary>
        /// <param name="SACode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SA/OnboardingApproveReject")]
        public IHttpActionResult OnboardingApproveReject([FromBody]OnboardingApprove postModel)
        {
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            OnboardingApprove model = new OnboardingApprove();
            model = _unitOfWork.SARepository.OnboardingApproveReject(postModel);
            return Ok(model);
        }

        #region Get Cluster Edit Request List
        [HttpGet]
        [Route("SA/GetClusterEditRequestList/{SACode}")]
        public List<clusterEditReqstModel> GetClusterEditRequestList(string SACode)
        {
            List<clusterEditReqstModel> clReqList = new List<clusterEditReqstModel>();
            try
            {
                clReqList = _unitOfWork.SARepository.GetClusterEditRequestList(SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterEditRequestList", "GetClusterMasterList Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return clReqList;
        }
        #endregion

        #region Approve Cluster Edit Request
        [HttpPost]
        [Route("SA/ApproveClusterEditRequest")]
        public int ApproveClusterEditRequest([FromBody]clusterEditReqstModel model)
        {
            int result = 0;
            try
            {
                result = _unitOfWork.SARepository.ApproveClusterEditRequest(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "ApproveClusterEditRequest", " " + "", BusinessCont.FailStatus, ex.Message);
            }
            return result;
        }
        #endregion

        #region Get Distributor For Activation By SA
        [HttpGet]
        [Route("SA/GetDistributorForActivationBySA/{SACode}")]
        public List<distributorActivateModel> GetDistributorForActivationBySA(string SACode)
        {
            List<distributorActivateModel> modellist = new List<distributorActivateModel>();
            try
            {
                modellist = _unitOfWork.SARepository.GetDistributorForActivationBySA(SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistributorForActivationBySA", "GetDistributorForActivationBySA Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return modellist;
        }
        #endregion

        #region Save Distributor Data For Activation
        [HttpPost]
        [Route("SA/SaveDistributorDataForActivation")]
        public int SaveDistributorDataForActivation([FromBody]distributorActivateModel model)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.SARepository.SaveDistributorDataForActivation(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveDistributorDataForActivation", "SaveDistributorDataForActivation Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

    }
}
