using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.Reimbursement.Models
{
    [Table("DWSystem.ReimbursementItems")]
    public class ReimbursementItemsModel
    {
        [Key]
        public Guid? Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        public byte RowNumber { get; set; }
        public static ModelMember COL_RowNumber = new ModelMember { Name = "RowNumber", Display = "" };


        public Guid? Reimbursements_Id { get; set; }
        public static ModelMember COL_Reimbursements_Id = new ModelMember { Name = "Reimbursements_Id", Display = "" };


        public Guid? ReimbursementCategories_Id { get; set; }
        public static ModelMember COL_ReimbursementCategories_Id = new ModelMember { Name = "ReimbursementCategories_Id", Display = "" };

        public string CategoryName { get; set; }
        public static ModelMember COL_CategoryName = new ModelMember { Name = "CategoryName", Display = "" };


        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "" };


        public int? Amount { get; set; }
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "" };

        /******************************************************************************************************************************************************/

    }
}