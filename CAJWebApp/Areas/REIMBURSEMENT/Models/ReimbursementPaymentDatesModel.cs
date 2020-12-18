using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.Reimbursement.Models
{
    [Table("DWSystem.ReimbursementPaymentDates")]
    public class ReimbursementPaymentDatesModel
    {
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };

        [Required]
        [Display(Name = "Tgl Bayar")]
        public byte PayDate { get; set; }
        public static ModelMember COL_PayDate = new ModelMember { Name = "PayDate", Display = "Tgl Bayar" };

        /******************************************************************************************************************************************************/
    }
}