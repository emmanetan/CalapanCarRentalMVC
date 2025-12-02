-- Add Coding column to Cars table
ALTER TABLE `Cars` ADD COLUMN `Coding` VARCHAR(20) NOT NULL DEFAULT '';

-- Update existing records with Coding values based on PlateNumber last digit
UPDATE `Cars` SET `Coding` = 
    CASE 
        WHEN RIGHT(`PlateNumber`, 1) IN ('1', '2') THEN 'Monday'
        WHEN RIGHT(`PlateNumber`, 1) IN ('3', '4') THEN 'Tuesday'
     WHEN RIGHT(`PlateNumber`, 1) IN ('5', '6') THEN 'Wednesday'
        WHEN RIGHT(`PlateNumber`, 1) IN ('7', '8') THEN 'Thursday'
        WHEN RIGHT(`PlateNumber`, 1) IN ('9', '0') THEN 'Friday'
 ELSE ''
    END;
