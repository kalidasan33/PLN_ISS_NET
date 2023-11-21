using System;
using System.Collections.Generic;
using System.Data;
using ISS.Core.Model.Information;
using ISS.BusinessRules.Contract.Information;
using ISS.Repository.Information;


namespace ISS.BusinessService.Implementation.Information
{
    public class InformationService : IInformationService
    {
      
        public InformationService()
        {
           
        }
        public IList<Releases> GetReleases(ReleasesSearch search)
        {          
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetReleases(search);
            return result;
        }

        public IList<ExceptionDetail> GetAS400Exceptions(string superOrder)
        {          
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetAS400Exceptions(superOrder);
            return result;
        }

        public IList<SuggestedException> GetSuggestedExceptions(ReleasesSearch search)
        {          
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetSuggestedExceptions(search);
            return result;
        }

        public IList<SuggestedExceptionDetial> GetSuggestedExceptionDetail(SuggestedExceptionDetial search)
        {          
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetSuggestedExceptionDetail(search);
            return result;
        }
        public IList<DCWorkOrder> GetDCWorkOrders(DCWorkOrderSearch search)
        {          
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetDCWorkOrders(search);
            return result;
        }

        public IList<StyleException> GetStyleExceptions(StyleSearch search)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetStyleExceptions(search);
            return result;
        }

        public IList<WOTextileGroup> GetWOTextileGroup(StyleSearch search)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetWOTextileGroup(search);
            return result;
        }

        public IList<BlownAwayLot> GetBlownAwayLots(StyleSearch search)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBlownAwayLots(search);
            return result;
        }

        public IList<BulksToAvyx> GetBulksToAvyx(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToAvyx(FromDate, ToDate);
            return result;
        }

        public IList<BulksToAvyx> GetBulksToComplete(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToComplete(FromDate, ToDate);
            return result;
        }

        public IList<BulksToAvyx> GetBulksNoData(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksNoData(FromDate, ToDate);
            return result;
        }

        public IList<BulksToAvyx> GetBulksToError(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToError(FromDate, ToDate);
            return result;
        }

        public string GetBulksActiveCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksActiveCount(FromDate, ToDate);
            return result;
        }
        public string GetCompleteCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetCompleteCount(FromDate, ToDate);
            return result;
        }

        public string GetErrorCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetErrorCount(FromDate, ToDate);
            return result;
        }


        public string GetBulksPulledCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksPulledCount(FromDate, ToDate);
            return result;
        }
        public string GetBulksSuccessfulCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksSuccessfulCount(FromDate, ToDate);
            return result;
        }

        public string GetErrorosCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetErrorosCount(FromDate, ToDate);
            return result;
        }

        public string GetErrorosSecondCount(String FromDate, String ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetErrorosSecondCount(FromDate, ToDate);
            return result;
        }

        public IList<BulkstoOneSource> GetBulksToPulled(string FromDate, string ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToPulled(FromDate, ToDate);
            return result;
        }

        public IList<BulkstoOneSource> GetBulksToSuccess(string FromDate, string ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToSuccess(FromDate, ToDate);
            return result;
        }

        public IList<BulkstoOneSource> GetBulksOSNoData(string FromDate, string ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksOSNoData(FromDate, ToDate);
            return result;
        }

        public IList<BulkstoOneSource> GetBulksToErrorOS(string FromDate, string ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToErrorOS(FromDate, ToDate);
            return result;
        }
        public IList<BulkstoOneSource> GetBulksToErrorOSSecond(string FromDate, string ToDate)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetBulksToErrorOSSecond(FromDate, ToDate);
            return result;
        }
        public IList<KnightsApparelExpedite> GetKnightsApparelExpedite(String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode)
        {
            var searchRepository = new InformationRepository();
            var result = searchRepository.GetKnightsApparelExpedite(FromDate, ToDate, StyleCode, ColorCode, AttributeCode, SizeCode);
            return result;
        }
    }
}
