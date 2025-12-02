-- Add IsVerifiedByAdmin and VerifiedDate columns to Users table
ALTER TABLE `Users` 
ADD COLUMN `IsVerifiedByAdmin` tinyint(1) NOT NULL DEFAULT 0,
ADD COLUMN `VerifiedDate` datetime(6) NULL;

-- Set all existing users to verified (to avoid locking out existing users)
-- Admin users should always be verified
UPDATE `Users` SET `IsVerifiedByAdmin` = 1 WHERE `is_Admin` = 0;

-- Optional: Set existing customers to verified as well (remove this line if you want to manually verify them)
UPDATE `Users` SET `IsVerifiedByAdmin` = 1 WHERE `is_Admin` = 1;
