using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Distributor;
using Aadyam.SDS.Business.Model.Email;
using Aadyam.SDS.Business.Model.Godown;
using Aadyam.SDS.Business.Model.User;
using Aadyam.SDS.Business.Model.Vehicle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class MastersController : BaseApiController
    {
        decimal LogId = 0;

        #region Vehicle Master
        /// <summary>
        /// Return Vehicle Masters list
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        [Route("Masters/GetVehicleMaster")]
        public IHttpActionResult GetVehicleMaster([FromBody]VehicleModel Vehicle)
        {
            if (Vehicle == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            VehicleDetails vehicleDetails = new VehicleDetails();
            vehicleDetails.VehicleMaster = new VehicleModel();
            vehicleDetails.VehicleMasterLst = new List<VehicleModel>();
            try
            {
                if (Vehicle.VehicleId > 0)
                {
                    vehicleDetails.VehicleMaster = _unitOfWork.vehicleRepository.GetVehicleData(Vehicle.DistributorId, Vehicle.VehicleId, Vehicle.Flag).FirstOrDefault();
                }
                else
                {
                    vehicleDetails.VehicleMasterLst = _unitOfWork.vehicleRepository.GetVehicleData(Vehicle.DistributorId, Vehicle.VehicleId, Vehicle.Flag);
                }
                vehicleDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                vehicleDetails.Status = BusinessCont.FailStatus;
                vehicleDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetVehicleMaster", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(vehicleDetails);
        }

        /// <summary>
        /// Add, Edit and delete vehicle details
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        [OverrideAuthorization]
        [Route("Masters/AddEditVehicleMaster")]
        public IHttpActionResult AddEditVehicleMaster()
        {
            VehicleDetails vehicleDetails = new VehicleDetails();
            try
            {
                var VehicleDtlsStr = HttpContext.Current.Request.Params.Get("VehicleDtls") == null ? "" : HttpContext.Current.Request.Params.Get("VehicleDtls");
                VehicleModel VehicleDtls = JsonConvert.DeserializeObject<VehicleModel>(VehicleDtlsStr);
                if (VehicleDtls == null)
                    return BadRequest(BusinessCont.InvalidClientRqst);
                vehicleDetails.VehicleId = _unitOfWork.vehicleRepository.AddEditVehicleMaster(VehicleDtls);
                long rtnvalue = 0;

                if (vehicleDetails.VehicleId > 0 && VehicleDtls.Flag != "DELETE")
                    rtnvalue = SaveImage(VehicleDtls.DistributorId.ToString(), vehicleDetails.VehicleId.ToString());
                if (vehicleDetails.VehicleId > 0 && rtnvalue == 0 && VehicleDtls.Flag != "DELETE")
                {
                    BusinessCont.SaveLog(LogId, 0, 0, 0, vehicleDetails.VehicleId, null, null, BusinessCont.FailStatus, "image error");
                }
                vehicleDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                vehicleDetails.VehicleId = -2;
                vehicleDetails.Status = BusinessCont.FailStatus;
                vehicleDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddEditVehicleMaster", null, BusinessCont.FailStatus, ex.Message);
                return BadRequest(BusinessCont.InvalidClientRqst);
            }
            return Ok(vehicleDetails);
        }

        private long SaveImage(string distributorId, string VehicleId)
        {
            long rtnvalue = 0;
            var httpRequest = HttpContext.Current.Request;
            try
            {
                if (distributorId.Length > 0 && VehicleId.Length > 0 && httpRequest.Files.Count > 0)
                {
                    VehicleModel vehicleModel = new VehicleModel();
                    foreach (string fileType in httpRequest.Files)
                    {
                        if (string.IsNullOrEmpty(fileType) == false)
                        {
                            var postedFile = httpRequest.Files[fileType];
                            if (postedFile != null && postedFile.ContentLength > 0)
                            {
                                IList<string> AllowedFileExtensions = new List<string> { ".pdf", ".PDF" };
                                var extension = System.IO.Path.GetExtension(postedFile.FileName.Trim()).ToLower();
                                string mainPath = ConfigurationManager.AppSettings["imagepathFile"];
                                if (!Directory.Exists(HttpContext.Current.Server.MapPath(mainPath)))
                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(mainPath));

                                string path = mainPath + distributorId + "/" + VehicleId + "/";
                                string filename = fileType + "_" + DateTime.Now.Year + extension;
                                //create path if doesnt exists
                                if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                                var filePath = HttpContext.Current.Server.MapPath(path + filename);
                                if (!File.Exists(filePath))
                                    File.Delete(filePath);
                                postedFile.SaveAs(filePath);

                                vehicleModel.VehicleId = Convert.ToInt32(VehicleId);
                                if (fileType == "RTOPasingScan")
                                    vehicleModel.RTOPasingScanFileName = filename;
                                if (fileType == "InsuranceScan")
                                    vehicleModel.InsuranceScanFileName = filename;

                                rtnvalue = 1;
                            }
                        }
                    }
                    vehicleModel.Flag = "UpdateFile";
                    _unitOfWork.vehicleRepository.AddEditVehicleMaster(vehicleModel);
                }
                else
                    rtnvalue = 1;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "Save Image", "DistributorId= " + distributorId + "VehicleId : " + VehicleId, BusinessCont.FailStatus, ex.Message);
                rtnvalue = 0;
            }
            return rtnvalue;
        }
        #endregion

        #region GodownMaster
        //Get the Godown List By Distributor Id
        [HttpPost]
        [Route("Masters/GetDistributorGodownList")]
        public IHttpActionResult GetDistributorGodownList([FromBody]GodownMasterModel _godownMasterModel)
        {
            DistributorGodownDetails _distributorGodownDetails = new DistributorGodownDetails();
            _distributorGodownDetails.GodownMasterModel = new GodownMasterModel();
            _distributorGodownDetails.GodownMasterList = new List<GodownMasterModel>();

            if (_godownMasterModel.DistributorId <= 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            try
            {
                if (_godownMasterModel.GodownId > 0)
                {
                    _distributorGodownDetails.GodownMasterModel = _unitOfWork.godownRepository.GetDistributorGodownList(_godownMasterModel.DistributorId, _godownMasterModel.GodownId, _godownMasterModel.ActiveFlag).FirstOrDefault();
                }
                else
                {
                    _distributorGodownDetails.GodownMasterList = _unitOfWork.godownRepository.GetDistributorGodownList(_godownMasterModel.DistributorId, _godownMasterModel.GodownId, _godownMasterModel.ActiveFlag);
                }
                _distributorGodownDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorGodownDetails.Status = BusinessCont.FailStatus;
                _distributorGodownDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get Distributor Godown List", "DistributorId= " + _godownMasterModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorGodownDetails);
        }

        //Add/Edit/Delete Godown Master
        [HttpPost]
        [Route("Masters/AddEditDistributorGodownMaster")]
        public IHttpActionResult AddEditDistributorGodownMaster([FromBody]GodownMasterModel _godowMasterModel)
        {
            if (_godowMasterModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorGodownDetails _godownDetails = new DistributorGodownDetails();
            try
            {
                _godownDetails.GodownId = _unitOfWork.godownRepository.AddGodownMaster(_godowMasterModel);
                _godownDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _godownDetails.Status = BusinessCont.FailStatus;
                _godownDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Add godown MasteModel", "godowMasterModel= " + _godowMasterModel, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_godownDetails);
        }
        #endregion

        #region StaffList

        //get the Staff List By Distributor Id
        [HttpPost]
        [Route("Masters/GetDistributorStaffList")]
        public IHttpActionResult GetDistributorStaffList([FromBody]DistributorStaffModel _distributorStaffModel)
        {
            if (_distributorStaffModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorStaffDetails _distributorStaffDetails = new DistributorStaffDetails();
            _distributorStaffDetails.StaffList = new List<DistributorStaffModel>();
            try
            {
                _distributorStaffDetails.StaffList = _unitOfWork.staffRepository.GetDistributorStaffList(_distributorStaffModel.DistributorId, _distributorStaffModel.ActiveFlag);
                _distributorStaffDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorStaffDetails.Status = BusinessCont.FailStatus;
                _distributorStaffDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, _distributorStaffModel.DistributorId, 0, 0, "Get Distributor Staff List", "DistributorId= " + _distributorStaffModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorStaffDetails);
        }

        //get the Godown Keeper List for Godown-GodownKeeper Mapping
        //Returns All Unmapped GodownKeepers
        [HttpPost]
        [Route("Masters/GetDistributorStaffListForGodownMapping")]
        public IHttpActionResult GetDistributorStaffListForGodownMapping([FromBody]DistributorStaffModel _distributorStaffModel)
        {
            if (_distributorStaffModel.DistributorId <= 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorStaffDetails _distributorStaffDetails = new DistributorStaffDetails();
            _distributorStaffDetails.StaffList = new List<DistributorStaffModel>();
            try
            {
                _distributorStaffDetails.StaffList = _unitOfWork.staffRepository.GetDistributorStaffListForGodownMapping(_distributorStaffModel.GodownId, _distributorStaffModel.DistributorId);
                _distributorStaffDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorStaffDetails.Status = BusinessCont.FailStatus;
                _distributorStaffDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, _distributorStaffModel.DistributorId, 0, 0, "Get Distributor Staff List", "DistributorId= " + _distributorStaffModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorStaffDetails);
        }
        #endregion        

        #region DistributorList
        [HttpPost]
        [Route("Masters/GetDistributorList")]
        public IHttpActionResult GetDistributorList([FromBody]DistributorModel _distributorModel)
        {
            DistributorDetails _distributorDetails = new DistributorDetails();
            _distributorDetails.DistributorList = new List<DistributorModel>();

            if (string.IsNullOrWhiteSpace(_distributorModel.SACode))
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                _distributorDetails.DistributorList = _unitOfWork.adminRepository.GetDistributorList(_distributorModel.SACode, _distributorModel.ActiveFlag);
                _distributorDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorDetails.Status = BusinessCont.FailStatus;
                _distributorDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get Distributor List", "SACode= " + _distributorModel.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorDetails);
        }
        #endregion

        #region Get Distributor Config
        [HttpPost]
        [Route("Masters/GetDistributorConfig")]
        public IHttpActionResult GetDistributorConfig([FromBody]DistributorConfig _distributorModel)
        {
            DistributorModel distributorConfigs = new DistributorModel();
            try
            {
                distributorConfigs = _unitOfWork.adminRepository.GetDistributorConfigSetup(_distributorModel.DistributorId, _distributorModel.ConfigValue);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, _distributorModel.DistributorId, 0, 0, "Get Distributor Config", "DistributorId= " + _distributorModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(distributorConfigs);
        }
        #endregion

        #region Distributor Area List
        //get the Godown Keeper List for Godown-GodownKeeper Mapping
        //Returns All Unmapped GodownKeepers
        [HttpPost]
        [OverrideAuthorization]
        [Route("Masters/GetDistributorAreaList")]
        public IHttpActionResult GetDistributorAreaList([FromBody]DistributorModel _distributorModel)
        {
            List<AreaListModel> _areaDetails = new List<AreaListModel>();
            try
            {
                _areaDetails = _unitOfWork.distributorRepository.GetDistributorList(_distributorModel.DistributorId, _distributorModel.ActiveFlag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, _distributorModel.DistributorId, 0, 0, "Get Distributor Area List", "Distributor Id= " + _distributorModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaDetails);
        }
        #endregion

        #region Appconfiguration Master

        /// <summary>
        /// Return Appconfiguration Masters list
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        [Route("Masters/GetAppconfigurationMaster/{PkId}")]
        public IHttpActionResult GetAppconfigurationMaster(int PkId)
        {
            AppconfigurationDetails AppconfigurationDetails = new AppconfigurationDetails();
            AppconfigurationDetails.AppconfigurationMasterLst = new List<AppconfigurationModel>();
            try
            {
                AppconfigurationDetails.AppconfigurationMasterLst = _unitOfWork.AppconfigurationRepository.GetAppconfigurationData(AppconfigurationDetails.PkId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAppconfigurationMaster", "PkId= " + AppconfigurationDetails.PkId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(AppconfigurationDetails);
        }

        [HttpPost]
        [Route("Masters/AddUpdateAppconfigurationMaster")]
        public IHttpActionResult AddUpdateAppconfigurationMaster([FromBody]AppconfigurationModel Appconfiguration)
        {
            if (Appconfiguration == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            AppconfigurationDetails AppconfigurationDetails = new AppconfigurationDetails();
            try
            {
                AppconfigurationDetails.PkId = _unitOfWork.AppconfigurationRepository.AddAppconfigurationMaster(Appconfiguration);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddUpdateAppconfigurationMaster", "PkId= " + AppconfigurationDetails.PkId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(Appconfiguration);
        }
        #endregion

        #region Get Area List For Cluster_Area Mapping
        //Getting Area List For creating Cluster Area Mapping
        [HttpPost]
        [Route("Masters/GetAreaListForCluster_AreaMapping")]
        public IHttpActionResult GetAreaListForCluster_AreaMapping([FromBody]AreaListModel _areaListModel)
        {
            if (_areaListModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaModel _areaModel = new AreaModel();
            _areaModel.areaList = new List<AreaListModel>();
            try
            {
                _areaModel.areaList = _unitOfWork.vehicleRepository.GetAreaListForCluster_AreaMapping(Convert.ToInt32(_areaListModel.DistributorId), _areaListModel.ClusterId);
                _areaModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaModel.Status = BusinessCont.FailStatus;
                _areaModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAreaListForCluster_AreaMapping", "DistributorID= " + _areaListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaModel);
        }
        #endregion

        #region Get DeliVery Boy List For _Cluster Mapping
        [HttpPost]
        [Route("Masters/GetDeliVeryBoyListFor_ClusterMapping")]
        public IHttpActionResult GetDeliVeryBoyListFor_ClusterMapping([FromBody]AreaListModel _areaListModel)
        {
            if (_areaListModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorStaffDetails _distributorStaffDetails = new DistributorStaffDetails();
            _distributorStaffDetails.StaffList = new List<DistributorStaffModel>();
            try
            {
                _distributorStaffDetails.StaffList = _unitOfWork.vehicleRepository.GetDeliVeryBoyListFor_ClusterMapping(_areaListModel.DistributorId, _areaListModel.ClusterId);
                _distributorStaffDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorStaffDetails.Status = BusinessCont.FailStatus;
                _distributorStaffDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDeliVeryBoyListFor_ClusterMapping", "DistributorID= " + _areaListModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorStaffDetails);
        }
        #endregion

        #region Get Region Type Master
        [HttpPost]
        [Route("Masters/GetRegionTypeMaster")]
        public IHttpActionResult GetRegionTypeMaster()
        {
            RegionAreaTypeDtls regionAreaTypeDtls = new RegionAreaTypeDtls();
            regionAreaTypeDtls.regionTypelist = new List<RegionAreaType>();
            try
            {
                regionAreaTypeDtls.regionTypelist = _unitOfWork.vehicleRepository.getRegionType();
                regionAreaTypeDtls.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                regionAreaTypeDtls.Status = BusinessCont.FailStatus;
                regionAreaTypeDtls.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetRegionTypeMaster", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(regionAreaTypeDtls);
        }
        #endregion

        #region Add Cluster Area Mapping
        [HttpPost]
        [Route("Masters/AddClusterAreaMapping")]
        public IHttpActionResult AddClusterAreaMapping([FromBody]AreaClusterModel _areaClusterModel)
        {

            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            try
            {
                if (_areaClusterModel.AreaClusterId > 0)
                {
                    _areaClusterModel.ClusterId = _areaClusterModel.AreaClusterId;
                }
                else
                {
                    _areaClusterModel.ClusterId = Convert.ToInt32(_unitOfWork.vehicleRepository.Add_Cluster(_areaClusterModel));
                }
                if (_areaClusterModel.ClusterId > 0)
                {
                    _areaClusterDetailsModel.ClusterId = _areaClusterModel.ClusterId;
                    if (_areaClusterModel.Action != BusinessCont.DeleteRecord)
                    {
                        _areaClusterDetailsModel.AreaClusterId = _unitOfWork.vehicleRepository.Add_ClusterAreaMapping(_areaClusterModel);
                    }
                }
                else
                {
                    _areaClusterDetailsModel.ClusterId = _areaClusterModel.ClusterId;
                }
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;

            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddClusterAreaMapping", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Edit Cluster Area Coordinates
        [HttpPost]
        [Route("Masters/EditClusterAreaCoordinates")]
        public IHttpActionResult EditClusterAreaCoordinates([FromBody]AreaClusterModel _areaClusterModel)
        {
            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            try
            {
                _areaClusterDetailsModel.AreaClusterId = _unitOfWork.vehicleRepository.DistributorClusterAddEdit(_areaClusterModel);
                _areaClusterDetailsModel.ClusterId = _areaClusterModel.ClusterId;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "EditClusterAreaCoordinates", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Area_Cluster Mapping Details
        [HttpPost]
        [Route("Masters/GetArea_ClusterMappingDetails")]
        public IHttpActionResult GetArea_ClusterMappingDetails([FromBody]AreaClusterModel _areaClusterModel)
        {
            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            _areaClusterDetailsModel.AreaClusterMappingList = new List<AreaClusterModel>();
            try
            {
                _areaClusterDetailsModel.AreaClusterMappingList = _unitOfWork.vehicleRepository.GetArea_ClusterMappingList(_areaClusterModel.DistributorId, _areaClusterModel.ClusterId, _areaClusterModel.IsActive);
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetArea_ClusterMappingDetails", "DistributorID= " + _areaClusterModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Cluster Details
        [HttpPost]
        [Route("Masters/GetClusterDetails")]
        public IHttpActionResult GetClusterDetails([FromBody]AreaClusterModel _areaClusterModel)
        {
            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            _areaClusterDetailsModel.AreaClusterSummaryList = new List<AreaClusterSummary>();
            try
            {
                _areaClusterDetailsModel.AreaClusterSummaryList = _unitOfWork.vehicleRepository.GetClusterDetails(_areaClusterModel.DistributorId, _areaClusterModel.IsActive);
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Master - GetClusterDetails", "DistributorID= " + _areaClusterModel.DistributorId + " , IsActive =  " + _areaClusterModel.IsActive, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Cluster Details With Remark
        [HttpPost]
        [Route("Masters/GetClusterDetailsWithRemark")]
        public IHttpActionResult GetClusterDetailsWithRemark([FromBody]AreaClusterModel _areaClusterModel)
        {
            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            _areaClusterDetailsModel.AreaClusterSummaryList = new List<AreaClusterSummary>();
            try
            {
                _areaClusterDetailsModel.AreaClusterSummaryList = _unitOfWork.vehicleRepository.GetClusterListWithRemark(_areaClusterModel.DistributorId, _areaClusterModel.IsActive);
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterDetailsWithRemark", "DistributorID= " + _areaClusterModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }

        #endregion

        #region Get Delivery Boy Details Report
        [HttpPost]
        [Route("Masters/GetDeliveryBoyDetailsReport")]
        public IHttpActionResult GetDeliveryBoyDetailsReport([FromBody]DeliveryBoyModel _DeliveryModel)
        {
            if (_DeliveryModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            DeliveryBoyDetailsModel _areaClusterDetailsModel = new DeliveryBoyDetailsModel();
            _areaClusterDetailsModel.DeliveryList = new List<DeliveryBoyModel>();
            try
            {
                _areaClusterDetailsModel.DeliveryList = _unitOfWork.vehicleRepository.GetDeliveryBoyDetailsList(_DeliveryModel.DistributorId, _DeliveryModel.StaffRefNo, _DeliveryModel.FromDate, _DeliveryModel.ToDate);
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDeliveryBoyDetailsReport", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Cluster Wise Performance
        [HttpPost]
        [Route("Masters/GetClusterWisePerformance")]
        public IHttpActionResult GetClusterWisePerformance([FromBody]ClusterwiseStatusModel _ClusterwiseStatusModel)
        {
            if (_ClusterwiseStatusModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            ClusterwiseStatusDetailsModel _ClusterwiseStatusDetailsModel = new ClusterwiseStatusDetailsModel();
            _ClusterwiseStatusDetailsModel.ClusterwiseStatusList = new List<ClusterwiseStatusModel>();
            try
            {
                _ClusterwiseStatusDetailsModel.ClusterwiseStatusList = _unitOfWork.vehicleRepository.GetClusterWisePerformance(_ClusterwiseStatusModel.DistributorId, _ClusterwiseStatusModel.ClusterId, _ClusterwiseStatusModel.FromDate, _ClusterwiseStatusModel.ToDate);
                _ClusterwiseStatusDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _ClusterwiseStatusDetailsModel.Status = BusinessCont.FailStatus;
                _ClusterwiseStatusDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterWisePerformance", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_ClusterwiseStatusDetailsModel);
        }
        #endregion

        #region Vehicle Delivery boy Mapping
        [HttpPost]
        [Route("Masters/GetVehicleMapDtls")]
        public IHttpActionResult GetVehicleMapDtls([FromBody]VehicleMapping Vehicle)
        {
            if (Vehicle == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            VehicleMappingDtls vehicleDetails = new VehicleMappingDtls();
            vehicleDetails.vehicleMapping = new VehicleMappinglst();
            vehicleDetails.vehicleMappinglst = new List<VehicleMappinglst>();
            try
            {
                if (Vehicle.DeliveryBoyId > 0)
                {
                    vehicleDetails.vehicleMapping = _unitOfWork.vehicleRepository.GetVehicleMapDtls(Vehicle.VehicleId, Vehicle.DeliveryBoyId, Vehicle.DistributorId, Vehicle.Status).FirstOrDefault();
                }
                else
                {
                    vehicleDetails.vehicleMappinglst = _unitOfWork.vehicleRepository.GetVehicleMapDtls(Vehicle.VehicleId, Vehicle.DeliveryBoyId, Vehicle.DistributorId, Vehicle.Status);
                }
                vehicleDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                vehicleDetails.Status = BusinessCont.FailStatus;
                vehicleDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, Vehicle.DistributorId, 0, 0, "GetVehicleMapDtls", "DistributorId= " + Vehicle.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(vehicleDetails);
        }

        [HttpPost]
        [Route("Masters/AddEditVehicleMappingMaster")]
        public IHttpActionResult AddEditVehicleMappingMaster([FromBody]VehicleMapping vehicleMapping)
        {
            if (vehicleMapping == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            VehicleDetails vehicleDetails = new VehicleDetails();
            try
            {
                vehicleDetails.VehicleId = _unitOfWork.vehicleRepository.AddEditVehicleMapping(vehicleMapping);
                vehicleDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                vehicleDetails.Status = BusinessCont.FailStatus;
                vehicleDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddEditVehicleMappingMaster", "vehicleMapping= " + vehicleMapping, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(vehicleDetails);
        }
        #endregion

        #region //Vehicle Delivery Boy Mapping    
        [HttpPost]
        [Route("Masters/GetMappedDeliveryBoyToVehicle")]
        public IHttpActionResult GetMappedDeliveryBoyToVehicle([FromBody]VehicleMapping vehicleMapping)
        {
            List<VehicleUnmappinglst> result = new List<VehicleUnmappinglst>();
            try
            {
                result = _unitOfWork.vehicleRepository.GetMappedDeliveryBoyToVehicle(vehicleMapping);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetMappedDeliveryBoyToVehicle", "vehicleMapping= " + vehicleMapping.VehicleId + vehicleMapping.StaffRefNo, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("Masters/SetMappedDeliveryBoyToVehicle")]
        public IHttpActionResult SetMappedDeliveryBoyToVehicle([FromBody]VehicleMapping vehicleMapping)
        {
            long result = 0;
            try
            {
                result = _unitOfWork.vehicleRepository.SetMappedDeliveryBoyToVehicle(vehicleMapping);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SetMappedDeliveryBoyToVehicle", "vehicleMapping= " + vehicleMapping.VehicleId + vehicleMapping.DeliveryBoyId + vehicleMapping.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region GetDeliveryBoyMappedVehicleTripsPvt
        [HttpGet]
        [Route("Masters/GetDeliveryBoyMappedVehicleTrips/{DistributorId}")]
        public IHttpActionResult GetDeliveryBoyMappedVehicleTrips(long DistributorId)
        {
            List<DeliveryBoyMappedVehicleTripsModel> deliveryBoyMappedVehicleTrips = new List<DeliveryBoyMappedVehicleTripsModel>();
            try
            {
                deliveryBoyMappedVehicleTrips = _unitOfWork.vehicleRepository.GetDeliveryBoyMappedVehicleTrips(DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDeliveryBoyMappedVehicleTrips", "DistributorId= " + DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(deliveryBoyMappedVehicleTrips);
        }
        #endregion

        #region Checking For Masters After Login
        [HttpPost]
        [Route("Masters/CheckMastersAfterLogin")]
        public List<CheckMasters> CheckMastersAfterLogin([FromBody] DistributorModel distributorModel)
        {
            List<CheckMasters> modelList = new List<CheckMasters>();
            try
            {
                modelList = _unitOfWork.distributorRepository.CheckMastersAfterLogin(distributorModel.DistributorId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, distributorModel.DistributorId, 0, 0, null, null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Distributor Consumer Data List
        [HttpPost]
        [Route("Masters/GetDistributorConsumerDataList")]
        public IHttpActionResult GetDistributorConsumerDataList()
        {
            DistributorConsumerDetails _distributorDetails = new DistributorConsumerDetails();
            _distributorDetails.DistributorConsumerList = new List<DistributorConsumerModel>();
            try
            {
                _distributorDetails.DistributorConsumerList = _unitOfWork.adminRepository.GetDistributorConsumerList();
                _distributorDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorDetails.Status = BusinessCont.FailStatus;
                _distributorDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistributorConsumerDataList", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorDetails);
        }

        //get the Godown Keeper List for Godown-GodownKeeper Mapping
        //Returns All Unmapped GodownKeepers
        [HttpPost]
        [Route("Masters/GetDistributorClusterList")]
        public IHttpActionResult GetDistributorClusterList([FromBody]DistributorModel _distributorModel)
        {
            DistributorClusterDetails _distributorDetails = new DistributorClusterDetails();
            _distributorDetails.DistributorClusterList = new List<DistributorClusterModel>();
            try
            {
                _distributorDetails.DistributorClusterList = _unitOfWork.adminRepository.GetDistributorClusterList(_distributorModel.DistributorId);
                _distributorDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorDetails.Status = BusinessCont.FailStatus;
                _distributorDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetDistributorClusterList", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorDetails);
        }
        #endregion

        #region Add Edit Cluster
        [HttpPost]
        [Route("Masters/AddEditCluster")]
        public IHttpActionResult AddEditCluster([FromBody]AreaClusterModel ClusterDtls)
        {
            AreaClusterDetailsModel _areaClusterDetailsModel = new AreaClusterDetailsModel();
            try
            {
                _areaClusterDetailsModel.AreaClusterId = _unitOfWork.vehicleRepository.DistributorClusterAddEdit(ClusterDtls);
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddEditCluster", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Officer Details By Role
        [HttpPost]
        [Route("Masters/GetOfficerDetailsByRole")]
        public IHttpActionResult GetOfficerDetailsByRole([FromBody]OfficerModel Officer)
        {
            if (Officer == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            OfficerDetails officerDetails = new OfficerDetails();
            officerDetails.OfficerLst = new List<OfficerModel>();
            try
            {
                officerDetails.OfficerLst = _unitOfWork.vehicleRepository.GetOfficerData(Officer.Role, Officer.Flag);
                officerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                officerDetails.Status = BusinessCont.FailStatus;
                officerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOfficerDetailsByRole", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(officerDetails);
        }

        [HttpPost]
        [Route("Masters/SaveOfficerDetailsByRole")]
        public IHttpActionResult SaveOfficerDetailsByRole([FromBody] List<OfficerModel> Officer)
        {
            if (Officer == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            OfficerDetails officerDetails = new OfficerDetails();
            try
            {
                officerDetails.ReturnId = _unitOfWork.vehicleRepository.SaveOfficerDetailsByRole(Officer);
                officerDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                officerDetails.Status = BusinessCont.FailStatus;
                officerDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SaveOfficerDetailsByRole", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(officerDetails);
        }
        #endregion

        #region Consumer Address Details
        [HttpPost]
        [Route("Masters/ConsumerAddressDetails")]
        public List<ConsumerAddressDetails> ConsumerAddressDetails([FromBody]ConsumerAddressDetails ConsAddress)
        {
            List<ConsumerAddressDetails> modelList = new List<ConsumerAddressDetails>();
            try
            {
                modelList = _unitOfWork.consumerRepository.ConsumerAddressDetails(ConsAddress.DistributorID, ConsAddress.UniqueConsumerId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, ConsAddress.DistributorID, 0, 0, "ConsumerAddressDetails", "DistributorID=" + ConsAddress.DistributorID + ", UniqueConsumerId=" + ConsAddress.UniqueConsumerId, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region AppVersion Master
        /// <summary>
        /// Return AppVersion Masters list
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        [Route("Masters/GetAppVersionMaster/{PkId}")]
        public IHttpActionResult GetAppVersionMaster(int PkId)
        {
            AppVersionDetails AppVersionDetails = new AppVersionDetails();
            AppVersionDetails.AppVersionMasterLst = new List<AppVersionModel>();
            try
            {
                AppVersionDetails.AppVersionMasterLst = _unitOfWork.AppconfigurationRepository.GetAppVersionData(AppVersionDetails.PkId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetAppVersionMaster", "PkId= " + AppVersionDetails.PkId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(AppVersionDetails);
        }

        [HttpPost]
        [Route("Masters/AddUpdateAppVersionMaster")]
        public IHttpActionResult AddUpdateAppVersionMaster([FromBody]AppVersionModel AppVersionDetailsModel)
        {
            if (AppVersionDetailsModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AppVersionDetails AppVersionDetails = new AppVersionDetails();
            try
            {
                AppVersionDetails.PkId = _unitOfWork.AppconfigurationRepository.AddAppVersionMaster(AppVersionDetailsModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddUpdateAppconfigurationMaster", "PkId= " + AppVersionDetails.PkId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(AppVersionDetails);
        }
        #endregion

        #region Get ZO List
        [HttpPost]
        [Route("Masters/GetZOList")]
        public IHttpActionResult GetZOList([FromBody]ZOModel zOModel)
        {
            if (string.IsNullOrWhiteSpace(zOModel.ActiveFlag))
                return BadRequest(BusinessCont.InvalidClientRqst);

            List<ZOModel> zoList = new List<ZOModel>();
            try
            {
                zoList = _unitOfWork.adminRepository.GetZOList(zOModel.ActiveFlag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get ZO List", "Flag= " + zOModel.ActiveFlag, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(zoList);
        }
        #endregion

        #region Get RO List
        [HttpPost]
        [Route("Masters/GetROList")]
        public IHttpActionResult GetROList([FromBody]ZOModel zOModel)
        {
            if (string.IsNullOrWhiteSpace(zOModel.ZOCode))
                return BadRequest(BusinessCont.InvalidClientRqst);

            List<ROModel> roList = new List<ROModel>();
            try
            {
                roList = _unitOfWork.adminRepository.GetROList(zOModel.ZOCode, zOModel.ActiveFlag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get RO List", "ZOCode= " + zOModel.ZOCode + ", Flag=" + zOModel.ActiveFlag, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(roList);
        }
        #endregion

        #region Get SA List
        [HttpPost]
        [Route("Masters/GetSAList")]
        public IHttpActionResult GetSAList([FromBody]ROModel rOModel)
        {
            if (string.IsNullOrWhiteSpace(rOModel.ZOCode) || string.IsNullOrWhiteSpace(rOModel.ROCode))
                return BadRequest(BusinessCont.InvalidClientRqst);

            List<SAModel> sAList = new List<SAModel>();
            try
            {
                sAList = _unitOfWork.adminRepository.GetSAList(rOModel.ZOCode, rOModel.ROCode, rOModel.ActiveFlag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get SA List", "ZOCode= " + rOModel.ZOCode + ", ROCode=" + rOModel.ROCode + ", Flag=" + rOModel.ActiveFlag, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(sAList);
        }
        #endregion

        #region Class Room Logs And Slot Allotment Logs
        [HttpPost]
        [Route("Masters/GetDistributorClassRoomLog")]
        public IHttpActionResult GetSAList([FromBody]SAModel saModel)
        {
            List<SlotAllotment> slotAllotments = new List<SlotAllotment>();
            try
            {
                slotAllotments = _unitOfWork.adminRepository.GetDistributorClassRoomLog(saModel.SACode);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get Distributor ClassRoom Log", "SACode= " + saModel.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(slotAllotments);
        }
        #endregion

        #region Get And Save Class Room Master
        [HttpPost]
        [Route("Masters/GetAndSaveClassRoomMaster")]
        public IHttpActionResult GetAndSaveClassRoomMaster([FromBody]ClassRoom classRoom)
        {
            List<ClassRoom> videoFiles = new List<ClassRoom>();
            try
            {
                videoFiles = _unitOfWork.adminRepository.GetAndSaveClassRoomMaster(classRoom);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get & Save Class Room", JsonConvert.SerializeObject(classRoom), BusinessCont.FailStatus, ex.Message);
            }
            return Ok(videoFiles);
        }
        #endregion

        #region Get Distributor List For Geocodings
        [HttpPost]
        [Route("Masters/GetDistributorListForGeocoding")]
        public IHttpActionResult GetDistributorListForGeocoding([FromBody]DistributorModel _distributorModel)
        {
            if (string.IsNullOrWhiteSpace(_distributorModel.SACode))
                return BadRequest(BusinessCont.InvalidClientRqst);

            DistributorDetails _distributorDetails = new DistributorDetails();
            _distributorDetails.ConsumerGeocodingList = new List<ConsumerGeocodingModel>();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                _distributorDetails.ConsumerGeocodingList = _unitOfWork.adminRepository.GetDistributorConsumerDtlsForGeocoding(_distributorModel.SACode, _distributorModel.ActiveFlag, AppConfigAPITimeout);
                _distributorDetails.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _distributorDetails.Status = BusinessCont.FailStatus;
                _distributorDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get Distributor List for Geocoding", "SACode= " + _distributorModel.SACode, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_distributorDetails);
        }
        #endregion

        #region Get CC Person Details
        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/GetCCPersonDetails")]
        public IHttpActionResult GetCCPersonDetails()
        {
            List<DetailsForEmail> CCPersonDetails = new List<DetailsForEmail>();
            string flag = "CCPerson";
            try
            {
                CCPersonDetails = _unitOfWork.adminRepository.GetPersonDetailsList(flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetEmailPurposeDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(CCPersonDetails);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/GetEmailPurposeDetails")]
        public IHttpActionResult GetEmailPurposeDetails()
        {
            List<DetailsForEmail> EmailPurposes = new List<DetailsForEmail>();
            string flag = "";
            try
            {
                EmailPurposes = _unitOfWork.adminRepository.GetPersonDetailsList(flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetEmailPurposeDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(EmailPurposes);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/GetOfficerDetails")]
        public IHttpActionResult GetOfficerDetails([FromBody]DataModel dataModel)
        {
            List<DetailsForEmail> EmailPurposes = new List<DetailsForEmail>();
            try
            {
                EmailPurposes = _unitOfWork.adminRepository.GetOfficerDetails(dataModel.Role, dataModel.Purpose);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOfficerDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(EmailPurposes);
        }

        [HttpPost]
        [Route("Masters/SaveEmailDetails")]
        public IHttpActionResult SaveEmailDetails([FromBody]EmailModel _emailModel)
        {
            if (_emailModel == null)
                return BadRequest(BusinessCont.InvalidClientRqst);

            long result = 0;
            try
            {
                result = _unitOfWork.adminRepository.SaveEmailList(_emailModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Get Distributor List for Geocoding", "SACode= " + null, null, ex.Message);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/GetEmailDetails")]
        public IHttpActionResult GetEmailDetails()
        {
            List<EmailDetails> EmailDetail = new List<EmailDetails>();
            try
            {
                EmailDetail = _unitOfWork.adminRepository.GetEmailDetails();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetEmailDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(EmailDetail);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/SubmitEmailPurposeDetails/{Id}")]
        public IHttpActionResult SubmitEmailPurposeDetails(int Id)
        {
            List<EmailModel> EmailModel = new List<EmailModel>();
            try
            {
                //EmailPurposes = _unitOfWork.adminRepository.GetOfficerDetails(Role);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOfficerDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(EmailModel);
        }
        #endregion

        #region Get Admin Details
        [AllowAnonymous]
        [HttpPost]
        [Route("Masters/GetAdminDetails/{EmailFor}")]
        public IHttpActionResult GetAdminDetails(int EmailFor)
        {
            List<DetailsForEmail> EmailPurposes = new List<DetailsForEmail>();
            try
            {
                EmailPurposes = _unitOfWork.adminRepository.GetAdminDetails(EmailFor);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetOfficerDetails", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(EmailPurposes);
        }
        #endregion

        #region Delete VehicleDetails from Distributor
        [HttpPost]
        [Route("Masters/DeleteVehicledetalisFromDist")]
        public DeleteVehicle DeleteVehicledetalisFromDist([FromBody]DeleteVehicle model)
        {
            DeleteVehicle modelDelete = new DeleteVehicle();
            try
            {
                modelDelete = _unitOfWork.vehicleRepository.DeleteVehicleDetails(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "DeleteVehicledetalisFromDist", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelDelete;
        }
        #endregion

        #region Check vehicle
        [HttpPost]
        [Route("Masters/Checkvehicle")]
        [AllowAnonymous]
        public CheckVehicle Checkvehicle([FromBody]CheckVehicle model)
        {
            CheckVehicle modelCheck = new CheckVehicle();
            try
            {
                modelCheck = _unitOfWork.vehicleRepository.CheckVehicleDetails(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "Checkvehicle", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelCheck;
        }
        #endregion

        #region Add All Trip Data
        [HttpPost]
        [Route("Masters/AddAllTripData")]
        public List<TripPlanningDetaForProd> AddAllTripData(TripData tripdata)
        {
            List<TripPlanningDetaForProd> modelList = new List<TripPlanningDetaForProd>();
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                modelList = _unitOfWork.adminRepository.AddAllTripData(tripdata, AppConfigAPITimeout);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, tripdata.DistributorId, 0, 0, "AddAllTripData", null, BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Save Trip Prod Data
        [HttpPost]
        [Route("Masters/SaveTripProdData")]
        public long SaveTripProdData([FromBody] GetTripDataDetails tripdata)
        {
            long InsertedCount = 0;
            int AppConfigAPITimeout = 0;
            try
            {
                var AppConfig = BusinessCont.GetAppConfiguration();
                AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
                InsertedCount = _unitOfWork.adminRepository.SaveTripProdData(tripdata.TripPlanningDetaForProd, tripdata.DistributorId, tripdata.ClusterId, AppConfigAPITimeout);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, tripdata.DistributorId, 0, 0, "SaveTripProdData", "Save Trip Prod Data", BusinessCont.FailStatus, ex.Message);
            }
            return InsertedCount;
        }
        #endregion

        #region Get Consumer Loaction History
        [HttpPost]
        [Route("Masters/GetConsumerLoactionHistory")]
        public List<ConsumerLoc> GetConsumerLoactionHistory([FromBody]PostModelConsumerHistory postModel)
        {
            List<ConsumerLoc> modelList = new List<ConsumerLoc>();
            try
            {
                modelList = _unitOfWork.orderRepository.GetConsumerLoactionHistory(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerLoactionHistory", "GetConsumer Loaction History", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Consumer Deviation
        [HttpPost]
        [Route("Masters/GetConsumerDeviation")]
        public List<ConsumerDeviation> GetConsumerDeviation([FromBody]PostModelConsumerDeviation postModel)
        {
            List<ConsumerDeviation> DeviationList = new List<ConsumerDeviation>();
            try
            {
                DeviationList = _unitOfWork.orderRepository.GetConsumerDeviation(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerDeviation", "GetConsumerDeviation", BusinessCont.FailStatus, ex.Message);
            }
            return DeviationList;
        }
        #endregion

        #region Get Clusterwise Consumer Inside or Outside & Logitude Latitude
        [HttpPost]
        [Route("Masters/GetConsumerLogitudeLatitude")]
        public List<ConsumerLocationInsideOutside> GetConsumerLogitudeLatitude([FromBody]ConsumerListLogiLati postModel)
        {
            List<ConsumerLocationInsideOutside> modelList = new List<ConsumerLocationInsideOutside>();
            try
            {
                modelList = _unitOfWork.orderRepository.GetConsumerLogitudeLatitude(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetConsumerLogitudeLatitude", "Get Consumer Logitude Latitude", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Get Cluster wise Consumer List
        [HttpPost]
        [Route("Masters/GetClusterwiseconsumerList")]
        [AllowAnonymous]
        public IHttpActionResult GetClusterwiseconsumerList([FromBody]AreaClusterModelNew _areaClusterModel)
        {
            if (_areaClusterModel.DistributorId < 0)
                return BadRequest(BusinessCont.InvalidClientRqst);

            AreaClusterDetailsModelNew _areaClusterDetailsModel = new AreaClusterDetailsModelNew();
            _areaClusterDetailsModel.AreaClusterMappingList = new List<AreaClusterModelNew>();
            try
            {
                _areaClusterDetailsModel.AreaClusterMappingList = _unitOfWork.vehicleRepository.GetClusterwiseconsumerList(_areaClusterModel.DistributorId, _areaClusterModel.IsActive);
                _areaClusterDetailsModel.Status = BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                _areaClusterDetailsModel.Status = BusinessCont.FailStatus;
                _areaClusterDetailsModel.ExMsg = BusinessCont.ExceptionMsg(ex);
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetClusterwiseconsumerList", "DistributorID= " + _areaClusterModel.DistributorId, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(_areaClusterDetailsModel);
        }
        #endregion

        #region Get Cash Memo Status
        [HttpPost]
        [Route("Masters/CashMemostatus")]
        public List<CashMemoModel> GetCashMemostatus([FromBody]PostModelcashmemoStatus postModel)
        {
            List<CashMemoModel> modelList = new List<CashMemoModel>();
            try
            {
                modelList = _unitOfWork.orderRepository.GetCashMemostatus(postModel);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetCashMemostatus", "Get Cash Memo status", BusinessCont.FailStatus, ex.Message);
            }
            return modelList;
        }
        #endregion

        #region Cluster List New
        [HttpPost]
        [Route("Masters/GetArea_ClusterMappingListNew")]
        public List<ClusterList> GetArea_ClusterMappingListNew([FromBody] ClusterList ClusterModel)
        {
            List<ClusterList> clusterLists = new List<ClusterList>();
            try
            {
                clusterLists = _unitOfWork.vehicleRepository.GetArea_ClusterMappingListNew(Convert.ToInt32(ClusterModel.DistributorId), ClusterModel.ClusterId, ClusterModel.IsActive);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetArea_ClusterMappingListNew", "DistributorID= " + Convert.ToInt32(ClusterModel.DistributorId), BusinessCont.FailStatus, ex.Message);
            }
            return clusterLists;
        }
        #endregion

    }
}
