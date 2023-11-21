using ISS.Core.Model.Textiles;
using ISS.DAL;
using ISS.Common;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace ISS.Repository.Textiles
{
    public partial class TextilesRepository : RepositoryBase
    {
        private string GetDatasetId()
        {
            string query = "select oprsql.iss_capacity_adjustment_pkg.get_dataset_id from dual";
            return Convert.ToString(ExecuteScalar(query));
        }

        public IList<TextileAllocation> GetTextileAllocations(TextilesSearch search)
        {
            string[] weekYears = search.FromWYear != null ? search.FromWYear.Split('/') : null;
            string[] toWeekYears = search.ToWYear != null ? search.ToWYear.Split('/') : null;
            int week = 0;
            int year = 0;
            int toWeek = 0;
            int toYear = 0;

            if (weekYears != null && weekYears.Count() > 0)
            {
                week = Convert.ToInt32(weekYears[0].Trim());
                year = Convert.ToInt32(weekYears[1].Trim());
            }

            if (toWeekYears != null && toWeekYears.Count() > 0)
            {
                toWeek = Convert.ToInt32(toWeekYears[0].Trim());
                toYear = Convert.ToInt32(toWeekYears[1].Trim());
            }

            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter()
            {
                ParameterName = "iBusUnit",
                Value = search.BusinessUnit
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iFabric",
                Value = search.TextileGroup
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iDatasetId",
                Value = GetDatasetId(),
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iYear",
                Value = year,
                DbType = DbType.Int32
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iWeek",
                Value = week,
                DbType = DbType.Int32
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iToYear",
                Value = toYear,
                DbType = DbType.Int32
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iToWeek",
                Value = toWeek,
                DbType = DbType.Int32
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iPlant",
                Value = string.Empty,
                DbType = DbType.String
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iPlanner",
                Value = search.Planner,
                DbType = DbType.String
            });
            parameters.Add(new OracleParameter()
            {
                ParameterName = "iVersion",
                Value = 1,
                DbType = DbType.Int32
            });
            try
            {
                IDataReader reader = ExecuteSPReader("OPRSQL.ISS_TEXTILES.RS_ALLOC_SUMMARY", parameters.ToArray());
                var result = (new DbHelper()).ReadData<TextileAllocation>(reader);
                
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }

        public string GetBeginEndDates(int week, int year)
        {
            string query = "select distinct WEEK_BEGIN_DATE from fiscal_calendar where fiscal_week = " + week + " and fiscal_year = " + year;
            return Convert.ToString(ExecuteScalar(query));
        }

        public List<YarnItem> GetYarnDesc(int week, int year)
        {
            System.Text.StringBuilder query = new System.Text.StringBuilder(string.Empty);

            query.Append("select distinct YARN_ITEM  \"YarnItm\",nvl(YARN_DESC,'Missing')  \"YarnDesc\", Plant_CD  \"Plant\" from iss_yarn_alloc a  ");
            query.Append("Where trunc(a.alloc_date) in (select calendar_date  from fiscal_calendar b where  b.fiscal_week = " + week + "  and b.fiscal_year = " + year + ")");

            IDataReader reader = ExecuteReader(query.ToString());
            var result = (new DbHelper()).ReadData<YarnItem>(reader);

            return result;

        }
    }
}
