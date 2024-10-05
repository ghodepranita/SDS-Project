using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Distributor;
using Aadyam.SDS.Business.Model.GeoCoordinates;
using Aadyam.SDS.Business.Model.Order;
using Aadyam.SDS.Business.Model.Stock;
using Aadyam.SDS.Business.Model.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("Order")]
    public class OrderController : BaseApiController
    {
        private decimal LogId = 0;

        #region Not In Use
        /// <summary>
        /// Order assign status is update if delivery boy accept to delivered order.
        /// </summary>
        /// <param name="OrderId">update is accepted status against order id</param>
        /// <param name="ExpectedDate">Expected delivery date</param>
        /// <returns>return success or fail string</returns>
        [HttpGet, Authorize]
        [Route("Order/AcceptOrder/{OrderId}/{ExpectedDate}")]
        public StatusDetails AcceptOrder(long OrderId, string ExpectedDate)
        {
            StatusDetails statusDetails = new StatusDetails();
            try
            {
                DateTime? ExptDate = null;
                if (!string.IsNullOrWhiteSpace(ExpectedDate))
                {
                    ExptDate = BusinessCont.StrConvertIntoDatetime(ExpectedDate);
                }
            }
            catch (Exception ex)
            {
                statusDetails.Status = BusinessCont.FailStatus;
                statusDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AcceptOrder", null, BusinessCont.FailStatus, ex.Message);
            }
            return statusDetails;
        }

        /// <summary>
        /// Method return open orders which is accepted and pending for accept by delivery boy
        /// </summary>
        /// <param name="DistId"></param>
        /// <param name="DeliveryBoyId">open orders against delivery boy id</param>
        /// <returns>return open orders</returns>
        [HttpGet, Authorize]
        [Route("Order/OrderDetails/{DistId}/{DeliveryBoyId}")]
        public OrderDetailList OrderDetails(int DistId, decimal DeliveryBoyId)
        {
            OrderDetailList objDeliveryBoyOrders = new OrderDetailList();
            try
            {
                objDeliveryBoyOrders.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                objDeliveryBoyOrders.Status = BusinessCont.FailStatus;
                objDeliveryBoyOrders.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OrderDetails", null, objDeliveryBoyOrders.Status, ex.Message);
            }
            return objDeliveryBoyOrders;
        }

        /// <summary>
        /// Method will be call when Delivery boy delivered filled cylinders and collect empty cylinders.
        /// </summary>
        /// <param name="DeliveryItems">JSON string contains OrderId, OrderItemId, Submitted cylinder quantity, Empty cylinder quantity</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        [Route("Order/OrderDelivered")]
        public StatusDetails OrderDelivered()
        {
            StatusDetails statusDetails = new StatusDetails();
            try
            {
                string DeliveryItems = HttpContext.Current.Request.Params.Get("DeliveryItems") != null ? HttpContext.Current.Request.Params.Get("DeliveryItems") : "";

                var OrderItems = JsonConvert.DeserializeObject<List<DeliveredOrder>>(DeliveryItems).FirstOrDefault();
                if (OrderItems != null)
                {
                    if (!string.IsNullOrEmpty(OrderItems.DeliveredDate))
                    {
                        DateTime DeliveredDt = BusinessCont.StrConvertIntoDatetime(OrderItems.DeliveredDate);
                    }
                }
            }
            catch (Exception ex)
            {
                statusDetails.Status = BusinessCont.FailStatus;
                statusDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OrderDelivered", null, statusDetails.Status, ex.Message);
            }
            return statusDetails;
        }

        /// <summary>
        /// Save consumer orders payment details.
        /// </summary>
        /// <param name="PaymentDetails">JSON string contains OrderId, Payment Amount,payment date, payment type.</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        [Route("Order/OrderPayment")]
        public StatusDetails OrderPayment()
        {
            StatusDetails statusDetails = new StatusDetails();
            try
            {
                string PaymentDetails = HttpContext.Current.Request.Params.Get("PaymentDetails") != null ? HttpContext.Current.Request.Params.Get("PaymentDetails") : "";

                if (!string.IsNullOrEmpty(PaymentDetails))
                {
                }
            }
            catch (Exception ex)
            {
                statusDetails.Status = BusinessCont.FailStatus;
                statusDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "OrderPayment", null, statusDetails.Status, ex.Message);
            }
            return statusDetails;
        }

        /// <summary>
        /// Save order details of instance sell cylinders
        /// </summary>
        /// <param name="OrderDtls">JSON string contains uniqueconsumer id, item id,Price,Payment amount,Payment type, delivered filled qty and empty qty etc </param>
        /// <returns></returns>
        [HttpPost, Authorize]
        [Route("Order/InstantSaleRegConsumer")]
        public StatusDetails InstantSaleRegConsumer()
        {
            string OrderDtls = HttpContext.Current.Request.Params.Get("OrderDtls") != null ? HttpContext.Current.Request.Params.Get("OrderDtls") : "";
            return null;
        }

        /// <summary>
        /// Save order details of un-registred consumer instant sale.
        /// </summary>
        /// <param name="OrderDtls">JSON string contains uniqueconsumer id, item id,Price,Payment amount,Payment type, delivered filled qty and empty qty etc</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        [Route("Order/InstantSaleUnRegConsumer")]
        public StatusDetails InstantSaleUnRegConsumer()
        {
            string OrderDtls = HttpContext.Current.Request.Params.Get("OrderDtls") != null ? HttpContext.Current.Request.Params.Get("OrderDtls") : "";
            return null;
        }

        /// <summary>
        /// return defective items - delivery boy collect defective items from consumer
        /// </summary>
        /// <param name="DefectiveRtnDetails">JSON string contains Defective items detail</param>
        /// <param name="ItemDetails">JSON string contains itemdetails</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        [Route("Order/DefectiveReturn")]
        public StatusDetails DefectiveReturn()
        {
            string DefectiveRtnDetails = HttpContext.Current.Request.Params.Get("DefectiveRtnDetails") != null ? HttpContext.Current.Request.Params.Get("DefectiveRtnDetails") : "";
            string ItemDetails = HttpContext.Current.Request.Params.Get("ItemDetails") != null ? HttpContext.Current.Request.Params.Get("ItemDetails") : "";
            return null;
        }

        /// <summary>
        /// Search registred consumer for instance sell. return consumer details and consumer cylinders details
        /// </summary>
        /// <param name="SearchDtls">search consumer name and consumer no against searchdtls string</param>
        /// <returns></returns>
        [HttpGet, Authorize]
        [Route("Order/ConsumerDetails/{DistId}/{SearchDtls}")]
        public ConsDtls ConsumerDetails(int DistId, string SearchDtls)
        {
            ConsDtls consDtls = new ConsDtls();
            try
            {
                List<ConsumerCylindersDetails> ConsCylDtls = new List<ConsumerCylindersDetails>();
                if (!string.IsNullOrEmpty(SearchDtls))
                {

                }
            }
            catch (Exception ex)
            {
                consDtls.Status = BusinessCont.FailStatus;
                consDtls.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "ConsumerDetails", null, consDtls.Status, ex.Message);
            }
            return consDtls;
        }
        #endregion

        #region Get Pending Orders Cluster wise
        [HttpGet]
        [Route("GetPendingOrdersClusterwise/{DistributorId}/{CLusterId}")]
        public IHttpActionResult GetPendingOrdersClusterwise(long DistributorId, int CLusterId)
        {
            return Ok(_unitOfWork.orderRepository.GetPendingOrdersClusterwise(DistributorId, CLusterId));
        }
        #endregion

        #region Get Pending Orders Details By Distributor Cluster
        [HttpGet]
        [Route("GetPendingOrdersDetailsByDistributorCluster/{DistributorId}/{CLusterId}")]
        public IHttpActionResult GetPendingOrdersDetailsByDistributorCluster(long DistributorId, int CLusterId)
        {
            return Ok(_unitOfWork.orderRepository.GetPendingOrdersDetailsByDistributorCluster(DistributorId, CLusterId));
        }
        #endregion

        #region Pending order distributor & deliveryboy wise

        /// <summary>
        /// Santosh - Get Pending Orders Deliveryboy wise
        /// </summary>
        /// <param name="DistributorId"></param>
        /// <param name="StaffRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPendingOrdersDeliveryBoywise/{DistributorId}/{StaffRefNo}")]
        public IHttpActionResult PendingBookingsDistributorDeliveryBoywise(int DistributorId, decimal StaffRefNo)
        {
            return Ok(_unitOfWork.orderRepository.PendingBookingsDistributorDeliveryBoywise(DistributorId, StaffRefNo));
        }
        #endregion

        #region Get Distributor wise Preferred Booking 

        [HttpPost]
        [Route("GetPreferredBookingList")]
        public IHttpActionResult GetDistributorwisePreferredBookingList([FromBody]PostModel postModel)
        {
            if (postModel == null && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            PendingOrderDetails _pendingOrderDetails = new PendingOrderDetails();
            _pendingOrderDetails.preferredBookingsList = new List<PreferredBooking>();
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);

                DateTime? BookingDate = null;
                if (!string.IsNullOrEmpty(postModel.BookingDate))
                    BookingDate = BusinessCont.StrConvertIntoDatetime(postModel.BookingDate);

                _pendingOrderDetails.preferredBookingsList = _unitOfWork.orderRepository.GetDistributorwisePreferredBookingList(postModel.DistributorId, postModel.StaffRefNo, BookingDate);
                _pendingOrderDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _pendingOrderDetails.Status = BusinessCont.FailStatus;
                _pendingOrderDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPreferredBookingList", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_pendingOrderDetails);
        }
        #endregion

        #region Get Pending Order 
        [HttpPost]
        [Route("GetPendingOrderList")]
        public IHttpActionResult GetPendingOrderList([FromBody]PostModel postModel)
        {
            if (postModel == null && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            PendingOrderDetails _pendingOrderDetails = new PendingOrderDetails();
            _pendingOrderDetails.tripDetails = new List<DBWisetripDetails>();
            _pendingOrderDetails.pendingOrders = new List<PendingOrder>();
            _pendingOrderDetails.LocationDetails = new List<Location>();
            List<Location> LocationDetails = new List<Location>();
            DateTime? FromDate = null, ToDate = null, TripDate = null;
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                if (!string.IsNullOrEmpty(postModel.FromDate))
                    FromDate = BusinessCont.StrConvertIntoDatetime(postModel.FromDate);
                if (!string.IsNullOrEmpty(postModel.ToDate))
                    ToDate = BusinessCont.StrConvertIntoDatetime(postModel.ToDate);
                if (!string.IsNullOrEmpty(postModel.TripDate))
                    TripDate = BusinessCont.StrConvertIntoDatetime(postModel.TripDate);

                _pendingOrderDetails.tripDetails = _unitOfWork.orderRepository.GetTripDetails(postModel.DistributorId, postModel.StaffRefNo, TripDate, FromDate, ToDate);
                if(_pendingOrderDetails.tripDetails.Count > 0 && _pendingOrderDetails.tripDetails[0].isPODHD == "POD")
                {
                    _pendingOrderDetails.pendingOrders = _unitOfWork.orderRepository.GetPendingOrdersListPOD(postModel.DistributorId, postModel.StaffRefNo, TripDate);
                    _pendingOrderDetails.LocationDetails = _unitOfWork.orderRepository.GetLocationDetailsPOD(postModel.DistributorId, postModel.StaffRefNo, TripDate);
                }
                else
                {
                    _pendingOrderDetails.pendingOrders = _unitOfWork.orderRepository.GetPendingOrderList(postModel.DistributorId, postModel.StaffRefNo, TripDate, FromDate, ToDate);
                    _pendingOrderDetails.LocationDetails = LocationDetails;
                }

                var AppConfig = BusinessCont.GetAppConfiguration();
                if (AppConfig != null)
                {
                    _pendingOrderDetails.DisplayTripBooking = AppConfig.Where(a => a.Key == BusinessCont.DisplayTripBooking).Select(a => a.Value).FirstOrDefault();
                }
                _pendingOrderDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _pendingOrderDetails.Status = BusinessCont.FailStatus;
                _pendingOrderDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPendingOrderList", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_pendingOrderDetails);
        }

        [HttpPost]
        [Route("GetPendingOrdersListAddDelBoy")]
        public PendingDelBoyDetails GetPendingOrdersListAddDelBoy([FromBody] DelBoyModel model)
        {
            PendingDelBoyDetails delList = new PendingDelBoyDetails();
            delList.tripDetails = new List<DBWisetripDetails>();
            delList.PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            delList.OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<DBWisetripDetails> tripDetails = new List<DBWisetripDetails>();
            try
            {
                tripDetails = _unitOfWork.orderRepository.GetTripDetails(model.DistributorId, model.StaffRefNo, model.TripDate,Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                PendingDelBoyDetailsList = _unitOfWork.orderRepository.GetPendingOrdersListAddDelBoy(model);

                delList.tripDetails = tripDetails;
                delList.PendingDelBoyDetailsList = PendingDelBoyDetailsList;
                delList.AssignOrderAddDelBoyDetailsList = AssignOrderAddDelBoyDetailsList;
                delList.DeliveredOrderAddDelBoyList = DeliveredOrderAddDelBoyList;
                delList.OrderStatusAddDelBoyList = OrderStatusAddDelBoyList;
                delList.PlayStoreVersion = _unitOfWork.loginRepository.GetCurrentVersion("SDS");
                delList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                delList.Status = BusinessCont.FailStatus;
                delList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPendingOrdersListAddDelBoy", null, BusinessCont.FailStatus, ex.Message);
                //throw;
            }
            return delList;
        }

        [HttpPost]
        [Route("GetAssignOrdersListAddDelBoy")]
        public PendingDelBoyDetails GetAssignOrdersListAddDelBoy([FromBody] DelBoyModel model)
        {
            PendingDelBoyDetails delList = new PendingDelBoyDetails();
            delList.tripDetails = new List<DBWisetripDetails>();
            delList.PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            delList.OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<DBWisetripDetails> tripDetails = new List<DBWisetripDetails>();
            try
            {
                AssignOrderAddDelBoyDetailsList = _unitOfWork.orderRepository.GetAssignOrdersListAddDelBoy(model);
                delList.tripDetails = tripDetails;
                delList.PendingDelBoyDetailsList = PendingDelBoyDetailsList;
                delList.AssignOrderAddDelBoyDetailsList = AssignOrderAddDelBoyDetailsList;
                delList.DeliveredOrderAddDelBoyList = DeliveredOrderAddDelBoyList;
                delList.OrderStatusAddDelBoyList = OrderStatusAddDelBoyList;
                delList.MobileLogs = _unitOfWork.loginRepository.GetMobileLogs();
                delList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                delList.Status = BusinessCont.FailStatus;
                delList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAssignOrdersListAddDelBoy", null, BusinessCont.FailStatus, ex.Message);
            }
            return delList;
        }

        [HttpPost]
        [Route("GetDeliveredOrderForAddiDelBoy")]
        public PendingDelBoyDetails GetDeliveredOrderForAddiDelBoy([FromBody] DelBoyModel model)
        {
            PendingDelBoyDetails delList = new PendingDelBoyDetails();
            delList.tripDetails = new List<DBWisetripDetails>();
            delList.PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            delList.OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<DBWisetripDetails> tripDetails = new List<DBWisetripDetails>();
            try
            {
                DeliveredOrderAddDelBoyList = _unitOfWork.orderRepository.GetDeliveredOrderForAddiDelBoy(model);
                delList.tripDetails = tripDetails;
                delList.PendingDelBoyDetailsList = PendingDelBoyDetailsList;
                delList.AssignOrderAddDelBoyDetailsList = AssignOrderAddDelBoyDetailsList;
                delList.DeliveredOrderAddDelBoyList = DeliveredOrderAddDelBoyList;
                delList.OrderStatusAddDelBoyList = OrderStatusAddDelBoyList;
                delList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                delList.Status = BusinessCont.FailStatus;
                delList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDeliveredOrderForAddiDelBoy", null, BusinessCont.FailStatus, ex.Message);
            }
            return delList;
        }

        [HttpPost]
        [Route("GetOrderStatusForAddiDelBoy")]
        public PendingDelBoyDetails GetOrderStatusForAddiDelBoy([FromBody] DelBoyModel model)
        {
            PendingDelBoyDetails delList = new PendingDelBoyDetails();
            delList.tripDetails = new List<DBWisetripDetails>();
            delList.PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            delList.DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            delList.OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> PendingDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> AssignOrderAddDelBoyDetailsList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> DeliveredOrderAddDelBoyList = new List<PendingDelBoyModel>();
            List<PendingDelBoyModel> OrderStatusAddDelBoyList = new List<PendingDelBoyModel>();
            List<DBWisetripDetails> tripDetails = new List<DBWisetripDetails>();
            try
            {
                OrderStatusAddDelBoyList = _unitOfWork.orderRepository.GetOrderStatusForAddiDelBoy(model);
                delList.tripDetails = tripDetails;
                delList.PendingDelBoyDetailsList = PendingDelBoyDetailsList;
                delList.AssignOrderAddDelBoyDetailsList = AssignOrderAddDelBoyDetailsList;
                delList.DeliveredOrderAddDelBoyList = DeliveredOrderAddDelBoyList;
                delList.OrderStatusAddDelBoyList = OrderStatusAddDelBoyList;
                delList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                delList.Status = BusinessCont.FailStatus;
                delList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOrderStatusForAddiDelBoy", null, BusinessCont.FailStatus, ex.Message);
            }
            return delList;
        }
        #endregion

        #region Assign Order To Helper
        [HttpPost]
        [Route("AssignDeliveryOrderToHelper")]
        public IHttpActionResult AssignDeliveryOrderToHelper([FromBody]UpdateTripDetails postModel)
        {
            if (postModel == null && postModel.Id == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            UpdateTripDetails model = new UpdateTripDetails();   
            try
            {
                model.Id = _unitOfWork.orderRepository.AssignDeliveryOrderToHelperDtls(postModel);
                model.UniqueId = postModel.UniqueId;
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AssignDeliveryOrderToHelper", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Update Trip Details
        [HttpPost]
        [Route("UpdateTripDetails")]
        public IHttpActionResult UpdateTripDetails([FromBody]UpdateTripDetails postModel)
        {
            if (postModel == null && postModel.Id == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            UpdateTripDetails model = new UpdateTripDetails();
            string jsonString = string.Empty;
            try
            {
                jsonString = JsonConvert.SerializeObject(postModel);
                model.Id = _unitOfWork.orderRepository.UpdateTripPlanningDtls(postModel);
                model.UniqueId = postModel.UniqueId;
                model.Status = BusinessCont.SuccessStatus;

                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateTripDetails", "jsonString: " + jsonString + " & Id: " + model.Id, "", "");
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateTripDetails", "jsonString: " + jsonString, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region order History
        [HttpPost]
        [Route("GetOrderHistory")]
        public IHttpActionResult GetOrderHistory([FromBody]PostModel postModel)
        {
            return Ok(_unitOfWork.orderRepository.GetOrderHistory(postModel));

        }
        #endregion

        #region TripWise Order Report
        [HttpPost]
        [Route("GetTripWiseOrderReport")]
        public IHttpActionResult GetTripWiseOrderReport([FromBody]PostModel postModel)
        {
            if (postModel == null && postModel.DistributorId == 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            TripWiseOrderModel tripWiseOrderModel = new TripWiseOrderModel();
            tripWiseOrderModel.TripWiseOrders = new List<OrderDetailsModel>();
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                tripWiseOrderModel.TripWiseOrders = _unitOfWork.orderRepository.GetTripWiseOrders(postModel);
                tripWiseOrderModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                tripWiseOrderModel.Status = BusinessCont.FailStatus;
                tripWiseOrderModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTripWiseOrderReport", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(tripWiseOrderModel);
        }
        #endregion

        #region Distributor Wise Pending Orders
        [HttpGet]
        [Route("GetPendingOrdersDistributorwise/{SACode}/{Flag}")]
        public IHttpActionResult GetPendingOrdersDistributorwise(string SACode, string Flag)
        {
            return Ok(_unitOfWork.orderRepository.GetPendingOrdersDistributorwise(SACode, Flag));
        }
        #endregion

        #region OrderReport
        [HttpPost]
        [Route("GetDistributorBacklogHistory")]
        public IHttpActionResult GetDistributorBacklogHistory([FromBody] DateModel model)
        {
            return Ok(_unitOfWork.orderRepository.GetDistributorBacklogHistory(model));
        }
        #endregion

        #region GoogleApiHistoryReport
        [HttpPost]
        [Route("GetDistributorGoogleAPIHistory")]
        public IHttpActionResult GetDistributorGoogleAPIHistory([FromBody] APIHitsDateModel model)
        {
            return Ok(_unitOfWork.orderRepository.GetDistributorGoogleAPIHistory(model));
        }
        #endregion

        #region MISGoogleApiReport
        [HttpGet]
        [Route("GetGoogleAPIHitSummary/{SACode}")]
        public IHttpActionResult GetGoogleAPIHitSummary(string SACode)
        {
            return Ok(_unitOfWork.orderRepository.GetGoogleAPIHitSummary(SACode));
        }

        [HttpPost]
        [Route("GetGoogleAPIHitDetails")]
        public IHttpActionResult GetGoogleAPIHitDetails([FromBody] DateModel model)
        {
            return Ok(_unitOfWork.orderRepository.GetGoogleAPIHitDetails(model));
        }
        #endregion

        #region Get Cluster wise Pending Booking List
        [HttpPost]
        [Route("GetClusterwisePendingBookingList")]
        public List<PendingBookingConsumers> GetClusterwisePendingBookingList([FromBody]ClusterwisePendingBookingCounts postModel)
        {
            List<PendingBookingConsumers> List = new List<PendingBookingConsumers>();
            try
            {
                List = _unitOfWork.orderRepository.GetClusterwisePendingBookingList(postModel.DistributorId, postModel.ClusterId); ;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterwisePendingBookingList", "Order/GetClusterwisePendingBookingList = DistributorId : " + postModel.DistributorId + "  & ClusterId = " + postModel.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return List;
        }
        #endregion

        #region Get Cluster wise Pending Booking Counts
        [HttpPost]
        [Route("GetClusterwisePendingBookingCounts")]
        public List<ClusterwisePendingBookingCounts> GetClusterwisePendingBookingCounts([FromBody]ClusterwisePendingBookingCounts postModel)
        {
            List<ClusterwisePendingBookingCounts> BookingCounts = new List<ClusterwisePendingBookingCounts>();
            try
            {
                BookingCounts = _unitOfWork.orderRepository.GetClusterwisePendingBookingCounts(postModel.DistributorId, postModel.ClusterId); ;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterwisePendingBookingCounts", null, BusinessCont.FailStatus, ex.Message);
            }
            return BookingCounts;
        }
        #endregion

        #region Get Yesterdays Delivered Orders
        [HttpPost]
        [Route("GetYesterdaysDeliveredOrders")]
        public List<YesterdaysDeliveredList> GetYesterdaysDeliveredOrders([FromBody]OrderDetailsModel postModel)
        {
            List<YesterdaysDeliveredList> CountLst = new List<YesterdaysDeliveredList>();
            try
            {
                CountLst = _unitOfWork.orderRepository.GetYesterdaysDeliveredList(postModel.DistributorId, postModel.StaffRefNo, postModel.DeliveryDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetYesterdaysDeliveredList", null, BusinessCont.FailStatus, ex.Message);
            }
            return CountLst;
        }
        #endregion

        #region Get Yesterdays Delivered Count 
        [HttpPost]
        [Route("GetYesterdaysDeliveredCount")]
        public List<DelBoyWiseLastDeliveryBookingCounts> GetYesterdaysDeliveredCount([FromBody]OrderDetailsModel postModel)
        {
            List<DelBoyWiseLastDeliveryBookingCounts> Yesterdaycount = new List<DelBoyWiseLastDeliveryBookingCounts>();
            try
            {
                Yesterdaycount = _unitOfWork.orderRepository.GetYesterdaysDeliveredCounts(postModel.DistributorId, postModel.StaffRefNo, postModel.DeliveryDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, postModel.DistributorId, 0, 0, "GetYesterdaysDeliveredCount", "StaffRefNo= " + postModel.StaffRefNo + ", DeliveryDate= " + postModel.DeliveryDate, BusinessCont.FailStatus, ex.Message);
            }
            return Yesterdaycount;
        }
        #endregion

        #region Get Pending Cons With Trip Dtls 
        [HttpPost]
        [Route("GetPendingConsWithTripDtls")]
        public List<PendingOrder> GetPendingConsWithTripDtls([FromBody]PostModel postModel)
        {
            List<PendingOrder> BookingList = new List<PendingOrder>();
            try
            {
                BookingList = _unitOfWork.orderRepository.GetPendingConsWithTripDtls(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, postModel.DistributorId, 0, 0, 0, "GetPendingConsWithTripDtls", null, BusinessCont.FailStatus, ex.Message);
            }
            return BookingList;
        }
        #endregion

        #region Get Cluster Wise Yest DLVD List
        [HttpPost]
        [Route("GetClusterwiseYestDLVDList")]
        public List<YestDLVDListModel> GetClusterwiseYestDLVDList([FromBody]ClusterwisePendingBookingCounts model)
        {
            List<YestDLVDListModel> dlvdList = new List<YestDLVDListModel>();
            try
            {
                dlvdList = _unitOfWork.orderRepository.GetClusterwiseYestDLVDList(model.DistributorId, model.ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterwiseYestDLVDList", "Order/GetClusterwiseYestDLVDList = DistributorId : " + model.DistributorId + " & ClusterId : " + model.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return dlvdList;
        }
        #endregion

        #region Get VitranGC Verify Counts
        [HttpPost]
        [Route("GetVitranGCVerifyCounts")]
        public VitranGCVerifyCounts GetVitranGCVerifyCounts([FromBody]VitranGCVerifyCounts postModel)
        {
            VitranGCVerifyCounts count = new VitranGCVerifyCounts();
            try
            {
                count = _unitOfWork.orderRepository.GetVitranGCVerifyCounts(postModel.DistributorId, postModel.ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetVitranGCVerifyCounts", "GetVitranGCVerifyCounts Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return count;
        }
        #endregion

        #region Get Cluster Master List
        [HttpPost]
        [Route("GetClusteMasterList")]
        public ClustermasterModel GetClusterMasterList(ClustermasterModel model)
        {
            ClustermasterModel clList = new ClustermasterModel();
            clList.ClusterMasterList = new List<ClusterMasterList>();
            try
            {
                clList.ClusterMasterList = _unitOfWork.orderRepository.GetClusterMasterList(model.DistributorId, model.ClusterId, model.IsActive);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "GetClusteMasterList", "GetClusteMasterList Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return clList;
        }
        #endregion
       
        #region GetArea_ClusterMappingDetails --Use this API to Get Cluster List
        [HttpPost]
        [Route("GetArea_ClusterMappingDetailsNew")]
        public List<AreaClusterLst> GetArea_ClusterMappingDetails([FromBody]AreaClusterModel _areaClusterModel)
        {
            List<AreaClusterLst> arealist = new List<AreaClusterLst>();
            try
            {
                arealist = _unitOfWork.orderRepository.GetArea_ClusterMappingList(_areaClusterModel.DistributorId, _areaClusterModel.ClusterId, _areaClusterModel.IsActive);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetArea_ClusterMappingDetailsNew", "DistributorID= " + _areaClusterModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return arealist;
        }
        #endregion

        #region Get Consumer Order Status For Mob
        [HttpPost]
        [Route("GetConsumerOrderStatus")]
        [AllowAnonymous]
        public OrderStatusModel GetConsumerOrderStatus(OrderStatusLst model)
        {
            OrderStatusModel OrderStatus = new OrderStatusModel();
            try
            {
                OrderStatus = _unitOfWork.orderRepository.GetConsumerOrderStatus(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerOrderStatus", "UniqueConsumerId= " + model.UniqueConsumerId + "-" + model.OrderRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return OrderStatus;
        }
        #endregion

        #region Get Order Status 
        [HttpPost]
        [Route("GetOrderStatus")]
        [AllowAnonymous] //Please do not remove this AllowAnonymous because this is public API For HPPAY
        public GetOrderStatus GetOrderStatus(OrderStatusInputModel model)
        {
            GetOrderStatus OrderStatus = new GetOrderStatus();
            try
            {
                OrderStatus = _unitOfWork.orderRepository.GetOrderStatus(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOrderStatus", "", BusinessCont.FailStatus, ex.Message);
            }
            return OrderStatus;
        }
        #endregion

        #region Get Consumer Live Order Status For Mob
        [HttpPost]
        [Route("GetConsumerLiveOrderStatus")]
        [AllowAnonymous]
        public IHttpActionResult GetConsumerLiveOrderStatus(JObject JSONData)
        {
            List<OrderStatusLst> modellist = new List<OrderStatusLst>();
            OrderStatusLst model;
            try
            {
                dynamic results = JsonConvert.DeserializeObject<dynamic>(JSONData.ToString());
                if (results.orderLists.Count > 0)
                {
                    for (int i = 0; i < results.orderLists.Count; i++)
                    {
                        model = new OrderStatusLst();
                        model = _unitOfWork.orderRepository.GetConsumerLiveOrderStatus(Convert.ToString(results.orderLists[i].OrderRefNo), Convert.ToString(results.orderLists[i].UniqueConsumerId), Convert.ToInt32(results.orderLists[i].DistributorId));
                        if (model != null)
                        {
                            modellist.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerLiveOrderStatus", "", BusinessCont.FailStatus, ex.Message);
            }
            return Ok(modellist);
        }
        #endregion

        #region Get Trip Pending Orders By TripId
        [HttpPost]
        [Route("GetTripPendingOrdersByTripId")]
        public List<PendingOrdersByTripId> GetPendingOrdersDetailsForCluster([FromBody] PendingOrdersByTripId Model)
        {
            List<PendingOrdersByTripId> BookingCounts = new List<PendingOrdersByTripId>();
            try
            {
                BookingCounts = _unitOfWork.orderRepository.GetTripPendingOrdersByTripIdRepository(Model.DistributorId, Model.ClusterId, Model.TripId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, Model.DistributorId, 0, 0, 0, "GetTripPendingOrdersByTripIdRepository", "", BusinessCont.FailStatus, ex.Message);
            }
            return BookingCounts;

        }
        #endregion 

        #region Get Clusterwise Trip Status For Mob
        [HttpPost]
        [Route("GetClusterwiseTripStatus")]
        [AllowAnonymous]
        public TripStatusModel GetClusterwiseTripStatus(TripStatusClusterwiseLst model)
        {
            TripStatusModel OrderStatus = new TripStatusModel();
            try
            {
                OrderStatus = _unitOfWork.orderRepository.GetClusterwiseTripStatus(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, Convert.ToInt32(model.DistributorId), 0, 0, 0, "GetClusterwiseTripStatus", "ClusterId" + Convert.ToInt32(model.ClusterId), BusinessCont.FailStatus, ex.Message);
            }
            return OrderStatus;
        }
        #endregion

        #region Get SA Wise Consumer List For Mob
        [HttpPost]
        [Route("GetSAWiseConsumerList")]
        [AllowAnonymous]
        public SAWiseConsumerModel GetSAWiseConsumerList(SAWiseConsumerLst model)
        {
            SAWiseConsumerModel SawiseConsumerModel = new SAWiseConsumerModel();
            try
            {
                SawiseConsumerModel = _unitOfWork.orderRepository.GetSAWiseConsumerList(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetSAWiseConsumerList", "DistributorId= " + model.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return SawiseConsumerModel;
        }
        #endregion

        #region Get Pending Booking for Current Data For Mob
        [HttpPost]
        [Route("GetPendingBookingforCurrentData_Mob")]
        public List<PendingBkgCurrentData> GetPendingBookingforCurrentData_Mob([FromBody] PendingBkgCurrentData postModel)
        {
            List<PendingBkgCurrentData> CurrentBkgList = new List<PendingBkgCurrentData>();
            try
            {
                CurrentBkgList = _unitOfWork.orderRepository.GetPendingBookingforCurrentData_Mob(Convert.ToInt32(postModel.DistributorID),Convert.ToDecimal(postModel.StaffRefNo));
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, Convert.ToInt32(postModel.DistributorID), 0, 0, 0, "GetPendingBookingforCurrentData_Mob", null, BusinessCont.FailStatus, ex.Message);
            }
            return CurrentBkgList;
        }
        #endregion

        #region Get Parameter wise Booking Summary
        [HttpGet]
        [Route("GetParameterwiseBookingSummary")]
        [AllowAnonymous]
        public List<ParameterwiseModel> GetParameterwiseBookingSummary()
        {
            List<ParameterwiseModel> ParameterwiseModel = new List<ParameterwiseModel>();
            try
            {
                ParameterwiseModel = _unitOfWork.orderRepository.GetParameterwiseBookingSummary();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetParameterwiseBookingSummary", "", BusinessCont.FailStatus, ex.Message);
            }
            return ParameterwiseModel;
        }
        #endregion

        #region Update Vitran DLVD Details For Mob
        [HttpPost]
        [Route("UpdateVitranDLVDDetails")]
        [AllowAnonymous]
        public IHttpActionResult UpdateVitranDLVDDetails([FromBody]UpdateTripDetails postModel)
        {
            UpdateTripDetails model = new UpdateTripDetails();
            try
            {
                string jsonString = JsonConvert.SerializeObject(postModel);
                model.Id = _unitOfWork.orderRepository.UpdateVitranDLVDDetails(postModel);
                model.UniqueId = postModel.UniqueId;
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                model.Status = BusinessCont.FailStatus;
                model.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, postModel.DistributorId, 0, 0, "UpdateVitranDLVDDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Save Parameter wise
        [HttpPost]
        [Route("SaveParameterwise")]
        public int SaveParameterwise([FromBody]SaveParawiseModel model)
        {
            int result = 0;
            try
            {
                result = _unitOfWork.orderRepository.SaveParameterwise(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveParameterwise", " " + "", BusinessCont.FailStatus, ex.Message);
            }
            return result;
        }
        #endregion

        #region Get Cluster wise Delivered Count
        [HttpPost]
        [Route("GetClusterwiseDeliveredCount")]
        public List<ClusterwiseBookingCounts> GetClusterwiseYesterdayDeliveredCount([FromBody]OrderDetailsModel postModel)
        {
            List<ClusterwiseBookingCounts> Yesterdaycount = new List<ClusterwiseBookingCounts>();
            try
            {
                Yesterdaycount = _unitOfWork.orderRepository.GetClusterwiseYesterdayDeliveredCount(postModel.DistributorId, postModel.ClusterId, postModel.DeliveryDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, postModel.DistributorId, 0, 0, "GetClusterwiseYesterdayDeliveredCount", "StaffRefNo= " + postModel.StaffRefNo + ", DeliveryDate= " + postModel.DeliveryDate, BusinessCont.FailStatus, ex.Message);
            }
            return Yesterdaycount;
        }
        #endregion

        #region Get Clust Wise Yesterdays Delivered List
        [HttpPost]
        [Route("GetClustWiseYesterdaysDeliveredList")]
        public List<ClustWiseYesterdaysDLVDLst> GetClustWiseYesterdaysDeliveredList([FromBody] ClustWiseYesterdaysDLVDLst postModel)
        {
            List<ClustWiseYesterdaysDLVDLst> CountLst = new List<ClustWiseYesterdaysDLVDLst>();
            try
            {
                CountLst = _unitOfWork.orderRepository.GetClutserWiseYesterdaysDeliveredList(Convert.ToInt32(postModel.DistributorId), Convert.ToInt32(postModel.ClusterId), postModel.DeliveryDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClustWiseYesterdaysDeliveredList", null, BusinessCont.FailStatus, ex.Message);
            }
            return CountLst;
        }
        #endregion

    }
}
