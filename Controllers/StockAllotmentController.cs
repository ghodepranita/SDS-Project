using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Order;
using Aadyam.SDS.Business.Model.Stock;
using Aadyam.SDS.Business.Model.User;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class StockAllotmentController : BaseApiController
    {
        private decimal LogId = 0;

        #region Delivery Boy
        /// <summary>
        /// Return stock alloted list which contains pending accept stock allot and history of stock alloted list
        /// </summary>
        /// <param name="DistId"></param>
        /// <param name="DeliveryBoyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockAllotment/StockAllotedDetails")]
        public StockAllotedList StockAllotedDetails([FromBody]StaffPostConfiguration staffDtls)
        {
            StockAllotedList stockAllotedList = new StockAllotedList();
            stockAllotedList.StockAltDetails = new List<StockAllotmentDeliveryBoy>();
            try
            {
                stockAllotedList.StockAltDetails = _unitOfWork.stockRepository.GetStockAllotmentList(staffDtls.DistId, staffDtls.StaffRefNo, null, null);
                stockAllotedList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                stockAllotedList.Status = BusinessCont.FailStatus;
                stockAllotedList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "StockAllotedDetails", null, stockAllotedList.Status, ex.Message);
            }
            return stockAllotedList;
        }

        /// <summary>
        /// Accept stock - Delivery boy accept stock request send by godown keeper
        /// </summary>
        /// <param name="StockAltId">if request accept is accept then update against stock allotment id</param>
        /// <param name="DistId"></param>
        /// <param name="DeliveryBoyId"></param>
        /// <returns>return success or fail string</returns>
        [HttpPost]
        [Route("StockAllotment/AcceptStock")]
        public StatusDetails AcceptStock(AcceptStkDtl acceptStkDtl)
        {
            StatusDetails statusDetails = new StatusDetails();
            try
            {
                statusDetails.Status = BusinessCont.FailStatus;
                statusDetails.Status = _unitOfWork.stockRepository.AcceptStockOfDeliveryBoy(acceptStkDtl.StockAltId, acceptStkDtl.DistId, acceptStkDtl.StaffRefNo, acceptStkDtl.IsAccepted, acceptStkDtl.Remark);
            }
            catch (Exception ex)
            {
                statusDetails.Status = BusinessCont.FailStatus;
                statusDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AcceptStock", null, BusinessCont.FailStatus, ex.Message);
            }
            return statusDetails;
        }
        #endregion

        #region Godown Keeper
        /// <summary>
        /// Godown keeper allot or transfer stock to delivery boy.
        /// </summary>
        /// <param name="stockaltDetails">JSON string contains details of stock allotment</param>
        /// <param name="AllotItems">JSON string contains cylinder ItemId and quantity</param>
        /// <returns>return success or fail string</returns>
        [HttpPost]
        [Route("StockAllotment/AddStockAllotment")]
        public StockAllot AddStockAllotment([FromBody]AddStockPost model)
        {
            StockAllot modelStockAllot = new StockAllot();
            modelStockAllot = _unitOfWork.stockRepository.AddStockAllotment(model.stockaltDetails, model.AllotItems, model.StockAllotmentId);
            return modelStockAllot;
        }
        #endregion Godown Keeper

        #region Stock Details By Distributor Id
        /// <summary>
        /// Santosh - Get Distributor Stock by DistributorId
        /// </summary>
        /// <param name="DistributorId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockAllotment/GetStockDetails")]
        public List<DistributorStock> GetStockDetails([FromBody]PostModelForDistStock Model)
        {
            List<DistributorStock> modelList = new List<DistributorStock>();
            modelList = _unitOfWork.stockRepository.GetStockDetails(Model.DistributorId, Model.TripDate);
            return modelList;
        }
        #endregion

        #region Stock Details By Distributor Id and TripDate (For Trip Planning)
        /// <summary>
        /// Santosh - Get Distributor Stock by DistributorId and TripDate
        /// </summary>
        /// <param name="DistributorId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockAllotment/GetStockDetailsForTrip")]
        public List<DistributorStock> GetStockDetailsForTrip([FromBody]PostModelForDistStock Model)
        {
            List<DistributorStock> modelList = new List<DistributorStock>();
            modelList = _unitOfWork.stockRepository.GetStockDetailsForTrip(Model.DistributorId, Model.TripDate);
            return modelList;
        }
        #endregion
  
        /// <summary>
        /// Returns list of records which is pending for accept alloted stock
        /// </summary>
        /// <param name="DistId">Display against distributor Id</param>
        /// <param name="DeliveryBoyId">Display against Delivery boy Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("StockAllotment/PendingAcceptStockAllot/{DistId}/{DeliveryBoyId}")]
        public StockAltList PendingAcceptStockAllot(int DistId, decimal DeliveryBoyId)
        {
            StockAltList objStockAltList = new StockAltList();
            objStockAltList.AcceptStockItemDetails = new List<StockAllotmentDetailsModel>();
            List<StockAllotmentDetailsModel> stkAltList = new List<StockAllotmentDetailsModel>();
            try
            {
                objStockAltList.AcceptStockItemDetails = stkAltList;
                objStockAltList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                objStockAltList.Status = BusinessCont.FailStatus;
                objStockAltList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "PendingAcceptStockAllot", null, objStockAltList.Status, ex.Message);
            }
            return objStockAltList;
        }

        /// <summary>
        /// Return stock allotment history. stock which is alloted by godown keeper to delivery boy.
        /// </summary>
        /// <param name="DistId"></param>
        /// <param name="GodownkeeperId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockAllotment/StockAllotmentHistory")]
        public StockAllotHistory StockAllotmentHistory([FromBody]commonPstMdl model)
        {
            StockAllotHistory stockAllotHistory = new StockAllotHistory();
            stockAllotHistory.StockAllotmentHistory = new List<StockAllotmentHistory>();
            List<StockAllotmentDetailsModel> stkAltList = new List<StockAllotmentDetailsModel>();
            DateTime? FromDate = null, ToDate = null;
            try
            {
                if (!string.IsNullOrEmpty(model.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(model.FromDate);
                if (!string.IsNullOrEmpty(model.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(model.ToDate);
                stockAllotHistory.StockAllotmentHistory = _unitOfWork.stockRepository.StockAllotHistory(model.DistId, model.StaffRefNo, model.Status, FromDate, ToDate);
                stockAllotHistory.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                stockAllotHistory.Status = BusinessCont.FailStatus;
                stockAllotHistory.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "StockAllotmentHistory", null, stockAllotHistory.Status, ex.Message);
            }
            return stockAllotHistory;
        }
        
    }
}
