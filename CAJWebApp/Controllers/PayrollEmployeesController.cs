using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Models;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Areas.Reimbursement.Models;
using LIBUtil;

namespace CAJWebApp.Controllers
{
    public class PayrollEmployeesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string search, string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                Helper.setFilterViewBag(this, null, null, null, null, null, null, search, null, null, FILTER_Keyword, FILTER_Active);
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        [HttpPost]
        public ActionResult Index(string search, string FILTER_Keyword, int? FILTER_Active)
        {
            Helper.setFilterViewBag(this, null, null, null, null, null, null, search, null, null, FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PayrollEmployees/Create
        public ActionResult Create()
        {
            populateViewBag();
            return View(new PayrollEmployeesModel());
        }

        // POST: PayrollEmployees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayrollEmployeesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Fullname))
                    ModelState.AddModelError(PayrollEmployeesModel.COL_Fullname.Name, "Sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            populateViewBag();
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PayrollEmployees/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            populateViewBag();
            return View(get((Guid)id));
        }

        // POST: PayrollEmployees/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollEmployeesModel modifiedModel)
        {
            if (modifiedModel.WorkHourPerDay == 0)
                ModelState.AddModelError("WorkHourPerDay", "Jam Kerja / Hari harus lebih dari 0");

            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Fullname))
                    ModelState.AddModelError(PayrollEmployeesModel.COL_Fullname.Name, "Sudah terdaftar");
                else
                {
                    PayrollEmployeesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;

                    log = Util.webAppendChange(log, originalModel.Identification, modifiedModel.Identification, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Identification.Display));
                    log = Util.webAppendChange(log, originalModel.Fullname, modifiedModel.Fullname, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Fullname.Display));
                    log = Util.webAppendChange(log, originalModel.Customer_Name, modifiedModel.Customer_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Customer_Name.Display));
                    log = Util.webAppendChange(log, originalModel.JoinDate, modifiedModel.JoinDate, ActivityLogsController.editDateFormat(PayrollEmployeesModel.COL_JoinDate.Display));
                    log = Util.webAppendChange(log, originalModel.WorkHourPerDay, modifiedModel.WorkHourPerDay, ActivityLogsController.editIntFormat(PayrollEmployeesModel.COL_WorkHourPerDay.Display));
                    log = Util.webAppendChange(log, originalModel.PayrollPaymentDates_PayDate, modifiedModel.PayrollPaymentDates_PayDate, ActivityLogsController.editIntFormat(PayrollEmployeesModel.COL_PayrollPaymentDates_PayDate.Display));
                    log = Util.webAppendChange(log, originalModel.ReimbursementPaymentDates_PayDate, modifiedModel.ReimbursementPaymentDates_PayDate, ActivityLogsController.editIntFormat(PayrollEmployeesModel.COL_ReimbursementPaymentDates_PayDate.Display));
                    log = Util.webAppendChange(log, originalModel.Banks_Name, modifiedModel.Banks_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Banks_Name.Display));
                    log = Util.webAppendChange(log, originalModel.AccountName, modifiedModel.AccountName, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_AccountName.Display));
                    log = Util.webAppendChange(log, originalModel.AccountNumber, modifiedModel.AccountNumber, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_AccountNumber.Display));
                    log = Util.webAppendChange(log, originalModel.EmploymentTypes_Name, modifiedModel.EmploymentTypes_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_EmploymentTypes_Name.Display));
                    log = Util.webAppendChange(log, originalModel.PayrollDepartments_Name, modifiedModel.PayrollDepartments_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_PayrollDepartments_Name.Display));
                    log = Util.webAppendChange(log, originalModel.CounterAreas_Name, modifiedModel.CounterAreas_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_CounterAreas_Name.Display));
                    log = Util.webAppendChange(log, originalModel.Birthdate, modifiedModel.Birthdate, ActivityLogsController.editDateFormat(PayrollEmployeesModel.COL_Birthdate.Display));
                    log = Util.webAppendChange(log, originalModel.Address, modifiedModel.Address, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Address.Display));
                    log = Util.webAppendChange(log, originalModel.Phone1, modifiedModel.Phone1, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Phone1.Display));
                    log = Util.webAppendChange(log, originalModel.Phone2, modifiedModel.Phone2, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Phone2.Display));
                    log = Util.webAppendChange(log, originalModel.MaritalStatuses_Name, modifiedModel.MaritalStatuses_Name, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_MaritalStatuses_Name.Display));
                    log = Util.webAppendChange(log, originalModel.DependentCount, modifiedModel.DependentCount, ActivityLogsController.editIntFormat(PayrollEmployeesModel.COL_DependentCount.Display));
                    log = Util.webAppendChange(log, originalModel.Active, modifiedModel.Active, ActivityLogsController.editStringFormat(PayrollEmployeesModel.COL_Active.Display));

                    if (!string.IsNullOrEmpty(log))
                    {
                        update(modifiedModel);
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            populateViewBag();
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public bool isExists(Guid? id, object value)
        {
            return 1 == db.Database.SqlQuery<int>(@"
	                    IF EXISTS (SELECT id FROM DWSystem.PayrollEmployees WHERE Fullname = @Fullname AND (@id IS NULL OR id != @id))
		                    SELECT 1
	                    ELSE
		                    SELECT 0
                    ",
                    DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Fullname.Name, value),
                    DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Id.Name, id)
                ).FirstOrDefault();
        }

        private void populateViewBag()
        {
            ViewBag.Customer = new SelectList(db.CustomerModel.OrderBy(x => x.Name).ToList(), CustomerModel.COL_CustomerID.Name, CustomerModel.COL_Name.Name);
            ViewBag.Banks = new SelectList(db.BanksModel.OrderBy(x => x.Name).ToList(), BanksModel.COL_Id.Name, BanksModel.COL_Name.Name);
            ViewBag.MaritalStatuses = new SelectList(db.MaritalStatusesModel.OrderBy(x => x.Name).ToList(), MaritalStatusesModel.COL_Id.Name, MaritalStatusesModel.COL_Name.Name);
            ViewBag.EmploymentTypes = new SelectList(db.EmploymentTypesModel.OrderBy(x => x.Name).ToList(), EmploymentTypesModel.COL_Id.Name, EmploymentTypesModel.COL_Name.Name);
            ViewBag.CounterAreas = new SelectList(db.CounterAreasModel.OrderBy(x => x.Name).ToList(), CounterAreasModel.COL_Id.Name, CounterAreasModel.COL_Name.Name);
            ViewBag.PayrollPaymentDates = new SelectList(db.PayrollPaymentDatesModel.OrderBy(x => x.PayDate).ToList(), PayrollPaymentDatesModel.COL_Id.Name, PayrollPaymentDatesModel.COL_PayDate.Name);
            ViewBag.ReimbursementPaymentDates = new SelectList(db.ReimbursementPaymentDatesModel.OrderBy(x => x.PayDate).ToList(), ReimbursementPaymentDatesModel.COL_Id.Name, ReimbursementPaymentDatesModel.COL_PayDate.Name);
            ViewBag.PayrollDepartments = new SelectList(db.PayrollDepartmentsModel.OrderBy(x => x.Name).ToList(), PayrollDepartmentsModel.COL_Id.Name, PayrollDepartmentsModel.COL_Name.Name);

            SettingsModel settings = SettingsController.get(db);
            ViewBag.DefaultWorkHoursPerDay = settings.WorkHoursPerDay;
        }

        public List<PayrollEmployeesModel> get(string FILTER_Keyword, int? FILTER_Active) { return getData(null, false, null, EnumActionTypes.All, FILTER_Keyword, FILTER_Active); }
        public List<PayrollEmployeesModel> get(bool onlyActive, DateTime? payPeriod, EnumActionTypes actionType) { return getData(null, onlyActive, payPeriod, actionType, null, null); }
        public PayrollEmployeesModel get(Guid id) { return getData(id, false, null, EnumActionTypes.All, null, null).FirstOrDefault(); }
        private List<PayrollEmployeesModel> getData(Guid? id, bool onlyActive, DateTime? payPeriod, EnumActionTypes actionType, string FILTER_Keyword, int? FILTER_Active)
        {
            List< PayrollEmployeesModel> list = db.Database.SqlQuery<PayrollEmployeesModel>(@"
                        SELECT PayrollEmployees.*,
							Customer.Name AS Customer_Name,
							Regions.Id AS Regions_Id,
                            Regions.Name AS Regions_Name,
							PayrollPaymentDates.PayDate AS PayrollPaymentDates_PayDate,
							ReimbursementPaymentDates.PayDate AS ReimbursementPaymentDates_PayDate,
							Banks.Name AS Banks_Name,
							MaritalStatuses.Name AS MaritalStatuses_Name,
							EmploymentTypes.Name AS EmploymentTypes_Name,
							CounterAreas.Name AS CounterAreas_Name,
							PayrollDepartments.Name AS PayrollDepartments_Name,
							ISNULL(PayrollItems.MandatoryDepositBalance,0) * -1 AS MandatoryDepositBalance,
							ISNULL(Debt.Balance,0) AS DebtBalance
                        FROM DWSystem.PayrollEmployees
							LEFT JOIN DWSystem.Customer ON Customer.CustomerID = PayrollEmployees.Customer_CustomerID
                            LEFT JOIN DWSystem.Regions ON Regions.Id = Customer.Regions_Id
							LEFT JOIN DWSystem.PayrollPaymentDates ON PayrollPaymentDates.Id = PayrollEmployees.PayrollPaymentDates_Id
							LEFT JOIN DWSystem.ReimbursementPaymentDates ON ReimbursementPaymentDates.Id = PayrollEmployees.ReimbursementPaymentDates_Id
                            LEFT JOIN DWSystem.Banks ON Banks.Id = PayrollEmployees.Banks_Id
							LEFT JOIN DWSystem.MaritalStatuses ON MaritalStatuses.Id = PayrollEmployees.MaritalStatuses_Id
							LEFT JOIN DWSystem.EmploymentTypes ON EmploymentTypes.Id = PayrollEmployees.EmploymentTypes_Id
							LEFT JOIN DWSystem.CounterAreas ON CounterAreas.Id = PayrollEmployees.CounterAreas_Id
							LEFT JOIN DWSystem.PayrollDepartments ON PayrollDepartments.Id = PayrollEmployees.PayrollDepartments_Id
							LEFT JOIN (
									SELECT Payrolls.PayrollEmployees_Id, 
										ISNULL(SUM(MandatoryDeposit_PayrollItems.Amount),0) AS MandatoryDepositBalance
									FROM DWSystem.Payrolls
										LEFT JOIN DWSystem.PayrollItems MandatoryDeposit_PayrollItems ON MandatoryDeposit_PayrollItems.Id = Payrolls.MandatoryDeposit_PayrollItems_Id
									WHERE Payrolls.ApprovalOperator_ID IS NOT NULL
									GROUP BY Payrolls.PayrollEmployees_Id
								) PayrollItems ON PayrollItems.PayrollEmployees_Id = PayrollEmployees.Id
							LEFT JOIN (
									SELECT Payrolls.PayrollEmployees_Id, 
										ISNULL(SUM(PayrollItems.Amount),0) AS Balance
									FROM DWSystem.PayrollItems
										LEFT JOIN DWSystem.Payrolls ON Payrolls.Id = PayrollItems.Payrolls_Id
										LEFT JOIN DWSystem.PayrollEmployees ON PayrollEmployees.Id = Payrolls.PayrollEmployees_Id
									WHERE PayrollItems.PayrollDebts_Id IS NOT NULL
										AND Payrolls.ApprovalOperator_ID IS NOT NULL
									GROUP BY Payrolls.PayrollEmployees_Id
								) Debt ON Debt.PayrollEmployees_Id = PayrollEmployees.Id
                        WHERE 1=1
                            AND (@Id IS NULL OR PayrollEmployees.Id = @Id)
                            AND (@FILTER_OnlyActive = 0 OR (@FILTER_OnlyActive = 1 AND PayrollEmployees.Active = 1))
                            AND (@FILTER_Active IS NULL OR PayrollEmployees.Active = @FILTER_Active)
    						AND (@FILTER_Keyword IS NULL OR (PayrollEmployees.Fullname LIKE '%'+@FILTER_Keyword+'%'))
							AND (@PayPeriod IS NULL OR (
									@FILTER_ActionType = 0 OR (
											@FILTER_ActionType = 1 AND 
												PayrollEmployees.Id NOT IN (
													SELECT Payrolls.PayrollEmployees_Id
													FROM DWSystem.Payrolls
													WHERE Payrolls.PayPeriod = @PayPeriod
												)
										) OR (
											@FILTER_ActionType = 2 AND 
												PayrollEmployees.Id NOT IN (
													SELECT Reimbursements.PayrollEmployees_Id
													FROM DWSystem.Reimbursements
													WHERE Reimbursements.PayPeriod = @PayPeriod
												)
										)
									)
								)
						ORDER BY PayrollEmployees.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Id.Name, id),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, payPeriod),
                    DBConnection.getSqlParameter("FILTER_OnlyActive", onlyActive),
                    DBConnection.getSqlParameter("FILTER_ActionType", actionType),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_Active", FILTER_Active)
                ).ToList();

            return list;
        }

        private void add(PayrollEmployeesModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                    INSERT INTO DWSystem.PayrollEmployees (
                        Id,
                        Fullname,
                        Identification,
                        Customer_CustomerID,
                        JoinDate,
                        WorkHourPerDay,
                        PayrollPaymentDates_Id,
                        ReimbursementPaymentDates_Id,
                        Banks_Id,
                        AccountNumber,
                        AccountName,
                        Birthdate,
                        Address,
                        Phone1,
                        Phone2,
                        MaritalStatuses_Id,
                        DependentCount,
                        EmploymentTypes_Id,
                        CounterAreas_Id,
                        PayrollDepartments_Id,
                        Active
                    ) VALUES (
                        @Id,
                        @Fullname,
                        @Identification,
                        @Customer_CustomerID,
                        @JoinDate,
                        @WorkHourPerDay,
                        @PayrollPaymentDates_Id,
                        @ReimbursementPaymentDates_Id,
                        @Banks_Id,
                        @AccountNumber,
                        @AccountName,
                        @Birthdate,
                        @Address,
                        @Phone1,
                        @Phone2,
                        @MaritalStatuses_Id,
                        @DependentCount,
                        @EmploymentTypes_Id,
                        @CounterAreas_Id,
                        @PayrollDepartments_Id,
                        @Active
                    )
                ",
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Fullname.Name, model.Fullname),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Identification.Name, model.Identification),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Customer_CustomerID.Name, model.Customer_CustomerID),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_JoinDate.Name, model.JoinDate),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_WorkHourPerDay.Name, model.WorkHourPerDay),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_PayrollPaymentDates_Id.Name, model.PayrollPaymentDates_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_ReimbursementPaymentDates_Id.Name, model.ReimbursementPaymentDates_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Banks_Id.Name, model.Banks_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_AccountNumber.Name, model.AccountNumber),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_AccountName.Name, model.AccountName),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Birthdate.Name, model.Birthdate),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Address.Name, model.Address),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Phone1.Name, model.Phone1),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Phone2.Name, model.Phone2),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_MaritalStatuses_Id.Name, model.MaritalStatuses_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_DependentCount.Name, model.DependentCount),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_EmploymentTypes_Id.Name, model.EmploymentTypes_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_CounterAreas_Id.Name, model.CounterAreas_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_PayrollDepartments_Id.Name, model.PayrollDepartments_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Active.Name, model.Active)
            );
        }

        private void update(PayrollEmployeesModel modifiedModel)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.PayrollEmployees 
                    SET 
                        Fullname = @Fullname,
                        Identification = @Identification,
                        Customer_CustomerID = @Customer_CustomerID,
                        JoinDate = @JoinDate,
                        WorkHourPerDay = @WorkHourPerDay,
                        PayrollPaymentDates_Id = @PayrollPaymentDates_Id,
                        ReimbursementPaymentDates_Id = @ReimbursementPaymentDates_Id,
                        Banks_Id = @Banks_Id,
                        AccountNumber = @AccountNumber,
                        AccountName = @AccountName,
                        Birthdate = @Birthdate,
                        Address = @Address,
                        Phone1 = @Phone1,
                        Phone2 = @Phone2,
                        MaritalStatuses_Id = @MaritalStatuses_Id,
                        DependentCount = @DependentCount,
                        EmploymentTypes_Id = @EmploymentTypes_Id,
                        CounterAreas_Id = @CounterAreas_Id,
                        PayrollDepartments_Id = @PayrollDepartments_Id,
                        Active = @Active
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Id.Name, modifiedModel.Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Fullname.Name, modifiedModel.Fullname),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Identification.Name, modifiedModel.Identification),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Customer_CustomerID.Name, modifiedModel.Customer_CustomerID),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_JoinDate.Name, modifiedModel.JoinDate),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_WorkHourPerDay.Name, modifiedModel.WorkHourPerDay),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_PayrollPaymentDates_Id.Name, modifiedModel.PayrollPaymentDates_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_ReimbursementPaymentDates_Id.Name, modifiedModel.ReimbursementPaymentDates_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Banks_Id.Name, modifiedModel.Banks_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_AccountNumber.Name, modifiedModel.AccountNumber),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_AccountName.Name, modifiedModel.AccountName),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Birthdate.Name, modifiedModel.Birthdate),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Address.Name, modifiedModel.Address),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Phone1.Name, modifiedModel.Phone1),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Phone2.Name, modifiedModel.Phone2),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_MaritalStatuses_Id.Name, modifiedModel.MaritalStatuses_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_DependentCount.Name, modifiedModel.DependentCount),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_EmploymentTypes_Id.Name, modifiedModel.EmploymentTypes_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_CounterAreas_Id.Name, modifiedModel.CounterAreas_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_PayrollDepartments_Id.Name, modifiedModel.PayrollDepartments_Id),
                DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Active.Name, modifiedModel.Active)
            );
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller, DateTime payPeriod, EnumActionTypes actionType)
        {

            controller.ViewBag.PayrollEmployees = new SelectList(
                    new PayrollEmployeesController().get(true, payPeriod, actionType),
                    PayrollEmployeesModel.COL_Id.Name, PayrollEmployeesModel.COL_Fullname.Name
                );
        }

        /******************************************************************************************************************************************************/
    }
}