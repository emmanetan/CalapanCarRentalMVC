# Navigation Layout Update Documentation

## Overview
The navigation system has been updated to provide different user experiences based on authentication status:

### For Visitors (Not Logged In)
- **Top Navigation Bar** remains visible with:
  - Home
  - Browse Cars
  - Contact
  - About
  - Login
  - Register
- Footer is displayed at the bottom

### For Logged-In Users (Admin & Customer)
- **Fixed Top Bar** with:
  - **Left Side**: Sidebar toggle button + Logo + Brand name + Role badge
  - **Right Side**: Notification bell icon with badge count
- **Left Sidebar Navigation** with:
  - User Profile Section (Avatar + Username + Role)
  - Home
  - Browse Cars
  - Contact
  - About
  - Role-specific menu items (Dashboard, Manage sections for Admin, My Rentals/Profile for Customer)
  - Logout button
- No footer displayed

## Key Changes

### 1. Layout Structure (_Layout.cshtml)
- Split the layout into two distinct sections based on user role
- **Fixed Top Navigation Bar** for logged-in users:
  - Logo and brand name positioned on the **left**
  - Notification icon positioned on the **right**
  - Responsive design with collapsible brand text on mobile
- Moved navigation items from top navbar to left sidebar for authenticated users
- Added user profile section at the top of the sidebar with avatar icon
- Kept visitor navigation in the traditional top navbar format

### 2. Top Bar for Logged-In Users
- **Fixed positioning** for consistent visibility while scrolling
- **Left side** contains:
  - Hamburger menu toggle button
  - Logo image
  - Brand name "Calapan Car Rental"
  - Role badge (Admin/Customer)
- **Right side** contains:
  - Notification bell icon with red badge showing unread count
  - Dropdown notification panel

### 3. Notification System
- **Bell Icon** with notification count badge
- **Dropdown Panel** featuring:
  - Header with total notification count
  - Individual notification items with:
    - Colored icon indicators (Info, Warning, Success)
    - Notification title and description
    - Timestamp
  - Different notifications for Admin vs Customer roles
  - "View All Notifications" link at bottom
  - Smooth scrollbar for long notification lists
  - Responsive design (350px on desktop, 300px on mobile)

### 4. Sidebar User Profile Section
- Displays user avatar (different icons for Admin vs Customer)
- Shows username
- Shows role badge
- Styled with circular avatar background and hover effects
- Positioned at the top of the sidebar

### 5. Navigation Consolidation
- All navigation links (Home, Browse Cars, Contact, About) moved to sidebar
- Account dropdown replaced with user profile section + logout link
- Role-specific items remain separated (Admin has Manage sections, Customer has My sections)

### 6. Styling Updates (admin.css)
- Added `.navbar.fixed-top` for fixed top bar positioning
- Added `.navbar-actions` for right-side action items
- Added `.notification-badge` for unread count styling
- Added `.notification-dropdown` for notification panel
- Added `.notification-item` for individual notification styling
- Added `.notification-icon` for colored icon backgrounds
- Added `.notification-content` for notification text
- Added `.sidebar-user-profile` styling
- Added `.user-avatar` with circular design and hover effects
- Added `.user-info`, `.user-name`, and `.user-role` styling
- Updated sidebar gradient background
- Adjusted sidebar height to account for fixed navbar
- Enhanced responsive behavior for mobile devices
- Added padding-top to body for fixed navbar offset

## User Experience Benefits

### For Visitors
- Clean, traditional top navigation
- Easy access to all main pages
- Clear call-to-action buttons (Login/Register)
- Full footer with company information

### For Authenticated Users
- **Always-visible top bar** with branding and notifications
- **Notification system** for real-time updates
- **Easy access to notifications** without leaving the current page
- Persistent left sidebar for easy navigation
- All options accessible from one place
- User identity always visible in sidebar
- More screen space for content (no bottom footer)
- Collapsible sidebar on desktop for more workspace
- Mobile-friendly overlay sidebar
- Quick logout access from sidebar

## Notification Features

### Admin Notifications
- New rental requests
- Car maintenance alerts
- Payment confirmations
- System updates

### Customer Notifications
- Booking confirmations
- Rental reminders
- Return due dates
- Special offers

### Notification Styling
- **Info (Blue)**: General information, bookings
- **Warning (Yellow)**: Reminders, due dates
- **Success (Green)**: Confirmations, completed actions
- **Danger (Red)**: Urgent alerts (via badge count)

## Responsive Behavior

### Desktop (?992px)
- Fixed top bar always visible
- Sidebar is always visible (280px width)
- Can be collapsed to 70px width using toggle button
- Content area adjusts automatically
- Notification dropdown appears on right

### Tablet (768px - 991px)
- Fixed top bar always visible
- Sidebar hidden by default
- Opens as overlay when toggle button clicked
- Brand text may be hidden to save space
- Notification dropdown adjusts to screen width

### Mobile (<768px)
- Fixed top bar with minimal content
- Brand text hidden on small screens
- Sidebar as full-screen overlay
- Full-width overlay darkens background
- Closes when clicking outside or on navigation link
- Notification dropdown max-width 90vw

## Technical Details

### Session-Based Rendering
The layout conditionally renders based on:
```csharp
Context.Session.GetString("UserRole") == "Admin"
Context.Session.GetString("UserRole") == "Customer"
Context.Session.GetString("Username")
```

### CSS Classes
- `.admin-body` - Applied to body when user is logged in (with padding-top for fixed navbar)
- `.navbar.fixed-top` - Fixed positioning for top bar
- `.navbar-actions` - Container for right-side actions
- `.notification-badge` - Red badge showing notification count
- `.notification-dropdown` - Dropdown panel for notifications
- `.notification-item` - Individual notification item
- `.notification-icon` - Colored icon background
- `.notification-content` - Notification text content
- `.admin-wrapper` - Flex container for sidebar + content
- `.admin-sidebar` - Left sidebar navigation (fixed position)
- `.sidebar-user-profile` - User profile section
- `.user-avatar` - Circular avatar container
- `.admin-content` - Main content area (with left margin for sidebar)
- `.sidebar-overlay` - Mobile overlay background

### Layout Structure
```
Fixed Top Bar (56px height)
??? Left: Toggle + Logo + Brand + Badge
??? Right: Notification Icon + Dropdown

Body Content (with padding-top: 56px)
??? Sidebar (fixed, 280px width)
?   ??? User Profile Section
?   ??? Navigation Links
?   ??? Role-specific Links
?   ??? Logout
??? Main Content (margin-left: 280px)
    ??? Page Content
```

### JavaScript
- Sidebar toggle functionality preserved
- Mobile responsive behavior maintained
- Collapse/expand state saved in localStorage (desktop only)
- Notification dropdown managed by Bootstrap

### Z-Index Hierarchy
- Fixed navbar: 1040
- Sidebar: 1030
- Mobile overlay: 1020
- Regular content: auto

## Browser Compatibility
- Modern browsers (Chrome, Firefox, Safari, Edge)
- Responsive design works on all screen sizes
- Touch-friendly for mobile devices
- Smooth animations and transitions

## Future Enhancements
- Add real-time notification updates via SignalR
- Add user profile image upload capability
- Add notification sound alerts
- Add mark as read/unread functionality
- Add notification filtering and search
- Add quick settings in user profile section
- Add theme switcher option
- Add notification preferences/settings
- Connect to backend notification system
- Add notification pagination for large lists
