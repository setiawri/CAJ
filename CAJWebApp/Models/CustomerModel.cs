using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAJWebApp.Models
{
    [Table("DWSystem.Customer")]
    public class CustomerModel
    {
        [Key]
        public string CustomerID { get; set; }
        public static ModelMember COL_CustomerID = new ModelMember { Name = "CustomerID" };

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name" };

        public string Address { get; set; }
        public string BillToName { get; set; }
        public string BillToAddress { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public int CountryID { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int SalesID { get; set; }
        public string MarketType { get; set; }
        public string OutletType { get; set; }
        public int CurrencyID { get; set; }
        public double Discount { get; set; }
        public double ARLimit { get; set; }
        public int ARTerm { get; set; }
        public int FreightCurrencyID { get; set; }
        public double Freight { get; set; }
        public bool Tax { get; set; }
        public string CompanyGroup { get; set; }
        public string NPWP { get; set; }
        public bool TaxInvoice { get; set; }
        public string TaxName { get; set; }
        public string TaxAddress { get; set; }
        public int? WarehouseID { get; set; }
        public Guid? Regions_Id { get; set; }
    }
}