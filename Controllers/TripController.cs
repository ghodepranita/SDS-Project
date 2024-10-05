using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Aadyam.SDS.Business.Model.Vehicle;
using Aadyam.SDS.API.Filters;
using Aadyam.SDS.Business.Model.GeoCoordinates;
using Aadyam.SDS.Business.Model.Login;
using Aadyam.SDS.Business.Model.Trip;
using System.Configuration;

namespace Aadyam.SDS.API.Controllers
{
    public class TripController : BaseApiController
    {
        decimal LogId = 0;

        #region Add Trip Expected Stock
        [HttpPost]
        [Route("Trip/AddTripExpectedStock")]
        [CustomExceptionFilter]
        public int AddTripExpectedStock([FromBody] List<Trip_ExpectedStockModel> _trip_ExpectedStock)
        {
            int ReturnId = 0;
            try
            {
                if (_trip_ExpectedStock.Any())
                {
                    ReturnId = _unitOfWork.stockRepository.AddTripExpectedStock(_trip_ExpectedStock);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "AddTripExpectedStock", null, BusinessCont.FailStatus, ex.Message);
            }
            return ReturnId;
        }
        #endregion

        #region Get Trip Expected Stock
        [HttpGet]
        [Route("Trip/GetTripExpectedStock/{DistributorId}/{SelDate}")]
        public List<Trip_ExpectedStockModel> GetTripExpectedStock(int DistributorId, string SelDate)
        {
            List<Trip_ExpectedStockModel> modelList = new List<Trip_ExpectedStockModel>();
            try
            {
                if (DistributorId > 0)
                {
                    modelList = _unitOfWork.stockRepository.GetTripExpectedStock(DistributorId, SelDate);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistributorId, 0, 0, "GetTripExpectedStock", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trip Item List
        [HttpGet]
        [Route("Trip/GetTripItemList/{DistributorId}")]
        public List<TripItemModel> GetTripItemList(int DistributorId)
        {
            List<TripItemModel> modelList = new List<TripItemModel>();
            try
            {
                modelList = _unitOfWork.stockRepository.GetTripItemList(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistributorId, 0, 0, "GetTripItemList", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save_ Trip Sequence Pending Orders
        [HttpPost]
        [Route("Trip/Save_TripSequencePendingOrders")]
        public int Save_TripSequencePendingOrders([FromBody] List<UnsavedConsumers> _tripSequenceModel)
        {
            int ReturnId = 0;
            try
            {
                if (_tripSequenceModel.Any())
                {
                    ReturnId = _unitOfWork._tripRepository.Save_TripSequencePendingOrders(_tripSequenceModel);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(ReturnId, 0, 0, 0, 0, "Save_TripSequencePendingOrders", null, BusinessCont.FailStatus, ex.Message);
            }
            return ReturnId;
        }
        #endregion

        #region Trip Planning Old
        [HttpPost]
        [Route("Trip/TripPlanningOld")]
        public int TripPlanningOld([FromBody] UnsavedConsumers tripPlanning)
        {
            int retVal = 0;
            try
            {
                retVal = _unitOfWork._tripRepository.TripPlanningOld(tripPlanning.DistributorId, tripPlanning.TripDate, tripPlanning.RoleId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "Trip/TripPlanningOld", null, BusinessCont.FailStatus, ex.Message);
            }
            return retVal;
        }
        #endregion

        #region Trip Planning
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanning")]
        public int ActiveCaseTripPlanning([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            try
            {
                RetVal = _unitOfWork._tripRepository.ActiveCaseTripPlanning(tripPlanning.DistributorId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Trip/TripPlanning - ActiveCaseTripPlanning", "DistributorId & TripDate = " + (tripPlanning.DistributorId + "  " + tripPlanning.TripDate), BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Trip Planning Density Case
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningDensityCase")]
        public int TripPlanningDensityCase([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            try
            {
                RetVal = _unitOfWork._tripRepository.TripPlanningDensityCase(tripPlanning.DistributorId, tripPlanning.TripDate, Convert.ToInt32(tripPlanning.RoleId));
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TripPlanningDensityCase", "DistributorId= " + tripPlanning.DistributorId + ", TripDate= " + tripPlanning.TripDate + ", RoleId= " + tripPlanning.RoleId, BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Get Cash Memo
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/GetCashMemo")]
        public CashMemo GetCashMemo([FromBody]CashMemo model)
        {
            CashMemo cashMemo = new CashMemo();
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.BatchNo))
                {
                    cashMemo = _unitOfWork._tripRepository.GetCashMemo(model);
                }
                else
                {
                    cashMemo.ExMsg = "input parameter is not correct";
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetCashMemo", "", BusinessCont.FailStatus, ex.Message);
            }
            return cashMemo;
        }
        #endregion

        #region Trip Planning Density Ageing Case
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningDensityAgeingCase")]
        public int TripPlanningDensityAgeingCase([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            bool IsSummary = true;
            try
            {
                RetVal = _unitOfWork._tripRepository.TripPlanningDensityAgeingCase(tripPlanning.DistributorId, tripPlanning.TripDate, Convert.ToInt32(tripPlanning.RoleId), IsSummary); // density Ageing case
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TripPlanningDensityAgeingCase", "DistributorId= " + tripPlanning.DistributorId + ", TripDate= " + tripPlanning.TripDate + ", RoleId= " + tripPlanning.RoleId, BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Trip Planning Ageing Density Case
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningAgeingDensityCase")]
        public int AgeingDensityTripPlanning([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            try
            {
                RetVal = _unitOfWork._tripRepository.AgeingDensityTripPlanning(tripPlanning.DistributorId, tripPlanning.TripDate, Convert.ToInt32(tripPlanning.RoleId), true);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "AgeingDensityTripPlanning", null, BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Trip Planning Density Ageing Case Multiple Cluster
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningDensityAgeingCaseMultipleCluster")]
        public int TripPlanningDensityAgeingCaseMultipleCluster([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            try
            {
                RetVal = _unitOfWork._tripRepository.TripPlanningDensityAgeingCaseMultipleCluster(tripPlanning.DistributorId, tripPlanning.TripDate, Convert.ToInt32(tripPlanning.RoleId), tripPlanning.MultiClusterStr);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "TripPlanningDensityAgeingCase", "DistributorId= " + tripPlanning.DistributorId + ", TripDate= " + tripPlanning.TripDate + ", RoleId= " + tripPlanning.RoleId, BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Trip Planning Only Ageing Case
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningOnlyAgeingCase")]
        public int TripPlanningOnlyAgeingCase([FromBody] UnsavedConsumers tripPlanning)
        {
            int RetVal = 0;
            try
            {
                RetVal = _unitOfWork._tripRepository.TripPlanningOnlyAgeingCase(tripPlanning.DistributorId, tripPlanning.TripDate, Convert.ToInt32(tripPlanning.RoleId), true);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "TripPlanningOnlyAgeingCase", "DistributorId= " + tripPlanning.DistributorId + ", TripDate= " + tripPlanning.TripDate + ", RoleId= " + tripPlanning.RoleId, BusinessCont.FailStatus, ex.Message);
            }
            return RetVal;
        }
        #endregion

        #region Trip Not Included Consumers
        [HttpPost]
        [Route("Trip/TripNotIncludedConsumers")]
        public List<ConsumerMaster> TripNotIncludedConsumers([FromBody] TripPlanningData tripPlanning)
        {
            List<ConsumerMaster> modelList = new List<ConsumerMaster>();
            try
            {
                modelList = _unitOfWork._tripRepository.TripNotIncludedConsumerList(tripPlanning);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TripNotIncludedConsumers", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Suggested Extra Tips
        [HttpPost]
        [Route("Trip/SuggestedExtraTips")] // Check available trips from another cluster
        public List<TripPlanningCardDataForExtraTrip> GetSuggestedExtraTrips([FromBody] TripsAvailability tripPlanning)
        {
            List<TripPlanningCardDataForExtraTrip> modelList = new List<TripPlanningCardDataForExtraTrip>();
            try
            {
                modelList = _unitOfWork._tripRepository.getSuggestedExtraTripAvailability(tripPlanning.DistributorId, Convert.ToInt32(tripPlanning.ClusterId), tripPlanning.TripDate.Date);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripAvailability", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Trips Availability
        [HttpPost]
        [Route("Trip/TripsAvailability")]//Check available trips from another cluster
        public List<TripsAvailability> GetTripAvailability([FromBody] TripsAvailability tripPlanning)
        {
            List<TripsAvailability> modelList = new List<TripsAvailability>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripsAvailability(tripPlanning.DistributorId, Convert.ToInt32(tripPlanning.ClusterId), tripPlanning.TripDate.Date);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripAvailability", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Extra Trip Planning
        [HttpPost]
        [Route("Trip/ExtraTripPlanning/{SourceClusterId}/{intDistributorId}")]
        public int ExtraTripPlanning(int SourceClusterId, int intDistributorId, List<TripPlanningCardDataForExtraTrip> lstTripplanningData)
        {
            int retVal = 0;
            try
            {
                retVal = _unitOfWork._tripRepository.ExtraTripPlanning(SourceClusterId, intDistributorId, lstTripplanningData);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, intDistributorId, 0, 0, "ExtraTripPlanning", "SourceClusterId= " + SourceClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return retVal;
        }
        #endregion

        #region Smart App Trip Planning
        [HttpPost]
        [Route("Trip/SmartAppTripPlanning")]
        public List<ConsumerMaster> SmartAppTripPlanning([FromBody] ConsumerMaster tripPlanning)
        {
            List<ConsumerMaster> modelList = new List<ConsumerMaster>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetShortDistenceOfSmartAppCons(tripPlanning.DistributorID, tripPlanning.StaffRefNo, tripPlanning.SelDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorID, 0, 0, "SmartAppTripPlanning", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trip Planning Card Data
        [HttpPost]
        [Route("Trip/GetTripPlanningCardData")]
        public List<PlanCardData> GetTripPlanningCardData(TripPlanningData Model)
        {
            List<PlanCardData> modelList = new List<PlanCardData>();
            try
            {
                modelList = _unitOfWork._tripRepository.TripPlanningCardDetails(Model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTripPlanningCardData", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trip Summary
        [HttpPost]
        [Route("Trip/GetTripSummary")]
        public TripSummaryDtls GetTripSummary(TripSummaryDtls postData)
        {
            TripSummaryDtls model = new TripSummaryDtls();
            model.tripSummarylist = new List<TripSummary>();
            model.Status = BusinessCont.FailStatus;
            try
            {
                DateTime? SelDate = null;
                if (!string.IsNullOrEmpty(postData.SelDate))
                    SelDate = BusinessCont.StrConvertIntoDatetime(postData.SelDate);

                model.tripSummarylist = _unitOfWork._tripRepository.getTripSummary(postData.DistributorId, SelDate, postData.ClusterId);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTripSummary", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get DB Current Location
        [HttpPost]
        [Route("Trip/GetDBCurrentLocation")]
        public CurrentLocation GetDBCurrentLocation(CurrentLocation currentLocation)
        {
            CurrentLocation model = new CurrentLocation();
            try
            {
                model = _unitOfWork._tripRepository.GetDBCurrentLocation(currentLocation.DistributorId, currentLocation.StaffRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, currentLocation.DistributorId, 0, 0, "GetDBCurrentLocation", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Update Vehicle Del Boy In Trip Planning
        [HttpPost]
        [Route("Trip/UpdateVehicleDelBoyInTripPlanning")]
        public IHttpActionResult UpdateVehicleDelBoyInTripPlanning([FromBody]TripPlanningData tripPlanningData)
        {
            if (tripPlanningData == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            long result = 0;
            try
            {
                result = _unitOfWork._tripRepository.UpdateDeliveryBoyVehicleInTripPlanning(tripPlanningData);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateVehicleDelBoyInTripPlanning", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Get Vehicle List For Trip
        [HttpPost]
        [Route("Trip/GetVehicleListForTrip")]
        public List<VehicleModel> GetVehicleListForTrip([FromBody]VehicleModel vehicle)
        {
            List<VehicleModel> modelList = new List<VehicleModel>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetVehicleListForTrips(vehicle.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, vehicle.DistributorId, 0, 0, "GetVehicleListForTrip", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Update Trip Status
        [HttpPost]
        [Route("Trip/UpdateTripStatus")]
        public IHttpActionResult UpdateTripStatus([FromBody]PostModelForUpdateTripStatus Model)
        {
            if (Model == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            int result = 0;
            try
            {
                result = _unitOfWork._tripRepository.UpdateTripStatus(Model.DistributorId, Model.TripIdStr, Model.TripStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateTripStatus", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Get Trip Details For Route
        [HttpPost]
        [Route("Trip/GetTripDetailsForRoute")]
        public List<TripDetailsForRoute> GetTripDetailsForRoute([FromBody]TripPlanningData tripPlanning)
        {
            List<TripDetailsForRoute> modelList = new List<TripDetailsForRoute>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripDetailsForRoute(tripPlanning.DistributorId, Convert.ToInt64(tripPlanning.TripId));
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "GetTripDetailsForRoute", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Route Details For Trip
        [HttpPost]
        [Route("Trip/GetRouteDetailsForTrip")]
        public dynamic GetRouteDetailsForTrip([FromBody]PostModelForRoute tripPlanning)
        {
            try
            {
                return _unitOfWork._tripRepository.TripRouteDetails(tripPlanning);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripDetailsForRoute", null, BusinessCont.FailStatus, ex.Message);
                throw;
            }
        }
        #endregion

        #region Get Route For Density Comparison
        [HttpPost]
        [Route("Trip/GetRouteForDensityComparison")]
        public dynamic GetRouteForDensityComparison([FromBody]UnsavedConsumers tripPlanning)
        {
            try
            {
                return _unitOfWork._tripRepository.TripForDensityComparison(tripPlanning.DistributorId, tripPlanning.TripDate, tripPlanning.RoleId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripDetailsForRoute", null, BusinessCont.FailStatus, ex.Message);
                throw;
            }
        }
        #endregion

        #region Update Trip Batch Status
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/UpdateTripBatchStatus")]
        public string UpdateTripBatchStatus([FromBody] BatchStatus UpdateBatchStatus)
        {
            string connectionStr = string.Empty, msg = string.Empty;
            try
            {
                connectionStr = ConfigurationManager.AppSettings["BatchconnectionString"];
                msg = _unitOfWork._tripRepository.GetDistributorTripBatchStatus(UpdateBatchStatus.DistributorId, UpdateBatchStatus.TripBatchDate, UpdateBatchStatus.TripBatchId, connectionStr);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "UpdateTripBatchStatus", null, BusinessCont.FailStatus, ex.Message);
            }
            return msg;
        }
        #endregion

        #region Trip Planning To Interface
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/TripPlanningToInterface")]
        public long TripPlanningToInterface([FromBody] UnsavedConsumers tripPlanning)
        {
            long retVal = 0;
            try
            {
                retVal = _unitOfWork._tripRepository.TripPlanningToInterface(tripPlanning.DistributorId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Trip/TripPlanningToInterface", "DistributorId & TripDate = " + (tripPlanning.DistributorId + "  " + tripPlanning.TripDate), BusinessCont.FailStatus, ex.Message);
            }
            return retVal;
        }
        #endregion

        #region Save Log
        [HttpPost]
        [Route("Trip/SaveLog")]
        public void SaveLog([FromBody] TimeModel time)
        {
            try
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "SaveLog: State -" + time.StateCode + ", District -" + time.DistrictCode + ", Start Time - " + time.StartTime + ", End Time-" + time.EndTime + ", Duration - " + time.Duration, null, BusinessCont.SuccessStatus, null);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "SaveLog", null, BusinessCont.FailStatus, ex.Message);
            }
        }
        #endregion

        #region TripDetailsList
        //get the Trip Details List By Distributor Id
        [HttpPost]
        [Route("Trip/GetDistributorTripDetailsList")]
        public IHttpActionResult GetDistributorTripDetailsList([FromBody]TripDetailsModel _distributorTripDetailsModel)
        {
            if (_distributorTripDetailsModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorTripDetails _DistributorTripDetails = new DistributorTripDetails();
            _DistributorTripDetails.TripDetailsList = new List<TripDetailsModel>();
            try
            {
                _DistributorTripDetails.TripDetailsList = _unitOfWork._tripRepository.GetDistributorTripDetailsList(_distributorTripDetailsModel.DistributorId, _distributorTripDetailsModel.FromDate, _distributorTripDetailsModel.ToDate);
                _DistributorTripDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _DistributorTripDetails.Status = BusinessCont.FailStatus;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistributorTripDetailsList", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_DistributorTripDetails);
        }

        [HttpGet]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Trip/MapMyIndiaDemo")]
        public UnsavedConsumers MapMyIndiaDemo()
        {
            UnsavedConsumers model = new UnsavedConsumers();
            try
            {
                model = _unitOfWork._tripRepository.MapMyIndiaDemo();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "MapMyIndiaDemo", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Trip Cases
        [HttpPost]
        [Route("Trip/GetTripCases")]
        public List<TripCaseMaster> GetTripCases()
        {
            List<TripCaseMaster> modelList = new List<TripCaseMaster>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripCaseDetails();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripCases", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }

        [HttpPost]
        [Route("Trip/UpdateTripCase")]
        public RtnPostModel UpdateTripCase([FromBody]List<TripCase> tripCase)
        {
            RtnPostModel model = new RtnPostModel();
            try
            {
                model = _unitOfWork._tripRepository.UpdateTripCase(tripCase);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "UpdateTripCase", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Trip Case Source Master
        [HttpPost]
        [Route("Trip/GetTripCaseSourceMaster")]
        public List<BookingSource> GetTripCaseSourceMaster([FromBody]int ParameterId)
        {
            List<BookingSource> modelList = new List<BookingSource>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripCaseSourceMaster(ParameterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetTripDetailsForRoute", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Trip Case Allotment
        [HttpPost]
        [Route("Trip/GetTripCasesDistributorwise")]
        public List<TripCaseMst> GetTripCasesDistributorwise(TripCaseMst model)
        {
            List<TripCaseMst> modelList = new List<TripCaseMst>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripCasesDistributorwise(model.DistributorId, model.CaseId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, Convert.ToInt32(model.DistributorId), 0, 0, "GetTripCasesDistributorwise", "CaseId= " + model.CaseId, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }

        [HttpPost]
        [Route("Trip/SaveTripCaseAllotment")]
        public RtnPostModel SaveTripCaseAllotment([FromBody]List<TripCaseMst> tripCase)
        {
            RtnPostModel model = new RtnPostModel();
            try
            {
                model = _unitOfWork._tripRepository.SaveTripCaseAllotment(tripCase);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "SaveTripCaseAllotment", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Distributorwise Active Trip Case
        [HttpPost]
        [Route("Trip/GetActiveTripCaseDistributorwise")]
        public List<ActiveTripStatus> GetActiveTripCaseDistributorwise(TripPlanningData model)
        {
            List<ActiveTripStatus> modelList = new List<ActiveTripStatus>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetDistributorWiseActiveTripCase(model.DistributorId, model.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.DistributorId, 0, 0, "GetActiveTripCaseDistributorwise", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Distributor wise  Trip Generation
        [HttpPost]
        [Route("Trip/GetDistwiseTripGeneration")]
        public List<ActiveTripGen> GetDistwiseTripGeneration()
        {
            List<ActiveTripGen> modelList = new List<ActiveTripGen>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetDistWiseActiveTripGen();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetDistwiseTripGeneration", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Update Dist Wise Active Trip Generation
        [HttpPost]
        [Route("Trip/UpdateDistWiseActiveTripGen")]
        public long UpdateDistWiseActiveTripGen(List<ActiveTripGen> model)
        {
            long retVal = 0;
            try
            {
                retVal = _unitOfWork._tripRepository.UpdateDistWiseActiveTripGen(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "UpdateDistWiseActiveTripGen", null, BusinessCont.FailStatus, ex.Message);
            }
            return retVal;
        }
        #endregion

        #region Trip Sequence
        [HttpPost]
        [Route("Trip/TripSequence")]
        [AllowAnonymous]
        public List<TripTesting> TripSequence([FromBody]TripTesting model)
        {
            List<TripTesting> modelList = new List<TripTesting>();
            try
            {
                modelList = _unitOfWork._tripRepository.TripSequenceTesting(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TripSequence", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Regenerate Trips
        [HttpPost]
        [Route("Trip/RegenerateTrips")]
        [AllowAnonymous]
        public int RegenerateTrips([FromBody] UnsavedConsumers tripPlanning)
        {
            int retVal = 0;
            try
            {
                retVal = _unitOfWork._tripRepository.GenerateTripClusterwise(tripPlanning.DistributorId, tripPlanning.ClusterId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, tripPlanning.DistributorId, 0, 0, "RegenerateTripDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return retVal;
        }
        #endregion

        #region Cancel Trip
        [HttpPost]
        [Route("Trip/CancelTrip")]
        [AllowAnonymous]
        public CancelTrip CancelTrip([FromBody]CancelTrip model)
        {
            CancelTrip modelCancelTrip = new CancelTrip();
            try
            {
                modelCancelTrip = _unitOfWork._tripRepository.CancelTrip(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "CancelTrip", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelCancelTrip;
        }
        #endregion

        #region GetAllClusterTripSummery
        [HttpPost]
        [Route("Trip/GetAllClusterTripSummary")]
        public TripSummaryDtls GetAllClusterTripSummary(TripSummaryDtls postData)
        {
            TripSummaryDtls model = new TripSummaryDtls();
            model.tripSummarylist = new List<TripSummary>();
            model.Status = BusinessCont.FailStatus;
            try
            {
                model.tripSummarylist = _unitOfWork._tripRepository.getAllClusterTripSummary(postData.DistributorId, postData.SelDate);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, postData.DistributorId, 0, 0, "GetAllClusterTripSummary", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Show LiveLocation Of DeliveryBoy
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/ShowLiveLocationOnMap")]
        public List<DelBoyLocation> ShowLiveLocationOnMap([FromBody] DelBoyLocation model)
        {
            List<DelBoyLocation> modelList = new List<DelBoyLocation>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripRoute(model.DistributorID, model.StaffRefNo, model.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.DistributorID, 0, 0, "ShowLiveLocationOnMap", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trip List Cluster wise
        [HttpPost]
        [Route("Trip/GetTripListClusterwise")]
        [AllowAnonymous]
        public List<TripPlanningData> GetTripListClusterwise([FromBody]TripPlanningData tripPlanningData)
        {
            List<TripPlanningData> modelList = new List<TripPlanningData>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetTripPlanningDetails(tripPlanningData);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTripListClusterwise", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save LiveLocation Of DeliveryBoy
        [AllowAnonymous]
        [HttpPost]
        [Route("Trip/SaveLocation")]
        public string SaveLocation([FromBody] SaveLocationModel model)
        {
            string msg = string.Empty;
            try
            {
                msg = _unitOfWork._tripRepository.SaveLocationDetails(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.DistributorId, 0, 0, "SaveLocation", null, BusinessCont.FailStatus, ex.Message);
            }
            return msg;
        }
        #endregion

        #region Get Cluster DeliveryBoy List
        [HttpPost]
        [Route("Trip/GetClusterDeliveryBoyList")]
        [AllowAnonymous]
        public List<GetClusterDeliveryBoyList> GetClusterDeliveryBoyList([FromBody]GetClusterDeliveryBoyList model)
        {
            List<GetClusterDeliveryBoyList> modelList = new List<GetClusterDeliveryBoyList>();
            try
            {
                modelList = _unitOfWork._tripRepository.GetClusterDeliveryBoyList(model.DistributorId, model.ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "GetClusterDeliveryBoyList", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion 

        #region Save Trip Kilo Meter 
        [HttpPost]
        [Route("Trip/SaveTripKM")]
        [AllowAnonymous]
        public string SaveTripKM([FromBody]DeliveryBoyList model)
        {
            List<DeliveryBoyList> DeliveryBoyDetails = new List<DeliveryBoyList>();
            string response = string.Empty;
            int Result = 0;
            try
            {
                DeliveryBoyDetails = _unitOfWork._tripRepository.getAllDeliveryBoyList(model.DistributorId);
                for (int i = 0; i < DeliveryBoyDetails.Count(); i++)
                {
                    Result = _unitOfWork._tripRepository.getAllClusterTripKMSummary(DeliveryBoyDetails[i].DistributorId, model.TripDate, Convert.ToString(DeliveryBoyDetails[i].DelBoyId));
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "SaveTripKM", "Kilometer Save", BusinessCont.FailStatus, ex.Message);
            }

            if (Result > 0) response = "Success"; else response = "Fail";

            return response;
        }
        #endregion

        #region Get Trip Kilo Meter 
        [HttpPost]
        [Route("Trip/GetTripKM")]
        [AllowAnonymous]
        public List<TripKMListModel> GetTripKM([FromBody]TripKMListModel model)
        {
            List<TripKMListModel> TripDetails = new List<TripKMListModel>();
            try
            {
                TripDetails = _unitOfWork._tripRepository.getAllTripKMList(model.FromDate, model.ToDate, model.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, model.DistributorId, 0, 0, "GetTripKM", "Get Trip KM Details", BusinessCont.FailStatus, ex.Message);
            }
            return TripDetails;
        }
        #endregion

        #region Test Distance Matrix API
        [HttpPost]
        [Route("Trip/TestTripDistMatrixAPI")]
        [AllowAnonymous]
        public List<TripTesting> TestTripDistMatrixAPI([FromBody] TripTesting model)
        {
            List<TripTesting> modelList = new List<TripTesting>();
            try
            {
                modelList = _unitOfWork._tripRepository.TestDistMatrixAPI(model.Source, model.Destination);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "TestTripDistMatrixAPI", "", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Test Route API
        [HttpGet]
        [Route("Trip/TestTripRouteAPI")]
        [AllowAnonymous]
        public List<TripRouteTest> TestTripRouteAPI()
        {
            List<TripRouteTest> modelList = new List<TripRouteTest>();
            try
            {
                modelList = _unitOfWork._tripRepository.TestRouteAPI();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "TestTripRouteAPI", "", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Generarte Trips With New Parameters
        [HttpPost]
        [Route("Trip/GenerarteTripsWithNewParameters")]
        public int GenerarteTripsWithNewParameters(TripCaseParameters model)
        {
            int CntScheduler = 0;
            try
            {
                CntScheduler = _unitOfWork._tripRepository.GenerateTripWithNewParameters(Convert.ToInt32(model.DistributorId), Convert.ToInt32(model.ClusterId),model.ParameterType);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, Convert.ToInt32(model.DistributorId), 0, 0, "GenerarteTripsWithNewParameters", "", BusinessCont.FailStatus, ex.Message);
            }
            return CntScheduler;
        }
        #endregion

        #region Get Trip Summary For Trip Status
        [HttpPost]
        [Route("Trip/GetTripSummaryforStatus")]
        [AllowAnonymous]
        public TripSummaryDtlsForTripStatus GetTripSummaryforStatus(TripSummaryDtlsForTripStatus postData)
        {
            TripSummaryDtlsForTripStatus model = new TripSummaryDtlsForTripStatus();
            model.tripSummarylist = new List<TripSummaryForTripStatus>();
            model.Status = BusinessCont.FailStatus;
            try
            {
                DateTime? SelDate = null;
                if (!string.IsNullOrEmpty(postData.SelDate))
                    SelDate = BusinessCont.StrConvertIntoDatetime(postData.SelDate);

                model.tripSummarylist = _unitOfWork._tripRepository.getTripSummaryforstatus(postData.DistributorId, SelDate);
                model.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTripSummaryforStatus", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

    }
}