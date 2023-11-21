using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Capacity
{
    public class CapacityWeek
    {
        #region Public Properties
        public decimal SpilledDz { get; set; }
        public decimal PlannedDz { get; set; }
        public decimal LockedDz { get; set; }
        public decimal ReleasedDz { get; set; }
        public decimal Locked { get; set; }
        public decimal Released { get; set; }
        public decimal Planned { get; set; }
        public decimal Initial { get; set; }
        public decimal Spill { get; set; }

        public decimal Net {get;set;}

        public decimal PlanNet { get; set; }
       
        public decimal PriorSpilledDz { get; set; }
        public decimal PriorPlannedDz { get; set; }
        public decimal PriorLockedDz { get; set; }
        public decimal PriorReleasedDz { get; set; }
        public decimal PriorLocked { get; set; }
        public decimal PriorReleased { get; set; }
        public decimal PriorPlanned { get; set; }
        public decimal PriorInitial { get; set; }
        public decimal PriorSpill { get; set; }

        public decimal PriorNet { get; set; }

        public decimal PriorPlanNet { get; set; }
        
        public decimal WeekSupply { get; set; }
        #endregion

    }       
}
