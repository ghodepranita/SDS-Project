using Aadyam.SDS.Business.Model.DensityAnalysis;
using Aadyam.SDS.Business.Model.User;
using System.Collections.Generic;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    [RoutePrefix("DensityAnalysis")]
    public class DensityAnalysisController : BaseApiController
    {
        [HttpPost]
        [Route("GetPendingOrderWithDensityAnalysis")]
        public List<PendingOrderDensityAnalysis> GetPendingOrderWithDensityAnalysis([FromBody]PostModel postModel)
        {
            List<PendingOrderDensityAnalysis> modelList = new List<PendingOrderDensityAnalysis>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetPendingOrderWithDensityAnalysis(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetDensityAnalysisSummary")]
        public List<DensityAnalysisSummary> GetDensityAnalysisSummary([FromBody]PostModel postModel)
        {
            List<DensityAnalysisSummary> modelList = new List<DensityAnalysisSummary>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetDensityAnalysisSummary(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetTripCasesSummary")]
        public List<DensityAnalysisSummary> GetTripCasesSummary([FromBody]PostModel postModel)
        {
            List<DensityAnalysisSummary> modelList = new List<DensityAnalysisSummary>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetTripCasesSummary(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetTripCasesSummaryforAD")]
        public List<DensityAnalysisSummary> GetTripCasesSummaryforAD([FromBody]PostModel postModel)
        {
            List<DensityAnalysisSummary> modelList = new List<DensityAnalysisSummary>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetTripCasesSummaryforAD(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetDensityAnalysisConsumerDetails")]
        public List<ConsumerListDensityAnalysis> GetDensityAnalysisConsumerDetails([FromBody]PostModel postModel)
        {
            List<ConsumerListDensityAnalysis> modelList = new List<ConsumerListDensityAnalysis>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetDensityAnalysisConsumerDetails(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetConsumerDetails")]
        public List<ConsumerListDensityAnalysis> GetConsumerDetails([FromBody]PostModel postModel)
        {
            List<ConsumerListDensityAnalysis> modelList = new List<ConsumerListDensityAnalysis>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetConsumerDetails(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetPendingBookingForDensity")]
        public List<PendingBookingForDensity> GetPendingBookingForDensity([FromBody]PostModel postModel)
        {
            List<PendingBookingForDensity> modelList = new List<PendingBookingForDensity>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetPendingBookingForDensity(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetDensityAnalysisSummary_RtSeq")]
        public List<DensityAnalysisSummary> GetDensityAnalysisSummary_RtSeq([FromBody]PostModel postModel)
        {
            List<DensityAnalysisSummary> modelList = new List<DensityAnalysisSummary>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetDensityAnalysisSummary_RtSeq(postModel);
            return modelList;
        }

        [HttpPost]
        [Route("GetDensityAnalysisConsumerDetails_RtSeq")]
        public List<ConsumerListDensityAnalysis> GetDensityAnalysisConsumerDetails_RtSeq([FromBody]PostModel postModel)
        {
            List<ConsumerListDensityAnalysis> modelList = new List<ConsumerListDensityAnalysis>();
            modelList = _unitOfWork._DensityAnalysisRepository.GetDensityAnalysisConsumerDetails_RtSeq(postModel);
            return modelList;
        }

    }
}
