using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Distributor;
using Aadyam.SDS.Business.Model.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Aadyam.SDS.Business.Model.Trip;
using System.Linq;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("Distributor")]
    public class DistributorController : BaseApiController
    {
        decimal LogId = 0;

        #region Distributor Dashboard
        /// <summary>
        /// dashboard stock details.
        /// </summary>
        /// <param name="staffConfiguration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DistributorDashboardDetails")]
        public DistributorDashboard DistributorDashboardDetails([FromBody]DistributorDashboard DistDetails)
        {
            DistributorDashboard distributorDashboard = new DistributorDashboard();
            distributorDashboard.Status = BusinessCont.FailStatus;
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                distributorDashboard.GetDistributorCounts = _unitOfWork.distributorRepository.GetDistributorCountDetails(DistDetails.DistributorId, AppConfigAPITimeout);
                distributorDashboard.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                distributorDashboard.Status = BusinessCont.FailStatus;
                distributorDashboard.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DistributorDashboardDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorDashboard;
        }

        /// <summary>
        /// dashboard stock details.
        /// </summary>
        /// <param name="staffConfiguration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DateWiseOrderCountDetails")]
        public DistributorDashboard DateWiseOrderCountDetails([FromBody]DistributorDashboard DistDetails)
        {
            DistributorDashboard distributorDashboard = new DistributorDashboard();
            distributorDashboard.Status = BusinessCont.FailStatus;
            try
            {
                DateTime FromDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;
                if (!String.IsNullOrEmpty(DistDetails.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(DistDetails.FromDate);
                if (!String.IsNullOrEmpty(DistDetails.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(DistDetails.ToDate);

                distributorDashboard.lineGraphModel = _unitOfWork.distributorRepository.GetDateWiseOrderCount(DistDetails.DistributorId, FromDate, ToDate);
                distributorDashboard.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {

                distributorDashboard.Status = BusinessCont.FailStatus;
                distributorDashboard.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DateWiseOrderCountDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorDashboard;
        }

        [HttpPost]
        [Route("DistributorNotificationDetails")]
        public DistributorNotification DistributorNotificationDetails([FromBody]DistributorNotificationModel model)
        {
            DistributorNotification distributorNotification = new DistributorNotification();
            distributorNotification.Status = BusinessCont.FailStatus;
            try
            {
                string JsonString = JsonConvert.SerializeObject(model);
                distributorNotification.distNotificationDtls = _unitOfWork.distributorRepository.GetDistributorNotification(model.DistributorId, model.NotiFor, null);
                distributorNotification.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                distributorNotification.Status = BusinessCont.FailStatus;
                distributorNotification.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DistributorNotificationDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorNotification;
        }
        #endregion

        #region Distributor Location
        /// <summary>
        /// Get Distributor Location
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDistributorLocation")]
        public IHttpActionResult GetDistributorLocation([FromBody]DistributorLocationModel postModel)
        {
            DistributorLocationModel model = new DistributorLocationModel();
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            model = _unitOfWork.distributorRepository.GetDistributorLocation(postModel.DistributorId);
            return Ok(model);
        }

        /// <summary>
        /// Add Update Distributor Location
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUpdateDistributorLocation")]
        public IHttpActionResult AddUpdateDistributorLocation([FromBody]DistributorLocationModel postModel)
        {
            int result = 0;
            if (postModel == null && postModel.DistributorId > 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            result = _unitOfWork.distributorRepository.AddDistributorLocation(postModel);
            return Ok(result);
        }
        #endregion

        #region On Boarding Check
        [HttpPost]
        [Route("OnBoardingCheck")]
        public OnBoardingCheck CheckAfterLogin([FromBody] DistributorModel distributorModel)
        {
            OnBoardingCheck model = new OnBoardingCheck();
            try
            {
                model = _unitOfWork.distributorRepository.AfterLoginCheck(distributorModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, distributorModel.DistributorId, 0, 0, "OnBoardingCheck", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region On Boarding Finished
        [HttpPost]
        [Route("OnBoardingFinished")]
        public List<OnBoardingFinish> CheckAfterBoardingFinished([FromBody] OnBoardingPostModel boardingModel)
        {
            List<OnBoardingFinish> modelList = new List<OnBoardingFinish>();
            try
            {
                modelList = _unitOfWork.distributorRepository.AfterOnBoadingFinished(boardingModel.DistributorId, boardingModel.IsOnBoardingStage1, boardingModel.IsOnBoardingStage2);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, boardingModel.DistributorId, 0, 0, "OnBoardingFinished", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Distributor wise Consumer Counts
        [HttpPost]
        [Route("GetDistributorwiseConsumerCounts")]
        public DistributorwiseConsumerCounts GetDistributorwiseConsumerCounts([FromBody] PostModel model)
        {
            DistributorwiseConsumerCounts modelCount = new DistributorwiseConsumerCounts();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                modelCount = _unitOfWork.distributorRepository.GetDistributorwiseCounts(model.DistributorId, model.AreaRefNo, AppConfigAPITimeout);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "GetDistributorwiseConsumerCounts", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelCount;
        }
        #endregion

        #region Onboarding Edit Request Update
        [HttpPost]
        [Route("OnboardingEditRequestUpdate")]
        public IHttpActionResult OnboardingEditRequestUpdate([FromBody]OnBoardingCheck postModel)
        {
            int result = 0;
            try
            {
                if (postModel == null)
                    return BadRequest(BusinessCont.InvalidClientRqst);
                result = _unitOfWork.distributorRepository.OnboardingEditRequestUpdate(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OnboardingEditRequestUpdate", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Onboarding Stage2 Update
        [HttpPost]
        [Route("OnboardingStage2Update")]
        public IHttpActionResult OnboardingStage2Update([FromBody]OnBoardingCheck postModel)
        {
            int result = 0;
            try
            {
                if (postModel == null)
                    return BadRequest(BusinessCont.InvalidClientRqst);
                result = _unitOfWork.distributorRepository.OnboardingStage2Update(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OnboardingStage2Update", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Onboarding Step Status On Edit
        [HttpPost]
        [Route("OnboardingStepStatusOnEdit")]
        public IHttpActionResult OnboardingStepStatusOnEdit([FromBody]OnBoardingCheck postModel)
        {
            int result = 0;
            try
            {
                if (postModel == null)
                    return BadRequest(BusinessCont.InvalidClientRqst);
                result = _unitOfWork.distributorRepository.OnboardingStepStatusOnEdit(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OnboardingStepStatusOnEdit", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region OnBoarding Finished ACL
        [HttpPost]
        [Route("OnBoardingFinishedACL")]
        public List<OnBoardingFinish> CheckAfterBoardingFinishedACL([FromBody] OnBoardingPostModel boardingModel)
        {
            List<OnBoardingFinish> modelList = new List<OnBoardingFinish>();
            try
            {
                modelList = _unitOfWork.distributorRepository.AfterOnBoadingFinishedACL(boardingModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, boardingModel.DistributorId, 0, 0, "OnBoardingFinishedACL", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Distributor Class Room Log
        [HttpPost]
        [Route("GetDistributorClassRoomLog")]
        public List<ClassRoomLog> GetDistributorClassRoomLog([FromBody]ClassRoomLog ClassRoomLog)
        {
            List<ClassRoomLog> modelList = new List<ClassRoomLog>();
            try
            {
                modelList = _unitOfWork.distributorRepository.GetDistributorClassRoomLog(ClassRoomLog.SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistributorClassRoomLog", "SACode= " + ClassRoomLog.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Delete Cluster Master
        [HttpPost]
        [Route("DeleteClusterMaster")] 
        public OutputMdl DeleteClusterMaster([FromBody]OutputMdl model)
        {
            OutputMdl outputMdl = new OutputMdl();
            try
            {
                outputMdl = _unitOfWork.distributorRepository.DeleteClusterMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "DeleteClusterMaster", "ClusterId= " + model.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return outputMdl;
        }
        #endregion

        #region Delete Cluster Master Aadyam
        [HttpPost]
        [Route("DeleteClusterMasterAadyam")]
        [AllowAnonymous]
        public OutputMdl DeleteClusterMasterAadyam([FromBody]OutputMdl model)
        {
            OutputMdl outputMdl = new OutputMdl();
            try
            {
                outputMdl = _unitOfWork.distributorRepository.DeleteClusterMasterForAadyam(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "DeleteClusterMasterAadyam", "ClusterId= " + model.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return outputMdl;
        }
        #endregion

        #region Get Distributor Cluster Lst
        [HttpPost]
        [Route("GetDistributorClusterLst")]
        public OutputMdl GetDistributorClusterLst([FromBody]OutputMdl model)
        {
            OutputMdl outputMdl = new OutputMdl();
            try
            {
                outputMdl = _unitOfWork.distributorRepository.GetDistributorClusterLst(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "GetDistributorClusterLst", "ClusterId= " + model.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return outputMdl;
        }
        #endregion

        #region Save Distributor Class Room Log
        [HttpPost]
        [Route("SaveDistributorClassRoomLog")]
        public IHttpActionResult SaveDistributorClassRoomLog([FromBody]ClassRoomLog postModel)
        {
            int result = 0;
            if (postModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            result = _unitOfWork.distributorRepository.SaveDistributorClassRoomLog(postModel);
            return Ok(result);
        }
        #endregion

        #region Save Distributor Allotment
        [HttpPost]
        [Route("SaveDistributorAllotment")]
        public RtnPostModel SaveDistributorAllotment([FromBody]SlotAllotmentPostModel slotAllotmentPostModel)
        {
            RtnPostModel model = new RtnPostModel();
            try
            {
                model = _unitOfWork.adminRepository.SaveDistributorAllotedDateTime(slotAllotmentPostModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveDistributorAllotment", "SuperAdmin", BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion
    }
}
