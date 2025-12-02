-- Add LocationTracking columns to Users table
ALTER TABLE Users 
ADD COLUMN LocationTrackingEnabled TINYINT(1) NOT NULL DEFAULT 0;

ALTER TABLE Users 
ADD COLUMN LocationTrackingEnabledDate DATETIME(6) NULL;
