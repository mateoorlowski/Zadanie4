-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2021-04-05 12:56:53.13

-- tables
-- Table: Order
CREATE TABLE "Order"
(
    IdOrder     INT      NOT NULL IDENTITY,
    IdProduct   INT      NOT NULL,
    Amount      INT      NOT NULL,
    CreatedAt   DATETIME NOT NULL,
    FulfilledAt DATETIME NULL,
    CONSTRAINT Order_pk PRIMARY KEY (IdOrder)
);

-- Table: Product
CREATE TABLE Product
(
    IdProduct   INT            NOT NULL IDENTITY,
    Name        NVARCHAR(200)  NOT NULL,
    Description NVARCHAR(200)  NOT NULL,
    Price       NUMERIC(25, 2) NOT NULL,
    CONSTRAINT Product_pk PRIMARY KEY (IdProduct)
);

-- Table: Product_Warehouse
CREATE TABLE Product_Warehouse
(
    IdProductWarehouse INT            NOT NULL IDENTITY,
    IdWarehouse        INT            NOT NULL,
    IdProduct          INT            NOT NULL,
    IdOrder            INT            NOT NULL,
    Amount             INT            NOT NULL,
    Price              NUMERIC(25, 2) NOT NULL,
    CreatedAt          DATETIME       NOT NULL,
    CONSTRAINT Product_Warehouse_pk PRIMARY KEY (IdProductWarehouse)
);

-- Table: Warehouse
CREATE TABLE Warehouse
(
    IdWarehouse INT           NOT NULL IDENTITY,
    Name        NVARCHAR(200) NOT NULL,
    Address     NVARCHAR(200) NOT NULL,
    CONSTRAINT Warehouse_pk PRIMARY KEY (IdWarehouse)
);

-- foreign keys
-- Reference: Product_Warehouse_Order (table: Product_Warehouse)
ALTER TABLE Product_Warehouse
    ADD CONSTRAINT Product_Warehouse_Order
        FOREIGN KEY (IdOrder)
            REFERENCES "Order" (IdOrder);

-- Reference: Receipt_Product (table: Order)
ALTER TABLE "Order"
    ADD CONSTRAINT Receipt_Product
        FOREIGN KEY (IdProduct)
            REFERENCES Product (IdProduct);

-- Reference: _Product (table: Product_Warehouse)
ALTER TABLE Product_Warehouse
    ADD CONSTRAINT _Product
        FOREIGN KEY (IdProduct)
            REFERENCES Product (IdProduct);

-- Reference: _Warehouse (table: Product_Warehouse)
ALTER TABLE Product_Warehouse
    ADD CONSTRAINT _Warehouse
        FOREIGN KEY (IdWarehouse)
            REFERENCES Warehouse (IdWarehouse);

-- End of file.

GO

INSERT INTO Warehouse(Name, Address)
VALUES ('Warsaw', 'Kwiatowa 12');

GO

INSERT INTO Product(Name, Description, Price)
VALUES ('Abacavir', '', 25.5),
       ('Acyclovir', '', 45.0),
       ('Allopurinol', '', 30.8);

GO

INSERT INTO "Order"(IdProduct, Amount, CreatedAt)
VALUES ((SELECT IdProduct FROM Product WHERE Name = 'Abacavir'), 125, GETDATE());