using System.Data.Entity;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Areas.Reimbursement.Models;
using CAJWebApp.Models;

namespace CAJWebApp
{
    public class DBContext : DbContext
    {
        /* ROOT ***********************************************************************************************************************************************/

        public DbSet<ActivityLogsModel> ActivityLogsModel { get; set; }
        public DbSet<SettingsModel> SettingsModel { get; set; }
        public DbSet<BanksModel> BanksModel { get; set; }
        public DbSet<RegionsModel> RegionsModel { get; set; }
        public DbSet<EmploymentTypesModel> EmploymentTypesModel { get; set; }
        public DbSet<MaritalStatusesModel> MaritalStatusesModel { get; set; }
        public DbSet<CounterAreasModel> CounterAreasModel { get; set; }
        public DbSet<PayrollDepartmentsModel> PayrollDepartmentsModel { get; set; }
        public DbSet<PayrollEmployeesModel> PayrollEmployeesModel { get; set; }
        public DbSet<CustomerModel> CustomerModel { get; set; }

        /* PAYROLL ********************************************************************************************************************************************/

        public DbSet<RegionPayratesModel> RegionPayratesModel { get; set; }
        public DbSet<PayrollEarningsModel> PayrollEarningsModel { get; set; }
        public DbSet<PayrollDeductionsModel> PayrollDeductionsModel { get; set; }
        public DbSet<PayrollDebtsModel> PayrollDebtsModel { get; set; }
        public DbSet<PayrollPaymentDatesModel> PayrollPaymentDatesModel { get; set; }
        public DbSet<PayrollItemsModel> PayrollItemsModel { get; set; }
        public DbSet<PayrollPaymentsModel> PayrollPaymentsModel { get; set; }

        /* REIMBURSEMENT **************************************************************************************************************************************/

        public DbSet<ReimbursementPaymentDatesModel> ReimbursementPaymentDatesModel { get; set; }
        public DbSet<ReimbursementCategoriesModel> ReimbursementCategoriesModel { get; set; }
        public DbSet<ReimbursementItemsModel> ReimbursementItemsModel { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<OperatorModel> OperatorModel { get; set; }
        public DbSet<OperatorPrivilegePayrollModel> OperatorPrivilegePayrollModel { get; set; }

        /******************************************************************************************************************************************************/
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<SettingsModel>().Property(x => x.Value_Decimal).HasPrecision(18, 12);
            //modelBuilder.Entity<PayrollsModel>().Property(x => x.RegularWorkDay).HasPrecision(18, 5);
            //modelBuilder.Entity<PayrollsModel>().Property(x => x.RegularOvertimeWorkHour).HasPrecision(18, 5);
            //modelBuilder.Entity<PayrollsModel>().Property(x => x.HolidayWorkDay).HasPrecision(18, 5);
            //modelBuilder.Entity<PayrollsModel>().Property(x => x.HolidayOvertimeWorkHour).HasPrecision(18, 5);
        }
    }
}