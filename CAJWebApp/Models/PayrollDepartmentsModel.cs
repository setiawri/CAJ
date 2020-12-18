using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Models
{
    [Table("DWSystem.PayrollDepartments")]
    public class PayrollDepartmentsModel
    {
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };

        [Required]
        [Display(Name = "Nama")]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Nama" };

        /******************************************************************************************************************************************************/
    }
}