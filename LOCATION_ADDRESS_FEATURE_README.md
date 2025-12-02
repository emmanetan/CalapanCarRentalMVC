# Location Tracking - Specific Address Feature

## Summary
Added specific address functionality to the location tracking system for both admin and customer views. The system now uses reverse geocoding to convert GPS coordinates (latitude/longitude) into human-readable addresses.

**NEW: Added comprehensive Users Location History section in the admin Live Location Tracking page with filtering, pagination, statistics, and export capabilities.**

## Changes Made

### 1. Database Model (LocationHistory.cs)
- **Added Field**: `Address` (string, max 500 characters, nullable)
- This field stores the specific address obtained from reverse geocoding

### 2. Controller (LocationController.cs)
- **Updated `LocationHistoryRequest` class**: Added `Address` property
- **Updated `SaveLocation` endpoint**: Now accepts and saves the address along with GPS coordinates
- **Updated `GetLatestLocations` endpoint**: Returns address in the location data for admin tracking
- **Updated `GetUserLocationHistory` endpoint**: Includes address in the location history records
- **NEW: `GetAllUsersLocationHistory` endpoint**: Retrieves location history for all users with filters
  - Parameters: hours, userId (optional), role (optional), page, pageSize
  - Returns: Paginated results with statistics
  - Includes: Total records count, unique users, average accuracy, date range
- **NEW: `GetAllUsersForFilter` endpoint**: Returns list of all users for filter dropdown

### 3. Admin Track View (Views/Location/Track.cshtml)
**Enhanced Display Features**:
- Map popup now shows the specific address for each user location
- User list displays the address below the user's name and role
- Location history modal includes an "Address" column in the table
- Address is shown as "Fetching address..." or "N/A" if not available

**NEW: Users Location History Section**:
- **Filter Controls**:
  - Filter by User (dropdown with all users)
  - Time Range (1 hour, 6 hours, 24 hours, 3 days, 7 days)
  - Filter by Role (All, Admin, Customer)
  - Apply Filters button

- **Statistics Dashboard**:
  - Total Records Count
  - Unique Users Count
  - Average Accuracy
  - Date Range Display

- **Location History Table**:
  - Displays: #, Username, Role, Address, Coordinates, Accuracy, Timestamp, Actions
  - Color-coded role badges (Admin = red, Customer = blue)
  - Color-coded accuracy badges (Green < 50m, Yellow < 100m, Gray >= 100m)
  - Shows row number, email below username
  - Timestamps with "time ago" display

- **Actions**:
  - View Detail: Opens modal with location details and mini-map
  - View History: Shows individual user's location history

- **Pagination**:
  - 50 records per page
  - Smart pagination (shows ... for large ranges)
  - Previous/Next navigation
  - Smooth scroll to table on page change

- **Export Functionality**:
  - Export to CSV button
  - Downloads all filtered records
  - Includes: Username, Email, Role, Address, Coordinates, Accuracy, Timestamp, Device Info
  - Filename includes current date

- **Location Detail Modal**:
  - Shows detailed information for a specific location
  - Displays mini-map with marker
  - "Open in Google Maps" button for navigation

### 4. Customer MyLocation View (Views/Location/MyLocation.cshtml)
**New Functionality**:
- Implements **reverse geocoding** using OpenStreetMap Nominatim API
- Automatically fetches address when location is updated
- **New function**: `getAddressFromCoordinates(lat, lng)` - Converts coordinates to address
- Location Details panel now shows:
  - **Address** (at the top for visibility)
  - Coordinates (Latitude/Longitude)
  - Accuracy
  - Last Update timestamp
- Map marker popup displays the address
- Address is sent to the server when saving location

### 5. Database Migration
- **SQL Script Created**: `add_address_column.sql`
- To apply the migration, run this SQL command on your MySQL database:
  ```sql
ALTER TABLE `LocationHistories` 
  ADD COLUMN `Address` VARCHAR(500) NULL;
  ```

## How It Works

### For Customers:
1. Customer enables location tracking on their "My Location" page
2. Browser captures GPS coordinates (latitude, longitude)
3. JavaScript calls the Nominatim reverse geocoding API
4. API returns the specific address for those coordinates
5. Address is displayed in the Location Details panel
6. Both coordinates and address are saved to the database

### For Admins:
1. Admin views the "Live Location Tracking" page
2. System fetches latest locations for all active users (real-time map)
3. Each location includes the stored address from the database
4. Map markers show popup with username, role, email, **address**, accuracy, and timestamp
5. User list displays the address for quick reference
6. Location history modal shows all past locations with their addresses

**NEW: Users Location History Section**:
1. Admin can view comprehensive location history for all users
2. Filter by specific user, time range, or role
3. View statistics: total records, unique users, average accuracy, date range
4. Browse through paginated results (50 per page)
5. Click "View Detail" to see location on mini-map with full details
6. Click "View History" to see individual user's 24-hour history
7. Export filtered results to CSV for reporting/analysis

## API Endpoints

### Existing Endpoints:
- `POST /Location/SaveLocation` - Save user location with address
- `GET /Location/GetLatestLocations` - Get active users' latest locations (last 5 minutes)
- `GET /Location/GetUserLocationHistory` - Get specific user's location history

### NEW Endpoints:
- `GET /Location/GetAllUsersLocationHistory` - Get all users' location history with filters
  - Query params: `hours`, `userId` (optional), `role` (optional), `page`, `pageSize`
  - Returns: Paginated data + statistics
  
- `GET /Location/GetAllUsersForFilter` - Get all users for filter dropdown
  - Returns: List of users with userId, username, email, role

## Technical Details

### Reverse Geocoding API
- **Service**: OpenStreetMap Nominatim
- **Endpoint**: `https://nominatim.openstreetmap.org/reverse`
- **Parameters**: 
  - `format=json`
  - `lat={latitude}`
  - `lon={longitude}`
  - `zoom=18` (detailed address)
  - `addressdetails=1`
- **Response**: Returns `display_name` field with the complete address

### Pagination Logic
- **Page Size**: 50 records per page
- **Smart Display**: Shows max 5 page numbers with "..." for gaps
- **Navigation**: Previous/Next buttons, direct page number clicking
- **Scroll Behavior**: Smooth scroll to table top on page change

### CSV Export Format
```csv
Username,Email,Role,Address,Latitude,Longitude,Accuracy,Timestamp,Device Info
"John Doe","john@example.com","Customer","123 Main St",13.4119,121.1794,25.5,"12/3/2025 10:30:00 AM","Mozilla/5.0..."
```

### Data Flow
```
Customer Device (GPS) 
  ? Browser (Lat/Lng) 
  ? Nominatim API (Reverse Geocoding) 
  ? Address String 
  ? Server (Save to DB) 
  ? Admin View (Display Real-time + History)
```

## Features Summary

### Real-Time Tracking (Existing + Enhanced):
? Live map with user markers  
? Auto-refresh every 5 seconds  
? Active users list  
? User location popup with address  
? Individual user history modal  

### NEW: Users Location History:
? Comprehensive filterable table  
? Filter by user, time range, and role  
? Statistics dashboard  
? Pagination (50 records/page)  
? Location detail modal with mini-map  
? Export to CSV  
? Color-coded badges for role and accuracy  
? Responsive design  
? Smooth animations and transitions  

## Benefits

1. **Better Tracking**: Admins can see human-readable locations instead of just coordinates
2. **User Transparency**: Customers can see exactly what address is being shared
3. **Audit Trail**: Historical addresses are stored for each location update
4. **No API Key Required**: Using OpenStreetMap Nominatim (free service)
5. **Privacy**: Address is only visible to admins and the user themselves
6. **NEW: Advanced Filtering**: Quickly find specific users or time periods
7. **NEW: Data Export**: Generate reports for compliance or analysis
8. **NEW: Visual Statistics**: Quick overview of tracking activity
9. **NEW: Detailed View**: Individual location inspection with map
10. **NEW: Scalability**: Pagination handles thousands of records efficiently

## Usage Notes

1. **Database Update Required**: Run the SQL script to add the Address column
2. **Internet Connection**: Reverse geocoding requires internet access
3. **API Limits**: Nominatim has usage limits - consider implementing caching if needed
4. **Accuracy**: Address accuracy depends on the GPS coordinates and OpenStreetMap data
5. **Fallback**: System gracefully handles cases where address cannot be retrieved
6. **NEW: Performance**: Pagination ensures fast page loads even with large datasets
7. **NEW: Export Limits**: CSV export limited to 10,000 records per export
8. **NEW: Browser Compatibility**: Tested on modern browsers (Chrome, Firefox, Edge, Safari)

## Admin Interface Guide

### Using Location History:

1. **Navigate**: Scroll down to "Users Location History" section
2. **Filter**: 
   - Select a specific user or leave as "All Users"
   - Choose time range (default: 24 hours)
   - Filter by role if needed (Admin/Customer)
   - Click "Apply Filters"
3. **View Results**: Browse through the table
4. **Actions**:
   - Click ??? (eye) to view location details with map
   - Click ?? (history) to view user's complete history
5. **Export**: Click "Export to CSV" to download filtered data
6. **Navigate Pages**: Use pagination at bottom to browse through results

### Statistics Explained:
- **Total Records**: Number of location updates in selected time range
- **Unique Users**: How many different users have location data
- **Avg. Accuracy**: Average GPS accuracy across all records (lower is better)
- **Date Range**: Earliest to latest timestamp in the results
