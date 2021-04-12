using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.PAYROLL.Models
{
    //[Table("DWSystem.Payrolls")] //not used because it creates error because of property List<PayrollItemsModel> -> in the database, this property doesn't exist. 
    //This model also cannot be initialized in DBContext to prevent error
    public class PayrollsModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        [Required]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


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
        [Display(Name = "Jam Kerja / Hari")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int WorkHourPerDay { get; set; }
        public static ModelMember COL_WorkHourPerDay = new ModelMember { Name = "WorkHourPerDay", Display = "Jam Kerja / Hari" };


        [Required]
        [Display(Name = "Hari Kerja Normal")]
        [DisplayFormat(DataFormatString = "{0:N5}", ApplyFormatInEditMode = true)]
        public decimal RegularWorkDay { get; set; }
        public static ModelMember COL_RegularWorkDay = new ModelMember { Name = "RegularWorkDay", Display = "Hari Kerja Normal" };


        [Required]
        [Display(Name = "Normal / Hari")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RegularPayrate { get; set; }
        public static ModelMember COL_RegularPayrate = new ModelMember { Name = "RegularPayrate", Display = "Normal / Hari" };


        [Required]
        [Display(Name = "Jam Kerja Normal")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int RegularWorkHour { get; set; }
        public static ModelMember COL_RegularWorkHour = new ModelMember { Name = "RegularWorkHour", Display = "Jam Kerja Normal" };


        [Required]
        [Display(Name = "Jam Kerja Lembur Normal")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal RegularOvertimeWorkHour { get; set; }
        public static ModelMember COL_RegularOvertimeWorkHour = new ModelMember { Name = "RegularOvertimeWorkHour", Display = "Jam Kerja Lembur Normal" };


        [Required]
        [Display(Name = "Lembur Normal / Jam")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RegularOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_RegularOvertimeHourlyPayrate = new ModelMember { Name = "RegularOvertimeHourlyPayrate", Display = "Lembur Normal / Jam" };


        [Required]
        [Display(Name = "Hari Kerja Hari Besar")]
        [DisplayFormat(DataFormatString = "{0:N5}", ApplyFormatInEditMode = true)]
        public decimal HolidayWorkDay { get; set; }
        public static ModelMember COL_HolidayWorkDay = new ModelMember { Name = "HolidayWorkDay", Display = "Hari Kerja Hari Besar" };


        [Required]
        [Display(Name = "Hari Besar / Hari")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int HolidayPayrate { get; set; }
        public static ModelMember COL_HolidayPayrate = new ModelMember { Name = "HolidayPayrate", Display = "Hari Besar / Hari" };


        [Required]
        [Display(Name = "Jam Kerja Lembur Hari Besar")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal HolidayOvertimeWorkHour { get; set; }
        public static ModelMember COL_HolidayOvertimeWorkHour = new ModelMember { Name = "HolidayOvertimeWorkHour", Display = "Jam Kerja Lembur Hari Besar" };


        [Required]
        [Display(Name = "Lembur Hari Besar / Jam")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int HolidayOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_HolidayOvertimeHourlyPayrate = new ModelMember { Name = "HolidayOvertimeHourlyPayrate", Display = "Lembur Hari Besar / Jam" };


        [Required]
        [Display(Name = "Balance Awal Cuti")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int LeaveDaysStartingBalance { get; set; }
        public static ModelMember COL_LeaveDaysStartingBalance = new ModelMember { Name = "LeaveDaysStartingBalance", Display = "Balance Awal Cuti" };


        [Required]
        [Display(Name = "Update Cuti")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int LeaveDaysAdjustment { get; set; }
        public static ModelMember COL_LeaveDaysAdjustment = new ModelMember { Name = "LeaveDaysAdjustment", Display = "Update Cuti" };


        [Required]
        [Display(Name = "Balance Awal Tabungan")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int MandatoryDepositStartingBalance { get; set; }
        public static ModelMember COL_MandatoryDepositStartingBalance = new ModelMember { Name = "MandatoryDepositStartingBalance", Display = "Balance Awal Tabungan" };


        [Display(Name = "Update Tabungan")]
        public Guid? MandatoryDeposit_PayrollItems_Id { get; set; }
        public static ModelMember COL_MandatoryDeposit_PayrollItems_Id = new ModelMember { Name = "MandatoryDeposit_PayrollItems_Id", Display = "Update Tabungan" };


        [Required]
        [Display(Name = "Balance Awal Hutang")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DebtStartingBalance { get; set; }
        public static ModelMember COL_DebtStartingBalance = new ModelMember { Name = "DebtStartingBalance", Display = "Balance Awal Hutang" };

        
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
        public int? ApprovalOperator_ID { get; set; } = null;
        public static ModelMember COL_ApprovalOperator_ID = new ModelMember { Name = "ApprovalOperator_ID", Display = "Approver" };


        [Display(Name = "Departemen")]
        public string PayrollDepartments_Name { get; set; }
        public static ModelMember COL_PayrollDepartments_Name = new ModelMember { Name = "PayrollDepartments_Name", Display = "Departemen" };


        [Display(Name = "Wilayah")]
        public string Regions_Name { get; set; }
        public static ModelMember COL_Regions_Name = new ModelMember { Name = "Regions_Name", Display = "Wilayah" };


        [Display(Name = "Persentase Dibayar")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PaymentPercentage { get; set; }
        public static ModelMember COL_PaymentPercentage = new ModelMember { Name = "PaymentPercentage", Display = "Persentase Dibayar" };


        [Display(Name = "Notes")]
        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes" };


        [Required]
        [Display(Name = "Tgl Bayar")]
        public byte PayrollPaymentDates_PayDate { get; set; }
        public static ModelMember COL_PayrollPaymentDates_PayDate = new ModelMember { Name = "PayrollPaymentDates_PayDate", Display = "Tgl Bayar" };


        [Required]
        [Display(Name = "Nomor Urut")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RowNumber { get; set; }
        public static ModelMember COL_RowNumber = new ModelMember { Name = "RowNumber", Display = "Nomor Urut" };

        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal RegularHourlyPayrate { get; set; }

        public List<PayrollItemsModel> PayrollEarningsList { get; set; }
        public string PayrollEarningsListString { get; set; }


        public List<PayrollItemsModel> PayrollDeductionsList { get; set; }
        public string PayrollDeductionsListString { get; set; }


        public List<PayrollItemsModel> PayrollDebtsList { get; set; }
        public string PayrollDebtsListString { get; set; }


        public List<PayrollItemsModel> MandatoryDepositList { get; set; }


        public List<PayrollPaymentsModel> PayrollPaymentList { get; set; }


        [Display(Name = "Update Hutang")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DebtUpdateAmount { get; set; }
        public static ModelMember COL_DebtUpdateAmount = new ModelMember { Name = "DebtUpdateAmount", Display = "Update Hutang" };


        [Display(Name = "Update Tabungan")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int MandatoryDepositUpdateAmount { get; set; }
        public static ModelMember COL_MandatoryDepositUpdateAmount = new ModelMember { Name = "MandatoryDepositUpdateAmount", Display = "Update Tabungan" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal SalaryAmount { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int EarningsAmount { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DeductionsAmount { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PayableAmount { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PaymentAmount { get; set; }



        public int? FILTER_PayPeriodYear { get; set; }
        public int? FILTER_PayPeriodMonth { get; set; }
        public int? FILTER_PayDate { get; set; }
        public int? FILTER_Approval { get; set; }
        public string FILTER_Banks_Id { get; set; }
        public string FILTER_Search { get; set; }


        /******************************************************************************************************************************************************/
    }
}