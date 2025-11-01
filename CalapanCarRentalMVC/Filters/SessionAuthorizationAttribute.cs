using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CalapanCarRentalMVC.Filters
{
    /// <summary>
    /// Custom authorization attribute that checks session-based authentication and role authorization.
    /// Redirects unauthorized users to the login page.
    /// </summary>
    public class SessionAuthorizationAttribute : ActionFilterAttribute
    {
        public string[]? Roles { get; set; }
        public bool RequireAuthentication { get; set; } = true;

   public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("UserRole");
          var userId = session.GetString("UserId");

  // Check if user is authenticated
            if (RequireAuthentication && string.IsNullOrEmpty(userId))
            {
      context.Result = new RedirectToActionResult("Login", "Account", null);
   return;
  }

      // Check if user has required role
  if (Roles != null && Roles.Length > 0)
            {
 if (string.IsNullOrEmpty(userRole) || !Roles.Contains(userRole))
      {
   // User is authenticated but doesn't have the required role
               context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
    return;
    }
    }

      base.OnActionExecuting(context);
    }
    }
}
