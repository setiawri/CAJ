using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Areas.Reimbursement.Models;

namespace CAJWebApp.Areas.Reimbursement.Controllers
{
    public class ReimbursementItemsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        public static List<ReimbursementItemsModel> get(DBContext db, Guid Reimbursements_Id)
        {
            return db.ReimbursementItemsModel.Where(x => x.Reimbursements_Id == Reimbursements_Id).OrderBy(x => x.RowNumber).ToList();
        }

        public static List<ReimbursementItemsModel> get(DBContext db)
        {
            return db.ReimbursementItemsModel.ToList();
        }

        /******************************************************************************************************************************************************/
    }
}