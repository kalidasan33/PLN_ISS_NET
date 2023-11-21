using ISS.BusinessRules.Contract.Textiles;
using ISS.Core.Model.Textiles;
using ISS.Repository.Textiles;
using System.Collections.Generic;

namespace ISS.BusinessService.Implementation.Textiles
{
    public class TextilesService : ITextilesService
    {

        public TextilesService()
        {

        }       

        public IList<TextileAllocation> GetTextileAllocations(TextilesSearch search)
        {
            var repository = new TextilesRepository();
            return repository.GetTextileAllocations(search);
        }

        public string GetBeginEndDates(int week, int year)
        {
            var repository = new TextilesRepository();
            return repository.GetBeginEndDates(week, year);
        }

        public List<YarnItem> GetYarnDesc(int week, int year)
        {
            var repository = new TextilesRepository();
            return repository.GetYarnDesc(week, year);

        }
    }
}
