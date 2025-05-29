CREATE TABLE IF NOT EXISTS admin (
    Admin_ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Phone_Number VARCHAR(15) NULL,
    Role VARCHAR(50) NULL,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL, -- Store hashed passwords, not plain text!
    Gender VARCHAR(10) NULL,
    Date_Of_Birth DATE NULL,
    Status VARCHAR(20) DEFAULT 'Active',
    Email VARCHAR(100) NULL UNIQUE,
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
    -- Audit fields - will make them nullable for now
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL
);

-- Dummy Data for admin
INSERT INTO admin (Name, Phone_Number, Role, Username, Password, Gender, Date_Of_Birth, Status, Email) VALUES
('Super Admin', '0300-1234567', 'System Administrator', 'superadmin', 'hashed_password_SA', 'Male', '1980-01-01', 'Active', 'superadmin@example.com'),
('Manager Admin', '0301-7654321', 'Manager', 'manageradmin', 'hashed_password_MA', 'Female', '1985-05-10', 'Active', 'manageradmin@example.com'),
('Support Admin', '0302-1122334', 'Support Lead', 'supportadmin', 'hashed_password_SUA', 'Male', '1990-07-15', 'Inactive', 'supportadmin@example.com');
CREATE TABLE IF NOT EXISTS employee (
    Employee_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Admin_ID INT NULL, -- Renamed from Admin_ID to avoid confusion with a primary key, make it nullable if an employee isn't always tied to an admin
    Name VARCHAR(100) NOT NULL,
    Phone_Number VARCHAR(15) NULL,
    Role VARCHAR(50) NULL,             -- e.g., 'Sales Rep', 'Accountant', 'Manager'
    Salary DECIMAL(10, 2) NULL,
    Username VARCHAR(50) NOT NULL UNIQUE, -- Assuming employees can also log in
    Password VARCHAR(255) NOT NULL,    -- Store hashed passwords
    Gender VARCHAR(10) NULL,
    Date_Of_Birth DATE NULL,
    Status VARCHAR(20) DEFAULT 'Active', -- e.g., 'Active', 'On Leave', 'Terminated'
    Email VARCHAR(100) NULL UNIQUE,
    Address VARCHAR(255) NULL,
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields from diagram [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,
    FOREIGN KEY (FK_Admin_ID) REFERENCES admin(Admin_ID) ON DELETE SET NULL -- Example: if admin is deleted, employee's FK_Admin_ID becomes NULL
);
INSERT INTO employee (FK_Admin_ID, Name, Phone_Number, Role, Salary, Username, Password, Gender, Date_Of_Birth, Status, Email, Address) VALUES
(1, 'John Doe', '0310-1234567', 'Sales Manager', 75000.00, 'johnd', 'hashed_password_JD', 'Male', '1988-03-15', 'Active', 'john.doe@example.com', '123 Main St, City A'),
(1, 'Jane Smith', '0311-7654321', 'Accountant', 60000.00, 'janes', 'hashed_password_JS', 'Female', '1992-07-20', 'Active', 'jane.smith@example.com', '456 Oak Ave, City B'),
(2, 'Mike Brown', '0312-1122334', 'Sales Representative', 50000.00, 'mikeb', 'hashed_password_MB', 'Male', '1995-11-01', 'On Leave', 'mike.brown@example.com', '789 Pine Rd, City C');


CREATE TABLE IF NOT EXISTS supplier (
    Supplier_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Admin_ID INT NULL, -- If an admin manages this supplier
    Name VARCHAR(100) NOT NULL,
    Phone_Number VARCHAR(15) NULL,
    Role VARCHAR(50) NULL,             -- Contact person's role or supplier type
    Plant_Name VARCHAR(100) NULL,
    Username VARCHAR(50) NULL UNIQUE,  -- If suppliers have login accounts
    Password VARCHAR(255) NULL,     -- Store hashed passwords if they log in
    Status VARCHAR(20) DEFAULT 'Active', -- e.g., 'Active', 'Preferred', 'Discontinued'
    Email VARCHAR(100) NULL UNIQUE,
    Supplier_Rating DECIMAL(3,1) NULL, -- e.g., 4.5
    BRN_TAX_ID VARCHAR(50) NULL UNIQUE,
    Address VARCHAR(255) NULL,
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,
    FOREIGN KEY (FK_Admin_ID) REFERENCES admin(Admin_ID) ON DELETE SET NULL
);

INSERT INTO supplier (FK_Admin_ID, Name, Phone_Number, Role, Plant_Name, Username, Password, Status, Email, Supplier_Rating, BRN_TAX_ID, Address) VALUES
(1, 'Global Tech Parts', '0320-1234567', 'Component Supplier', 'Main Plant A', 'globaltech', 'hashed_password_GT', 'Active', 'contact@globaltech.com', 4.5, 'GT12345TAX', '10 Industrial Ave'),
(2, 'Local Packaging Inc.', '0321-7654321', 'Packaging Provider', 'Unit B Packaging', 'localpack', 'hashed_password_LP', 'Active', 'sales@localpack.com', 4.0, 'LP67890TAX', '25 Warehouse Rd'),
(1, 'Premium Materials Co.', '0322-1122334', 'Raw Material Supplier', 'Primary Mill', NULL, NULL, 'Preferred', 'info@premiummaterials.com', 4.8, 'PMC11223TAX', '60 Resource Blvd');


CREATE TABLE IF NOT EXISTS product (
    Product_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Supplier_ID INT NULL, -- Changed from Supplier_ID for clarity as FK
    Category VARCHAR(100) NOT NULL,
    Description TEXT NULL,
    Price DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    Quantity INT NOT NULL DEFAULT 0,      -- Current stock quantity
    Status VARCHAR(20) DEFAULT 'In Stock', -- e.g., 'In Stock', 'Out of Stock', 'Low Stock'
    Batch_No VARCHAR(50) NULL,
    Weight DECIMAL(10,2) NULL,             -- As seen in orders diagram, might be relevant here
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,
    FOREIGN KEY (FK_Supplier_ID) REFERENCES supplier(Supplier_ID) ON DELETE SET NULL -- If supplier is deleted, product's supplier link becomes NULL
);

-- Dummy Data for product
INSERT INTO product (FK_Supplier_ID, Category, Description, Price, Quantity, Status, Batch_No, Weight) VALUES
(1, 'Electronics', 'High-performance CPU Model X2000', 250.00, 100, 'In Stock', 'CPU-X2000-001', 0.2),
(1, 'Electronics', 'Motherboard Z590 Pro Series', 180.50, 75, 'In Stock', 'MB-Z590-005', 1.1),
(2, 'Packaging', 'Corrugated Shipping Boxes (Medium)', 1.25, 2000, 'In Stock', 'BOX-M-050', 0.1),
(3, 'Raw Materials', 'Grade A Aluminum Ingots', 1500.00, 50, 'Low Stock', 'AL-ING-010', 25.0); -- Price per unit, e.g. per Ton



CREATE TABLE IF NOT EXISTS customer (
    Customer_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Admin_ID INT NULL, -- If an admin manages this customer
    Name VARCHAR(100) NOT NULL,
    Phone_Number VARCHAR(15) NULL,
    Address VARCHAR(255) NULL,
    Gender VARCHAR(10) NULL,
    Status VARCHAR(20) DEFAULT 'Active', -- e.g., 'Active', 'Inactive', 'Prospect'
    Email VARCHAR(100) NULL UNIQUE,
    Customer_Type VARCHAR(50) NULL,      -- e.g., 'Individual', 'Business'
    Business_Name VARCHAR(100) NULL,     -- If Customer_Type is 'Business'
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,
    FOREIGN KEY (FK_Admin_ID) REFERENCES admin(Admin_ID) ON DELETE SET NULL
);

INSERT INTO customer (FK_Admin_ID, Name, Phone_Number, Address, Gender, Status, Email, Customer_Type, Business_Name) VALUES
(2, 'Alice Wonderland', '0330-1234567', '7 Wonder Lane, Fantasytown', 'Female', 'Active', 'alice@example.com', 'Individual', NULL),
(2, 'Bob The Builder Co.', '0331-7654321', '1 Construction Rd, Buildville', NULL, 'Active', 'contact@bobbuilder.com', 'Business', 'Bob The Builder Co.'),
(1, 'Charlie Brown', '0332-1122334', '9 Peanuts Ave, Comicstrip City', 'Male', 'Inactive', 'charlie@example.com', 'Individual', NULL);



CREATE TABLE IF NOT EXISTS orders (
    Order_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Customer_ID INT NOT NULL,
    FK_Product_ID INT NOT NULL,
    FK_Supplier_ID INT NULL,               -- Supplier of the product in this order, or general order supplier
    Order_Date DATE NOT NULL,
    Delivery_Date DATE NULL,
    Quantity INT NOT NULL,
    Carton INT NULL,                       -- Number of cartons
    Weight DECIMAL(10,2) NULL,             -- Total weight for this order/item
    Total_Amount DECIMAL(12, 2) NOT NULL,  -- Could be Quantity * Product.Price or a specific order amount
    Order_Status VARCHAR(50) DEFAULT 'Pending', -- e.g., 'Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'
    Batch_No VARCHAR(50) NULL,             -- Product batch number for this order
    Vehicle_No VARCHAR(50) NULL,           -- Vehicle number for delivery
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,

    FOREIGN KEY (FK_Customer_ID) REFERENCES customer(Customer_ID) ON DELETE RESTRICT, -- Don't delete customer if they have orders
    FOREIGN KEY (FK_Product_ID) REFERENCES product(Product_ID) ON DELETE RESTRICT,   -- Don't delete product if it's in orders
    FOREIGN KEY (FK_Supplier_ID) REFERENCES supplier(Supplier_ID) ON DELETE SET NULL -- If supplier linked to order is deleted, set to NULL
);


-- Dummy Data for orders
INSERT INTO orders (FK_Customer_ID, FK_Product_ID, FK_Supplier_ID, Order_Date, Delivery_Date, Quantity, Carton, Weight, Total_Amount, Order_Status, Batch_No, Vehicle_No) VALUES
(1, 1, 1, '2024-05-20', '2024-05-25', 2, 1, 0.4, 500.00, 'Shipped', 'CPU-X2000-001', 'TRK-001'),
(2, 3, 2, '2024-05-22', '2024-05-28', 100, 5, 10.0, 125.00, 'Processing', 'BOX-M-050', NULL),
(1, 2, 1, '2024-05-23', NULL, 1, 1, 1.1, 180.50, 'Delivered', 'MB-Z590-005', 'TRK-002'),
(3, 4, 3, '2024-05-24', NULL, 10, 2, 250.0, 15000.00, 'Pending', 'AL-ING-010', NULL);




CREATE TABLE IF NOT EXISTS report (
    Report_ID INT AUTO_INCREMENT PRIMARY KEY,
    FK_Employee_ID INT NULL,       -- Employee who generated or is related to the report
    FK_Supplier_ID INT NULL,       -- If the report is about a specific supplier
    FK_Customer_ID INT NULL,       -- If the report is about a specific customer
    FK_Order_ID INT NULL,          -- If the report is about a specific order
    FK_Product_ID INT NULL,        -- If the report is about a specific product
    Report_Type VARCHAR(100) NOT NULL, -- e.g., 'Sales Summary', 'Inventory Status', 'Employee Performance'
    Date_Generated DATETIME NOT NULL,
    Status VARCHAR(50) DEFAULT 'Completed', -- e.g., 'Generated', 'Pending Review', 'Archived'
    Report_Details TEXT NULL,              -- Could store summary, parameters, or even generated content path
    Created_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Updated_At TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Audit fields from your diagram (Changed_Field VARCHAR(10...) needs full length) [cite: 1]
    -- New_Value TEXT NULL,
    -- Old_Value TEXT NULL,
    -- Changed_Field VARCHAR(100) NULL,
    -- Changed_At DATETIME NULL,

    FOREIGN KEY (FK_Employee_ID) REFERENCES employee(Employee_ID) ON DELETE SET NULL,
    FOREIGN KEY (FK_Supplier_ID) REFERENCES supplier(Supplier_ID) ON DELETE SET NULL,
    FOREIGN KEY (FK_Customer_ID) REFERENCES customer(Customer_ID) ON DELETE SET NULL,
    FOREIGN KEY (FK_Order_ID) REFERENCES orders(Order_ID) ON DELETE SET NULL,
    FOREIGN KEY (FK_Product_ID) REFERENCES product(Product_ID) ON DELETE SET NULL
);


INSERT INTO report (FK_Employee_ID, Report_Type, Date_Generated, Status, Report_Details) VALUES
(1, 'Monthly Sales Summary', '2024-05-01 00:00:00', 'Completed', 'Summary of all sales in April 2024.'),
(2, 'Product Inventory Status', '2024-05-28 10:00:00', 'Generated', 'Current stock levels for all products.'),
(1, 'Supplier Performance Review Q1', '2024-04-05 00:00:00', 'Archived', 'Performance metrics for supplier ID 101 for Q1.');


CREATE VIEW DailyRevenue AS
SELECT
    DATE(Order_Date) AS SaleDate,
    SUM(Total_Amount) AS DailyTotalRevenue
FROM orders
WHERE Order_Status IN ('Completed', 'Shipped') -- Or whatever statuses count as revenue
GROUP BY DATE(Order_Date);





ALTER TABLE employee
ADD COLUMN Nationality VARCHAR(100) NULL AFTER Address,
ADD COLUMN Hire_Date DATE NULL AFTER Date_Of_Birth,
ADD COLUMN Last_Login DATETIME NULL AFTER Password, 
ADD COLUMN Login_Notifications_Enabled BOOLEAN DEFAULT TRUE NULL AFTER Email,
ADD COLUMN Language_Preference VARCHAR(10) DEFAULT 'en-US' NULL AFTER Login_Notifications_Enabled,
ADD COLUMN Time_Zone VARCHAR(50) NULL AFTER Language_Preference;


-- For Employee_ID = 1 (John Doe)
UPDATE employee
SET 
    Nationality = 'American',
    Hire_Date = '2020-06-01',
    Last_Login = '2025-05-27 09:00:00',
    Login_Notifications_Enabled = TRUE,
    Language_Preference = 'en-US',
    Time_Zone = 'America/New_York'
WHERE Employee_ID = 1;

-- For Employee_ID = 2 (Jane Smith)
UPDATE employee
SET 
    Nationality = 'Canadian',
    Hire_Date = '2019-08-15',
    Last_Login = '2025-05-28 10:30:00',
    Login_Notifications_Enabled = TRUE,
    Language_Preference = 'en-CA',
    Time_Zone = 'America/Toronto'
WHERE Employee_ID = 2;

-- For Employee_ID = 3 (Mike Brown)
UPDATE employee
SET 
    Nationality = 'British',
    Hire_Date = '2021-01-10',
    Last_Login = '2025-05-25 14:15:00',
    Login_Notifications_Enabled = FALSE,
    Language_Preference = 'en-GB',
    Time_Zone = 'Europe/London'
WHERE Employee_ID = 3;


ALTER TABLE orders
ADD COLUMN Export_Through VARCHAR(255) NULL COMMENT 'Jis ke through export hua' AFTER FK_Supplier_ID,
ADD COLUMN Plant VARCHAR(255) NULL COMMENT 'Plant ka naam jahan se order process hua' AFTER Export_Through,
ADD COLUMN Importer VARCHAR(255) NULL COMMENT 'Importer ka naam' AFTER Plant,
ADD COLUMN Phyto_Number VARCHAR(100) NULL COMMENT 'Phytosanitary certificate number' AFTER Batch_No,
ADD COLUMN Rate DECIMAL(10, 2) NULL COMMENT 'Per unit rate jo Total_Amount calculate karne ke liye use hua' AFTER Weight,
ADD COLUMN Amount_Received DECIMAL(12, 2) NULL DEFAULT 0.00 COMMENT 'Kitna amount receive hua hai' AFTER Total_Amount,
ADD COLUMN Payment_Status VARCHAR(50) NULL DEFAULT 'Pending' COMMENT 'Payment ka status (e.g., Pending, Partially Paid, Paid)' AFTER Amount_Received;


ALTER TABLE orders
ADD CONSTRAINT UQ_Phyto_Number UNIQUE (Phyto_Number);

UPDATE orders
SET 
    Export_Through = 'Sea Link Logistics',
    Plant = 'Main Processing Unit',
    Importer = 'International Goods Co.',
    Phyto_Number = 'PN-EXP-2025-001',
    Rate = 150.75, -- Example rate
    Amount_Received = 1000.00,
    Payment_Status = 'Partially Paid'
WHERE Order_ID = 1; -- Apne existing Order_ID ke hisab se change karein

UPDATE orders
SET 
    Export_Through = 'Air Express Cargo',
    Plant = 'Alpha Packaging Facility',
    Importer = 'National Retailers',
    Phyto_Number = 'PN-EXP-2025-002',
    Rate = 75.50,
    Amount_Received = 500.00,
    Payment_Status = 'Paid'
WHERE Order_ID = 2; 

INSERT INTO orders 
(FK_Customer_ID, FK_Product_ID, FK_Supplier_ID, Order_Date, Delivery_Date, Quantity, Carton, Weight, Total_Amount, Export_Through, Plant, Importer, Phyto_Number, Rate, Amount_Received, Payment_Status, Batch_No, Vehicle_No) 
VALUES
(1, 1, 1, '2025-06-10', '2025-06-15', 50, 5, 100.00, 7537.50, 'Global Shippers', 'Primary Plant', 'ImportEx Ltd.', 'PN-EXP-2025-003', 150.75, 7000.00, 'Partially Paid', 'BATCH-A001', 'TRUCK-007'),
(2, 3, 2, '2025-06-12', NULL, 200, 10, 50.00, 15100.00, 'Oceanic Freight', 'Beta Production', 'Worldwide Goods', 'PN-EXP-2025-004', 75.50, 0.00, 'Pending', 'BATCH-C003', NULL);

