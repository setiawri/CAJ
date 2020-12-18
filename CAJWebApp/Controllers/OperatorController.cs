using CAJWebApp.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAJWebApp.Controllers
{
    public class AccessList
    {
        public OperatorPrivilegePayrollModel OperatorPrivilegePayrollModel;

        public AccessList() { }

        public void populate(OperatorPrivilegePayrollModel operatorPrivilegePayrollModel)
        {
            OperatorPrivilegePayrollModel = operatorPrivilegePayrollModel;
        }
    }

    public class OperatorController : Controller
    {
        public const string LOGIN_ACTIONNAME = "Login";
        public const string LOGIN_CONTROLLERNAME = "Operator";
        public const string LOGIN_AREANAME = "";

        public const string SESSION_UserId = "UserId";
        public const string SESSION_Username = "Username";

        public const string SESSION_OperatorPrivilegePayrollModel_PayrollApproval = "OperatorPrivilegePayrollModel_PayrollApproval";
        public const string SESSION_OperatorPrivilegePayrollModel_ReimbursementApproval = "OperatorPrivilegePayrollModel_ReimbursementApproval";

        private readonly DBContext db = new DBContext();

        /* LOGIN PAGE *****************************************************************************************************************************************/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(OperatorModel model, string returnUrl)
        {
            //bypass login
            if (true && Server.MachineName == "RQ-ASUS")
            {
                if(string.IsNullOrEmpty(model.UserName))
                    model.UserName = "ricky";
                if (string.IsNullOrEmpty(model.Password))
                    model.Password = "A2cdefGH";
            }

            string hashedPassword = HashPassword(model.Password);
            var result = (
                        from OperatorModel in db.OperatorModel
                        join OperatorPrivilegePayrollModel in db.OperatorPrivilegePayrollModel on OperatorModel.UserName equals OperatorPrivilegePayrollModel.UserName
                        where OperatorModel.UserName.ToLower() == model.UserName.ToLower()
                            && OperatorModel.Password.ToLower() == hashedPassword.ToLower()
                            && OperatorPrivilegePayrollModel.PayrollModule == true
                        select new { OperatorModel, OperatorPrivilegePayrollModel }
                ).FirstOrDefault();

            if (result == null)
                ModelState.AddModelError("", "Invalid username or password");
            else
            {
                AccessList accessList = new AccessList();
                accessList.populate(result.OperatorPrivilegePayrollModel);

                setLoginSession(Session, result.OperatorModel.ID, result.OperatorModel.UserName, accessList);
                return RedirectToLocal(returnUrl);
            }

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public ActionResult LogOff()
        {
            setLoginSession(Session, null, null, new AccessList());
            return RedirectToAction(nameof(Login));
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public static string HashPassword(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            else
            {
                var hash = new System.Security.Cryptography.SHA1Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
        }

        public static int getUserId(HttpSessionStateBase Session)
        {
            return int.Parse(Session[SESSION_UserId].ToString());
        }

        public static bool isLoggedIn(HttpSessionStateBase Session)
        {
            return Session[SESSION_UserId] != null;
        }

        private static void setLoginSession(HttpSessionStateBase Session, object userId, object username, AccessList accessList)
        {
            Session[SESSION_UserId] = userId == null ? null : userId.ToString();
            Session[SESSION_Username] = username == null ? null : username.ToString();

            if (Session[SESSION_UserId] != null)
            {
                Session[SESSION_OperatorPrivilegePayrollModel_PayrollApproval] = accessList.OperatorPrivilegePayrollModel.Approval;
                Session[SESSION_OperatorPrivilegePayrollModel_ReimbursementApproval] = accessList.OperatorPrivilegePayrollModel.ReimbursementApproval;
            }
        }

        /******************************************************************************************************************************************************/
    }
}