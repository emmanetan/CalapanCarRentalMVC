# ?? Testing Google Authentication

## Step-by-Step Testing Guide

---

## ? Prerequisites

1. Application is running
2. Database is up to date (migration applied)
3. Google credentials are in `appsettings.json`
4. Google Cloud Console has correct redirect URI

---

## ?? Test Scenarios

### Test 1: New User Registration with Google

**Steps:**
1. Navigate to `https://localhost:PORT/Account/Register`
2. Click **"Sign up with Google"** button
3. Select a Google account (or sign in)
4. Grant permissions when prompted
5. **Expected Result:**
   - Redirected to Customer Dashboard
   - Success message: "Account created successfully with Google! Please complete your profile."
   - User appears in database

**Verify in Database:**
```sql
SELECT * FROM Users WHERE ExternalLoginProvider = 'Google' ORDER BY CreatedAt DESC LIMIT 1;
```

Expected data:
- `Username`: Generated from email (e.g., john.doe)
- `Password`: Empty string
- `Email`: Your Google email
- `ExternalLoginProvider`: "Google"
- `ExternalLoginProviderId`: Long numeric string
- `Role`: "Customer"

---

### Test 2: Returning Google User Login

**Steps:**
1. Logout from the application
2. Navigate to `https://localhost:PORT/Account/Login`
3. Click **"Sign in with Google"** button
4. Google should remember you (instant login)
5. **Expected Result:**
   - Redirected to Customer Dashboard immediately
- No success message (already registered)
   - Same session established

**Verify:**
- Check session by viewing profile
- UserId should match database record

---

### Test 3: Traditional Login Still Works

**Steps:**
1. Logout
2. Navigate to Login page
3. Enter existing email/password credentials
4. Click **"Login"** button
5. **Expected Result:**
   - Traditional login works as before
   - No interference from Google auth

---

### Test 4: Google Login from Register Page

**Steps:**
1. Navigate to Register page
2. Click **"Sign up with Google"**
3. Use the same Google account as Test 1
4. **Expected Result:**
   - Should login (not create duplicate)
   - Redirected to dashboard
   - Same user record in database

---

### Test 5: Car Rental Redirect Flow

**Steps:**
1. Logout
2. Browse to a car (e.g., `https://localhost:PORT/Cars/Details/1`)
3. Click **"Rent This Car"**
4. Redirected to Login (not logged in)
5. Click **"Sign in with Google"**
6. Complete Google authentication
7. **Expected Result:**
   - After login, redirected to rental creation page
   - Car ID preserved in the flow

---

### Test 6: Email Conflict (Security Test)

**Steps:**
1. Register normally with email: `test@gmail.com` and password
2. Logout
3. Try to login with Google using `test@gmail.com`
4. **Expected Result:**
   - Error message: "An account with this email already exists. Please login with your password or contact support."
   - No account hijacking possible

---

### Test 7: Multiple Google Accounts

**Steps:**
1. Login with Google Account A
2. Logout
3. Login with Google Account B
4. **Expected Result:**
   - Two separate accounts created
   - Each has unique username
   - No conflicts

---

## ?? Verification Checklist

After each test, verify:

### Database Checks
```sql
-- View all users
SELECT UserId, Username, Email, ExternalLoginProvider, Role FROM Users;

-- View Google users only
SELECT * FROM Users WHERE ExternalLoginProvider = 'Google';

-- View Customers created for Google users
SELECT c.* FROM Customers c
JOIN Users u ON c.Email = u.Email
WHERE u.ExternalLoginProvider = 'Google';
```

### Session Checks
- Open Developer Tools ? Application ? Cookies
- Look for authentication cookies
- Check session data

### UI Checks
- [ ] Google button visible on Login page
- [ ] Google button visible on Register page
- [ ] Google icon displays correctly
- [ ] "OR" separator shows properly
- [ ] Traditional form still visible and working
- [ ] Success/error messages display correctly

---

## ?? Debugging

### Enable Detailed Logging

Add to `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore.Authentication": "Debug"
  }
}
```

### Check Browser Console
Open Developer Tools (F12) and look for:
- JavaScript errors
- Network requests to Google
- Cookie/session issues

### Common Error Messages

**"External login failed. Please try again."**
- Check Google credentials in `appsettings.json`
- Verify redirect URI in Google Console
- Check application logs

**"Unable to retrieve email from Google account."**
- Google account might not have email permission
- Check OAuth consent screen in Google Console
- Ensure email scope is requested

**"An account with this email already exists."**
- This is expected behavior (security feature)
- User should use original login method
- No action needed

---

## ?? Expected Database State

### After Test 1 (New Google User)

**Users Table:**
```
UserId: 2
Username: john.doe
Password: (empty)
Email: john.doe@gmail.com
Role: Customer
ExternalLoginProvider: Google
ExternalLoginProviderId: 102938475610293847561
CreatedAt: 2024-11-01 10:15:00
```

**Customers Table:**
```
CustomerId: 2
FirstName: John
LastName: Doe
Email: john.doe@gmail.com
PhoneNumber: (empty)
Address: (empty)
LicenseNumber: (empty)
LicenseExpiryDate: 2025-11-01
CreatedAt: 2024-11-01 10:15:00
```

---

## ?? Performance Testing

### Test Google Authentication Speed

1. Clear browser cache
2. Time the login process:
   - Click "Sign in with Google"
   - Complete authentication
   - Land on dashboard
3. **Expected:** 2-5 seconds (depending on network)

### Test Session Persistence

1. Login with Google
2. Close browser tab
3. Reopen application URL
4. **Expected:** Still logged in (30-minute session)

---

## ?? Security Testing

### Test 1: Cannot Bypass Google Auth
Try accessing:
```
https://localhost:PORT/signin-google?code=fake_code
```
**Expected:** Error or redirect to login

### Test 2: CSRF Protection
Try POST to ExternalLogin without anti-forgery token
**Expected:** 400 Bad Request

### Test 3: Session Hijacking Prevention
1. Login
2. Copy session cookie
3. Open incognito window
4. Try to use copied cookie
**Expected:** Session validation fails or re-login required

---

## ?? Test Report Template

```markdown
## Google Authentication Test Report

**Date:** [Date]
**Tester:** [Name]
**Environment:** Development/Production

### Test Results

| Test | Status | Notes |
|------|--------|-------|
| New User Registration | ? Pass | |
| Returning User Login | ? Pass | |
| Traditional Login | ? Pass | |
| Register Page Google | ? Pass | |
| Car Rental Redirect | ? Pass | |
| Email Conflict | ? Pass | |
| Multiple Accounts | ? Pass | |

### Issues Found
- None

### Recommendations
- None

### Sign-Off
- [ ] All tests passed
- [ ] Ready for production
```

---

## ?? Production Testing

Before going live, test with:

1. **Real Google Accounts**
   - Personal Gmail
   - Work Google account
   - Different email domains

2. **Different Browsers**
   - Chrome
 - Firefox
   - Edge
   - Safari (if available)

3. **Different Devices**
   - Desktop
   - Mobile browser
   - Tablet

4. **Different Networks**
   - Home WiFi
   - Mobile data
   - Corporate network

---

## ?? Mobile Testing

### Test on Mobile Browsers

1. Open on phone: `https://your-domain.com/Account/Login`
2. Click "Sign in with Google"
3. **Expected:**
   - Google's mobile-optimized sign-in
   - Smooth redirect back to app
   - Session persists

### Test Responsive Design

1. Resize browser window
2. Check Google button displays correctly
3. Check form layout
4. **Expected:** Buttons stack properly on small screens

---

## ?? Edge Cases

### Test: User Changes Google Email

1. Login with `old@gmail.com`
2. User changes Google email to `new@gmail.com`
3. Tries to login with Google
4. **Expected:** Treated as new user (different Google ID)

### Test: User Revokes App Permissions

1. Login with Google
2. Go to Google Account ? Security ? Third-party apps
3. Revoke permissions for your app
4. Try to login again
5. **Expected:** Re-request permissions

### Test: Network Interruption

1. Click "Sign in with Google"
2. Disconnect internet during Google auth
3. **Expected:** Error message, graceful failure

---

## ? Final Checklist

Before marking as complete:

- [ ] All 7 test scenarios pass
- [ ] Database records correct
- [ ] Sessions working properly
- [ ] UI looks professional
- [ ] Error handling works
- [ ] No console errors
- [ ] Traditional login unaffected
- [ ] Mobile responsive
- [ ] Security tests pass
- [ ] Performance acceptable

---

## ?? Support

If any test fails:
1. Check logs in Visual Studio Output window
2. Review browser console
3. Verify Google Console settings
4. Check database state
5. Review `GOOGLE_AUTHENTICATION_GUIDE.md`

---

**Happy Testing! ??**
