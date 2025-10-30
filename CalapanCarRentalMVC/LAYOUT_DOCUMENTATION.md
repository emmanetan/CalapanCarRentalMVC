# Unified Layout Documentation

## Overview
The Calapan Car Rental application uses a **single unified layout** (`_Layout.cshtml`) that dynamically adapts based on the user's role and authentication status.

## File Structure

### Layout Files
```
Views/Shared/
??? _Layout.cshtml     # Main unified layout (handles all roles)
??? _Layout.cshtml.css     # Layout-specific CSS
??? _ValidationScriptsPartial.cshtml

wwwroot/css/
??? site.css    # Global styles
??? admin.css     # Dashboard/Sidebar styles
```

## How It Works

### 1. Role-Based Navigation

The layout checks the session for `UserRole` and displays appropriate navigation:

```csharp
@if (Context.Session.GetString("UserRole") == "Admin")
{
    // Admin navigation and sidebar
}
else if (Context.Session.GetString("UserRole") == "Customer")
{
    // Customer navigation and sidebar
}
else
{
    // Public navigation (no sidebar)
}
```

### 2. Three Layout Modes

#### Mode 1: Public User (Not Logged In)
- **Top Navigation**: Home, Browse Cars, About, Contact, Login, Register
- **Layout**: Standard full-width layout
- **Footer**: Full footer with company info
- **No Sidebar**

#### Mode 2: Admin Dashboard
- **Top Navigation**: Home, Browse Cars, Dashboard, Admin dropdown
- **Sidebar**: Fixed left sidebar with:
  - Dashboard
  - Manage Cars
  - Manage Customers
  - Manage Rentals
  - Reports (collapsible)
  - Back to Website
- **Layout**: Sidebar + content area
- **No Footer**

#### Mode 3: Customer Dashboard
- **Top Navigation**: Home, Browse Cars, My Dashboard, Customer dropdown
- **Sidebar**: Fixed left sidebar with:
  - Dashboard
  - My Rentals
  - Browse Cars
  - My Profile
  - About Us
  - Contact Us
- **Layout**: Sidebar + content area
- **No Footer**

### 3. Responsive Behavior

#### Desktop (?992px)
- Sidebar is fixed and visible
- Can be collapsed/expanded using toggle button
- State saved in localStorage
- Collapsed width: 70px, Expanded width: 260px

#### Mobile (<992px)
- Sidebar hidden by default
- Slides in from left when toggled
- Dark overlay covers content
- Closes when:
  - Overlay is clicked
  - Link is clicked
  - Toggle button is clicked again

### 4. CSS Organization

#### site.css
Contains global styles:
- Color variables
- Typography
- Buttons
- Cards
- Forms
- Utilities

#### admin.css
Contains dashboard-specific styles:
- `.admin-wrapper` - Main container
- `.admin-sidebar` - Sidebar styles
- `.admin-content` - Content area
- `.stat-card` - Statistics cards
- `.dashboard-widget` - Dashboard components
- Responsive breakpoints

#### _Layout.cshtml.css
Contains layout-specific styles:
- Navbar customizations
- Logo sizing
- Dropdown menus
- Footer styles
- Responsive navbar adjustments

## Key Features

### 1. Dynamic Sidebar Content
The sidebar menu items change based on user role:
```razor
@if (Context.Session.GetString("UserRole") == "Admin")
{
    <!-- Admin menu items -->
}
else if (Context.Session.GetString("UserRole") == "Customer")
{
    <!-- Customer menu items -->
}
```

### 2. Active Link Highlighting
Current page is automatically highlighted:
```razor
@(ViewContext.RouteData.Values["Controller"]?.ToString() == "Admin" ? "active" : "")
```

### 3. Mobile Overlay
For touch-friendly mobile experience:
```html
<div class="sidebar-overlay" id="sidebarOverlay"></div>
```

### 4. Persistent State
Desktop sidebar state persists across page loads:
```javascript
localStorage.setItem('sidebarCollapsed', isCollapsed);
```

## JavaScript Functionality

### Main Functions
1. **`isMobile()`** - Detects screen size
2. **`toggleSidebar()`** - Handles sidebar open/close logic
3. **Event Listeners**:
   - Sidebar toggle button
   - Overlay click
   - Link clicks (mobile)
   - Window resize

### Mobile-Specific Features
- Body scroll lock when sidebar open
- Touch-friendly overlay dismiss
- Auto-close on link click

### Desktop-Specific Features
- Collapse/expand animation
- localStorage persistence
- Smooth transitions

## Usage in Views

### Default Usage
All views automatically use `_Layout.cshtml` via `_ViewStart.cshtml`:
```razor
@{
    Layout = "_Layout";
}
```

### Override Layout (if needed)
To use a different layout in a specific view:
```razor
@{
    ViewData["Title"] = "Page Title";
    Layout = "_CustomLayout"; // Optional override
}
```

## Session Variables Used

- `Context.Session.GetString("UserRole")` - "Admin" | "Customer" | null
- `Context.Session.GetString("UserId")` - User ID
- `Context.Session.GetString("Username")` - Display name

## Styling Classes

### Navigation Classes
- `.navbar-logo` - Logo sizing
- `.nav-link` - Navigation links
- `.dropdown-item` - Dropdown menu items

### Sidebar Classes
- `.admin-sidebar` - Main sidebar container
- `.sidebar-header` - Sidebar header section
- `.sidebar-menu` - Menu list
- `.sidebar-item` - Individual menu item
- `.sidebar-link` - Menu link
- `.sidebar-link.active` - Active menu link
- `.sidebar-submenu` - Collapsible submenu
- `.sidebar-divider` - Visual separator

### Layout Classes
- `.admin-wrapper` - Main container for dashboard layout
- `.admin-content` - Content area (with sidebar)
- `.sidebar-overlay` - Mobile overlay

### State Classes
- `.collapsed` - Sidebar is collapsed (desktop)
- `.expanded` - Content area expanded (desktop)
- `.show` - Sidebar visible (mobile)
- `.sidebar-open` - Body scroll locked (mobile)

## Best Practices

1. **Always check user role** before displaying role-specific content
2. **Use session variables** for authentication state
3. **Test on multiple screen sizes** for responsive design
4. **Keep sidebar menu items organized** by functionality
5. **Maintain consistent styling** between Admin and Customer dashboards

## Troubleshooting

### Sidebar not showing
- Check if user is logged in
- Verify `UserRole` session variable is set
- Check browser console for JavaScript errors

### Sidebar state not persisting
- Check if localStorage is enabled in browser
- Verify JavaScript is running without errors
- Clear localStorage and try again

### Mobile overlay not working
- Ensure JavaScript is loaded
- Check z-index values in CSS
- Verify overlay element exists in DOM

## Future Enhancements

Potential improvements:
1. Add transition animations for page changes
2. Implement keyboard navigation for sidebar
3. Add search functionality in sidebar
4. Support for custom themes
5. Dark mode toggle

## Migration Notes

### Removed Files
- `_AdminLayout.cshtml` - Merged into `_Layout.cshtml`

### Benefits of Unified Layout
- Single source of truth
- Easier maintenance
- Consistent user experience
- Reduced code duplication
- Better SEO (consistent structure)
