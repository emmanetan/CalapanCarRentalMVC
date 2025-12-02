# Location Tracking - GPS Consent Implementation

## Overview
This implementation enables permanent location tracking for users who agree to GPS tracking during the vehicle rental process.

## Changes Made

### 1. Database Changes
- Added `LocationTrackingEnabled` (boolean) field to `Users` table
- Added `LocationTrackingEnabledDate` (datetime) field to `Users` table

### 2. Backend Changes
- **User.cs**: Added new fields for tracking location consent
- **RentalsController.cs**: Updated the `Create` method to enable location tracking when GPS consent is given
- **LocationController.cs**: Added `CheckTrackingStatus` endpoint to verify if tracking is enabled for the current user

### 3. Frontend Changes
- **MyLocation.cshtml**: Updated to automatically enable location tracking if the user has previously consented via GPS tracking checkbox

## How to Apply Database Changes

You need to run the SQL script to add the new columns to the Users table.

### Option 1: Using MySQL Workbench or Command Line
```sql
USE calapancarrental;

ALTER TABLE Users 
ADD COLUMN LocationTrackingEnabled TINYINT(1) NOT NULL DEFAULT 0;

ALTER TABLE Users 
ADD COLUMN LocationTrackingEnabledDate DATETIME(6) NULL;
```

### Option 2: Using the provided SQL file
The SQL migration file is located at:
`CalapanCarRentalMVC\Migrations\AddLocationTrackingColumns.sql`

Run this file in your MySQL database.

## How It Works

1. **During Rental Creation**:
   - Customer must check the GPS tracking consent checkbox to proceed
   - When clicking "Confirm Rental", the system:
     - Saves the rental with GPS consent
     - Enables permanent location tracking for the user (`LocationTrackingEnabled = true`)
     - Records the consent date

2. **On MyLocation Page**:
   - The page checks the server for the user's tracking status
   - If `LocationTrackingEnabled = true`, location tracking starts automatically
   - The user can still manually disable tracking, but it will re-enable on page reload

3. **Permanent Tracking**:
   - Once consent is given, tracking remains enabled forever (or until manually changed in the database)
   - Users don't need to re-enable tracking on each visit
   - Location updates are sent to the server automatically

## Testing Instructions

1. Apply the database migration using one of the options above
2. Log in as a customer
3. Go to rent a vehicle
4. Check the GPS tracking consent checkbox
5. Complete the rental process
6. Navigate to "My Location" page
7. Verify that location tracking starts automatically without manual intervention
8. The tracking status badge should show "Enabled" immediately

## Notes

- The GPS consent checkbox is required for rental creation
- Location tracking can be disabled manually by the user on the MyLocation page
- However, it will automatically re-enable on the next page load
- This ensures continuous tracking for security and safety purposes as agreed during rental
