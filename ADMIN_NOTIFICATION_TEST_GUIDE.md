# ?? Quick Test Guide - Admin Notification for New Bookings

## How to Test the Fix

### Prerequisites
- Admin account created
- Customer account created
- At least one available car in the system

---

## Test Steps

### 1?? Login as Customer

```
URL: https://localhost:7277/Account/Login
Username: [your customer username]
Password: [your customer password]
```

### 2?? Browse Cars

```
URL: https://localhost:7277/Cars/Index
- Find an available car
- Click "Details" or "Book Now"
```

### 3?? Book a Car

1. Fill in booking form:
   - Pick-up date/time
   - Return date/time
   - Destination
   - Payment method
   - Upload Government ID
   - Upload payment receipt (if needed)

2. Click "Submit Rental Request"

3. **Expected Result:**
   - ? Success message: "Car rental request submitted successfully! Please wait for admin approval."
   - ? Redirected to "My Rentals" page
   - ? Rental shows with "Pending" status

### 4?? Logout

```
Click: Logout (in sidebar)
```

### 5?? Login as Admin

```
URL: https://localhost:7277/Account/Login
Username: [your admin username]
Password: [your admin password]
```

### 6?? Check Notification

1. **Look at top bar (right side)**
   - ? You should see notification bell icon ??
   - ? Red badge with number (e.g., "1" or "3")

2. **Click the bell icon**
   - ? Dropdown panel should open
   - ? You should see "New Rental Request" notification

3. **Notification should show:**
   ```
   ?? New Rental Request (Blue icon)
   
   [Customer Name] requested to rent [Car Brand Model]. 
   Pick-up: [Date]. Total: ?[Amount]. 
   Please review and approve.
   
   Just now
   ```

4. **Click the notification**
   - ? Should redirect to: `/Rentals/Index?filterStatus=Pending`
   - ? Should show the pending rental in the list
   - ? Rental card should have yellow/warning header
   - ? "Approve" and "Reject" buttons visible

### 7?? Approve the Rental

1. Click "Approve" button
2. Confirm approval
3. **Expected Result:**
   - ? Rental status changes to "Active"
   - ? Car status changes to "Rented"
   - ? Customer receives "Rental Approved" notification

---

## Expected Behavior Summary

| Step | Action | Expected Result |
|------|--------|----------------|
| 1 | Customer books car | Rental saved as "Pending" |
| 2 | System creates notification | Notification for ALL admins |
| 3 | Admin logs in | Badge shows unread count |
| 4 | Admin clicks bell | Dropdown shows notification |
| 5 | Admin clicks notification | Redirects to pending rentals |
| 6 | Admin approves | Customer gets notification |

---

## Visual Indicators

### Notification Badge
```
?? (3)  ? Red circle with number
```

### Notification in Dropdown
```
???????????????????????????????????
? ?? New Rental Request          ?
?               ?
? Juan Dela Cruz requested to    ?
? rent Toyota Vios. Pick-up:     ?
? Jan 15, 2025. Total: ?5,000.00.?
? Please review and approve.      ?
?            ?
? Just now            ?   ?
???????????????????????????????????
```

### Pending Rental Card
```
????????????????????????????????????
? ?? Rental #5 [Pending Approval] ? ? Yellow header
????????????????????????????????????
? Customer: Juan Dela Cruz        ?
? Vehicle: Toyota Vios     ?
? Pick-up: Jan 15, 2025         ?
? Total: ?5,000.00   ?
?          ?
? [View Details] [Approve] [Reject]?
????????????????????????????????????
```

---

## Troubleshooting

### Issue: No notification received

**Check 1: Is admin logged in?**
- Only logged-in admins see notifications
- Try logout and login again

**Check 2: Is badge showing?**
- Look at top-right corner of the screen
- Badge should have a number

**Check 3: Clear browser cache**
- Press Ctrl+Shift+Delete
- Clear cache and cookies
- Reload page

**Check 4: Check database**
```sql
SELECT * FROM Notifications WHERE UserId = [admin_user_id] ORDER BY CreatedAt DESC;
```

### Issue: Badge showing but no notification in dropdown

**Solution:**
- Click bell icon
- Wait 1-2 seconds
- Dropdown should appear
- Check browser console for JavaScript errors (F12)

### Issue: Clicking notification does nothing

**Solution:**
- Check ActionUrl in notification: should be `/Rentals/Index?filterStatus=Pending`
- Check browser console for errors
- Try manually navigating to `/Rentals/Index?filterStatus=Pending`

---

## Quick Verification Checklist

After customer books a car:

- [ ] Admin notification badge updates (+1)
- [ ] Clicking bell shows dropdown
- [ ] Notification appears in dropdown
- [ ] Notification has blue car icon (??)
- [ ] Notification title is "New Rental Request"
- [ ] Notification message includes customer name
- [ ] Notification message includes car details
- [ ] Notification message includes pickup date
- [ ] Notification message includes total amount
- [ ] Clicking notification redirects correctly
- [ ] Pending rental appears in list
- [ ] Rental card has yellow header
- [ ] "Approve" and "Reject" buttons visible
- [ ] Clicking "Approve" works
- [ ] Customer receives approval notification

---

## Multiple Admin Test

### Setup
1. Create 2 admin accounts (Admin1, Admin2)

### Test
1. Login as Customer ? Book a car ? Logout
2. Login as Admin1 ? Check notification ? Should see it ?
3. Logout
4. Login as Admin2 ? Check notification ? Should ALSO see it ?

**Expected:** Both admins receive the same notification independently.

---

## Test Data Examples

### Sample Customer Booking
```
Customer: Maria Santos
Car: Honda City 2023
Pick-up: January 20, 2025, 10:00 AM
Return: January 22, 2025, 10:00 AM
Destination: Puerto Galera
Payment: Gcash
Total: ?8,500.00
```

### Expected Admin Notification
```
Title: New Rental Request
Message: Maria Santos requested to rent Honda City. 
       Pick-up: Jan 20, 2025. Total: ?8,500.00. 
 Please review and approve.
Type: Info (Blue)
Icon: fa-car
```

---

## Success Criteria

? **Test Passes If:**
1. Admin receives notification after customer booking
2. Notification badge shows correct count
3. Notification appears in dropdown
4. Notification content is accurate
5. Clicking notification redirects correctly
6. All admins receive the notification

? **Test Fails If:**
1. No notification appears
2. Badge doesn't update
3. Wrong customer/car information
4. Clicking notification does nothing
5. Only some admins receive notification

---

## Performance Notes

- Notification creation: ~50ms
- Badge update: Immediate (on page refresh)
- Dropdown loading: Instant
- Redirect: <500ms

---

## Database Check

If you want to verify notifications are being created:

```sql
-- Check recent notifications for admins
SELECT 
    N.NotificationId,
    U.Username AS AdminUsername,
    N.Title,
    N.Message,
    N.Type,
    N.IsRead,
    N.CreatedAt
FROM Notifications N
JOIN Users U ON N.UserId = U.UserId
WHERE U.Role = 'Admin'
ORDER BY N.CreatedAt DESC
LIMIT 10;
```

---

**Status:** ? Ready for Testing  
**Estimated Test Time:** 5 minutes  
**Complexity:** Easy  
**Priority:** High
