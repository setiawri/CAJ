using CAJWebApp.Areas.PAYROLL.Controllers;
using CAJWebApp.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace CAJWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginValidationAttribute());
        }
    }

    public class LoginValidationAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.ActionDescriptor.ActionName != nameof(OperatorController.Login)
                && context.ActionDescriptor.ActionName != nameof(PayrollsController.PrintToPdf)
                && context.ActionDescriptor.ActionName != nameof(PayrollsController.Print)
            ) 
            {
                if (!OperatorController.isLoggedIn(context.HttpContext.Session))
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new
                        {
                            action = OperatorController.LOGIN_ACTIONNAME,
                            controller = OperatorController.LOGIN_CONTROLLERNAME,
                            Area = OperatorController.LOGIN_AREANAME,
                            returnUrl = context.HttpContext.Request.RawUrl
                        })
                    );
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
