using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CAJWebApp.Areas.PAYROLL.Models;

namespace CAJWebApp.Areas.Reimbursement.Models
{
    //[Table("DWSystem.Reimbursements")] //not used because it creates error because of property List<ReimbursementItemsModel> -> in the database, this property doesn't exist. 
    //This model also cannot be initialized in DBContext to prevent error
    public class ReimbursementsModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        [Required]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        [Required]
        [Display(Name = "Nomor Urut")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RowNumber { get; set; }
        public static ModelMember COL_RowNumber = new ModelMember { Name = "RowNumber", Display = "Nomor Urut" };


        [Required]
        [Display(Name = "Nama")]
        public Guid PayrollEmployees_Id { get; set; }
        public static ModelMember COL_PayrollEmployees_Id = new ModelMember { Name = "PayrollEmployees_Id", Display = "Nama" };


        [Required]
        [Display(Name = "Nama")]
        public string EmployeeFullName { get; set; }
        public static ModelMember COL_EmployeeFullName = new ModelMember { Name = "EmployeeFullName", Display = "Nama" };


        [Required]
        [Display(Name = "Tgl Join")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime JoinDate { get; set; }
        public static ModelMember COL_JoinDate = new ModelMember { Name = "JoinDate", Display = "Tgl Join" };


        [Required]
        [Display(Name = "Counter")]
        public string Customer_CustomerID { get; set; }
        public static ModelMember COL_Customer_CustomerID = new ModelMember { Name = "Customer_CustomerID", Display = "Counter" };


        [Required]
        [Display(Name = "Counter")]
        public string CustomerName { get; set; }
        public static ModelMember COL_CustomerName = new ModelMember { Name = "CustomerName", Display = "Counter" };


        [Required]
        [Display(Name = "Periode")]
        [DisplayFormat(DataFormatString = "{0:MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PayPeriod { get; set; }
        public static ModelMember COL_PayPeriod = new ModelMember { Name = "PayPeriod", Display = "Periode" };


        [Required]
        [Display(Name = "Tgl Bayar")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        public static ModelMember COL_PaymentDate = new ModelMember { Name = "PaymentDate", Display = "Tgl Bayar" };


        [Required]
        [Display(Name = "Nama Approver")]
        public string ApproverName { get; set; }
        public static ModelMember COL_ApproverName = new ModelMember { Name = "ApproverName", Display = "Nama Approver" };


        [Required]
        [Display(Name = "Title Approver")]
        public string ApproverTitle { get; set; }
        public static ModelMember COL_ApproverTitle = new ModelMember { Name = "ApproverTitle", Display = "Title Approver" };


        [Display(Name = "Tanda Tangan Approver")]
        public string ApproverSignatureFilename { get; set; }
        public static ModelMember COL_ApproverSignatureFilename = new ModelMember { Name = "ApproverSignatureFilename", Display = "Tanda Tangan Approver" };


        [Display(Name = "Approver")]
        public int? ApprovalOperator_ID { get; set; }
        public static ModelMember COL_ApprovalOperator_ID = new ModelMember { Name = "ApprovalOperator_ID", Display = "Approver" };


        [Display(Name = "Notes")]
        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes" };


        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/

        public List<ReimbursementItemsModel> ReimbursementItemsList { get; set; }
        public string ReimbursementItemsListString { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PayableAmount { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PaymentAmount { get; set; }


        public List<PayrollPaymentsModel> PaymentList { get; set; }


        public int? FILTER_PayPeriodYear { get; set; }
        public int? FILTER_PayPeriodMonth { get; set; }
        public int? FILTER_Approval { get; set; }
        public string FILTER_Banks_Id { get; set; }
        public string FILTER_Search { get; set; }


        /******************************************************************************************************************************************************/
    }
}