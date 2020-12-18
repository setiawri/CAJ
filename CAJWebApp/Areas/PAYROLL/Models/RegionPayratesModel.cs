using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Areas.PAYROLL.Models
{
    [Table("DWSystem.RegionPayrates")]
    public class RegionPayratesModel
    {
        /* DATABASE COLUMNS ***********************************************************************************************************************************/

        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };

        [Display(Name = "Wilayah")]
        public Guid Regions_Id { get; set; }
        public static ModelMember COL_Regions_Id = new ModelMember { Name = "Regions_Id", Display = "Wilayah" };

        [Display(Name = "Realisasi UMP")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int MinimumPayrate { get; set; }
        public static ModelMember COL_MinimumPayrate = new ModelMember { Name = "MinimumPayrate", Display = "Realisasi UMP" };

        [Display(Name = "Normal / Hari")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RegularPayrate { get; set; }
        public static ModelMember COL_RegularPayrate = new ModelMember { Name = "RegularPayrate", Display = "Normal / Hari" };

        [Display(Name = "Lembur Normal / Jam")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int RegularOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_RegularOvertimeHourlyPayrate = new ModelMember { Name = "RegularOvertimeHourlyPayrate", Display = "Lembur Normal / Jam" };

        [Display(Name = "Hari Besar / Hari")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int HolidayPayrate { get; set; }
        public static ModelMember COL_HolidayPayrate = new ModelMember { Name = "HolidayPayrate", Display = "Hari Besar / Hari" };

        [Display(Name = "Lembur Hari Besar / Jam")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int HolidayOvertimeHourlyPayrate { get; set; }
        public static ModelMember COL_HolidayOvertimeHourlyPayrate = new ModelMember { Name = "HolidayOvertimeHourlyPayrate", Display = "Lembur Hari Besar / Jam" };

        /* ADDITIONAL PROPERTIES ******************************************************************************************************************************/

        [Display(Name = "Wilayah")]
        public string Regions_Name { get; set; }
        public const string COL_Regions_Name = "Regions_Name";

        /******************************************************************************************************************************************************/
    }
}