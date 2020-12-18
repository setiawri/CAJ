using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Models
{
    [Table("DWSystem.PayrollEmployees")]
    public class PayrollEmployeesModel
    {
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        [Required]
        [Display(Name = "NIK")]
        public string Identification { get; set; }
        public static ModelMember COL_Identification = new ModelMember { Name = "Identification", Display = "NIK" };


        [Required]
        [Display(Name = "Nama")]
        public string Fullname { get; set; }
        public static ModelMember COL_Fullname = new ModelMember { Name = "Fullname", Display = "Nama" };


        [Required]
        [Display(Name = "Counter")]
        public string Customer_CustomerID { get; set; }
        public static ModelMember COL_Customer_CustomerID = new ModelMember { Name = "Customer_CustomerID", Display = "Counter" };
        [Display(Name = "Counter")]
        public string Customer_Name { get; set; }
        public static ModelMember COL_Customer_Name = new ModelMember { Name = "Customer_Name", Display = "Counter" };


        [Required]
        [Display(Name = "Tgl Join")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime JoinDate { get; set; }
        public static ModelMember COL_JoinDate = new ModelMember { Name = "JoinDate", Display = "Tgl Join" };


        [Required]
        [Display(Name = "Jam Kerja / Hari")]
        public int WorkHourPerDay { get; set; }
        public static ModelMember COL_WorkHourPerDay = new ModelMember { Name = "WorkHourPerDay", Display = "Jam Kerja / Hari" };


        [Required]
        [Display(Name = "Tgl Bayar Payroll")]
        public Guid PayrollPaymentDates_Id { get; set; }
        public static ModelMember COL_PayrollPaymentDates_Id = new ModelMember { Name = "PayrollPaymentDates_Id", Display = "Tgl Bayar Payroll" };
        [Display(Name = "Tgl Bayar Payroll")]
        public byte PayrollPaymentDates_PayDate { get; set; }
        public static ModelMember COL_PayrollPaymentDates_PayDate = new ModelMember { Name = "PayrollPaymentDates_PayDate", Display = "Tgl Bayar Payroll" };


        [Required]
        [Display(Name = "Tgl Bayar Reimbursement")]
        public Guid ReimbursementPaymentDates_Id { get; set; }
        public static ModelMember COL_ReimbursementPaymentDates_Id = new ModelMember { Name = "ReimbursementPaymentDates_Id", Display = "Tgl Bayar Reimbursement" };
        [Display(Name = "Tgl Bayar Reimbursement")]
        public byte ReimbursementPaymentDates_PayDate { get; set; }
        public static ModelMember COL_ReimbursementPaymentDates_PayDate = new ModelMember { Name = "ReimbursementPaymentDates_PayDate", Display = "Tgl Bayar Reimbursement" };


        [Required]
        [Display(Name = "Bank")]
        public Guid Banks_Id { get; set; }
        public static ModelMember COL_Banks_Id = new ModelMember { Name = "Banks_Id", Display = "Bank" };
        [Display(Name = "Bank")]
        public string Banks_Name { get; set; }
        public static ModelMember COL_Banks_Name = new ModelMember { Name = "Banks_Name", Display = "Bank" };


        [Required]
        [Display(Name = "Nomor Rekening")]
        public string AccountNumber { get; set; }
        public static ModelMember COL_AccountNumber = new ModelMember { Name = "AccountNumber", Display = "Nomor Rekening" };


        [Required]
        [Display(Name = "Nama Rekening")]
        public string AccountName { get; set; }
        public static ModelMember COL_AccountName = new ModelMember { Name = "AccountName", Display = "Nama Rekening" };


        [Display(Name = "Tgl Lahir")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Birthdate { get; set; }
        public static ModelMember COL_Birthdate = new ModelMember { Name = "Birthdate", Display = "Tgl Lahir" };


        [Display(Name = "Alamat")]
        public string Address { get; set; }
        public static ModelMember COL_Address = new ModelMember { Name = "Address", Display = "Alamat" };


        [Display(Name = "Phone 1")]
        public string Phone1 { get; set; }
        public static ModelMember COL_Phone1 = new ModelMember { Name = "Phone1", Display = "Phone 1" };


        [Display(Name = "Phone 2")]
        public string Phone2 { get; set; }
        public static ModelMember COL_Phone2 = new ModelMember { Name = "Phone2", Display = "Phone 2" };


        [Display(Name = "Status Kawin")]
        public Guid? MaritalStatuses_Id { get; set; }
        public static ModelMember COL_MaritalStatuses_Id = new ModelMember { Name = "MaritalStatuses_Id", Display = "Status Kawin" };
        [Display(Name = "Status Kawin")]
        public string MaritalStatuses_Name { get; set; }
        public static ModelMember COL_MaritalStatuses_Name = new ModelMember { Name = "MaritalStatuses_Name", Display = "Status Kawin" };


        [Display(Name = "Tanggungan")]
        public int DependentCount { get; set; }
        public static ModelMember COL_DependentCount = new ModelMember { Name = "DependentCount", Display = "Tanggungan" };


        [Display(Name = "Tipe Kerja")]
        public Guid? EmploymentTypes_Id { get; set; }
        public static ModelMember COL_EmploymentTypes_Id = new ModelMember { Name = "EmploymentTypes_Id", Display = "Tipe Kerja" };
        [Display(Name = "Tipe Kerja")]
        public string EmploymentTypes_Name { get; set; }
        public static ModelMember COL_EmploymentTypes_Name = new ModelMember { Name = "EmploymentTypes_Name", Display = "Tipe Kerja" };


        [Display(Name = "Area")]
        public Guid? CounterAreas_Id { get; set; }
        public static ModelMember COL_CounterAreas_Id = new ModelMember { Name = "CounterAreas_Id", Display = "Area" };
        [Display(Name = "Area")]
        public string CounterAreas_Name { get; set; }
        public static ModelMember COL_CounterAreas_Name = new ModelMember { Name = "CounterAreas_Name", Display = "Area" };


        [Display(Name = "Aktif")]
        public bool Active { get; set; }
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Aktif" };


        [Display(Name = "Departemen")]
        public Guid? PayrollDepartments_Id { get; set; }
        public static ModelMember COL_PayrollDepartments_Id = new ModelMember { Name = "PayrollDepartments_Id", Display = "Departemen" };
        [Display(Name = "Departemen")]
        public string PayrollDepartments_Name { get; set; }
        public static ModelMember COL_PayrollDepartments_Name = new ModelMember { Name = "PayrollDepartments_Name", Display = "Departemen" };


        /* ADDITIONAL MEMBERS *********************************************************************************************************************************/

        [Display(Name = "Wilayah")]
        public Guid? Regions_Id { get; set; }
        public static ModelMember COL_Regions_Id = new ModelMember { Name = "Regions_Id", Display = "Wilayah" };
        [Display(Name = "Wilayah")]
        public string Regions_Name { get; set; }
        public static ModelMember COL_Regions_Name = new ModelMember { Name = "Regions_Name", Display = "Wilayah" };


        [Display(Name = "Saldo Hutang")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DebtBalance { get; set; }
        public static ModelMember COL_DebtBalance = new ModelMember { Name = "DebtBalance", Display = "Saldo Hutang" };


        [Display(Name = "Saldo Tabungan")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int MandatoryDepositBalance { get; set; }
        public static ModelMember COL_MandatoryDepositBalance = new ModelMember { Name = "MandatoryDepositBalance", Display = "Saldo Tabungan" };

        /******************************************************************************************************************************************************/
    }
}