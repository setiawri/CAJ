using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.PAYROLL.Models
{
    [Table("DWSystem.PayrollPayments")]
    public class PayrollPaymentsModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        public Guid? Payrolls_Id { get; set; }
        public static ModelMember COL_Payrolls_Id = new ModelMember { Name = "Payrolls_Id", Display = "" };


        public Guid? Reimbursements_Id { get; set; }
        public static ModelMember COL_Reimbursements_Id = new ModelMember { Name = "Reimbursements_Id", Display = "" };


        [Display(Name = "Tgl Bayar")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime PaymentDate { get; set; }
        public static ModelMember COL_PaymentDate = new ModelMember { Name = "PaymentDate", Display = "" };


        [Display(Name = "Jumlah")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Amount { get; set; }
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "Jumlah" };


        public Guid Banks_Id { get; set; }
        public static ModelMember COL_Banks_Id = new ModelMember { Name = "Banks_Id", Display = "" };


        [Required]
        public string Banks_Name { get; set; }
        public static ModelMember COL_Banks_Name = new ModelMember { Name = "Banks_Name", Display = "" };


        public string AccountNumber { get; set; }
        public static ModelMember COL_AccountNumber = new ModelMember { Name = "AccountNumber", Display = "" };


        [Required]
        [Display(Name = "Rekening")]
        public string AccountName { get; set; }
        public static ModelMember COL_AccountName = new ModelMember { Name = "AccountName", Display = "" };


        public bool Cancelled { get; set; }
        public static ModelMember COL_Cancelled = new ModelMember { Name = "Cancelled", Display = "" };


        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/

        [Display(Name = "Nama")]
        public string EmployeeFullName { get; set; }
        public static ModelMember COL_Payrolls_EmployeeFullName = new ModelMember { Name = "Payrolls_EmployeeFullName", Display = "Nama" };


        [Display(Name = "No")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RowNumber { get; set; }
        public static ModelMember COL_Payrolls_RowNumber = new ModelMember { Name = "Payrolls_RowNumber", Display = "No" };


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime JoinDate { get; set; }
        public static ModelMember COL_JoinDate = new ModelMember { Name = "JoinDate", Display = "" };


        [Display(Name = "Periode")]
        [DisplayFormat(DataFormatString = "{0:MMM yyyy}")]
        public DateTime PayPeriod { get; set; }
        public static ModelMember COL_Payrolls_PayPeriod = new ModelMember { Name = "Payrolls_PayPeriod", Display = "Periode" };


        [Display(Name = "Counter")]
        public string CustomerName { get; set; }
        public static ModelMember COL_Payrolls_CustomerName = new ModelMember { Name = "Payrolls_CustomerName", Display = "Counter" };


        [Display(Name = "Wilayah")]
        public string Regions_Name { get; set; }
        public static ModelMember COL_Payrolls_Regions_Name = new ModelMember { Name = "Payrolls_Regions_Name", Display = "Wilayah" };


        /******************************************************************************************************************************************************/
    }
}