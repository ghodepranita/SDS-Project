using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Stock;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class StockTransferController : BaseApiController
    {
        decimal LogId = 0;

        #region Godown Keeper 
        /// <summary>
        /// Return stock transfer list for accept by godown keeper
        /// </summary>
        /// <param name="DistId">Display data against distributor id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockTransfer/PendingAcceptStockLst")]
        public PendingStockAcceptList PendingAcceptStock([FromBody]STpostMdl model)
        {
            PendingStockAcceptList objPendingStockAcceptList = new PendingStockAcceptList();
            objPendingStockAcceptList.AccpetStockTransDetail = new List<PendingStkAccept>();
            List<StockTransferDetailsModel> StocktransList = new List<StockTransferDetailsModel>();
            DateTime? FromDate = null, ToDate = null;
            try
            {
                if (!string.IsNullOrEmpty(model.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(model.FromDate);
                if (!string.IsNullOrEmpty(model.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(model.ToDate);

                objPendingStockAcceptList.AccpetStockTransDetail = _unitOfWork.stockRepository.StockTransferDetails(model.DistId, model.GodownkeeperId, FromDate,ToDate);
                objPendingStockAcceptList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                objPendingStockAcceptList.Status = BusinessCont.FailStatus;
                objPendingStockAcceptList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "PendingAcceptStockLst", null, objPendingStockAcceptList.Status, ex.Message);
            }
            return objPendingStockAcceptList;
        }

        /// <summary>
        /// Accept return stock from delivery boy at the end of day.
        /// </summary>
        /// <param name="StockAllotmentId"></param>
        /// <param name="DistId"></param>
        /// <param name="GodownKeeperId">Accept by godown keeper</param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockTransfer/GodownKeeperAcceptStock/")]
        public AcceptStk GodownKeeperAcceptStock([FromBody]STpostMdl model)
        {
            AcceptStk acceptStk = new AcceptStk();
            acceptStk.Status = BusinessCont.FailStatus;
            try
            {
                bool AcceptedStock = _unitOfWork.stockRepository.AcceptStockTransfer(model.StockAllotmentId, model.DistId, model.GodownKeeperId, model.Status, model.Remark);
                if (AcceptedStock == true)
                {
                        acceptStk.Status = BusinessCont.SuccessStatus;
                        acceptStk.AcceptStockId = model.AcceptStockId;
                }
            }
            catch (Exception ex)
            {
                acceptStk.Status = BusinessCont.FailStatus;
                acceptStk.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GodownKeeperAcceptStock", null, acceptStk.Status, ex.Message);
            }
            return acceptStk;
        }
        #endregion

        #region Adding Defective Item Details
        [HttpPost]
        [Route("StockTransfer/AddDefectiveItemDetails/")]
        public DefectiveItemModel AddDefectiveItemDetails([FromBody]DefectiveItemModel _defective)
        {
            DefectiveItemModel ReturnData = new DefectiveItemModel();
            try
            {
                ReturnData = _unitOfWork.stockRepository.AddDefectiveItemDetails(_defective);
            }
            catch (Exception ex)
            {
                ReturnData.Status = BusinessCont.FailStatus;
                ReturnData.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddDefectiveItemDetails", null, ReturnData.Status, ex.Message);
            }
            return ReturnData;
        }

        [HttpPost]
        [Route("StockTransfer/GetDefectiveItemDetails/")]
        public DefectiveItemListModel GetDefectiveItemDetails([FromBody]DefectiveItemPostModel _defectiveItem)
        {
            DefectiveItemModel postModel = new DefectiveItemModel();
            DefectiveItemListModel _defectiveItemListModel = new DefectiveItemListModel();
            _defectiveItemListModel.DefectiveItemList = new List<DefectiveItemModel>();
            try
            {
                if(_defectiveItem != null &&_defectiveItem.DistributorId >0)
                {
                    postModel.DistributorId = _defectiveItem.DistributorId;
                    if(!string.IsNullOrEmpty(_defectiveItem.FromDate) && !string.IsNullOrEmpty(_defectiveItem.ToDate))
                    {
                        postModel.FromDate = BusinessCont.StrConvertIntoDatetime(_defectiveItem.FromDate);
                        postModel.ToDate = BusinessCont.StrConvertIntoDatetime(_defectiveItem.ToDate);
                    }
                    _defectiveItemListModel.DefectiveItemList = _unitOfWork.stockRepository.GetDefectiveItemDetails(postModel);
                    _defectiveItemListModel.Status = BusinessCont.SuccessStatus;
                }               
            }
            catch (Exception ex)
            {
                _defectiveItemListModel.Status = BusinessCont.FailStatus;
                _defectiveItemListModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDefectiveItemDetails", null, _defectiveItemListModel.Status, ex.Message);
            }
            return _defectiveItemListModel;
        }
        #endregion

        #region Delivery Boy
        /// <summary>
        /// Delivery boy submit stock ie empty and filled cylinder stock to godown keeper.
        /// </summary>
        /// <param name="StockTransDetails">JSON string contains details of stock transfer</param>
        /// <param name="AllotItems"> JSON string contains cylinder filled and empty quantity and itemId</param>
        /// <returns>return success or fail string</returns>
        [HttpPost]
        [Route("StockTransfer/AddStockTrans")]
        public StatusDetails AddStockTrans([FromBody]PstAddStkTran  model)
        {
            StatusDetails statusDetails = new StatusDetails();
            statusDetails = _unitOfWork.stockRepository.AddStockTrans(model.StockTransDetails, model.TransItems);
            return statusDetails;
        }

        /// <summary>
        /// Return history of stock transfer which is transfer to godown keeper from delivery boy at the end of day. 
        /// </summary>
        /// <param name="DistId"></param>
        /// <param name="DeliveryboyId">Against delivery boy</param>
        /// <returns></returns>
        [HttpPost]
        [Route("StockTransfer/StockTransHistory/")]
        public StockTrans StockTransHistory([FromBody]PstStkhist model)
        {
            StockTrans stockTrans = new StockTrans();
            stockTrans.StockTransDetails = new List<StockTransferMasterModel>();
            DateTime? FromDate = null, ToDate = null;
            try
            {
                if (!string.IsNullOrEmpty(model.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(model.FromDate);
                if (!string.IsNullOrEmpty(model.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(model.ToDate);
                stockTrans.StockTransDetails = _unitOfWork.stockRepository.DisplayAllTransferStockOfDeliveryBoy(model.DistId, model.DeliveryboyId, FromDate, ToDate);
                stockTrans.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                stockTrans.Status = BusinessCont.FailStatus;
                stockTrans.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId,0, model.DistId, 0, model.DeliveryboyId, "Stock Transfer History", "DistId" + model.DistId + " ,DeliveryboyId" + model.DeliveryboyId, stockTrans.Status, ex.Message);
            }
            return stockTrans;
        }
        #endregion

    }
}
