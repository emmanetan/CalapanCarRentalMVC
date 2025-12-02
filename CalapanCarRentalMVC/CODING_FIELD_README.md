# Coding Field Implementation Summary

## Changes Made:

### 1. **Car Model** (`CalapanCarRentalMVC\Models\Car.cs`)
- Added `Coding` property to store the coding day (Monday-Friday)
- Field is not required to avoid migration issues with existing data
- Default value is empty string

### 2. **Create View** (`CalapanCarRentalMVC\Views\Vehicle\Create.cshtml`)
- Added Coding input field (readonly) after PlateNumber field
- Added JavaScript to automatically calculate and set Coding based on PlateNumber
- Rules:
  - PlateNumber ending in 1 or 2 ? Monday
  - PlateNumber ending in 3 or 4 ? Tuesday
  - PlateNumber ending in 5 or 6 ? Wednesday
  - PlateNumber ending in 7 or 8 ? Thursday
  - PlateNumber ending in 9 or 0 ? Friday

### 3. **Edit View** (`CalapanCarRentalMVC\Views\Vehicle\Edit.cshtml`)
- Added same Coding field and JavaScript logic as Create view
- Automatically updates Coding when PlateNumber is changed

### 4. **Database Context** (`CalapanCarRentalMVC\Data\CarRentalContext.cs`)
- Updated seed data to include Coding values for sample cars

### 5. **SQL Script** (`add_coding_column.sql`)
- Script to manually add the Coding column to the database
- Includes UPDATE statement to populate existing records

## Next Steps:

### **IMPORTANT: Run the SQL Script**
You need to execute the `add_coding_column.sql` file on your MySQL database:

**Option 1 - MySQL Workbench/phpMyAdmin:**
1. Open your MySQL client
2. Connect to the `calapan_car_rental` database
3. Open and execute `add_coding_column.sql`

**Option 2 - Command Line:**
```bash
mysql -u root -p calapan_car_rental < "C:\Users\emman\source\repos\CalapanCarRentalMVC\CalapanCarRentalMVC\add_coding_column.sql"
```

### After Running the SQL:
1. The Coding column will be added to the Cars table
2. Existing cars will automatically have their Coding values set based on their plate numbers
3. The application will work without errors
4. When adding new cars, the Coding field will automatically populate as you type the PlateNumber

## How It Works:

1. User types a PlateNumber (e.g., "ABC1234")
2. JavaScript detects the last character ("4")
3. Based on the last digit, automatically sets Coding to appropriate day ("Tuesday")
4. Field is readonly but can be viewed by user
5. Value is submitted with the form when creating/editing a car

## Testing:

After running the SQL script, test by:
1. Going to Add New Car page
2. Enter a PlateNumber ending in different digits (1-9, 0)
3. Verify the Coding field automatically updates correctly
4. Submit the form and verify the car is saved with the correct Coding value
