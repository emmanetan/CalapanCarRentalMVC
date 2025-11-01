# ?? Developer Quick Reference - Authorization Usage

## How to Use SessionAuthorization Attribute

### Basic Usage

```csharp
using CalapanCarRentalMVC.Filters;

// 1. Require any authenticated user
[SessionAuthorization(RequireAuthentication = true)]
public IActionResult MyAction() { }

// 2. Require specific role
[SessionAuthorization(Roles = new[] { "Admin" })]
public IActionResult AdminOnly() { }

// 3. Require multiple roles (OR logic)
[SessionAuthorization(Roles = new[] { "Admin", "Customer" })]
public IActionResult AdminOrCustomer() { }

// 4. Apply to entire controller
[SessionAuthorization(Roles = new[] { "Admin" })]
public class MyController : Controller
{
 // All actions require Admin role
}

// 5. Override controller-level authorization for specific action
[SessionAuthorization(Roles = new[] { "Admin" })]
public class MyController : Controller
{
    // This action requires Admin (inherited from controller)
    public IActionResult Action1() { }
    
    // This action also requires Admin AND Customer
    [SessionAuthorization(Roles = new[] { "Admin", "Customer" })]
    public IActionResult Action2() { }
}
```

---

## Common Patterns

### Pattern 1: Public Index, Protected CRUD
```csharp
public class CarsController : Controller
{
    // Public - anyone can view
    public async Task<IActionResult> Index() { }
    
    // Public - anyone can view details
    public async Task<IActionResult> Details(int id) { }
    
    // Protected - Admin only
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public IActionResult Create() { }
    
  [HttpPost]
 [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Create(Car car) { }
    
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Edit(int id) { }
    
    [HttpPost]
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Edit(int id, Car car) { }
    
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Delete(int id) { }
}
```

### Pattern 2: Entire Controller Protected
```csharp
[SessionAuthorization(Roles = new[] { "Admin" })]
public class CustomersController : Controller
{
    // All actions require Admin role
    public async Task<IActionResult> Index() { }
    public async Task<IActionResult> Details(int id) { }
    public IActionResult Create() { }
    public async Task<IActionResult> Edit(int id) { }
    public async Task<IActionResult> Delete(int id) { }
}
```

### Pattern 3: Authentication Only (Any Role)
```csharp
[SessionAuthorization(RequireAuthentication = true)]
public class NotificationsController : Controller
{
    // Any authenticated user can access
    public async Task<IActionResult> Index() { }
    public async Task<IActionResult> GetRecent() { }
}
```

### Pattern 4: Mixed Permissions
```csharp
[SessionAuthorization(RequireAuthentication = true)]
public class RentalsController : Controller
{
    // Any authenticated user
    public async Task<IActionResult> Index() { }
    
    // Any authenticated user
    public async Task<IActionResult> Details(int id) { }
    
    // Customer only
[SessionAuthorization(Roles = new[] { "Customer" })]
    public IActionResult Create() { }
    
    // Admin only
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Edit(int id) { }
    
    // Admin only
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Delete(int id) { }
}
```

---

## What Happens When Authorization Fails?

### Scenario 1: User Not Logged In
```
User: Guest (not logged in)
Tries: /Customers/Index
Result: Redirects to /Account/Login
```

### Scenario 2: User Logged In But Wrong Role
```
User: Customer (logged in)
Tries: /Customers/Index (requires Admin)
Result: Redirects to /Account/AccessDenied
```

### Scenario 3: User Has Correct Role
```
User: Admin (logged in)
Tries: /Customers/Index (requires Admin)
Result: Access granted ?
```

---

## Checking User Role in Views

```razor
@* Check if user is logged in *@
@if (!string.IsNullOrEmpty(Context.Session.GetString("UserId")))
{
    <p>Welcome, @Context.Session.GetString("Username")!</p>
}

@* Check user role *@
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <a asp-action="Create">Add New</a>
}

@* Show different content based on role *@
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <div class="admin-panel">...</div>
}
else if (Context.Session.GetString("UserRole") == "Customer")
{
    <div class="customer-panel">...</div>
}
else
{
    <div class="guest-panel">...</div>
}
```

---

## Checking User Role in Controllers

```csharp
public async Task<IActionResult> MyAction()
{
    var userRole = HttpContext.Session.GetString("UserRole");
    var userId = HttpContext.Session.GetString("UserId");
    var username = HttpContext.Session.GetString("Username");
    
    if (userRole == "Admin")
    {
        // Admin-specific logic
    }
 else if (userRole == "Customer")
    {
   // Customer-specific logic
    }
  
  return View();
}
```

---

## Best Practices

### ? DO

```csharp
// ? Use controller-level authorization when all actions need same role
[SessionAuthorization(Roles = new[] { "Admin" })]
public class AdminOnlyController : Controller { }

// ? Be explicit about authentication requirement
[SessionAuthorization(RequireAuthentication = true)]
public IActionResult MyAction() { }

// ? Apply to both GET and POST
[SessionAuthorization(Roles = new[] { "Admin" })]
public IActionResult Create() { }

[HttpPost]
[SessionAuthorization(Roles = new[] { "Admin" })]
public async Task<IActionResult> Create(MyModel model) { }

// ? Check user owns resource
[SessionAuthorization(RequireAuthentication = true)]
public async Task<IActionResult> Edit(int id)
{
    var userId = HttpContext.Session.GetString("UserId");
    var item = await _context.Items.FindAsync(id);
    
    if (item.OwnerId != int.Parse(userId))
    {
        return RedirectToAction("AccessDenied", "Account");
    }
  
    return View(item);
}
```

### ? DON'T

```csharp
// ? Don't forget POST actions
[SessionAuthorization(Roles = new[] { "Admin" })]
public IActionResult Create() { }

[HttpPost]  // ? MISSING AUTHORIZATION!
public async Task<IActionResult> Create(MyModel model) { }

// ? Don't rely only on UI hiding
// View.cshtml
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <a asp-action="Delete">Delete</a>  // ? Hidden in UI but URL still accessible!
}

// ? Don't forget to validate in controller
public IActionResult Delete(int id)  // ? No authorization check!
{
    // Anyone can delete by typing URL!
}
```

---

## Common Mistakes to Avoid

### Mistake 1: Securing GET but not POST
```csharp
// ? WRONG
[SessionAuthorization(Roles = new[] { "Admin" })]
public IActionResult Create() { }

[HttpPost]
public async Task<IActionResult> Create(Car car)  // ? Missing authorization!
{
    // Attacker can POST directly!
}

// ? CORRECT
[SessionAuthorization(Roles = new[] { "Admin" })]
public IActionResult Create() { }

[HttpPost]
[SessionAuthorization(Roles = new[] { "Admin" })]
public async Task<IActionResult> Create(Car car)
{
    // Now secure!
}
```

### Mistake 2: UI-Only Security
```csharp
// ? WRONG - Only hiding in UI
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <a asp-action="Delete">Delete</a>
}

// Controller has no authorization
public async Task<IActionResult> Delete(int id)  // ? No protection!
{
    // Attacker can still access /Controller/Delete/1
}

// ? CORRECT - Secure controller AND hide in UI
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <a asp-action="Delete">Delete</a>
}

// Controller properly secured
[SessionAuthorization(Roles = new[] { "Admin" })]
public async Task<IActionResult> Delete(int id)
{
    // Now protected!
}
```

### Mistake 3: Inconsistent Authorization
```csharp
// ? WRONG - Inconsistent
public class CarsController : Controller
{
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public IActionResult Create() { }
    
    public async Task<IActionResult> Edit(int id) { }  // ? Missing!
    
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Delete(int id) { }
}

// ? CORRECT - Consistent
public class CarsController : Controller
{
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public IActionResult Create() { }
    
    [SessionAuthorization(Roles = new[] { "Admin" })]
    public async Task<IActionResult> Edit(int id) { }
    
    [SessionAuthorization(Roles = new[] { "Admin" })]
  public async Task<IActionResult> Delete(int id) { }
}
```

---

## Testing Your Authorization

### Manual Testing
```bash
# 1. Test as guest
# Open incognito window
# Try: https://localhost:7277/Customers/Index
# Expected: Redirect to /Account/Login

# 2. Test as customer
# Login as customer
# Try: https://localhost:7277/Customers/Index
# Expected: Redirect to /Account/AccessDenied

# 3. Test as admin
# Login as admin
# Try: https://localhost:7277/Customers/Index
# Expected: Success! Page loads
```

### Unit Testing
```csharp
[Fact]
public void CustomersIndex_WithoutLogin_RedirectsToLogin()
{
    // Arrange
    var controller = new CustomersController(_context);
    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };
    
    // Act
    var result = controller.Index();
  
    // Assert
    Assert.IsType<RedirectToActionResult>(result);
}
```

---

## Troubleshooting

### Issue: Always Redirects to Login
**Problem:** Authorization attribute applied to Login/Register actions

**Solution:**
```csharp
public class AccountController : Controller
{
    // ? WRONG - Don't add authorization to Login!
    [SessionAuthorization(RequireAuthentication = true)]
    public IActionResult Login() { }  // ? Creates redirect loop!
    
    // ? CORRECT - No authorization on Login/Register
    public IActionResult Login() { }
    public IActionResult Register() { }
}
```

### Issue: Session Lost After Page Change
**Problem:** Session not configured properly

**Solution:** Check `Program.cs`:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
options.Cookie.IsEssential = true;
});

// Must call UseSession() before UseAuthorization()
app.UseSession();
app.UseAuthorization();
```

---

## Quick Checklist for New Controllers

When creating a new controller, ask:

- [ ] Should guests access this? ? No authorization needed
- [ ] Should logged-in users access this? ? `[SessionAuthorization(RequireAuthentication = true)]`
- [ ] Should only admins access this? ? `[SessionAuthorization(Roles = new[] { "Admin" })]`
- [ ] Should only customers access this? ? `[SessionAuthorization(Roles = new[] { "Customer" })]`
- [ ] Mixed permissions? ? Apply authorization per action
- [ ] Secured both GET and POST? ? Check both have authorization
- [ ] Tested as guest? ? Should redirect to login
- [ ] Tested as wrong role? ? Should redirect to access denied
- [ ] Tested as correct role? ? Should work

---

## Summary

**3 Simple Rules:**

1. **Always secure admin actions** with `[SessionAuthorization(Roles = new[] { "Admin" })]`
2. **Secure both GET and POST** actions
3. **Test your authorization** - try accessing as guest, customer, and admin

**When in doubt, secure it!** It's better to be too restrictive than too permissive.

---

**Reference:** See `SECURITY_FIXES_DOCUMENTATION.md` for complete details.
