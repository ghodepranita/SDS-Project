using Aadyam.SDS.API.SchedulerClasses;
using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Hubs;
using Aadyam.SDS.Business.Model.Context;
using Aadyam.SDS.Business.Model.Distributor;
using Aadyam.SDS.Business.Model.GeoCoordinates;
using Aadyam.SDS.Business.Model.Stock;
using Aadyam.SDS.Business.Model.Taluka;
using Aadyam.SDS.Business.Model.Trip;
using Aadyam.SDS.Business.Model.User;
using Aadyam.SDS.Business.Repositories;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class GeoCoordinateController : BaseApiController
    {
        decimal LogId = 0;

        #region Get Consumer Master
        [HttpPost]
        [Route("GeoCoordinate/GetConsumerMaster")]
        public ConsumerDetails GetConsumerMaster([FromBody]ConsumerMaster consumerDtls)
        {
            ConsumerDetails consumerDetails = new ConsumerDetails();
            consumerDetails.consumerMasters = new List<ConsumerMaster>();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                consumerDetails.consumerMasters = _unitOfWork.geoCoordinateRepository.GetConsumerMaster(consumerDtls.DistributorID, consumerDtls.AreaRefNo.ToString(), AppConfigAPITimeout);
                consumerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                consumerDetails.Status = BusinessCont.FailStatus;
                consumerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerMaster", null, BusinessCont.FailStatus, ex.Message);
            }
            return consumerDetails;
        }
        #endregion

        #region Get Unsaved Consumer Master
        [HttpPost]
        [Route("GeoCoordinate/GetUnsavedConsumerMaster")]
        public ConsumerDetails GetUnsavedConsumerMaster([FromBody]ConsumerMaster consumerDtls)
        {
            ConsumerDetails consumerDetails = new ConsumerDetails();
            consumerDetails.unsavedConsumerMasters = new List<UnsavedConsumers>();
            try
            {
                consumerDetails.unsavedConsumerMasters = _unitOfWork.geoCoordinateRepository.GetUnsavedConsumerMaster(consumerDtls.DistributorID);
                consumerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                consumerDetails.Status = BusinessCont.FailStatus;
                consumerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetUnsavedConsumerMaster", null, BusinessCont.FailStatus, ex.Message);
            }
            return consumerDetails;
        }
        #endregion

        #region Distributor Area
        //to get the Distributor Unmapped Area Counts
        [HttpPost]
        [Route("GeoCoordinate/GetDistributor_UnmappedAreaCount")]
        public IHttpActionResult GetDistributor_UnmappedAreaCount([FromBody]DistributorUnmappedAreaCount _distributorUnmappedAreaCount)
        {
            try
            {
                _distributorUnmappedAreaCount.UnmappedAreaCount = _unitOfWork.geoCoordinateRepository.Get_UnmappedAreaCount(_distributorUnmappedAreaCount.DistributorId);
                _distributorUnmappedAreaCount.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorUnmappedAreaCount.Status = BusinessCont.FailStatus;
                _distributorUnmappedAreaCount.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _distributorUnmappedAreaCount.DistributorId, 0, 0, "GetDistributor_UnmappedAreaCount", "DistributorId= " + _distributorUnmappedAreaCount.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorUnmappedAreaCount);
        }

        [HttpPost]
        [Route("GeoCoordinate/AddDistributorAreaMapping")]
        public IHttpActionResult AddDistributorAreaMapping([FromBody]DistributorUnmappedAreaCount _distributorUnmappedAreaCount)
        {
            try
            {
                _distributorUnmappedAreaCount.UnmappedAreaCount = _unitOfWork.geoCoordinateRepository.AddDistributorAreaMapping(_distributorUnmappedAreaCount.DistributorId);
                _distributorUnmappedAreaCount.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorUnmappedAreaCount.Status = BusinessCont.FailStatus;
                _distributorUnmappedAreaCount.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _distributorUnmappedAreaCount.DistributorId, 0, 0, "Add Distributor Area Mapping", "DistributorId= " + _distributorUnmappedAreaCount.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorUnmappedAreaCount);
        }

        [HttpPost]
        [Route("GeoCoordinate/GetDistributorAreaMappingList")]
        public IHttpActionResult GetDistributorAreaMappingList([FromBody]AreaMappingListModel _areaMappingListModel)
        {
            AreaMappingDetails _areaMappingDetails = new AreaMappingDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetDistributorAreaMappingList(_areaMappingListModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _areaMappingListModel.DistributorId, 0, 0, "GetDistributorAreaMappingList", "DistributorId= " + _areaMappingListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }

        [HttpPost]
        [Route("GeoCoordinate/UpdateDistributorAreaMapping")]
        public IHttpActionResult UpdateDistributorAreaMapping([FromBody]AreaMappingListModel _areaMappingListModel)
        {
            DistributorUnmappedAreaCount _distributorUnmappedAreaCount = new DistributorUnmappedAreaCount();
            try
            {
                _distributorUnmappedAreaCount.UnmappedAreaCount = _unitOfWork.geoCoordinateRepository.UpdateDistributorAreaMapping(_areaMappingListModel);
                _distributorUnmappedAreaCount.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorUnmappedAreaCount.Status = BusinessCont.FailStatus;
                _distributorUnmappedAreaCount.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _distributorUnmappedAreaCount.DistributorId, 0, 0, "Add Distributor Area Mapping", "DistributorId= " + _distributorUnmappedAreaCount.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorUnmappedAreaCount);
        }

        [HttpPost]
        [Route("GeoCoordinate/GetAreaListForMapping")]
        public IHttpActionResult GetAreaListForMapping([FromBody]AreaMappingListModel _areaMappingListModel)
        {
            AreaMappingDetails _areaMappingDetails = new AreaMappingDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetAreaListForMapping(_areaMappingListModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _areaMappingListModel.DistributorId, 0, 0, "GetAreaListForMapping", "DistributorId= " + _areaMappingListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }

        [HttpPost]
        [Route("GeoCoordinate/GetAreaListForTest")]
        public IHttpActionResult GetAreaListForTest([FromBody]AreaMappingListModel _areaMappingListModel)
        {
            AreaMappingDetails _areaMappingDetails = new AreaMappingDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetAreaListForTest(_areaMappingListModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _areaMappingListModel.DistributorId, 0, 0, "GetAreaListForMapping", "DistributorId= " + _areaMappingListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }
        [HttpPost]
        [Route("GeoCoordinate/GetAreaListByDistrictAndTaluka")]
        public IHttpActionResult GetAreaListByDistrictAndTaluka([FromBody]DistTalukaModel distTalukaModel)
        {
            AreaMappingDetails _areaMappingDetails = new AreaMappingDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetAreaListByDistrictAndTaluka(distTalukaModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, distTalukaModel.DistributorId, 0, 0, "GetAreaListForMapping", "DistrictCode = " + distTalukaModel.DistrictCode + ", TalukaCode = " + distTalukaModel.TalukaCode, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }
        #endregion

        #region Reverce Geo Coordinates
        // Reverse geo coding - Save consumers location using consumer address.      
        [HttpPost]
        [Route("GeoCoordinate/ReverceGeoCoordinates")]
        public int ReverceGeoCoordinates([FromBody]ConsumerMaster consumerDtls)
        {
            int retValue = 0;
            try
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
                hubContext.Clients.All.sendMessage(0, 0);
                retValue = _unitOfWork.geoCoordinateRepository.ReverceGeoCoordinates(consumerDtls.DistributorID, consumerDtls.DistAreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerDtls.DistributorID, 0, 0, "ReverceGeoCoordinates", "DistAreaRefNo = " + consumerDtls.DistAreaRefNo, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Area Cluster Co Ordinares
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetAreaClusterCoOrdinares")]
        public List<AreaClusterCoordinates> GetAreaClusterCoOrdinares([FromBody]ConsumerMaster consumerDtls)
        {
            List<AreaClusterCoordinates> modelList = new List<AreaClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetAreaClusterCoOrdinares(consumerDtls.DistributorID);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, consumerDtls.DistributorID, 0, 0, "GetAreaClusterCoOrdinares", "Get Area Cluster CoOrdinares For Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Cluster Co Ordinares
        [HttpPost]
        [Route("GeoCoordinate/GetClusterCoOrdinares")]
        [AllowAnonymous]
        public List<AreaClusterCoordinates> GetClusterCoOrdinares([FromBody]ConsumerMaster consumerDtls)
        {
            List<AreaClusterCoordinates> modelList = new List<AreaClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClusterCoOrdinares(consumerDtls.DistributorID, consumerDtls.ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, consumerDtls.DistributorID, 0, 0, "GetAreaClusterCoOrdinares", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Reverce Geo Coordinates For UnSaved
        [HttpPost]
        [Route("GeoCoordinate/ReverceGeoCoordinatesForUnSaved")]
        public int ReverceGeoCoordinatesForUnSaved([FromBody]ConsumerMaster consumerDtls)
        {
            int retValue = 0;
            try
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
                hubContext.Clients.All.sendMessage(0, 0);
                retValue = _unitOfWork.geoCoordinateRepository.ReverceGeoCoordinates(consumerDtls.DistributorID, null);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, consumerDtls.DistributorID, 0, 0, 0, "ReverceGeoCoordinatesForUnSaved", "GeoCoordinate/ReverceGeoCoordinatesForUnSaved : DistributorID = " + consumerDtls.DistributorID, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Save Consumer Location
        // Save consumers location manually.        
        [HttpPost]
        [Route("GeoCoordinate/SaveConsumerLocation")]
        public long SaveConsumerLocation([FromBody]ConsumerMaster consumerDtls)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SaveConsumerLocation(consumerDtls);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveConsumerLocation", null, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Update Consumer Area Address
        [HttpPost]
        [Route("GeoCoordinate/UpdateConsumerAreaAddress")]
        public long UpdateConsumerAreaAddress([FromBody]ConsumerMaster consumerDtls)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.UpdateConsumerAreaAddress(consumerDtls);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 33, consumerDtls.DistributorID, 0, 0, "UpdateConsumerAreaAddress", "UniqueConsumerId= " + consumerDtls.UniqueConsumerId + ", Latitude= " + consumerDtls.Latitude + ", Longitude= " + consumerDtls.Longitude, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Get Pending Bookings For Trip
        [HttpPost]
        [Route("GeoCoordinate/GetPendingBookingsForTrip")]
        public List<PendingBookings> GetPendingBookingsForTrip([FromBody]PostModelForPendingBookings Model)
        {
            List<PendingBookings> pendingBookings = new List<PendingBookings>();
            try
            {
                pendingBookings = _unitOfWork.geoCoordinateRepository.GetPendingBookingsForTrip(Model.DistributorId, Model.ClusterId, Model.OrderCount);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPendingBookingsForTrip", null, BusinessCont.FailStatus, ex.Message);
            }
            return pendingBookings;
        }
        #endregion

        #region Get Dist Area Coordinates     
        [HttpPost]
        [Route("GeoCoordinate/GetDistAreaCoordinates")]
        public List<AreaGeoCoordinates> GetDistAreaGeoCoordinates([FromBody]ConsumerMaster consumerDtls)
        {
            List<AreaGeoCoordinates> AreaGeoCoordinatesList = new List<AreaGeoCoordinates>();
            try
            {
                AreaGeoCoordinatesList = _unitOfWork.geoCoordinateRepository.GetDistAreaGeoCoordinates(consumerDtls.DistributorID, consumerDtls.DistAreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistAreaCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return AreaGeoCoordinatesList;
        }
        #endregion

        #region Get Dist Area GeoCoordinates With Cons
        [HttpPost]
        [Route("GeoCoordinate/GetDistAreaGeoCoordinatesWithCons")]
        public ConsumerCoordinates GetDistAreaGeoCoordinatesWithCons([FromBody]ConsumerMaster consumerDtls)
        {
            ConsumerCoordinates model = new ConsumerCoordinates();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.ConsumerDetailsWithArea(consumerDtls.DistributorID, consumerDtls.DistAreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerDtls.DistributorID, 0, 0, "GetDistAreaGeoCoordinatesWithCons", "DistAreaRefNo= " + consumerDtls.DistAreaRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Dist Cluster Geo Coordinates With Cons
        [HttpPost]
        [Route("GeoCoordinate/GetDistClusterGeoCoordinatesWithCons")]
        public ConsumerCoordinates GetDistClusterGeoCoordinatesWithCons([FromBody]ConsumerMaster consumerDtls)
        {
            ConsumerCoordinates model = new ConsumerCoordinates();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.ConsumerDetailsWithCluster(consumerDtls.DistributorID, consumerDtls.ClusterId, consumerDtls.AreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerDtls.DistributorID, 0, 0, "GetDistClusterGeoCoordinatesWithCons", "DistAreaRefNo= " + consumerDtls.DistAreaRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Dist Cluster Coordinates
        [HttpPost]
        [Route("GeoCoordinate/GetDistClusterCoordinates")]
        public DistributorClusterCoordinates GetDistClusterCoordinates([FromBody]DistributorClusterCoordinates ClusterDtls)
        {
            DistributorClusterCoordinates distributorClusterCoordinates = new DistributorClusterCoordinates();
            try
            {
                distributorClusterCoordinates = _unitOfWork.geoCoordinateRepository.GetDistributorCluster(ClusterDtls.DistributorId, ClusterDtls.AreaClusterId, ClusterDtls.AreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistClusterCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorClusterCoordinates;
        }
        #endregion

        #region Get Trading Area Coordinate
        [HttpPost]
        [Route("GeoCoordinate/GetTradingAreaCoordinate")]
        public DistributorClusterCoordinates GetTradingAreaCoordinate([FromBody]DistributorModel distributorDtls)
        {
            DistributorClusterCoordinates distributorClusterCoordinates = new DistributorClusterCoordinates();
            try
            {
                distributorClusterCoordinates = _unitOfWork.geoCoordinateRepository.GetTradingAreaCoordinates(distributorDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, distributorDtls.DistributorId, 0, 0, "GetTradingAreaCoordinate", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorClusterCoordinates;
        }
        #endregion

        #region Get SA Trading Area Coordinate
        [HttpPost]
        [Route("GeoCoordinate/GetSATradingAreaCoordinate")]
        public List<DistributorClusterCoordinates> GetSATradingAreaCoordinate([FromBody]DistributorModel distributorDtls)
        {
            List<DistributorClusterCoordinates> distributorClusterCoordinates = new List<DistributorClusterCoordinates>();
            try
            {
                distributorClusterCoordinates = _unitOfWork.geoCoordinateRepository.GetSATradingAreaCoordinates(distributorDtls.SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetSATradingAreaCoordinate", null, BusinessCont.FailStatus, ex.Message);
            }
            return distributorClusterCoordinates;
        }
        #endregion

        #region Get Dist Consumer Dtls Count
        [HttpPost]
        [Route("GeoCoordinate/GetDistConsumerDtlsCount")]
        public TradingAreaConsDtlsCount GetDistConsumerDtlsCount([FromBody]DistributorModel distributorDtls)
        {
            TradingAreaConsDtlsCount tradingAreaConsDtlsCount = new TradingAreaConsDtlsCount();
            try
            {
                tradingAreaConsDtlsCount = _unitOfWork.distributorRepository.GetTradingAreawiseCounts(distributorDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistConsumerDtlsCount", null, BusinessCont.FailStatus, ex.Message);
            }
            return tradingAreaConsDtlsCount;
        }
        #endregion

        #region Get Distributor Cluster Coordinates By DistributorId
        [HttpPost]
        [Route("GeoCoordinate/GetDistributorClusterCoordinatesByDistributorId")]
        public List<DistributorClusterCoordinates> GetDistributorClusterCoordinatesByDistributorId([FromBody]DistributorModel distributorDtls)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetDistributorClusterCoordinatesByDistributorId(distributorDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, distributorDtls.DistributorId, 0, 0, "GetDistributorClusterCoordinatesByDistributorId", "GeoCoordinate/GetDistributorClusterCoordinatesByDistributorId", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Cluster Area List By AreaRefNo
        [HttpPost]
        [Route("GeoCoordinate/GetClusterAreaListByAreaRefNo")]
        public List<DistributorClusterCoordinates> GetClusterAreaListByAreaRefNo([FromBody] DistributorClusterCoordinates _distributorClusterCoordinates)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClusterAreaListByAreaRefNo(_distributorClusterCoordinates);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, _distributorClusterCoordinates.DistributorId, 0, 0, null, null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Add Edit Trading Area Coordinates
        [HttpPost]
        [Route("GeoCoordinate/AddEditTradingAreaCoordinates")]
        public long AddEditTradingAreaCoordinates([FromBody]DistributorClusterCoordinates _DistributorClusterCoordinates)
        {
            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddEditTradingAreaCoordinates", null, BusinessCont.FailStatus, ex.Message);
                return 0;
            }
        }
        #endregion

        #region Cons Location Deviation
        [HttpPost]
        [Route("GeoCoordinate/ConsLocationDeviation")]
        public List<ConsLocandDlvLocDetails> ConsLocationDeviation([FromBody]ConsLocandDlvLocDetails ConsumerDetails)
        {
            List<ConsLocandDlvLocDetails> modelList = new List<ConsLocandDlvLocDetails>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.ConsLocationDeviation(ConsumerDetails.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, ConsumerDetails.DistributorId, 0, "ConsLocationDeviation", "DistributorId= " + ConsumerDetails.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Area Geo Coordinates
        [HttpPost]
        [Route("GeoCoordinate/GetAreaGeoCoordinates")]
        public List<AreaGeoCoordinates> GetAreaGeoCoordinates([FromBody]AreaGeoCoordinates areaGeoCoordinates)
        {
            List<AreaGeoCoordinates> modelList = new List<AreaGeoCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetAreaGeoCoordinates(areaGeoCoordinates.AreaCode, areaGeoCoordinates.DistributorId, areaGeoCoordinates.AreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, areaGeoCoordinates.DistributorId, 0, 0, "GetAreaGeoCoordinates", "AreaCode= " + areaGeoCoordinates.AreaCode + ", AreaRefNo=" + areaGeoCoordinates.AreaRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trading Area
        [HttpPost]
        [Route("GeoCoordinate/GetTradingArea")]
        public List<DistributorClusterCoordinates> GetTradingArea([FromBody]DistributorClusterCoordinates tradingAreaRequired)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetTradingArea(tradingAreaRequired.DistributorId, tradingAreaRequired.SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, tradingAreaRequired.DistributorId, 0, 0, "GetTradingArea", "SACode= " + tradingAreaRequired.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trading Area JSON For SA
        [HttpPost]
        [Route("GeoCoordinate/GetTradingAreaJSONForSA")]
        public List<DistributorClusterCoordinates> GetTradingAreaJSONForSA([FromBody]DistributorClusterCoordinates tradingAreaRequired)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetTradingAreaJSONForSA(tradingAreaRequired.SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, tradingAreaRequired.DistributorId, 0, 0, "GetTradingAreaJSONForSA", "DistributorIds= " + tradingAreaRequired.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Adjacent Trading Areas
        [HttpPost]
        [Route("GeoCoordinate/SaveAdjacentTradingAreas")]
        public long SaveAdjacentTradingAreas([FromBody]AdjacentTradingArea tradingAreaRequired)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SaveAdjacentTradingAreas(tradingAreaRequired);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, tradingAreaRequired.DistributorId, 0, 0, "SaveAdjacentTradingAreas", "SACode= " + tradingAreaRequired.SAId, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Adjacent Distributor List
        [HttpPost]
        [Route("GeoCoordinate/GetAdjacentDistributorList")]
        public List<AdjacentTradingArea> GetAdjacentDistributorList([FromBody]AdjacentTradingArea Model)
        {
            List<AdjacentTradingArea> modelList = new List<AdjacentTradingArea>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetAdjacentDistributorList(Model.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAdjacentDistributorList", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Transfer Booking
        [HttpPost]
        [Route("GeoCoordinate/SaveTransferBooking")]
        public long SaveTransferBooking([FromBody]TransferBooking transferBooking)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SaveTransferBooking(transferBooking);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveAdjacentTradingAreas", "SACode= " + transferBooking.AddedBy, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Transfer Bookings
        [HttpPost]
        [Route("GeoCoordinate/GetTransferBookings")]
        public List<TransferBooking> GetTransferBookings([FromBody]DistributorModel Model)
        {
            List<TransferBooking> modelList = new List<TransferBooking>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetTransferBookings(Model.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTransferBookings", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Dist Cluster Coordinates
        [HttpPost]
        [Route("GeoCoordinate/DistClusterCoordinates")]
        public List<DistributorClusterCoordinates> DistClusterCoordinates([FromBody]DistributorClusterCoordinates ClusterDtls)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetDistClusterCoordinates(ClusterDtls.DistributorId, ClusterDtls.AreaClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DistClusterCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Users Current Location
        [HttpPost]
        [Route("GeoCoordinate/SaveUsersCurrentLocation")]
        public string SaveUsersCurrentLocation([FromBody]CurrentLocation currentLocation)
        {
            string msg = string.Empty;
            try
            {
                _unitOfWork.geoCoordinateRepository.SaveUsersCurrentLocation(currentLocation);
                msg = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, currentLocation.DistributorId, 0, currentLocation.StaffRefNo, "SaveUsersCurrentLocation", "Latitude=" + currentLocation.Latitude + ", Longitude= " + currentLocation.Longitude, BusinessCont.FailStatus, ex.Message);
                msg = BusinessCont.FailStatus;
            }
            return msg;
        }
        #endregion

        #region Get Delivery boy Current Location
        [HttpPost]
        [Route("GeoCoordinate/GetDeliveryboyCurrentLocation")]
        public DeliveryboyCurrentLocation GetDeliveryboyCurrentLocation([FromBody]CurrentLocation currentLocation)
        {
            DeliveryboyCurrentLocation deliveryboyCurrentLocation = new DeliveryboyCurrentLocation();
            deliveryboyCurrentLocation.DeliveryBoyCurLoc = new DeliveryBoyCurLoc();
            try
            {
                deliveryboyCurrentLocation.DeliveryBoyCurLoc = _unitOfWork.geoCoordinateRepository.GetDeliveryboyCurrentLocation(currentLocation);
                deliveryboyCurrentLocation.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, currentLocation.DistributorId, 0, 0, "Get Deliveryboy Current Location", "UniqueConsumerId=" + currentLocation.UniqueConsumerId, BusinessCont.FailStatus, ex.Message);
                deliveryboyCurrentLocation.Status = BusinessCont.FailStatus;
                deliveryboyCurrentLocation.ExMsg = ex.Message;
            }
            return deliveryboyCurrentLocation;
        }
        #endregion

        #region Original And Dist Area Coordinates
        [HttpPost]
        [Route("GeoCoordinate/OriginalAndDistAreaCoordinates")]
        public AreaCoordinates OriginalAndDistAreaCoordinates([FromBody]AreaGeoCoordinates areaGeoCoordinates)
        {
            AreaCoordinates model = new AreaCoordinates();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetOriginalAndDistAreaCoordinates(areaGeoCoordinates.AreaCode, areaGeoCoordinates.DistributorId, areaGeoCoordinates.AreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, areaGeoCoordinates.DistributorId, 0, 0, "GetOriginalAndDistAreaCoordinates", "AreaCode= " + areaGeoCoordinates.AreaCode + ", AreaRefNo=" + areaGeoCoordinates.AreaRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Distributor Area By Cluster
        [HttpPost]
        [Route("GeoCoordinate/GetDistributorAreaByCluster")]
        public List<AreaListModel> GetDistributorAreaByCluster([FromBody]AreaListModel areaListModel)
        {
            List<AreaListModel> modelList = new List<AreaListModel>();
            try
            {
                modelList = _unitOfWork.distributorRepository.GetDistributorAreaListByCluster(areaListModel.DistributorId, areaListModel.ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, areaListModel.DistributorId, 0, 0, "GetDistributorAreaByCluster", "ClusterId= " + areaListModel.ClusterId, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Co Ordinates Of Duplicate Address
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/SaveCoOrdinatesOfDuplicateAddress")]
        public int SaveCoOrdinatesOfDuplicateAddress([FromBody]SaveCoOrdinatesOfDuplicateAddress DuplicateRecords)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SaveCoOrdinatesOfDuplicateAddress(DuplicateRecords);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, DuplicateRecords.DistributorId, 0, 0, "SaveCoOrdinatesOfDuplicateAddress", "NoOfCount= " + DuplicateRecords.NoOfCount, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Get Counsumer Counts Source wise
        [HttpPost]
        [Route("GeoCoordinate/GetCounsumerCountsSourcewise")]
        public List<ConsumerSourcewise> GetCounsumerCountsSourcewise([FromBody]DistributorModel distributorModel)
        {
            List<ConsumerSourcewise> model = new List<ConsumerSourcewise>();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetCounsumerCountsSourcewise(distributorModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GR - Source wise Geo-Coordinate Status", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Cluster Area wise Counts
        [HttpPost]
        [Route("GeoCoordinate/GetClusterAreawiseCounts")]
        public List<ClusterAreawiseCounts> GetClusterAreawiseCounts([FromBody]AreaClusterModel clusterModal)
        {
            List<ClusterAreawiseCounts> modelList = new List<ClusterAreawiseCounts>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClusterAreawiseCounts(clusterModal.DistributorId, clusterModal.ClusterId, clusterModal.AreaRefNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterAreawiseCounts", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Counsumer List Source wise
        [HttpPost]
        [Route("GeoCoordinate/GetCounsumerListSourcewise")]
        public List<ConsumerMaster> GetCounsumerListSourcewise([FromBody]ConsumerMaster consumerMaster)
        {
            List<ConsumerMaster> modelList = new List<ConsumerMaster>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetConsumerListSourcewise(consumerMaster.DistributorID, consumerMaster.Source, consumerMaster.StaffRefNo, consumerMaster.SelDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerMaster.DistributorID, 0, 0, "GetCounsumerListSourcewise", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Counsumer List Source wise For Smart Map
        [HttpPost]
        [Route("GeoCoordinate/GetCounsumerListSourcewiseForSmartMap")]
        [AllowAnonymous]
        public GetTripConsumerDtls GetCounsumerListSourcewiseForSmartMap([FromBody]ConsumerMaster consumerMaster)
        {
            GetTripConsumerDtls model = new GetTripConsumerDtls();
            model.SaveLocationModel = new List<DelBoyLocation>();
            model.SmartConsumerlist = new List<SmartTripRoute>();
            try
            {
                model.SaveLocationModel = _unitOfWork._tripRepository.GetTripRoute(consumerMaster.DistributorID, consumerMaster.StaffRefNo, consumerMaster.TripDate);
                consumerMaster.DelBoyId = Convert.ToInt64(consumerMaster.StaffRefNo);
                model.SmartConsumerlist = _unitOfWork._tripRepository.GetSmartTripRoute(consumerMaster.DistributorID, consumerMaster.DelBoyId, consumerMaster.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerMaster.DistributorID, 0, 0, "GetCounsumerListSourcewiseForSmartMap", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Ezy Gas Cons Reverce CoOrdintes
        [HttpPost]
        [Route("GeoCoordinate/EzyGasConsReverceCoOrdintes")]
        public int EzyGasConsReverceCoOrdintes([FromBody]ConsumerMaster consumerMaster)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.EzyGasConsReverceCoOrdintes(consumerMaster.DistributorID, consumerMaster.Source, consumerMaster.RGeoFlag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, consumerMaster.DistributorID, 0, 0, "EzyGasConsReverceCoOrdintes", null, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Cons Distance From Agency
        [HttpPost]
        [Route("GeoCoordinate/ConsDistanceFromAgency")]
        public SourceDetails ConsDistanceFromAgency([FromBody]SourceDetails sourceDetails)
        {
            SourceDetails model = new SourceDetails();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.ConsDistanceFromAgency(sourceDetails.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, sourceDetails.DistributorId, 0, 0, "ConsDistanceFromAgency", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Find InSide OutSide Poly
        [HttpPost]
        [Route("GeoCoordinate/FindInSideOutSidePoly")]
        public void FindInSideOutSidePoly([FromBody] InOutPolyConsumer InOutPoly)
        {
            _unitOfWork.geoCoordinateRepository.FindInSideOutSidePoly(InOutPoly.DistributorId, InOutPoly.Code, InOutPoly.Flag);
        }
        #endregion

        #region GetPuneWardsCoordinates
        [HttpGet]
        [Route("GeoCoordinate/GetPuneWardsCoordinates")]
        public void GetPuneWardsCoordinates()
        {
            try
            {
                _unitOfWork.geoCoordinateRepository.GetPuneWardsCoordinates();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPuneWardsCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
        }
        #endregion

        #region Majaor Area Coordinates   
        [HttpPost]
        [Route("GeoCoordinate/GetLastMappedMajorArea")]
        public AreaMappingListModel GetLastMappedMajorArea([FromBody] AreaMappingListModel _areaMappingListModel)
        {
            AreaMappingListModel MajorAreaCoordinates = new AreaMappingListModel();
            try
            {
                MajorAreaCoordinates = _unitOfWork.geoCoordinateRepository.GetLastMappedMajorArea(_areaMappingListModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetLastMappedMajorArea", null, BusinessCont.FailStatus, ex.Message);
            }
            return MajorAreaCoordinates;
        }
        #endregion

        #region Get Staff Ref Delivery App
        [HttpPost]
        [Route("GeoCoordinate/GetStaffRefDeliveryApp")]
        public List<StaffRefNoDeliveryApp> GetStaffRefDeliveryApp([FromBody] StaffRefNoDeliveryApp StaffRefNoDeliveryApp)
        {
            List<StaffRefNoDeliveryApp> GetStaffRefNoDeliveryApp = new List<StaffRefNoDeliveryApp>();
            try
            {
                GetStaffRefNoDeliveryApp = _unitOfWork.geoCoordinateRepository.GetStaffRefDeliveryApp(StaffRefNoDeliveryApp.DistributorId, StaffRefNoDeliveryApp.Date);
                StaffRefNoDeliveryApp.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                StaffRefNoDeliveryApp.Status = BusinessCont.FailStatus;
                StaffRefNoDeliveryApp.ExMsg = ex.ToString();
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetStaffRefDeliveryApp", null, BusinessCont.FailStatus, ex.Message);
            }
            return GetStaffRefNoDeliveryApp;
        }
        #endregion

        #region Get Distributor Area Mapping For POD List
        [HttpPost]
        [Route("GeoCoordinate/GetDistributorAreaMappingForPODList")]
        public IHttpActionResult GetDistributorAreaMappingForPODList([FromBody]AreaMappingListForPODModel _areaMappingListModel)
        {
            AreaMappingForPODDetails _areaMappingDetails = new AreaMappingForPODDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListForPODModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetDistributorAreaMappingListForPOD(_areaMappingListModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, _areaMappingListModel.DistributorId, 0, 0, "GetDistributorAreaMappingForPODList", "DistributorId= " + _areaMappingListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }
        #endregion

        #region Save POD Delivery Location
        [HttpPost]
        [Route("GeoCoordinate/SavePODDeliveryLocation")]
        public long SavePODDeliveryLocation([FromBody]PODDeliveryDetails PODDeliveryDetailsDtls)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SavePODDeliveryDetails(PODDeliveryDetailsDtls);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SavePODDeliveryLocation", null, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Pickup Locations
        [HttpPost]
        [Route("GeoCoordinate/GetPickupLocations")]
        public List<ClusterPickupLocation> GetPickupLocations([FromBody] AreaMappingListModel _areaMappingListModel)
        {
            List<ClusterPickupLocation> PickupLocations = new List<ClusterPickupLocation>();
            try
            {
                PickupLocations = _unitOfWork.geoCoordinateRepository.GetClusterPickupLocations(_areaMappingListModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetPickupLocations", null, BusinessCont.FailStatus, ex.Message);
            }
            return PickupLocations;
        }
        #endregion

        #region Get Area Coordinates
        [HttpPost]
        [Route("GeoCoordinate/GetAreaCoordinates")]
        public List<AreaClusterCoordinates> GetAreaCoordinates([FromBody] AreaMappingListModel DistDtls)
        {
            List<AreaClusterCoordinates> modelList = new List<AreaClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetAreaCoordinates(DistDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistDtls.DistributorId, 0, 0, "GetAreaCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Generate Coordinates
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/GenerateCoordinates")]
        public int GenerateCoordinates([FromBody] AreaClusterCoordinates areaCoordinates)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.GenerateCoordinates(areaCoordinates.Code, areaCoordinates.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, Convert.ToInt32(areaCoordinates.Code), 0, 0, "GenerateCoordinates", "Code = " + areaCoordinates.Code + " & Flag = " + areaCoordinates.Flag, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Execute Scheduler
        [HttpGet]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/ExecuteScheduler")]
        public void ExecuteScheduler()
        {
            try
            {
                JobScheduler.DeleteJob();
                JobScheduler.Start();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "ExecuteScheduler", "GeoCoordinate/ExecuteScheduler", BusinessCont.FailStatus, ex.Message);
            }
        }
        #endregion

        #region Map My India Generate Coordinates
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/MapMyIndiaGenerateCoordinates")]
        public int MapMyIndiaGenerateCoordinates([FromBody] InOutPolyConsumer DistDtls)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.MapMyIndiaReverceGeoCoordinates(DistDtls.DistributorId, DistDtls.AreaRefNo.ToString());
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistDtls.DistributorId, 0, 0, "MapMyIndiaGenerateCoordinates", "GeoCoordinate/MapMyIndiaGenerateCoordinates DistributorId = " + DistDtls.DistributorId + "  & AreaRefNo = " + Convert.ToString(DistDtls.AreaRefNo), BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Update Inside Outside Flag
        [HttpPost]
        [Route("GeoCoordinate/UpdateInsideOutsideFlag")]
        public long UpdateInsideOutsideFlag([FromBody]InOutPolyConsumer DistDtls)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.UpdateInsideOutsideFlag(DistDtls.DistributorId, DistDtls.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistDtls.DistributorId, 0, 0, "UpdateInsideOutsideFlag", "GeoCoordinate/UpdateInsideOutsideFlag DistributorId = " + DistDtls.DistributorId + "  & Flag = " + Convert.ToString(DistDtls.Flag), BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Dist wise Update Inside Outside Flag
        [HttpPost]
        [Route("GeoCoordinate/DistwiseUpdateInsideOutsideFlag")]
        public int DistwiseUpdateInsideOutsideFlag([FromBody]InOutPolyConsumer DistDtls)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.DistwiseUpdateInsideOutsideFlag(DistDtls.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, DistDtls.DistributorId, 0, 0, "DistwiseUpdateInsideOutsideFlag", "GeoCoordinate/DistwiseUpdateInsideOutsideFlag Flag = " + Convert.ToString(DistDtls.Flag), BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Save Google API Hits History
        [HttpPost]
        [Route("GeoCoordinate/SaveGoogleAPIHitsHistory")]
        public long SaveGoogleAPIHitsHistory([FromBody]GoogleHitsCount Model)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork._tripRepository.AddGoogleAPIHitsHistory(Model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveGoogleAPIHitsHistory", "DistributorId= " + Model.DistributorId, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Consumer List For Verification
        [HttpPost]
        [Route("GeoCoordinate/GetConsumerListForVerification")]
        public ConsumerDetails GetConsumerListForVerification([FromBody]ConsumerMaster consumerDtls)
        {
            ConsumerDetails consumerDetails = new ConsumerDetails();
            consumerDetails.consumerMasters = new List<ConsumerMaster>();
            try
            {
                consumerDetails.consumerMasters = _unitOfWork.geoCoordinateRepository.GetConsumerListForVerification(consumerDtls.DistributorID, consumerDtls.AreaRefNo.ToString());
                consumerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                consumerDetails.Status = BusinessCont.FailStatus;
                consumerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerListForVerification", null, BusinessCont.FailStatus, ex.Message);
            }
            return consumerDetails;
        }
        #endregion

        #region Rpt Duplicate Address
        [HttpPost]
        [Route("GeoCoordinate/RptDuplicateAddress")]
        public List<RptDuplicateAddressData> RptDuplicateAddress()
        {
            List<RptDuplicateAddressData> modelList = new List<RptDuplicateAddressData>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.RptDuplicateAddress();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "RptDuplicateAddress", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Customer Details
        [HttpPost]
        [Route("GeoCoordinate/GetCustomerDetails")]
        public List<CustomerDetails> GetCustomerDetails(CustomerDetails ConsDetls)
        {
            List<CustomerDetails> modelList = new List<CustomerDetails>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetCustomerDetails(ConsDetls);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetCustomerDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Generate CoOrdinates For Comparison
        [HttpPost]
        [Route("GeoCoordinate/GenerateCoOrdinatesForComparison")]
        public void GenerateCoOrdinatesForComparison()
        {
            try
            {
                _unitOfWork.geoCoordinateRepository.GenerateCoOrdinatesForComparison();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GenerateCoOrdinatesForComparison", "GeoCoordinate/GenerateCoOrdinatesForComparison", BusinessCont.FailStatus, ex.Message);
            }
        }
        #endregion

        #region Find Shortest Route
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpGet]
        [Route("GeoCoordinate/FindShortestRoute")]
        public void FindShortestRoute()
        {
            try
            {
                _unitOfWork.geoCoordinateRepository.FindShortestRoute();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "FindShortestRoute", "GeoCoordinate/FindShortestRoute", BusinessCont.FailStatus, ex.Message);
            }
        }
        #endregion

        #region Find Consumer Sequence
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/FindConsumerSequence")]
        public List<CustomerDetails> FindConsumerSequence()
        {
            List<CustomerDetails> modelList = new List<CustomerDetails>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.FindConsumerSequence("", 0, "", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "FindShortestRoute", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Cluster Area Coordinates
        [HttpPost]
        [Route("GeoCoordinate/GetClusterAreaCoordinates")]
        public List<DistributorClusterCoordinates> GetClusterAreaCoordinates(DistributorClusterCoordinates DistDtls)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClusterAreaCoordinates(DistDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, DistDtls.DistributorId, 0, 0, "GetClusterAreaCoordinates", "GeoCoordinate/GetClusterAreaCoordinates", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Find Consumer Density
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/FindConsumerDensity")]
        public MapDensity FindConsumerDensity(MapDensity mapDensity)
        {
            MapDensity model = new MapDensity();
            model.customerDetails = new List<CustomerDetails>();
            model.consumerDensityList = new List<ConsumerDensityUpto>();
            model.Flag = mapDensity.Flag;
            try
            {
                if (mapDensity.Flag == "ConsumerDetails")
                {
                    model.customerDetails = _unitOfWork.geoCoordinateRepository.FindConsumerSequence(mapDensity.UniqueConsumerIdStr, mapDensity.ConsumerNo, mapDensity.recordFor, mapDensity.CntFlag);
                }
                else if (mapDensity.Flag == "Between")
                {
                    model.consumerDensityList = _unitOfWork.geoCoordinateRepository.GetConsumerDensityUpto();
                }
                else if (mapDensity.Flag == "Upto")
                {
                    model.consumerDensityList = _unitOfWork.geoCoordinateRepository.GetConsumerDensityBetween();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "FindShortestRoute", "GeoCoordinate/FindConsumerDensity : Flag = " + mapDensity.Flag, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Consumer Dtls Cluster Wise
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/GetConsumerDtlsClusterWise")]
        public List<ConsumerMaster> GetConsumerDtlsClusterWise(CustomerDetails ConsDetls)
        {
            List<ConsumerMaster> modelList = new List<ConsumerMaster>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetConsumerDtlsClusterWise(ConsDetls.DistributorID, ConsDetls.ClusterId, ConsDetls.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerDtlsClusterWise", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Update Area And Address
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/UpdateAreaAndAddress")]
        public dynamic UpdateAreaAndAddress(dynamic CDCMSUpdate)
        {
            try
            {
                return _unitOfWork._LocationRepository.UpdateCDCMSDetls(CDCMSUpdate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "UpdateAreaAndAddress", null, BusinessCont.FailStatus, ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Test Generate Coordinates
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/TestGenerateCoordinates")]
        public string TestGenerateCoordinates([FromBody] CustomerDetails CustDtls)
        {
            string msg = string.Empty;
            try
            {
                msg = _unitOfWork.geoCoordinateRepository.TestGenerateCoordinates(CustDtls.Address);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TestGenerateCoordinates", "Address= " + CustDtls.Address, BusinessCont.FailStatus, ex.Message);
            }
            return msg;
        }
        #endregion

        #region Test Distance API
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/TestDistanceAPI")]
        public dynamic TestDistanceAPI()
        {
            try
            {
                return _unitOfWork.geoCoordinateRepository.TestDistanceAPI();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TestDistanceAPI", null, BusinessCont.FailStatus, ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Test Direction API
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/TestDirectionAPI")]
        public dynamic TestDirectionAPI()
        {
            try
            {
                return _unitOfWork.geoCoordinateRepository.TestDirectionAPI();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "TestDirectionAPI", null, BusinessCont.FailStatus, ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Cluster Area Fencing Add
        [HttpPost]
        [Route("GeoCoordinate/ClusterAreaFencingAdd")]
        public long ClusterAreaFencingAdd([FromBody]ClusterAreaFencing cluster)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.AddClusterAreaFencing(cluster.DistributorId, cluster.ClusterId, cluster.ClusterJSON);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, cluster.DistributorId, 0, 0, "ReverceGeoCoordinates", "ClusterId = " + cluster.ClusterId, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Cluster Area Selection Fencing Add
        [HttpPost]
        [Route("GeoCoordinate/ClusterAreaSelectionFencingAdd")]
        public long ClusterAreaSelectionFencingAdd([FromBody]ClusterAreaFencing cluster)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.AddClusterSelectionFencing(cluster.DistributorId, cluster.ClusterId, cluster.MajorArea, cluster.ClusterJSON);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, cluster.DistributorId, 0, 0, "ReverceGeoCoordinates", "ClusterId = " + cluster.ClusterId, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Cluster Area Coordinates New
        [HttpPost]
        [Route("GeoCoordinate/GetClusterAreaCoordinatesNew")]
        public List<DistributorClusterCoordinates> GetClusterAreaCoordinatesNew(DistributorClusterCoordinates DistDtls)
        {
            List<DistributorClusterCoordinates> modelList = new List<DistributorClusterCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClusterAreaCoordinatesNew(DistDtls.DistributorId, DistDtls.AreaClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterAreaCoordinatesNew", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Trading Area Selection Fencing Add
        [HttpPost]
        [Route("GeoCoordinate/TradingAreaSelectionFencingAdd")]
        public long TradingAreaSelectionFencingAdd([FromBody]ClusterAreaFencing cluster)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.AddTradingSelectionFencing(cluster.DistributorId, cluster.MajorArea, cluster.ClusterJSON);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, cluster.DistributorId, 0, 0, "ReverceGeoCoordinates", "ClusterId = " + cluster.ClusterId, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Trading Area Coordinates New
        [HttpPost]
        [Route("GeoCoordinate/GetTradingAreaCoordinatesNew")]
        public DistributorClusterCoordinates GetTradingAreaCoordinatesNew(DistributorClusterCoordinates DistDtls)
        {
            DistributorClusterCoordinates model = new DistributorClusterCoordinates();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetTradingAreaCoordinatesNew(DistDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, DistDtls.DistributorId, 0, 0, "GetClusterAreaCoordinatesNew", "GeoCoordinate/GetTradingAreaCoordinatesNew", BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Add Updated Area
        [HttpPost]
        [Route("GeoCoordinate/AddUpdatedArea")]
        public long AddUpdatedArea([FromBody]UpdatedArea area)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.AddUpdatedArea(area);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, area.DistributorId, 0, 0, "AddUpdatedArea", "AreaCode = " + area.MajorAreaCode, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Get Updated Area Coordinates
        [HttpPost]
        [Route("GeoCoordinate/GetUpdatedAreaCoordinates")]
        public List<AreaGeoCoordinates> GetUpdatedAreaCoordinates(AreaGeoCoordinates DistDtls)
        {
            List<AreaGeoCoordinates> modelList = new List<AreaGeoCoordinates>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetUpdatedAreaCoordinates(DistDtls.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetUpdatedAreaCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Add Edit Cluster Fencing
        [HttpPost]
        [Route("GeoCoordinate/AddEditClusterFencing")]
        public IHttpActionResult AddClusterFencing([FromBody]AreaClusterModel ClusterFencing)
        {
            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            try
            {
                _areaClusterDetailsModel.AreaClusterId = _unitOfWork.geoCoordinateRepository.DistributorClusterCoordinatesAddEdit(ClusterFencing);
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddEditClusterFencing", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Add New Area with Geo Coordinates
        [HttpPost]
        [Route("GeoCoordinate/AddNewAreawithGeoCoordinates")]
        public long AddNewAreawithGeoCoordinates([FromBody]NewAreaModel area)
        {
            long retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.AddNewAreawithGeoCoordinates(area);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddNewAreawithGeoCoordinates", "TalukaCode = " + area.TalukaCode + ", AreaName = " + area.AreaName, BusinessCont.FailStatus, ex.Message);
                retValue = -1;
            }
            return retValue;
        }
        #endregion

        #region Density Wise Trip Planning
        [HttpPost]
        [Route("GeoCoordinate/DensityWiseTripPlanning")]
        public int DensityWiseTripPlanning([FromBody] UnsavedConsumers tripPlanning)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.DensityWiseTripPlanning(tripPlanning.DistributorId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DensityWiseTripPlanning", null, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Density Group Wise Trip Planning
        [HttpPost]
        [Route("GeoCoordinate/DensityGroupWiseTripPlanning")]
        public int DensityGroupWiseTripPlanning([FromBody] UnsavedConsumers tripPlanning)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.DensityGroupWiseTripPlanning(tripPlanning.DistributorId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DensityGroupWiseTripPlanning", null, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Density Wise Trip Planning Card Data
        [HttpPost]
        [Route("GeoCoordinate/DensityWiseTripPlanningCardData")]
        public List<DensitywiseTrip> DensityWiseTripPlanningCardData([FromBody] UnsavedConsumers tripPlanning)
        {
            List<DensitywiseTrip> modelList = new List<DensitywiseTrip>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetDensityWiseTrips(tripPlanning.DistributorId, tripPlanning.TripDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DensityWiseTripPlanningCardData", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Area Taluka District Master
        [HttpPost]
        [Route("GeoCoordinate/GetAreaTalukaDistrictMaster")]
        public List<AreaTalukaDistrictMaster> GetAreaTalukaDistrictMaster(AreaTalukaDistrictMaster master)
        {
            List<AreaTalukaDistrictMaster> modelList = new List<AreaTalukaDistrictMaster>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetAreaTalukaDistrictMaster(master.Code, master.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAreaTalukaDistrictMaster", "Code= " + master.Code + ", Flag= " + master.Flag, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Density Data Cluster wise For Map
        [HttpPost]
        [Route("GeoCoordinate/GetDensityDataClusterwiseForMap")]
        public ConsumerWiseDensity GetDensityDataClusterwiseForMap([FromBody]ConsumerWiseDensity postModel)
        {
            ConsumerWiseDensity model = new ConsumerWiseDensity();
            model.densityData = new List<DensityDataClusterwise>();
            model.Status = BusinessCont.FailStatus;
            string inputData = "";
            try
            {
                inputData = JsonConvert.SerializeObject(postModel);
                if (!string.IsNullOrEmpty(postModel.TripDate) && postModel.ClusterId > 0)
                {
                    DateTime tripDate = BusinessCont.StrConvertIntoDatetime(postModel.TripDate);
                    model.densityData = _unitOfWork.geoCoordinateRepository.GetDensityDataClusterwiseForMap(postModel.DistributorId, postModel.ClusterId, tripDate);
                    model.Status = BusinessCont.SuccessStatus;
                }
                else
                {
                    BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDensityDataClusterwiseForMap", "inputData= " + inputData, BusinessCont.FailStatus, "");
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDensityDataClusterwiseForMap", "inputData= " + inputData, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Email Send
        [HttpPost]
        [Route("GeoCoordinate/GetEmailSend")]
        public int GetEmailSend()
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.ExecuteEmailScheduler();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetEmailSend", null, BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Get Area List By State
        [HttpPost]
        [Route("GeoCoordinate/GetAreaListByState")]
        public IHttpActionResult GetAreaListByState([FromBody]DistTalukaModel distTalukaModel)
        {
            AreaMappingDetails _areaMappingDetails = new AreaMappingDetails();
            _areaMappingDetails.areaMappingList = new List<AreaMappingListModel>();
            try
            {
                _areaMappingDetails.areaMappingList = _unitOfWork.geoCoordinateRepository.GetAreaListByState(distTalukaModel);
                _areaMappingDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaMappingDetails.Status = BusinessCont.FailStatus;
                _areaMappingDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 100, distTalukaModel.DistributorId, 0, 0, "GetAreaListForMapping", "DistrictCode = " + distTalukaModel.DistrictCode + ", TalukaCode = " + distTalukaModel.TalukaCode, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaMappingDetails);
        }
        #endregion

        #region Get Onboarding Stage2 For Email Scheduler ACL
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetOnboardingStage2ForEmailSchedulerACL")]
        public List<OnboardingDetails> GetOnboardingStage2ForEmailSchedulerACL([FromBody] OnboardingDetails ordDtls)
        {
            List<OnboardingDetails> modelList = new List<OnboardingDetails>();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                modelList = _unitOfWork.geoCoordinateRepository.GetOnboardingStage2ForEmailSchedulerACL(ordDtls.DistributorId.ToString(), AppConfigAPITimeout);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetOnboardingStage2ForEmailSchedulerACL", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Onboarding Stage2 For Lowest InComplete Step Count Email Scheduler ACL
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetOnboardingStage2ForLowestInCompleteStepCountEmailSchedulerACL")]
        public List<OnboardingDetailsLowestCount> GetOnboardingStage2ForLowestInCompleteStepCountEmailSchedulerACL([FromBody] OnboardingDetailsLowestCount ordDtls)
        {
            List<OnboardingDetailsLowestCount> modelList = new List<OnboardingDetailsLowestCount>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetOnboardingStage2ForLowestInCompleteStepCountEmailSchedulerACL(Convert.ToString(ordDtls.DistributorId));
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, Convert.ToInt32(ordDtls.DistributorId), 0, 0, "GetOnboardingStage2ForLowestInCompleteStepCountEmailSchedulerACL", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Missing Area
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/SaveMissingArea")]
        public int SaveMissingArea(AreaDetailsForMissingArea areaDetailsForMissingArea)
        {
            int retValue = 0;
            try
            {
                retValue = _unitOfWork.geoCoordinateRepository.SaveMissingArea(areaDetailsForMissingArea);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 100, 0, 0, 0, "SaveMissingArea", "", BusinessCont.FailStatus, ex.Message);
            }
            return retValue;
        }
        #endregion

        #region Get State wise Summary
        [HttpPost]
        [Route("GeoCoordinate/GetStatewiseSummary")]
        public IHttpActionResult GetStatewiseSummary()
        {
            List<StatewiseSummary> statewiseSummry = new List<StatewiseSummary>();
            try
            {
                statewiseSummry = _unitOfWork.geoCoordinateRepository.GetStatewiseAreaSummary();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 100, 0, 0, 0, "GetStatewiseSummary", "", BusinessCont.FailStatus, ex.Message);
            }
            return Ok(statewiseSummry);
        }
        #endregion

        #region Get Email Scheduler For Stage Completed
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpPost]
        [Route("GeoCoordinate/GetEmailSchedulerForStageCompleted")]
        public int GetEmailSchedulerForStageCompleted([FromBody] OnboardingDetails ordDtls)
        {
            int result = 0;
            try
            {
                result = _unitOfWork.geoCoordinateRepository.EmailSchedulerForSAPendingApproval(ordDtls);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetEmailSchedulerForStageCompleted", null, BusinessCont.FailStatus, ex.Message);
            }
            return result;
        }
        #endregion

        #region Get Consumer GC Sequnce No List
        [HttpPost]
        [Route("GeoCoordinate/GetConsumerGCSequnceNoList")]
        public IHttpActionResult GetConsumerGCSequnceNoList([FromBody]ConsumerGCSequence consumerGCSequence)
        {
            List<ConsumerGCSequence> consumerGcSequences = new List<ConsumerGCSequence>();
            try
            {
                consumerGcSequences = _unitOfWork.geoCoordinateRepository.GetConsumerGCSequnceNoList(consumerGCSequence);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 100, 0, 0, 0, "GetConsumerGCSequnceNoList", "", BusinessCont.FailStatus, ex.Message);
            }
            return Ok(consumerGcSequences);
        }
        #endregion

        #region Save GC Sequnce No List
        [HttpPost]
        [Route("GeoCoordinate/SaveGCSequnceNoList")]
        public IHttpActionResult SaveGCSequnceNoList([FromBody]PostModelForTripSequnce postModelForTripSequnce)
        {
            PostModelForTripSequnce model = new PostModelForTripSequnce();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.SaveGCSequnceNoList(postModelForTripSequnce);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 100, 0, 0, 0, "SaveGCSequnceNoList", "", BusinessCont.FailStatus, ex.Message);
            }
            return Ok(model);
        }
        #endregion

        #region Save Geo coding Location  
        [HttpPost]
        [Route("GeoCoordinate/SaveGeocodingLocation")]
        public long SaveGeocodingLocation([FromBody]DistributorDetails distributorDtls)
        {
            long ReturnId = 0;
            try
            {
                if (distributorDtls.ConsumerGeocodingList.Count > 0)
                {
                    for (int i = 0; i < distributorDtls.ConsumerGeocodingList.Count; i++)
                    {
                        ReturnId = _unitOfWork.geoCoordinateRepository.GenerateCoordinates(Convert.ToString(distributorDtls.ConsumerGeocodingList[i].DistributorId), "DIST");
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveGeocodingLocation", "Save consumers location Geocoding - " + (ex.Message), BusinessCont.FailStatus, ex.Message);
                ReturnId = -1;
            }
            return ReturnId;
        }
        #endregion

        #region Generate Geo Cordinate By Place
        [HttpPost]
        [Route("GeoCoordinate/GenerateGeoCordinateByPlace")]
        public PlaceDetails GetPlaceGeoCordinates([FromBody] PlaceDetails areaCoordinates)
        {
            PlaceDetails model = new PlaceDetails();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GenerateGeoCordinatesfromString(areaCoordinates.strSearchString, areaCoordinates.intDistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "GenerateCoordinates", "SearchString=" + areaCoordinates.strSearchString + ", DistributorId=" + areaCoordinates.intDistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Unmapped User Count
        [HttpPost]
        [Route("GeoCoordinate/GetUnmappedUserCount")]
        public UnmappedCustomerCount GetUnmappedUserCount([FromBody] UnmappedCustomerCount unmapped)
        {
            UnmappedCustomerCount model = new UnmappedCustomerCount();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetUnmappedUserCount(unmapped);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "GetUnmappedUserCount", "DistributorId=" + unmapped.DistributorId + ", ConsumerNo=" + unmapped.ConsumerNo, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Unmapped User Details
        [HttpPost]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetUnmappedUserDetails")]
        public List<UnmappedCustomerCount> GetUnmappedUserDetails([FromBody] UnmappedCustomerCount unmapped)
        {
            List<UnmappedCustomerCount> modelList = new List<UnmappedCustomerCount>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetUnmappedUserDetails(unmapped);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "GetUnmappedUserDetails", "DistributorId=" + unmapped.DistributorId + ", ConsumerNo=" + unmapped.ConsumerNo, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Check Inside Or Outside Consumers
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/CheckInsideOrOutsideConsumers")]
        public int CheckInsideOrOutsideConsumersDetails([FromBody] AreaClusterCoordinates areaCoordinates)
        {
            int result = 0;
            try
            {
                result = _unitOfWork.geoCoordinateRepository.CheckInsideOrOutsideConsumers(areaCoordinates.DistributorID, Convert.ToInt32(areaCoordinates.ClusterId));
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "CheckInsideOrOutsideConsumersDetails", "DistributorID=" + areaCoordinates.DistributorID, BusinessCont.FailStatus, ex.Message);
            }
            return result;
        }
        #endregion

        #region Check Inside Or Outside Consumers Count
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("GeoCoordinate/CheckInsideOrOutsideConsumersCount")]
        public InsideOutsideConsumer CheckInsideOrOutsideConsumersCount(InsideOutsideConsumer consumers)
        {
            InsideOutsideConsumer objIn = new InsideOutsideConsumer();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                objIn = _unitOfWork.geoCoordinateRepository.GetInOutConsCount(consumers.DistributorID, consumers.ClusterID, AppConfigAPITimeout);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "CheckInsideOrOutsideConsumersCount", "DistributorId=" + consumers.DistributorID + ", ClusterId=" + consumers.ClusterID, BusinessCont.FailStatus, ex.Message);
            }
            return objIn;
        }
        #endregion

        #region Get Distributor InCluster Consumer Counts Details
        [HttpPost]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetDistributorInClusterConsumerCountsDetails")]
        public List<InclustetCount> GetDistributorInClusterConsumerCountsDetails(InclustetCount inclustetCount)
        {
            List<InclustetCount> model = new List<InclustetCount>();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetDistributorInClusterConsumerCounts(inclustetCount.SACode, inclustetCount.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "GetDistributorInClusterConsumerCountsDetails", "SACode=" + inclustetCount.SACode + ", DistributorId=" + inclustetCount.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Get Distributor InCluster Consumers
        [HttpPost]
        [AllowAnonymous]
        [Route("GeoCoordinate/GetDistributorInClusterConsumers")]
        public long GetDistributorInClusterConsumers(GetDistributorClusterList model)
        {
            long ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.SaveDistributoAndClusterId(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, model.DistributorID, 0, 0, "GetDistributorInClusterConsumers", null, BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion

        #region Get Consumer Vitran Lat Long
        [HttpGet]
        [Route("GeoCoordinate/GetConsumerVitranLatLong/{UniqueConsumerId}/{DistributorId}")]
        public ConsumerLatLongModel GetConsumerVitranLatLong(decimal UniqueConsumerId, int DistributorId)
        {
            ConsumerLatLongModel model = new ConsumerLatLongModel();
            try
            {
                model = _unitOfWork.geoCoordinateRepository.GetConsumerVitranLatLong(UniqueConsumerId, DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, DistributorId, 0, 0, "GetConsumerVitranLatLong", null, BusinessCont.FailStatus, ex.Message);
            }
            return model;
        }
        #endregion

        #region Testing Check InOut Consumer Flag
        [HttpPost]
        [AllowAnonymous]
        [Route("GeoCoordinate/TestingCheckInOutConsumerFlag")]
        public AreaClusterCoordinates TestingCheckInOutConsumerFlag([FromBody] TestInOutModel model)
        {
            AreaClusterCoordinates InOutFlag = new AreaClusterCoordinates();
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    GeoCoordinateRepository geoCoordinateRepository = new GeoCoordinateRepository(_contextManager);
                    var CoordinatesDetls = geoCoordinateRepository.GetAreaCoordinates(model.DId);
                    InOutFlag = _unitOfWork.geoCoordinateRepository.CheckConsumerInsideOrOutSideCluster(CoordinatesDetls, model.Latitude, model.Longitude, model.AreaRefNo, model.ClusterId);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "TestingCheckInOutConsumerFlag", null, BusinessCont.FailStatus, ex.Message);
            }
            return InOutFlag;
        }
        #endregion

        #region Save Consumer Location History logsS
        [HttpPost]
        [Route("GeoCoordinate/SaveConsumerLocationHistorylog")]
        public int SaveConsumerLocationHistorylog(ConsumerLocationModel model)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.SaveConsumerLocationHistorylog(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, model.DistributorID, 0, 0, "SaveConsumerLocationHistorylog", null, BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion

        #region Start - FindInsideOutsideScheduler specific dealer
        [HttpGet]
        [Route("GeoCoordinate/GetFindInsideOutsideScheduler/{DistributorId}")]
        [AllowAnonymous]
        public int GetFindInsideOutsideScheduler(int DistributorId)
        {
            int IsCluster = 0;
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    GeoCoordinateRepository geoCoordinateRepository = new GeoCoordinateRepository(_contextManager);
                    IsCluster = geoCoordinateRepository.UpdateInsideOutsideFlagNew(DistributorId);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, DistributorId, 0, 0, "API - GetFindInsideOutsideScheduler", "IsCluster = " + IsCluster + "API - GetFindInsideOutsideScheduler -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return IsCluster;
        }
        #endregion End - FindInsideOutsideScheduler specific dealer

        #region Start - Save Invalid Cluster Json
        [HttpPost]
        [Route("GeoCoordinate/SaveInvalidClusterJson")]
        [AllowAnonymous]
        public bool SaveInvalidClusterJson()
        {
            bool IsCluster = false;
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    GeoCoordinateRepository geoCoordinateRepository = new GeoCoordinateRepository(_contextManager);
                    IsCluster = geoCoordinateRepository.SaveInvalidClusterJson();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - SaveInvalidClusterJson", "IsCluster = " + IsCluster + "API - SaveInvalidClusterJson -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return IsCluster;
        }
        #endregion End - Save Invalid Cluster Json

        #region Start - Update GC Of Distributor
        [HttpGet]
        [Route("GeoCoordinate/UpdateGCDistributorWise/{DistributorId}")]
        [AllowAnonymous]
        public long UpdateGCDistributorWise(int DistributorId)
        {
            long ResultId = 0;
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    GeoCoordinateRepository geoCoordinateRepository = new GeoCoordinateRepository(_contextManager);
                    ResultId = _unitOfWork.geoCoordinateRepository.GetGCWiseConsumer_New(DistributorId);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - UpdateGCDistributorWise", "DistributorId = " + DistributorId + "API - UpdateGCOfDistributor -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return ResultId;
        }
        #endregion

        #region Start - Update Latest GC Of Distributor
        [HttpGet]
        [Route("GeoCoordinate/UpdateLatestGCDistributorWise/{DistributorId}")]
        [AllowAnonymous]
        public long UpdateLatestGCDistributorWise(int DistributorId)
        {
            long ResultId = 0;
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    GeoCoordinateRepository geoCoordinateRepository = new GeoCoordinateRepository(_contextManager);
                    ResultId = _unitOfWork.geoCoordinateRepository.CheckLatestGCInsOutNew(DistributorId);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - UpdateLatestGCDistributorWise", "DistributorId = " + DistributorId + "API - UpdateGCOfDistributor -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return ResultId;
        }
        #endregion

        #region Start - Update Second GC Data In Bulk
        [HttpPost]
        [Route("GeoCoordinate/UpdateSecGCDataInBulk/{DistributorId}")]
        public int UpdateSecGCDataInBulk(int DistributorId)
        {
            int ResultId = 0;
            try
            {
                using (SDSDBEntities _contextManager = new SDSDBEntities())
                {
                    ResultId = _unitOfWork.geoCoordinateRepository.UpdateSecGCDataInBulk(DistributorId);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - UpdateSecGCDataInBulk", "DistributorId = " +DistributorId + "API - UpdateSecGCDataInBulk -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return ResultId;
        }
        #endregion

        #region Start - Raise Request For Edit Clusters
        [HttpPost]
        [Route("GeoCoordinate/RaiseRequestForEditCluster")]
        public int RaiseRequestForEditCluster([FromBody] RaiseRequestForCluster model)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.RaiseRequestForEditCluster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - RaiseRequestForEditCluster", "DistributorId = " + model.DistributorId + "API - RaiseRequestForEditCluster -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return ResultId;
        }
        #endregion

        #region Start - Get Transfer Booking By Id
        [HttpGet]
        [Route("GeoCoordinate/GetTransferBookingById/{DistributorId}")]
        public List<GetTransferBookingByIdModel> GetTransferBookingById(int DistributorId)
        {
            List<GetTransferBookingByIdModel> modelList = new List<GetTransferBookingByIdModel>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetTransferBookingById(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - GetTransferBookingById", "DistributorId = " + DistributorId + "API - GetTransferBookingById -> Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion End - Get Transfer Booking By Id

        #region Start - Transfer Booking Add New
        [HttpPost]
        [Route("GeoCoordinate/TransferBookingAddNew")]
        public long TransferBookingAddNew([FromBody] TransferBookingAddModel model)
        {
            long RetValue = 0;
            try
            {
                RetValue = _unitOfWork.geoCoordinateRepository.TransferBookingAddNew(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, Convert.ToInt32(model.SourceDistributorId), 0, 0, 0, "API - TransferBookingAddNew", "DistributorId = " + model.DestDistributorId + "API - TransferBookingAddNew -> Exception = " + (ex.Message), BusinessCont.FailStatus, ex.Message);
            }
            return RetValue;
        }
        #endregion End - Transfer Booking AddNew

        #region Start - Check Transfer Booking Consumer Details
        [HttpGet]
        [Route("GeoCoordinate/GetTransferBookingConsumerDetails/{FromDistributorId}/{ToDistributorId}/{ClusterId}")]
        public int GetTransferBookingConsumerDetails(int FromDistributorId, int ToDistributorId, long ClusterId)
        {
            int RetValue = 0; 
            try
            {
                RetValue = _unitOfWork.geoCoordinateRepository.GetTransferBookingConsumerDetails(FromDistributorId, ToDistributorId, ClusterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, FromDistributorId, 0, 0, "API - GetTransferBookingConsumerDetails", "ToDistributorId = " + ToDistributorId + " & API - GetTransferBookingConsumerDetails -> Exception = " + ex.Message, BusinessCont.FailStatus, ex.Message);
            }
            return RetValue;
        }
        #endregion End - Check Transfer Booking Consumer Details

        #region Start - Get Raise Request For Edit Clusters
        [HttpGet]
        [Route("GeoCoordinate/GetRaiseRequest/{DistributorId}/{ClusterId}/{FlagFor}")]
        public List<GetRaiseRequestEdit> GetRaiseRequest(int DistributorId, int ClusterId, string FlagFor)
        {
            List<GetRaiseRequestEdit> modelList = new List<GetRaiseRequestEdit>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetRaiseRequest(DistributorId, ClusterId, FlagFor);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - GetRaiseRequest", "DistributorId = " + DistributorId + "API - GetRaiseRequest -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return modelList;
        }
        #endregion

        #region Get Overlapping Consumer List
        [HttpPost]
        [Route("GeoCoordinate/GetOverlappingConsumerList")]
        public List<OverlapConsumerModel> GetOverlappingConsumerList([FromBody]OverlapConsumerModel Model)
        {
            List<OverlapConsumerModel> modelList = new List<OverlapConsumerModel>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetOverlappingConsumerList(Model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOverlappingConsumerList", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Trf Consumer Lst For Map
        [HttpPost]
        [Route("GeoCoordinate/GetTrfConsumerLstForMap")]
        public List<OverlapConsumerModel> GetTrfConsumerLstForMap([FromBody]OverlapConsumerModel Model)
        {
            List<OverlapConsumerModel> modelList = new List<OverlapConsumerModel>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetTrfConsumerLstForMap(Model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetTrfConsumerLstForMap", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Backlog Calculation Distributor wise --Testing
        [HttpGet]
        [Route("GeoCoordinate/BacklogCalculationDistributorwise/{DistributorId}")]
        [AllowAnonymous]
        public int BacklogCalculationDistributorwise(int DistributorId)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.BacklogCalculationDistributorwise(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, DistributorId, 0, 0, "BacklogCalculationDistributorwise", null, BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion

        #region Check Consumer Transferred Or Not --Testing
        [HttpGet]
        [Route("GeoCoordinate/CheckConsumerTransferredOrNot/{DistributorId}")]
        [AllowAnonymous]
        public int CheckConsumerTransferredOrNot(int DistributorId)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.CheckConsumerTransferredOrNot(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, DistributorId, 0, 0, "CheckConsumerTransferredOrNot", null, BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion

        #region Get Updated Stock Of Distributor
        [HttpGet]
        [Route("GeoCoordinate/GetUpdatedStockOfDistributor/{DistributorId}")]
        public int GetUpdatedStockOfDistributor(int DistributorId)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.DistributorStockUpdateDCMS(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, DistributorId, 0, 0, "GetUpdatedStockOfDistributor", null, BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion

        #region Start - Get Scheduler Status Summary Distributor wise -- Testing
        [HttpGet]
        [Route("GeoCoordinate/GetDailyDistSchedulerCheck")]
        [AllowAnonymous]
        public int GetDailyDistSchedulerCheck()
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.GetDailyDistSchedulerCheck();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 100, 0, 0, 0, "GetDailyDistSchedulerCheck", "Get Scheduler Status Summary Distributor wise", BusinessCont.FailStatus, ex.Message);
            }
            return ResultId;
        }
        #endregion End - Get Scheduler Status Summary Distributor wise -- Testing

        #region Get Clusters In Out Cnt With Mapp Area List
        [HttpPost]
        [Route("GeoCoordinate/GetClustersInOutCntWithMappAreaList")]
        public List<ClusterInOutCntModel> GetClustersInOutCntWithMappAreaList([FromBody]ClusterInOutCntModel model)
        {
            List<ClusterInOutCntModel> modelList = new List<ClusterInOutCntModel>();
            try
            {
                modelList = _unitOfWork.geoCoordinateRepository.GetClustersInOutCntWithMappAreaList(model.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClustersInOutCntWithMappAreaList", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Start - Change Cluster Area
        [HttpPost]
        [Route("GeoCoordinate/ChangeClusterAreas")]
        public int ChangeClusterAreas([FromBody] ChangeClusterAreasModel model)
        {
            int ResultId = 0;
            try
            {
                ResultId = _unitOfWork.geoCoordinateRepository.ChangeClusterAreas(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "API - ChangeClusterAreas", "DistributorId = " + model.DistributorId + "API - RaiseRequestForEditCluster -> Exception = " + (ex.Message), BusinessCont.FailStatus, "Fail");
            }
            return ResultId;
        }
        #endregion

        // TestInOutModel
        public class TestInOutModel
        {
            public int DId { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string AreaRefNo { get; set; }
            public int ClusterId { get; set; }
        }

    }
}
