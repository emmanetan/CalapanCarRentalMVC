# URGENT: Add Coding Column to Database

## ?? Why You're Seeing Empty Coding Field

The Coding field is showing but **empty** because the database column hasn't been created yet!

---

## ?? Step-by-Step Instructions for XAMPP phpMyAdmin:

### **Step 1: Open phpMyAdmin**
1. Make sure XAMPP is running (Apache and MySQL should be started)
2. Open your web browser
3. Go to: `http://localhost/phpmyadmin`

### **Step 2: Select Your Database**
1. Look at the left sidebar
2. Click on **`calapan_car_rental`** database
3. The database name should be highlighted

### **Step 3: Execute SQL Commands**
1. Click the **"SQL"** tab at the top (looks like a page with code)
2. You'll see a text area where you can type SQL commands
3. **Copy and paste this ENTIRE code block:**

```sql
ALTER TABLE `Cars` ADD COLUMN `Coding` VARCHAR(20) NOT NULL DEFAULT '';

UPDATE `Cars` SET `Coding` = 
    CASE 
        WHEN RIGHT(`PlateNumber`, 1) = '1' THEN 'Monday'
        WHEN RIGHT(`PlateNumber`, 1) = '2' THEN 'Monday'
        WHEN RIGHT(`PlateNumber`, 1) = '3' THEN 'Tuesday'
        WHEN RIGHT(`PlateNumber`, 1) = '4' THEN 'Tuesday'
        WHEN RIGHT(`PlateNumber`, 1) = '5' THEN 'Wednesday'
        WHEN RIGHT(`PlateNumber`, 1) = '6' THEN 'Wednesday'
        WHEN RIGHT(`PlateNumber`, 1) = '7' THEN 'Thursday'
        WHEN RIGHT(`PlateNumber`, 1) = '8' THEN 'Thursday'
        WHEN RIGHT(`PlateNumber`, 1) = '9' THEN 'Friday'
      WHEN RIGHT(`PlateNumber`, 1) = '0' THEN 'Friday'
        ELSE ''
    END;
```

4. Click the **"Go"** button (bottom right)

### **Step 4: Verify Success**
You should see a green message saying:
- ? "X row(s) affected" or
- ? "Query OK"

### **Step 5: Check the Data**
1. Click on **"Cars"** table in the left sidebar
2. Click the **"Browse"** tab
3. Scroll right - you should see a new **"Coding"** column
4. Each car should show values like: **Monday**, **Tuesday**, etc.

---

## ?? What Each Car's Coding Should Be:

Based on the **last digit** of the PlateNumber:

| Last Digit | Coding Day |
|------------|------------|
| 1 or 2 | Monday |
| 3 or 4 | Tuesday |
| 5 or 6 | Wednesday |
| 7 or 8 | Thursday |
| 9 or 0 | Friday |

---

## ? After Running the SQL:

1. **Stop your application** in Visual Studio (press the Stop button or Shift+F5)
2. **Run it again** (press F5)
3. **Navigate to:**
   - Home page ? Featured vehicles should show coding
   - Fleet/Vehicle Index ? Each car card should show coding
   - Vehicle Details ? Should show "Coding Day: Monday" (or appropriate day)

---

## ?? If You Get an Error:

### Error: "Column 'Coding' already exists"
**Solution:** The column exists but is empty. Run only the UPDATE part:

```sql
UPDATE `Cars` SET `Coding` = 
CASE 
        WHEN RIGHT(`PlateNumber`, 1) = '1' THEN 'Monday'
    WHEN RIGHT(`PlateNumber`, 1) = '2' THEN 'Monday'
  WHEN RIGHT(`PlateNumber`, 1) = '3' THEN 'Tuesday'
        WHEN RIGHT(`PlateNumber`, 1) = '4' THEN 'Tuesday'
        WHEN RIGHT(`PlateNumber`, 1) = '5' THEN 'Wednesday'
        WHEN RIGHT(`PlateNumber`, 1) = '6' THEN 'Wednesday'
        WHEN RIGHT(`PlateNumber`, 1) = '7' THEN 'Thursday'
        WHEN RIGHT(`PlateNumber`, 1) = '8' THEN 'Thursday'
        WHEN RIGHT(`PlateNumber`, 1) = '9' THEN 'Friday'
        WHEN RIGHT(`PlateNumber`, 1) = '0' THEN 'Friday'
    ELSE ''
    END;
```

### Error: "Table 'Cars' doesn't exist"
**Solution:** Your table might have a different name. Check the left sidebar in phpMyAdmin to see the exact table name.

---

## ?? Visual Checklist:

**Before running SQL:**
- Coding Day: _(empty)_
- This vehicle cannot be rented on _(nothing)_

**After running SQL:**
- Coding Day: **Monday** ??
- This vehicle cannot be rented on **Monday**

---

## ?? You're Almost Done!

Just run that SQL in phpMyAdmin and you'll see the coding days appear immediately on all your car listings!
