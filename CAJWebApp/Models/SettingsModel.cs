using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CAJWebApp.Models
{
    public class SettingsModel
    {
        [Key]
        [Required]
        [Display(Name = "Jam Kerja / Hari")]
        public int WorkHoursPerDay { get; set; }
        public static ModelMember COL_WorkHoursPerDay = new ModelMember { Name = "WorkHoursPerDay", Display= "Jam Kerja / Hari", Id = new Guid("3FD87986-B8A3-4FEB-A0C2-4E02128EC23A") };

        public string WorkHoursPerDay_Notes { get; set; }
        public static ModelMember COL_WorkHoursPerDay_Notes = new ModelMember { Name = "WorkHoursPerDay_Notes" };


        [Required]
        [Display(Name = "Hari Kerja / Bulan")]
        public int WorkDaysPerMonth { get; set; }
        public static ModelMember COL_WorkDaysPerMonth = new ModelMember { Name = "WorkDaysPerMonth", Display= "Hari Kerja / Bulan", Id = new Guid("CFE2DB87-9460-4873-932D-644C9FB9BEFE") };

        public string WorkDaysPerMonth_Notes { get; set; }
        public static ModelMember COL_WorkDaysPerMonth_Notes = new ModelMember { Name = "WorkDaysPerMonth_Notes" };


        [Required]
        [Display(Name = "Total Hari / Tahun")]
        public int LeaveDaysPerYear { get; set; }
        public static ModelMember COL_LeaveDaysPerYear = new ModelMember { Name = "LeaveDaysPerYear", Display = "Cuti / Tahun", Id = new Guid("71B83A50-D2D4-4E94-A9C5-AD5597D401D4") };

        public string LeaveDaysPerYear_Notes { get; set; }
        public static ModelMember COL_LeaveDaysPerYear_Notes = new ModelMember { Name = "LeaveDaysPerYear_Notes" };


        [Required]
        [Display(Name = "Potongan / Bulan")]
        public int DepositAmountPerMonth { get; set; }
        public static ModelMember COL_DepositAmountPerMonth = new ModelMember { Name = "DepositAmountPerMonth", Display = "Tabungan Wajib / Bulan", Id = new Guid("CBAC31E0-A04F-451F-9589-CF6D7A8CD953") };

        public string DepositAmountPerMonth_Notes { get; set; }
        public static ModelMember COL_DepositAmountPerMonth_Notes = new ModelMember { Name = "DepositAmountPerMonth_Notes" };


        [Required]
        [Display(Name = "Total")]
        public int DepositAmountTotal { get; set; }
        public static ModelMember COL_DepositAmountTotal = new ModelMember { Name = "DepositAmountTotal", Display = "Total Tabungan Wajib", Id = new Guid("A91EF3B8-7C59-45AA-982B-DD7C7F80FFE9") };

        public string DepositAmountTotal_Notes { get; set; }
        public static ModelMember COL_DepositAmountTotal_Notes = new ModelMember { Name = "DepositAmountTotal_Notes" };


        [Required]
        [Display(Name = "Bulan Pertama Dipotong")]
        public int DepositFirstMonthAfterJoin { get; set; }
        public static ModelMember COL_DepositFirstMonthAfterJoin = new ModelMember { Name = "DepositFirstMonthAfterJoin", Display = "Bulan Pertama Dipotong", Id = new Guid("62A34778-E9BD-4166-9BCF-FA899377B9EA") };

        public string DepositFirstMonthAfterJoin_Notes { get; set; }
        public static ModelMember COL_DepositFirstMonthAfterJoin_Notes = new ModelMember { Name = "DepositFirstMonthAfterJoin_Notes" };


        [Required]
        [Display(Name = "Nama")]
        public string PayrollApproverName { get; set; }
        public static ModelMember COL_PayrollApproverName = new ModelMember { Name = "PayrollApproverName", Display = "Nama Payroll Approver", Id = new Guid("E5152405-9589-40BE-95CA-7A42144D1AF5") };

        public string PayrollApproverName_Notes { get; set; }
        public static ModelMember COL_PayrollApproverName_Notes = new ModelMember { Name = "PayrollApproverName_Notes" };


        [Required]
        [Display(Name = "Jabatan")]
        public string PayrollApproverTitle { get; set; }
        public static ModelMember COL_PayrollApproverTitle = new ModelMember { Name = "PayrollApproverTitle", Display = "Jabatan Payroll Approver", Id = new Guid("3EC14792-5609-4273-9EF3-3E343A83A1B5") };

        public string PayrollApproverTitle_Notes { get; set; }
        public static ModelMember COL_PayrollApproverTitle_Notes = new ModelMember { Name = "PayrollApproverTitle_Notes" };


        [Display(Name = "Tanda Tangan")]
        public string PayrollApproverSignature { get; set; }
        public static ModelMember COL_PayrollApproverSignature = new ModelMember { Name = "PayrollApproverSignature", Display = "Tanda Tangan Payroll Approver", Id = new Guid("56B25D19-CABB-4361-B6E2-E9B86BA76359") };

        public string PayrollApproverSignature_Notes { get; set; }
        public static ModelMember COL_PayrollApproverSignature_Notes = new ModelMember { Name = "PayrollApproverSignature_Notes" };


        [Required]
        [Display(Name = "Multiplier Normal")]
        public decimal MultiplierRegularPayrate { get; set; }
        public static ModelMember COL_MultiplierRegularPayrate = new ModelMember { Name = "MultiplierRegularPayrate", Display = "Multiplier Normal", Id = new Guid("D0D835CC-C1A9-424E-AFF3-4E6F2079FFD9") };

        public string MultiplierRegularPayrate_Notes { get; set; }
        public static ModelMember COL_MultiplierRegularPayrate_Notes = new ModelMember { Name = "MultiplierRegularPayrate_Notes" };


        [Required]
        [Display(Name = "Multiplier Hari Besar")]
        public decimal MultiplierHolidayPayrate { get; set; }
        public static ModelMember COL_MultiplierHolidayPayrate = new ModelMember { Name = "MultiplierHolidayPayrate", Display = "Multiplier Hari Besar", Id = new Guid("BFD8FC4E-2DB3-47C8-9118-4F3C8E073EA1") };

        public string MultiplierHolidayPayrate_Notes { get; set; }
        public static ModelMember COL_MultiplierHolidayPayrate_Notes = new ModelMember { Name = "MultiplierHolidayPayrate_Notes" };


        [Required]
        [Display(Name = "Multiplier Lembur Normal")]
        public decimal MultiplierRegularOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_MultiplierRegularOvertimeHourlyPayrate = new ModelMember { Name = "MultiplierRegularOvertimeHourlyPayrate", Display = "Multiplier Lembur Normal", Id = new Guid("523DF960-4092-4D32-BBB4-CFC1C42F244D") };

        public string MultiplierRegularOvertimeHourlyPayrate_Notes { get; set; }
        public static ModelMember COL_MultiplierRegularOvertimeHourlyPayrate_Notes = new ModelMember { Name = "MultiplierRegularOvertimeHourlyPayrate_Notes" };


        [Required]
        [Display(Name = "Multiplier Lembur Hari Besar")]
        public decimal MultiplierHolidayOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_MultiplierHolidayOvertimeHourlyPayrate = new ModelMember { Name = "MultiplierHolidayOvertimeHourlyPayrate", Display = "Multiplier Lembur Hari Besar", Id = new Guid("C75040C4-D93B-433C-8EB3-0825DABAAC10") };

        public string MultiplierHolidayOvertimeHourlyPayrate_Notes { get; set; }
        public static ModelMember COL_MultiplierHolidayOvertimeHourlyPayrate_Notes = new ModelMember { Name = "MultiplierHolidayOvertimeHourlyPayrate_Notes" };


        [Required]
        [Display(Name = "% Dibayar Bulan 1")]
        public int PaymentPercentageMonth1 { get; set; }
        public static ModelMember COL_PaymentPercentageMonth1 = new ModelMember { Name = "PaymentPercentageMonth1", Display = "% Dibayar Bulan 1", Id = new Guid("BBBF258E-9B3F-4AFC-9692-045DD5E4B223") };

        public string PaymentPercentageMonth1_Notes { get; set; }
        public static ModelMember COL_PaymentPercentageMonth1_Notes = new ModelMember { Name = "PaymentPercentageMonth1_Notes" };
    

        [Required]
        [Display(Name = "% Dibayar Bulan 2")]
        public int PaymentPercentageMonth2 { get; set; }
        public static ModelMember COL_PaymentPercentageMonth2 = new ModelMember { Name = "PaymentPercentageMonth2", Display = "% Dibayar Bulan 2", Id = new Guid("29FE987D-7989-4C89-A8D2-592B61454F2E") };

        public string PaymentPercentageMonth2_Notes { get; set; }
        public static ModelMember COL_PaymentPercentageMonth2_Notes = new ModelMember { Name = "PaymentPercentageMonth2_Notes" };


        [Required]
        [Display(Name = "% Dibayar Bulan 3")]
        public int PaymentPercentageMonth3 { get; set; }
        public static ModelMember COL_PaymentPercentageMonth3 = new ModelMember { Name = "PaymentPercentageMonth3", Display = "% Dibayar Bulan 3", Id = new Guid("6E52F202-7E40-4F6C-B714-2F855365D8E6") };

        public string PaymentPercentageMonth3_Notes { get; set; }
        public static ModelMember COL_PaymentPercentageMonth3_Notes = new ModelMember { Name = "PaymentPercentageMonth3_Notes" };

        /******************************************************************************************************************************************************/
    }
}