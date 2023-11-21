using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Core.Model.Common;
using ISS.BusinessService.Contract.Summary;
using ISS.Repository.Summary;
using System.Data;
using ISS.Core.Model.Summary;

namespace ISS.BusinessService.Implementation.Common
{
    public class SummaryService : ISummaryService
    {
        public string CurrentDataBaseKey { get; set; }

        public DataTable GetFirstQuery(SummaryFilterModal filterModal)
        {
            var searchRepository = new SummaryRepository() { CurrentDataBaseKey = CurrentDataBaseKey };
            var result = searchRepository.GetFirstQuery(filterModal);
            return result;
        }
    }
}
