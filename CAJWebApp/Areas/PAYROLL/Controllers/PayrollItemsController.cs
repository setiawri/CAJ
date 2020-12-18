using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollItemsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        public const string MANDATORYDEPOSITDESCRIPTION = "Tabungan Wajib";

        public static List<PayrollItemsModel> GetEarnings(DBContext db, Guid Payrolls_Id) { return Get(db, Payrolls_Id, true, false, false, false); }
        public static List<PayrollItemsModel> GetDeductions(DBContext db, Guid Payrolls_Id) { return Get(db, Payrolls_Id, false, true, false, false); }
        public static List<PayrollItemsModel> GetDebts(DBContext db, Guid Payrolls_Id) { return Get(db, Payrolls_Id, false, false, true, false); }
        public static List<PayrollItemsModel> GetMandatoryDeposits(DBContext db, Guid Payrolls_Id) { return Get(db, Payrolls_Id, false, false, false, true); }
        public static List<PayrollItemsModel> Get(DBContext db, Guid Payrolls_Id, bool earningsOnly, bool deductionsOnly, bool debtsOnly, bool mandatoryDepositsOnly)
        {
            if (earningsOnly)
                return db.PayrollItemsModel
                    .Where(x => x.Payrolls_Id == Payrolls_Id && x.PayrollEarnings_Id != null)
                    .OrderBy(x => x.RowNo)
                    .ToList();
            else if (deductionsOnly)
                return db.PayrollItemsModel
                    .Where(x => x.Payrolls_Id == Payrolls_Id && x.PayrollDeductions_Id != null)
                    .OrderBy(x => x.RowNo)
                    .ToList();
            else if (debtsOnly)
                return db.PayrollItemsModel
                    .Where(x => x.Payrolls_Id == Payrolls_Id && x.PayrollDebts_Id != null)
                    .OrderBy(x => x.RowNo)
                    .ToList();
            else if (mandatoryDepositsOnly)
                return db.PayrollItemsModel
                    .Where(x => x.Payrolls_Id == Payrolls_Id && x.Description == MANDATORYDEPOSITDESCRIPTION
                            && x.PayrollEarnings_Id == null && x.PayrollDeductions_Id == null && x.PayrollDebts_Id == null)
                    .OrderBy(x => x.RowNo)
                    .ToList();
            else
                return null;
        }

        public static Guid? update(DBContext db, PayrollsModel originalModel, PayrollsModel modifiedModel)
        {
            //Mandatory Deposit
            if (originalModel.MandatoryDepositUpdateAmount != modifiedModel.MandatoryDepositUpdateAmount)
            {
                if (modifiedModel.MandatoryDeposit_PayrollItems_Id.HasValue) //edit existing
                {
                    PayrollItemsModel model = db.PayrollItemsModel.Find(modifiedModel.MandatoryDeposit_PayrollItems_Id);
                    model.Amount = -1 * modifiedModel.MandatoryDepositUpdateAmount;
                    db.Entry(model).State = EntityState.Modified;
                }
                else //create new
                {
                    modifiedModel.MandatoryDeposit_PayrollItems_Id = add(db, modifiedModel, modifiedModel.MandatoryDepositUpdateAmount);
                }
            }

            return modifiedModel.MandatoryDeposit_PayrollItems_Id;
        }

        public static Guid? add(DBContext db, PayrollsModel model, int amount)
        {
            model.MandatoryDeposit_PayrollItems_Id = Guid.NewGuid();
            db.PayrollItemsModel.Add(new PayrollItemsModel
            {
                Id = model.MandatoryDeposit_PayrollItems_Id,
                Payrolls_Id = model.Id,
                RowNo = 0,
                Description = PayrollItemsController.MANDATORYDEPOSITDESCRIPTION,
                Amount = -1 * amount
            });

            return model.MandatoryDeposit_PayrollItems_Id;
        }

        /******************************************************************************************************************************************************/
    }
}