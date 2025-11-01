# License Expiry Validation Feature

## Overview
Implemented validation to prevent customers with expired driver's licenses from booking cars in the Calapan Car Rental system.

## Changes Made

### 1. RentalsController.cs
**Location**: `CalapanCarRentalMVC\Controllers\RentalsController.cs`

#### GET: Rentals/Create
- Added license expiry validation before displaying the rental form
- If license is expired, redirects to customer profile with error message
- Error message: "Your driver's license has expired. Please update your license information before booking a car."

#### POST: Rentals/Create
- Added license expiry validation before processing the rental
- Validates customer's license expiry date against current date
- Prevents rental creation if license is expired
- Redirects to customer profile if validation fails

### 2. Cars/Details.cshtml
**Location**: `CalapanCarRentalMVC\Views\Cars\Details.cshtml`

- Added TempData error message display at the top of car details page
- Shows license expiry errors when user attempts to rent with expired license

### 3. Customer/Profile.cshtml
**Location**: `CalapanCarRentalMVC\Views\Customer\Profile.cshtml`

#### Added License Expiry Warnings
- **Expired License Alert** (Red/Danger):
  - Displayed when license expiry date is in the past
  - Shows expiry date and blocks booking functionality
  - Clear message: "You cannot book any cars until you update your license information"

- **Expiring Soon Alert** (Yellow/Warning):
  - Displayed when license expires within 1 month
  - Warns users to renew soon to avoid service interruption

#### Modified Quick Actions Section
- **Browse Cars Button**:
  - Enabled only when license is valid (expiry date >= current date)
  - Disabled when license is expired with tooltip "License expired"
  - Visual feedback to prevent confusion

#### Updated Edit Profile Modal
- Added minimum date validation for license expiry date (must be today or later)
- Shows warning message if current license is expired
- Updated info note to emphasize valid license requirement

## Validation Flow

1. **Customer attempts to book a car**
   - System checks customer's license expiry date
   - If expired: Redirects to profile with error message
   - If valid: Proceeds with booking

2. **Profile page display**
   - Shows prominent alert if license is expired
   - Disables "Browse Cars" button
   - Guides user to update license information

3. **Admin perspective**
   - License expiry status visible in customer details
   - Badges show: "Expired" (red), "Expiring Soon" (yellow), or "Valid" (green)

## User Experience

### For Customers with Expired License:
1. Sees prominent red alert on profile page
2. Cannot access car booking functionality
3. Browse Cars button is disabled
4. Clear instructions to update license information
5. Edit profile form highlights the need to update license

### For Customers with Valid License:
1. No restrictions on booking
2. Early warning (1 month before) if license is expiring soon
3. Smooth booking experience

## Technical Details

### Date Comparison
```csharp
if (customer.LicenseExpiryDate < DateTime.Now)
{
 // License is expired - block booking
}
```

### Error Messages
- Primary error: "Your driver's license has expired. Please update your license information before booking a car."
- Profile warning: "You cannot book any cars until you update your license information."

### UI Components
- Alert classes: `alert-danger` (expired), `alert-warning` (expiring soon)
- Icons: `fa-exclamation-triangle`, `fa-exclamation-circle`
- Button states: `disabled` for expired licenses

## Testing Recommendations

1. **Test with expired license**:
   - Set a customer's license expiry date to past
 - Attempt to book a car
   - Verify redirection to profile with error

2. **Test with expiring soon**:
   - Set license expiry date within 1 month
   - Verify yellow warning appears
   - Verify booking still works

3. **Test with valid license**:
   - Set license expiry date in future (>1 month)
   - Verify no warnings
   - Verify booking works normally

4. **Test profile update**:
   - Update expired license to valid date
   - Verify booking functionality restored

## Benefits

1. **Safety & Compliance**: Ensures only customers with valid licenses can rent vehicles
2. **User Guidance**: Clear messaging helps users understand and resolve issues
3. **Proactive Warnings**: One-month advance notice for expiring licenses
4. **Admin Visibility**: Easy identification of license status in customer management
5. **Data Integrity**: Maintains accurate rental records with valid license information
