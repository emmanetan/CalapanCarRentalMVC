# ?? Security Testing Quick Reference

## Quick Test URLs (Copy & Paste into Browser)

### ? **These URLs Should NOW Redirect to Login (When Not Logged In)**

```
https://localhost:7277/Cars/Create
https://localhost:7277/Cars/Edit/1
https://localhost:7277/Cars/Delete/1
https://localhost:7277/Customers/Index
https://localhost:7277/Customers/Details/1
https://localhost:7277/Customers/Create
https://localhost:7277/Customers/Edit/1
https://localhost:7277/Customers/Delete/1
https://localhost:7277/Maintenance/Index
https://localhost:7277/Maintenance/Details/1
https://localhost:7277/Maintenance/Create
https://localhost:7277/Maintenance/Edit/1
https://localhost:7277/Maintenance/Delete/1
https://localhost:7277/Rentals/Index
https://localhost:7277/Rentals/Details/1
https://localhost:7277/Rentals/Create
https://localhost:7277/Rentals/Edit/1
https://localhost:7277/Rentals/Delete/1
https://localhost:7277/Rentals/Return/1
https://localhost:7277/Reports/Index
https://localhost:7277/Notifications/Index
https://localhost:7277/Admin/Dashboard
https://localhost:7277/Customer/Dashboard
```

### ? **These URLs Should STILL Be Accessible (Public Pages)**

```
https://localhost:7277/
https://localhost:7277/Home/Index
https://localhost:7277/Home/About
https://localhost:7277/Home/Contact
https://localhost:7277/Cars/Index
https://localhost:7277/Cars/Details/1
https://localhost:7277/Account/Login
https://localhost:7277/Account/Register
```

---

## Test Scenarios

### ?? **Test 1: Guest Access (Not Logged In)**

**Steps:**
1. Open browser in incognito/private mode
2. Navigate to `https://localhost:7277`
3. Try accessing protected URLs directly

**Expected Results:**
- ? All admin/customer pages redirect to `/Account/Login`
- ? Public pages (Home, Cars Index, Cars Details) remain accessible

---

### ?? **Test 2: Customer Role Access**

**Steps:**
1. Login as Customer
2. Try accessing admin-only URLs

**Try These URLs:**
```
https://localhost:7277/Cars/Create        ? Should redirect to Access Denied
https://localhost:7277/Customers/Index       ? Should redirect to Access Denied
https://localhost:7277/Maintenance/Index     ? Should redirect to Access Denied
https://localhost:7277/Reports/Index         ? Should redirect to Access Denied
https://localhost:7277/Admin/Dashboard       ? Should redirect to Access Denied
```

**These Should Work:**
```
https://localhost:7277/Customer/Dashboard    ? Allowed
https://localhost:7277/Customer/MyRentals    ? Allowed
https://localhost:7277/Customer/Profile      ? Allowed
https://localhost:7277/Rentals/Create        ? Allowed
https://localhost:7277/Cars/Index    ? Allowed
```

---

### ?? **Test 3: Admin Role Access**

**Steps:**
1. Login as Admin
2. Try accessing all admin URLs

**All These Should Work:**
```
https://localhost:7277/Admin/Dashboard       ? Allowed
https://localhost:7277/Cars/Create   ? Allowed
https://localhost:7277/Cars/Edit/1           ? Allowed
https://localhost:7277/Customers/Index    ? Allowed
https://localhost:7277/Maintenance/Index     ? Allowed
https://localhost:7277/Rentals/Index  ? Allowed
https://localhost:7277/Reports/Index         ? Allowed
```

---

## Expected Behavior Summary

| URL Pattern | Guest | Customer | Admin |
|-------------|-------|----------|-------|
| `/Home/*` | ? | ? | ? |
| `/Cars/Index` | ? | ? | ? |
| `/Cars/Details` | ? | ? | ? |
| `/Cars/Create` | ? Login | ? Denied | ? |
| `/Cars/Edit` | ? Login | ? Denied | ? |
| `/Cars/Delete` | ? Login | ? Denied | ? |
| `/Customers/*` | ? Login | ? Denied | ? |
| `/Maintenance/*` | ? Login | ? Denied | ? |
| `/Rentals/Index` | ? Login | ? | ? |
| `/Rentals/Details` | ? Login | ? | ? |
| `/Rentals/Create` | ? Login | ? | ? |
| `/Rentals/Edit` | ? Login | ? Denied | ? |
| `/Rentals/Delete` | ? Login | ? Denied | ? |
| `/Rentals/Return` | ? Login | ? Denied | ? |
| `/Reports/*` | ? Login | ? Denied | ? |
| `/Notifications/*` | ? Login | ? | ? |
| `/Admin/*` | ? Login | ? Denied | ? |
| `/Customer/*` | ? Login | ? | ? Denied |

**Legend:**
- ? = Allowed
- ? Login = Redirects to login page
- ? Denied = Redirects to Access Denied page

---

## What Changed?

### Before (Vulnerable) ?
```
Guest types: https://localhost:7277/Customers/Index
Result: Shows customer list (SECURITY BREACH!)
```

### After (Secured) ?
```
Guest types: https://localhost:7277/Customers/Index
Result: Redirects to /Account/Login
```

---

## Testing Checklist

- [ ] Test all protected URLs as guest (should redirect to login)
- [ ] Test all public URLs as guest (should be accessible)
- [ ] Login as Customer and test admin URLs (should see Access Denied)
- [ ] Login as Customer and test customer URLs (should work)
- [ ] Login as Admin and test all admin URLs (should work)
- [ ] Test logout functionality (session should be cleared)
- [ ] Test session timeout (after 30 minutes of inactivity)
- [ ] Check that Access Denied page displays correctly
- [ ] Verify navigation links don't show for unauthorized roles

---

## How to Verify Security

### Method 1: Browser DevTools
1. Open DevTools (F12)
2. Go to Network tab
3. Try accessing protected URL
4. Look for redirect (302) to `/Account/Login` or `/Account/AccessDenied`

### Method 2: Incognito Mode
1. Open incognito/private window
2. Directly type protected URL
3. Should redirect to login

### Method 3: Session Inspection
1. Login as Customer
2. Open DevTools > Application > Cookies
3. Note session cookie
4. Try accessing admin URL
5. Should redirect to Access Denied

---

## Common Issues & Solutions

### Issue: Still Can Access Protected Pages
**Solution:** Clear browser cache and cookies, then test again

### Issue: Access Denied Page Not Showing
**Solution:** Check that `AccessDenied.cshtml` exists in `Views/Account/`

### Issue: Redirect Loop
**Solution:** Check that Login and AccessDenied actions don't have `[SessionAuthorization]` attribute

### Issue: Session Lost After Page Refresh
**Solution:** Check session timeout settings in `Program.cs`

---

## Security Best Practices Implemented ?

- ? Authentication required for sensitive pages
- ? Role-based authorization (Admin vs Customer)
- ? Session-based security
- ? Redirect to login for unauthenticated users
- ? Access Denied page for unauthorized users
- ? Anti-forgery tokens on POST actions
- ? Server-side validation
- ? Consistent authorization across all controllers

---

## Next Steps

After confirming all tests pass:

1. ? Document any custom roles or permissions
2. ? Update user training materials
3. ? Conduct penetration testing
4. ? Review audit logs regularly
5. ? Plan for password hashing implementation
6. ? Set up monitoring for unauthorized access attempts

---

**Status:** ?? **SECURED**  
**Date Fixed:** January 2025  
**Build Status:** ? Successful  
**Tests:** All security tests should pass
