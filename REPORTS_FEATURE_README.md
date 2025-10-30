# Reports & Analytics Feature

## Overview
A comprehensive reporting dashboard has been added to the Calapan Car Rental MVC application with interactive charts and graphs that can filter data by daily, weekly, and monthly periods.

## Files Created

### 1. ReportsController.cs
**Location:** `CalapanCarRentalMVC\Controllers\ReportsController.cs`

**Features:**
- Admin-only access control
- API endpoints for chart data:
  - `GetRevenueData(period)` - Revenue trends with rental counts
  - `GetRentalStatusData()` - Distribution of rental statuses
  - `GetPopularCarsData(period)` - Most rented vehicles
  - `GetPaymentMethodData(period)` - Payment method usage statistics

**Filtering Periods:**
- **Daily:** Last 30 days
- **Weekly:** Last 12 weeks
- **Monthly:** Last 12 months

### 2. Index.cshtml
**Location:** `CalapanCarRentalMVC\Views\Reports\Index.cshtml`

**Components:**

#### Summary Statistics Cards
- Total Revenue (Completed rentals)
- Total Rentals (All statuses)
- Active Rentals (Currently active)
- Completed Rentals (Finished rentals)

#### Interactive Charts

##### 1. Revenue Trends (Line Chart)
- **Filters:** Daily, Weekly, Monthly
- **Data Displayed:**
  - Revenue (?) - Primary Y-axis
  - Number of Rentals - Secondary Y-axis
- **Features:** 
  - Dual-axis for comparing revenue vs volume
  - Smooth line curves with area fill
- Responsive tooltips with formatted currency

##### 2. Rental Status Distribution (Doughnut Chart)
- Shows breakdown of all rental statuses:
  - Active (Yellow)
  - Completed (Blue)
  - Pending (Orange)
  - Cancelled (Red)
  - Rejected (Gray)
- **Features:** Interactive legend, percentage display

##### 3. Most Rented Vehicles (Horizontal Bar Chart)
- **Filters:** Daily, Weekly, Monthly
- **Data Displayed:**
  - Top 10 most rented vehicles
  - Rental count
  - Total revenue (shown in tooltip)
- **Features:** Horizontal bars for better readability

##### 4. Payment Methods (Pie Chart)
- **Filters:** Daily, Weekly, Monthly
- **Data Displayed:**
  - Cash (Green)
  - Gcash (Blue)
  - Bank Transfer (Gray)
  - Rental count and revenue per method
- **Features:** Color-coded by payment type

## Technology Stack

### Frontend
- **Chart.js** - Version: Latest (CDN)
  - Modern, responsive charting library
  - Interactive tooltips and legends
  - Mobile-responsive

### Backend
- **ASP.NET Core MVC** (.NET 9)
- **Entity Framework Core** (MySQL)
- **LINQ** for data aggregation

## How to Access

1. **Login as Admin**
2. **Navigate to Reports:**
   - Click "Reports & Analytics" in the sidebar
   - Or click "View Reports" in Quick Actions on Dashboard

## Features

### Responsive Design
- Charts automatically adjust to screen size
- Print-friendly layout
- Mobile-optimized controls

### Real-time Filtering
- Click filter buttons (Daily/Weekly/Monthly)
- Charts update instantly via AJAX
- No page reload required

### Data Accuracy
- Revenue calculated from completed rentals only
- Actual return dates used for historical data
- Late fees included in total revenue

## Integration Points

### Updated Files

1. **Admin Dashboard** (`Views\Admin\Dashboard.cshtml`)
   - Updated "View Reports" quick action to link to Reports controller

2. **Layout Navigation** (`Views\Shared\_Layout.cshtml`)
   - Added "Reports & Analytics" menu item in admin sidebar
   - Active state highlighting for Reports page

## Usage Examples

### Daily Revenue Analysis
```
- View last 30 days of revenue
- Identify peak rental days
- Compare revenue vs rental volume
```

### Popular Vehicles Tracking
```
- See which cars are rented most frequently
- Compare performance across different time periods
- Track revenue generation per vehicle
```

### Payment Method Insights
```
- Monitor payment method preferences
- Track digital payment adoption
- Analyze revenue by payment type
```

## Security

- **Admin-only access:** Session-based authentication required
- **Role verification:** Checks UserRole == "Admin"
- **Redirect on unauthorized:** Sends to login page with error message

## Performance Considerations

- Efficient LINQ queries with proper indexing
- Data grouped at database level (GroupBy)
- Minimal data transfer (aggregated results only)
- Client-side chart rendering (reduces server load)

## Future Enhancements

Potential additions:
- Export to PDF/Excel
- Custom date range selection
- Car utilization rates
- Customer demographics
- Maintenance cost analysis
- Predictive analytics

## Testing Recommendations

1. **Test different time periods** - Ensure data displays correctly
2. **Verify calculations** - Cross-reference with database
3. **Check responsive design** - Test on mobile devices
4. **Test with no data** - Verify graceful handling of empty datasets
5. **Performance testing** - Check load times with large datasets

## Browser Compatibility

Tested and compatible with:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Dependencies

```html
<!-- Chart.js CDN -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
```

Included automatically in the Reports Index view.

---

**Created:** January 2025  
**Version:** 1.0  
**Author:** Calapan Car Rental Development Team
