using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Core.Model.Order
{
  public  class OrderComment
    {
      [Display(Name = "Requisition Id")]
      public string RequisitionId { set; get; }

      [Display(Name = "Planner Comments")]
      public string PlannerComment { set; get; }

      [Display(Name = "Approver Comments")]
      public string ApproverComment { set; get; }
     
      [Display(Name = "Requisition Version")]
      public decimal RequisitionVersion { get; set; }

      [Display(Name = "Planning Contact")]
      public string PlanningContact { get; set; }

      [Display(Name = "Approver User ID")]
      public string RequisitionApproverId { get; set; }

      [Display(Name = "Planner User ID")]
      public string CreatedBy { get; set; }

      public DateTime UpdatedDate { get; set; }
      public DateTime CreatedDate { get; set; }

      [Display(Name = "Requisition Approver")]
      public string RequisitionApprover { get; set; }

      public decimal NoteSeqNo { get; set; }

      public string NoteType { get; set; }

      public List<string> PlannerCommentLst { get; set; }
      public List<string> ApproverCommentLst { get; set; }
    }
}
