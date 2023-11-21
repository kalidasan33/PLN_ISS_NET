using ISS.Core.Model.Textiles;
using System.Collections.Generic;

namespace ISS.BusinessRules.Contract.Textiles
{
    public interface ITextilesService
    {        
        IList<TextileAllocation> GetTextileAllocations(TextilesSearch search);

        string GetBeginEndDates(int week, int year);

        List<YarnItem> GetYarnDesc(int week, int year);
    }

    public interface ICachingInformationService : ITextilesService
    {

    }
}
