using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISS.Core.Model.Information;


namespace ISS.BusinessRules.Contract.Information
{
    public interface IInformationService
    {
        IList<Releases> GetReleases(ReleasesSearch search);

        IList<ExceptionDetail> GetAS400Exceptions(string superOrder);

        IList<SuggestedException> GetSuggestedExceptions(ReleasesSearch search);

        IList<SuggestedExceptionDetial> GetSuggestedExceptionDetail(SuggestedExceptionDetial search);

        IList<DCWorkOrder> GetDCWorkOrders(DCWorkOrderSearch search);

        IList<StyleException> GetStyleExceptions(StyleSearch search);

        IList<WOTextileGroup> GetWOTextileGroup(StyleSearch search);

        IList<BlownAwayLot> GetBlownAwayLots(StyleSearch search);

        IList<BulksToAvyx> GetBulksToAvyx(String FromDate, String ToDate);

        IList<BulksToAvyx> GetBulksToComplete(String FromDate, String ToDate);

        IList<BulksToAvyx> GetBulksNoData(String FromDate, String ToDate);

        IList<BulksToAvyx> GetBulksToError(String FromDate, String ToDate);

        string GetBulksActiveCount(String FromDate, String ToDate);

        string GetCompleteCount(String FromDate, String ToDate);

        string GetErrorCount(String FromDate, String ToDate);

        string GetBulksPulledCount(String FromDate, String ToDate);

        string GetBulksSuccessfulCount(String FromDate, String ToDate);

        string GetErrorosCount(String FromDate, String ToDate);

        string GetErrorosSecondCount(String FromDate, String ToDate);

        IList<BulkstoOneSource> GetBulksToPulled(String FromDate, String ToDate);

        IList<BulkstoOneSource> GetBulksToSuccess(String FromDate, String ToDate);

        IList<BulkstoOneSource> GetBulksOSNoData(String FromDate, String ToDate);

        IList<BulkstoOneSource> GetBulksToErrorOS(String FromDate, String ToDate);

        IList<BulkstoOneSource> GetBulksToErrorOSSecond(String FromDate, String ToDate);

        IList<KnightsApparelExpedite> GetKnightsApparelExpedite(String FromDate, String ToDate, String StyleCode, String ColorCode, String AttributeCode, String SizeCode);
    }

    public interface ICachingInformationService : IInformationService
    {

    }
}
