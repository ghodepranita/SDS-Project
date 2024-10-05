using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Deliveryboy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("Deliveryboy")]
    public class DeliveryboyController : BaseApiController
    {
        private decimal LogId = 0;

        #region Get Deliveryboy Deviation Details
        /// <summary>
        /// Get Deliveryboy Deviation Details
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDeliveryboyDeviationDtls")]
        public IHttpActionResult GetDeliveryboyDeviationDtls([FromBody]PostModelDel postModel)
        {
            GetDeliveryboyDeviation model = new GetDeliveryboyDeviation();
            string jsonString = string.Empty;
            if (postModel == null && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                jsonString = JsonConvert.SerializeObject(postModel);
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (!String.IsNullOrEmpty(postModel.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(postModel.FromDate);
                if (!String.IsNullOrEmpty(postModel.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(postModel.ToDate);
                using (_unitOfWork)
                {
                    model = _unitOfWork._DeliveryboyRespository.deliveryboyDeviationList(postModel.DistributorId, Convert.ToDecimal(postModel.StaffRefNo), FromDate, ToDate); 
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Get Deliveryboy Deviation Details", "Input string= " + jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Get Delivery Boy Overhead Less Orders Details
        /// <summary>
        /// Get Delivery Boy Overhead Less Orders Details
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDelOverheadLessOrdersDtls")]
        public IHttpActionResult GetDelOverheadLessOrdersDtls([FromBody]PostModelDel postModel)
        {
            GetDeliveryBoyOverheadLessOrders model = new GetDeliveryBoyOverheadLessOrders();
            string jsonString = string.Empty;
            if (postModel == null && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                jsonString = JsonConvert.SerializeObject(postModel);
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (!String.IsNullOrEmpty(postModel.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(postModel.FromDate);
                if (!String.IsNullOrEmpty(postModel.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(postModel.ToDate);
                model = _unitOfWork._DeliveryboyRespository.DeliveryBoyOverheadLessOrders(postModel.DistributorId,Convert.ToDecimal(postModel.StaffRefNo), FromDate, ToDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Get Delivery Boy Overhead Less Orders Details", "Input string= " + jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion
        
        #region Add Edit Consumer Delivery Details
        /// <summary>
        /// Get Consumer Delivery Details
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddEditConsumerDeliveryDtls")]
        public IHttpActionResult AddEditConsumerDeliveryDtls([FromBody]ConsModelDel postModel)
        {
            string jsonString = string.Empty;
            GetConsModelDel model = new GetConsModelDel();
            if (postModel == null && postModel.DistributorId == 0 && postModel.ConsumerNo == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                jsonString = JsonConvert.SerializeObject(postModel);
                model = _unitOfWork._DeliveryboyRespository.AddEditConsumerDeliveryDtls(postModel.Id, postModel.ConsumerNo,postModel.DistributorId,postModel.StaffRefNo,postModel.Lat,postModel.Lng,postModel.InsertedDatetime);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Get Consumer Delivery Details", "Input string= " + jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion
        
        #region Save Delivery Route Coordinates
        /// <summary>
        ///  Save Delivery Route Coordinates
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveDeliveryRouteCoordinates")]
        public IHttpActionResult SaveDeliveryRouteCoordinates([FromBody]DeliveryRouteCoordinates postModel)
        {
            DeliveryRouteCoordinates deliveryRouteCoordinates = new DeliveryRouteCoordinates();
            if (postModel == null && postModel.DistributorId > 0)
                return BadRequest(BusinessCont.InvalidClientRqst);
            deliveryRouteCoordinates = _unitOfWork._DeliveryboyRespository.SaveDeliveryRouteCoordinates(postModel);
            return Ok(deliveryRouteCoordinates);
        }
        #endregion

        #region Get 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consModelDel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDeliveryRouteCoordinates")]
        public IHttpActionResult GetDeliveryRouteCoordinates(CSpostModel postModel)
        {
            GetCSDeliveryRoute model = new GetCSDeliveryRoute();
            string jsonString = "";
            model.deliveryRoute = null;
            model.Status = BusinessCont.FailStatus;
            if (postModel == null && postModel.DistributorId == 0 && postModel.StaffRefNo == "")
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                jsonString = JsonConvert.SerializeObject(postModel);
                DateTime? SelDate = null;
                if (!String.IsNullOrEmpty(postModel.SelDate))
                    SelDate = BusinessCont.StrConvertIntoDatetime(postModel.SelDate);

                model.deliveryRoute = _unitOfWork._DeliveryboyRespository.GetDeliveryRouteCoordinates(postModel.DistributorId, postModel.StaffRefNo, SelDate);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetDeliveryRouteCoordinates", jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion
        
        /// <summary>
        /// GetConsumerDeliveryDetailsByStaffRefNo
        /// </summary>
        /// <param name="consModelDel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetConsumerDeliveryDetailsByStaffRefNo")]
        public IHttpActionResult GetConsumerDeliveryDetailsByStaffRefNo(ConsModelDel consModelDel)
        {
            GetConsModelDel model = new GetConsModelDel();
            string jsonString = "";
            model.ConsModelDelList = null;
            if (consModelDel == null && consModelDel.DistributorId == 0 && consModelDel.StaffRefNo == "")
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                 jsonString = JsonConvert.SerializeObject(consModelDel);
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (!String.IsNullOrEmpty(consModelDel.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(consModelDel.FromDate);
                if (!String.IsNullOrEmpty(consModelDel.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(consModelDel.ToDate);

                model.ConsModelDelList = _unitOfWork._DeliveryboyRespository.GetConsumerDeliveryDetailsByStaffRefNo(consModelDel.DistributorId, consModelDel.StaffRefNo, FromDate, ToDate);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(0, 0, 0, 0, 0,"Get Consumer Delivery Details Route", jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        
        [HttpPost]
        [Route("DBwiseConsRecorrectionCnt")]
        public DBwiseConsRecorrectionCnt GetClusterAreawiseCounts([FromBody]PostModelDel consModelDel)
        {
            DBwiseConsRecorrectionCnt consumerDetails = new DBwiseConsRecorrectionCnt();
            string jsonString = "";
            try
            {
                jsonString = JsonConvert.SerializeObject(consModelDel);
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (!String.IsNullOrEmpty(consModelDel.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(consModelDel.FromDate);
                if (!String.IsNullOrEmpty(consModelDel.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(consModelDel.ToDate);
                consumerDetails.DbwiseCnt = new List<ConsumerRecorrectionCnt>();
                consumerDetails = _unitOfWork._DeliveryboyRespository.GetDbwiseConsumerRecorrectionCount(consModelDel.DistributorId, consModelDel.StaffRefNo, FromDate);
                consumerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                consumerDetails.Status = BusinessCont.FailStatus;
                consumerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "DBwiseConsRecorrectionCnt", jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return consumerDetails;
        }

        [HttpPost]
        [Route("UnVerifiedConsumerLst")]
        public UnVerifiedConsumerLst GetConsumerMaster([FromBody]UnVerifiedConsumerLst postModel)
        {
            UnVerifiedConsumerLst unVerifiedConsumerLst = new UnVerifiedConsumerLst();
            unVerifiedConsumerLst.Status = BusinessCont.FailStatus;
            try
            {
                if (!String.IsNullOrEmpty(postModel.StaffRefNo) && !String.IsNullOrEmpty(postModel.RecordFor))
                    unVerifiedConsumerLst = _unitOfWork._DeliveryboyRespository.GetUnVerifiedConsumerLst(postModel);
            }
            catch (Exception ex)
            {
                unVerifiedConsumerLst.Status = BusinessCont.FailStatus;
                unVerifiedConsumerLst.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UnVerifiedConsumerLst", null, BusinessCont.FailStatus, ex.Message);
            }
            return unVerifiedConsumerLst;
        }
    }
}
