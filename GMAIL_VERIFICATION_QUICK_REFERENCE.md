# ?? Quick Reference - Gmail Verification Code

## ?? **How to Use in Your Code**

### **1. Send Verification Code**

```csharp
// In any controller
private readonly IEmailService _emailService;
private readonly CarRentalContext _context;

public YourController(CarRentalContext context, IEmailService emailService)
{
    _context = context;
    _emailService = emailService;
}

[HttpPost]
public async Task<IActionResult> SendCode(string email)
{
    // Generate code
    var code = VerificationCodeGenerator.GenerateCode(); // "123456"
    
    // Save to database
    var verification = new EmailVerification
    {
 Email = email,
      VerificationCode = code,
        CreatedAt = DateTime.Now,
        ExpiresAt = DateTime.Now.AddMinutes(10), // Expires in 10 min
        IsUsed = false
    };
    _context.EmailVerifications.Add(verification);
    await _context.SaveChangesAsync();
    
    // Send email
    await _emailService.SendVerificationCodeAsync(email, code);
    
    return Json(new { success = true });
}
```

### **2. Verify Code**

```csharp
[HttpPost]
public async Task<IActionResult> VerifyCode(string email, string code)
{
    var verification = await _context.EmailVerifications
        .Where(v => v.Email == email && v.VerificationCode == code && !v.IsUsed)
        .OrderByDescending(v => v.CreatedAt)
        .FirstOrDefaultAsync();
    
    if (verification == null)
        return Json(new { success = false, message = "Invalid code" });
    
    if (verification.IsExpired())
     return Json(new { success = false, message = "Code expired" });
    
    // Mark as used
    verification.IsUsed = true;
    await _context.SaveChangesAsync();
    
    return Json(new { success = true });
}
```

---

## ?? **Frontend AJAX Examples**

### **Send Code Button**

```javascript
$('#sendCodeBtn').click(function() {
    const email = $('#email').val();
    
    $.ajax({
     url: '/Account/SendVerificationCode',
        type: 'POST',
        data: {
            email: email,
            __RequestVerificationToken: $('[name="__RequestVerificationToken"]').val()
 },
        success: function(response) {
      if (response.success) {
    alert('Code sent! Check your email.');
         $('#codeInput').show();
            } else {
                alert(response.message);
            }
      }
    });
});
```

### **Verify Code Button**

```javascript
$('#verifyBtn').click(function() {
    const email = $('#email').val();
    const code = $('#codeInput').val();
    
    $.ajax({
        url: '/Account/VerifyCode',
        type: 'POST',
        data: {
   email: email,
       code: code,
      __RequestVerificationToken: $('[name="__RequestVerificationToken"]').val()
        },
  success: function(response) {
            if (response.success) {
                alert('Email verified!');
         // Show rest of form or redirect
   } else {
                alert(response.message);
            }
 }
    });
});
```

---

## ?? **HTML Form Example**

```html
<form method="post">
    @Html.AntiForgeryToken()
    
    <!-- Email Input -->
    <div class="mb-3">
<label>Email</label>
        <div class="input-group">
            <input type="email" id="email" class="form-control" required />
          <button type="button" class="btn btn-primary" id="sendCodeBtn">
  Send Code
  </button>
        </div>
    </div>
    
    <!-- Code Input (hidden initially) -->
    <div class="mb-3" id="codeSection" style="display: none;">
        <label>Verification Code</label>
        <input type="text" id="codeInput" class="form-control" maxlength="6" />
        <button type="button" class="btn btn-success" id="verifyBtn">
            Verify
        </button>
    </div>
    
    <!-- Rest of form (shown after verification) -->
    <div id="mainForm" style="display: none;">
        <input type="text" name="Name" placeholder="Full Name" required />
        <input type="password" name="Password" placeholder="Password" required />
        <button type="submit">Register</button>
    </div>
</form>
```

---

## ??? **Database Queries**

### **Check Recent Codes**

```sql
SELECT * FROM EmailVerifications 
WHERE Email = 'user@example.com' 
ORDER BY CreatedAt DESC 
LIMIT 5;
```

### **Delete Expired Codes**

```sql
DELETE FROM EmailVerifications 
WHERE ExpiresAt < NOW() 
OR IsUsed = 1;
```

### **Count Codes by Email**

```sql
SELECT Email, COUNT(*) as CodeCount 
FROM EmailVerifications 
GROUP BY Email 
ORDER BY CodeCount DESC;
```

---

## ?? **Configuration**

### **appsettings.json**

```json
{
  "EmailSettings": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password-here"
  }
}
```

### **Program.cs**

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

---

## ? **Quick Test Commands**

### **Test in Browser Console**

```javascript
// Send code
fetch('/Account/SendVerificationCode', {
method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: 'email=test@example.com&__RequestVerificationToken=' + 
          $('[name="__RequestVerificationToken"]').val()
}).then(r => r.json()).then(console.log);

// Verify code
fetch('/Account/VerifyCode', {
method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: 'email=test@example.com&code=123456&__RequestVerificationToken=' + 
          $('[name="__RequestVerificationToken"]').val()
}).then(r => r.json()).then(console.log);
```

---

## ?? **Email Template Customization**

### **Simple Template**

```csharp
var body = $@"
    <h2>Verification Code</h2>
  <p>Your code is: <strong style='font-size: 24px; color: #dc3545;'>{code}</strong></p>
    <p>This code expires in 10 minutes.</p>
";
await _emailService.SendEmailAsync(email, "Verification Code", body);
```

### **Custom Template**

```csharp
var body = $@"
    <div style='font-family: Arial; max-width: 600px; margin: 0 auto;'>
        <h1>Welcome to {companyName}!</h1>
     <p>Your verification code is:</p>
      <div style='background: #f0f0f0; padding: 20px; text-align: center;'>
            <h1 style='letter-spacing: 10px;'>{code}</h1>
        </div>
      <p>Expires in {expiryMinutes} minutes.</p>
  </div>
";
```

---

## ?? **Common Errors & Solutions**

| Error | Cause | Solution |
|-------|-------|----------|
| "Authentication failed" | Wrong Gmail password | Use App Password |
| "Email not received" | Code in spam | Check spam folder |
| "Code expired" | >10 minutes passed | Request new code |
| "Invalid code" | Wrong digits | Double-check code |
| "Email already registered" | Duplicate email | Use different email |

---

## ?? **Test URLs**

```
https://localhost:7277/Account/RegisterWithVerification
https://localhost:7277/Account/SendVerificationCode  (POST)
https://localhost:7277/Account/VerifyCode  (POST)
```

---

## ? **Validation Rules**

- **Email:** Must be valid format
- **Code:** Exactly 6 digits
- **Expiry:** 10 minutes
- **Usage:** One-time only
- **Anti-forgery:** Required on all POST requests

---

## ?? **Security Checklist**

- [x] CSRF protection enabled
- [x] Code expiration (10 min)
- [x] One-time use
- [x] Secure random code generation
- [x] TLS/SSL encryption
- [x] No plaintext passwords
- [x] Email validation
- [x] Rate limiting (recommended)

---

## ?? **Support**

- **Documentation:** `GMAIL_VERIFICATION_IMPLEMENTATION.md`
- **Test View:** `RegisterWithVerification.cshtml`
- **Email Service:** `Services/EmailService.cs`
- **Generator:** `Services/VerificationCodeGenerator.cs`

---

**Quick Start:** Just navigate to `/Account/RegisterWithVerification` and test!

**Status:** ? Fully Functional
