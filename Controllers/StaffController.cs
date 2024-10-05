using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Godown;
using Aadyam.SDS.Business.Model.Order;
using Aadyam.SDS.Business.Model.Staff;
using Aadyam.SDS.Business.Model.User;
using Aadyam.SDS.Business.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class StaffController : BaseApiController
    {
        decimal LogId = 0;

        #region Delivery Boy
        /// <summary>
        /// Return list of delivery boy and vehicles against destributor Id
        /// </summary>
        /// <param name="DistId">return list against destributor id</param>
        /// <param name="DeliveryBoyId">Check given delivery boy is not return in delivery boy list</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Staff/DeliveryBoyConfiguration")]
        public DistStaffDetailsList DeliveryBoyConfiguration([FromBody]StaffPostConfiguration staffConfiguration)
        {
            DistStaffDetailsList DistStaffDtls = new DistStaffDetailsList();
            DistStaffDtls.deliveryBoyVehicle = new DeliveryBoyVehicle();
            DistStaffDtls.DrawArea = false;
            DistStaffDtls.DeliveryBoyDetails = new List<DistDbList>();
            DistStaffDtls.CylindersDetails = new List<DistributorItems>();
            DistStaffDtls.VehicleDetails = new List<VehicleModel>();
            DistStaffDtls.deliveryBoyVehicle = new DeliveryBoyVehicle();
            List<DistStaff> objDistributorStaffModel = new List<DistStaff>();
            try
            {
                DistStaffDtls.DeliveryBoyDetails = _unitOfWork.staffRepository.GetDistributorDbLst(staffConfiguration.DistId, "ALL");

                _unitOfWork.loginRepository.ActiveUser(staffConfiguration.DistId, staffConfiguration.StaffRefNo, BusinessCont.UserTypeDeliveryBoy, staffConfiguration.MobileNo, staffConfiguration.VersionNo,staffConfiguration.DeviceId);

               DistStaffDtls.PlayStoreVersion = _unitOfWork.loginRepository.GetCurrentVersion(staffConfiguration.ApplicationName);
                DistStaffDtls.MobileLogs = _unitOfWork.loginRepository.GetMobileLogs();
                
               DistStaffDtls.CylindersDetails = new List<DistributorItems>();
               DistStaffDtls.CylindersDetails = _unitOfWork.distributorRepository.GetDistributorItems(staffConfiguration.DistId);
               DistStaffDtls.VehicleDetails = _unitOfWork.vehicleRepository.GetVehicleData(staffConfiguration.DistId,0,BusinessCont.StatusYes);
               DistStaffDtls.deliveryBoyVehicle = _unitOfWork.vehicleRepository.GetVehicleMapDtls(0, staffConfiguration.StaffRefNo, staffConfiguration.DistId, "").Select(x=> new DeliveryBoyVehicle() {
                   VehicleId=x.VehicleId,
                   VehicleNo=x.VehicleNo
               }).FirstOrDefault();
                if (DistStaffDtls.deliveryBoyVehicle == null)
                    DistStaffDtls.deliveryBoyVehicle = new DeliveryBoyVehicle();

                DistStaffDtls.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                DistStaffDtls.Status = BusinessCont.FailStatus;
                DistStaffDtls.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId,0, staffConfiguration.DistId, 0, staffConfiguration.StaffRefNo, "DeliveryBoy Configuration", "DistId= " + staffConfiguration.DistId + ", DeliveryBoyId=" + staffConfiguration.StaffRefNo + ", MobileNo=" + staffConfiguration.MobileNo + ", VersionNo=" + staffConfiguration.VersionNo, BusinessCont.FailStatus, ex.Message);
            }
            return DistStaffDtls;
        }

        /// <summary>
        /// Todays delivered order count,Pending orders, current stock, Transfer stock, sale and opening stock against delivery boy will be return through this method 
        /// </summary>
        /// <param name="DistributorId"></param>
        /// <param name="StaffRefNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Staff/DeliveryBoyDashboard")]
        public DeliveryBoyStockDetails DeliveryBoyDashboard([FromBody]StaffPostConfiguration staffConfiguration)
        {
            DeliveryBoyStockDetails deliveryBoyStockDetails = new DeliveryBoyStockDetails();
            deliveryBoyStockDetails.StockDetails = new List<DeliveryBoyStock>();
            deliveryBoyStockDetails.DeliveryCnt = new DBoyDeliveryCnt();
            try
            {
                DateTime? SelDate = null;
                if (!string.IsNullOrEmpty(staffConfiguration.SelDate))
                    SelDate = BusinessCont.StrConvertIntoDatetime(staffConfiguration.SelDate);

                deliveryBoyStockDetails.StockDetails = _unitOfWork.stockRepository.getCurrentStockOfDeliveryBoy(staffConfiguration.StaffRefNo);
                deliveryBoyStockDetails.DeliveryCnt = _unitOfWork.orderRepository.OrderDeliveredCntDBoyWise(staffConfiguration.DistId,staffConfiguration.StaffRefNo, SelDate);

                var AppConfig = BusinessCont.GetAppConfiguration();
                if (AppConfig != null)
                {
                    deliveryBoyStockDetails.showCountOnMarker = AppConfig.Where(a => a.Key == BusinessCont.AppConfigShowCountOnMarker).Select(a => a.Value).FirstOrDefault();
                }
                deliveryBoyStockDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                deliveryBoyStockDetails.Status = BusinessCont.FailStatus;
                deliveryBoyStockDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId,0, 0, 0, staffConfiguration.StaffRefNo, "DeliveryBoy Dashboard", null, deliveryBoyStockDetails.Status, ex.Message);
            }
            return deliveryBoyStockDetails;
        }
        #endregion

        #region Godown Keeper
        /// <summary>
        /// Return godown keeper required data which is used in all application ie Delivery boy list, Vechicle No. list
        /// and dashboard stock details.
        /// </summary>
        /// <param name="DistId">Delivery boy and vehicle no list against distributor</param>
        /// <param name="GodownKeeperId">Dashboard stock details against godown keeper</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Staff/GodownKeeperConfiguration")]
        public GodownKeeperConfigList GodownKeeperConfiguration([FromBody]StaffPostConfiguration staffConfiguration)
        {
            GodownKeeperConfigList godownKeeperConfigList = new GodownKeeperConfigList();
            godownKeeperConfigList.godownkprStk = new List<GodownkprStk>();
            godownKeeperConfigList.CylindersDetails = new List<DistributorItems>();
            godownKeeperConfigList.VehicleDetails = new List<VehicleModel>();
            godownKeeperConfigList.DeliveryBoyDetails = new List<DistDbList>();
            godownKeeperConfigList.godownkprDelStk = new List<GodownkprDelStk>();
            godownKeeperConfigList.GetDeliveryboyTripDetails = new List<GetDeliveryboyTripDetails>();
            godownKeeperConfigList.Status = BusinessCont.FailStatus;
            try
            {
                godownKeeperConfigList = _unitOfWork.stockRepository.GodownKeeperConfiguration(staffConfiguration.DistId, staffConfiguration.StaffRefNo, staffConfiguration.MobileNo, staffConfiguration.VersionNo,staffConfiguration.DeviceId);
                godownKeeperConfigList.godownkprStk = _unitOfWork.godownRepository.GetGodownStk(staffConfiguration.DistId);
                godownKeeperConfigList.PlayStoreVersion = _unitOfWork.loginRepository.GetCurrentVersion(staffConfiguration.ApplicationName);
                godownKeeperConfigList.MobileLogs = _unitOfWork.loginRepository.GetMobileLogs();
                godownKeeperConfigList.CylindersDetails = _unitOfWork.distributorRepository.GetDistributorItems(staffConfiguration.DistId);
                godownKeeperConfigList.VehicleDetails = _unitOfWork.vehicleRepository.GetVehicleData(staffConfiguration.DistId, 0, BusinessCont.StatusYes);
                godownKeeperConfigList.DeliveryBoyDetails = _unitOfWork.staffRepository.GetDistributorDbLst(staffConfiguration.DistId, "ALL");
                godownKeeperConfigList.godownkprDelStk = _unitOfWork.staffRepository.GetGodownDeliveryLst(staffConfiguration.DistId);
                godownKeeperConfigList.GetDeliveryboyTripDetails = _unitOfWork.staffRepository.GetDeliveryboyTripDetailsLst(staffConfiguration.DistId,staffConfiguration.SelDate);
                godownKeeperConfigList.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                godownKeeperConfigList.Status = BusinessCont.FailStatus;
                godownKeeperConfigList.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GodownKeeperConfiguration", null, BusinessCont.FailStatus, ex.Message);
            }
            return godownKeeperConfigList;
        }
        #endregion

        #region Customer
        /// <summary>
        /// Return godown keeper required data which is used in all application ie Delivery boy list, Vechicle No. list
        /// and dashboard stock details.
        /// </summary>
        /// <param name="DistId">Delivery boy and vehicle no list against distributor</param>
        /// <param name="GodownKeeperId">Dashboard stock details against godown keeper</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Staff/CustomerConfiguration")]
        public string CustomerConfiguration([FromBody]StaffPostConfiguration staffConfiguration)
        {
            string msg = string.Empty;
            try
            {
                msg = _unitOfWork.loginRepository.ActiveUser(staffConfiguration.DistId, staffConfiguration.UniqueConsumerId, staffConfiguration.UserTypeId, staffConfiguration.MobileNo, staffConfiguration.VersionNo, staffConfiguration.DeviceId);
                msg = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "CustomerConfiguration", null, BusinessCont.FailStatus, ex.Message);
                msg = BusinessCont.FailStatus;
            }
            return msg;
        }
        #endregion

        [HttpPost]
        [Route("Staff/DeliveryBoyList")]
        public List<DistDbList> DeliveryBoyList([FromBody]DistributorStaffModel distributorStaffModel)
        {
            List<DistDbList> modelList = new List<DistDbList>();
            modelList = _unitOfWork.staffRepository.GetDistributorDbLst(distributorStaffModel.DistributorId, distributorStaffModel.ActiveFlag);
            return modelList;
        }

        #region Consumer location (Customer Survey)
        [HttpPost]
        [Route("Staff/DBConfiguration")]
        public DistStaffDetailsList DBConfiguration([FromBody]StaffPostConfiguration staffConfiguration)
        {
            DistStaffDetailsList DistStaffDtls = new DistStaffDetailsList();
            try
            {
                _unitOfWork.loginRepository.ActiveUser(staffConfiguration.DistId, staffConfiguration.StaffRefNo, BusinessCont.UserTypeDeliveryBoy, staffConfiguration.MobileNo, staffConfiguration.VersionNo, staffConfiguration.DeviceId);
                DistStaffDtls.PlayStoreVersion = _unitOfWork.loginRepository.GetCurrentVersion(staffConfiguration.ApplicationName);
                DistStaffDtls.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DBConfiguration", null, BusinessCont.FailStatus, ex.Message);
                DistStaffDtls.Status = BusinessCont.FailStatus;
            }
            return DistStaffDtls;
        }
        #endregion

    }
}
