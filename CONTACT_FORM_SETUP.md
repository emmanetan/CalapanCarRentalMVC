# Contact Form Email Setup Guide

## Overview
The "Send Us a Message" contact form is now fully functional and will send emails to **carrentalcalapan@gmail.com** when visitors submit the form.

## How It Works

### 1. **Contact Form Submission**
When someone fills out the contact form on your website (`/Home/Contact`), they provide:
- Full Name
- Email Address
- Phone Number (optional)
- Message

### 2. **Email Delivery**
The system automatically sends a professionally formatted email to **carrentalcalapan@gmail.com** containing:
- Contact person's information (name, email, phone)
- The message they wrote
- Timestamp and formatting for easy reading

### 3. **User Confirmation**
After successful submission, the user sees a success message:
> "Thank you for contacting us! We'll get back to you soon."

## Gmail Configuration

Your Gmail account is already configured in `appsettings.json`:

```json
"EmailSettings": {
  "From": "carrentalcalapan@gmail.com",
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "Username": "carrentalcalapan@gmail.com",
  "Password": "jozf hjyc hzxd ibwq"
}
```

### ?? Important: Gmail App Password

The password shown (`jozf hjyc hzxd ibwq`) is a **Google App Password**, not your regular Gmail password.

#### If emails are NOT sending, verify your Google App Password:

1. **Sign in to Google Account**: https://myaccount.google.com/
2. **Go to Security**
3. **Enable 2-Step Verification** (required for App Passwords)
4. **Generate App Password**:
   - Go to: https://myaccount.google.com/apppasswords
   - Select app: "Mail"
   - Select device: "Other (Custom name)" ? Enter "Calapan Car Rental"
   - Click **Generate**
   - Copy the 16-character password (format: xxxx xxxx xxxx xxxx)
   - Update the password in `appsettings.json`

#### App Password Format:
```
Original: abcd efgh ijkl mnop
In config: "abcdefghijklmnop" (no spaces) or "abcd efgh ijkl mnop" (with spaces)
```

## Testing the Contact Form

### Test Steps:
1. Run your application
2. Navigate to **Contact** page
3. Fill out the form:
   - Name: Test User
   - Email: test@example.com
   - Phone: 09123456789
   - Message: This is a test message
4. Click **Send Message**
5. Check **carrentalcalapan@gmail.com** inbox

### Expected Result:
You should receive a professionally formatted email with:
- Subject: "New Contact Message from Test User"
- All contact details
- The submitted message

## Troubleshooting

### Problem: Emails not sending

**Check these items:**

1. **App Password is correct**
   - Must be a Google App Password, not your Gmail password
   - Must have 2-Step Verification enabled

2. **Internet Connection**
   - Server needs internet access to connect to Gmail SMTP

3. **Gmail Account Status**
 - Account is not suspended or locked
   - "Less secure app access" is NOT needed (App Passwords are secure)

4. **Check Application Logs**
   - Look for error messages in the console/logs
   - Errors will show connection or authentication issues

### Problem: Form validation errors

The form validates:
- **Name**: Required, max 100 characters
- **Email**: Required, valid email format
- **Phone**: Optional, valid phone format
- **Message**: Required, 10-1000 characters

### Problem: Success message not showing

- Make sure you're redirecting after POST (already implemented)
- TempData is being used correctly (already implemented)

## Security Recommendations

### ?? For Production:

1. **Move email settings to User Secrets or Environment Variables**
   ```bash
   dotnet user-secrets set "EmailSettings:Password" "your-app-password"
   ```

2. **Never commit passwords to Git**
   - Add `appsettings.json` to `.gitignore` if it contains sensitive data
   - Use `appsettings.Development.json` for local development
   - Use environment variables or Azure Key Vault for production

3. **Rate Limiting**
   - Consider adding rate limiting to prevent spam
   - Limit submissions per IP address

## Email Template

The contact form email includes:
- ? Professional branding (Calapan Car Rental logo/header)
- ? Clear contact information section
- ? Message content with proper formatting
- ? Action required notice
- ? Company footer with contact details
- ? Mobile-friendly HTML design

## Features Implemented

? Contact form with validation
? Email sending via Gmail SMTP
? Professional HTML email template
? Success/error message handling
? Optional phone number field
? Client-side and server-side validation
? Responsive design
? User-friendly error messages

## Support

If you continue to have issues:
1. Check the application logs for detailed error messages
2. Verify Gmail settings at https://myaccount.google.com/
3. Test with a simple email tool to verify App Password works
4. Ensure the Gmail account has sufficient quota (Gmail has daily sending limits)

---

**Last Updated**: 2025
**Contact**: carrentalcalapan@gmail.com
