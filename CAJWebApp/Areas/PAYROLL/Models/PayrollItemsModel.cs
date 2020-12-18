using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.PAYROLL.Models
{
    [Table("DWSystem.PayrollItems")]
    public class PayrollItemsModel
    {
        public Guid? Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        public Guid? Payrolls_Id { get; set; }
        public static ModelMember COL_Payrolls_Id = new ModelMember { Name = "Payrolls_Id", Display = "" };


        public Guid? PayrollDeductions_Id { get; set; }
        public static ModelMember COL_PayrollDeductions_Id = new ModelMember { Name = "PayrollDeductions_Id", Display = "" };


        public Guid? PayrollEarnings_Id { get; set; }
        public static ModelMember COL_PayrollEarnings_Id = new ModelMember { Name = "PayrollEarnings_Id", Display = "" };


        public Guid? PayrollDebts_Id { get; set; }
        public static ModelMember COL_PayrollDebts_Id = new ModelMember { Name = "PayrollDebts_Id", Display = "" };


        public string CategoryName { get; set; }
        public static ModelMember COL_CategoryName = new ModelMember { Name = "CategoryName", Display = "" };


        public byte RowNo { get; set; }
        public static ModelMember COL_RowNo = new ModelMember { Name = "RowNo", Display = "" };


        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "" };


        public int? Amount { get; set; }
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "" };

        /******************************************************************************************************************************************************/

    }
}