-- Crear la base de datos
create DATABASE CommerceDB;
GO

USE CommerceDB;
GO

CREATE TABLE States (
    StateId nvarchar(50) PRIMARY KEY,
    StateName NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE Commerce (
    CommerceId nvarchar(50) PRIMARY KEY,
    CommerceName NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255),
    RUC NVARCHAR(50),
    State nvarchar(50),
    FOREIGN KEY (State) REFERENCES States(StateId)
);
GO

CREATE TABLE Users (
    UserId  nvarchar(50) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Owner', 'Employee')),
    CommerceId nvarchar(50),
    State nvarchar(50),
    FOREIGN KEY (CommerceId) REFERENCES Commerce(CommerceId),
    FOREIGN KEY (State) REFERENCES States(StateId)
);
GO

CREATE TABLE Sales (
    SaleId nvarchar(50) PRIMARY KEY,
    SaleDate DATETIME NOT NULL DEFAULT GETDATE(),
    UserId nvarchar(50),
    CommerceId nvarchar(50),
    State nvarchar(50) ,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CommerceId) REFERENCES Commerce(CommerceId),
    FOREIGN KEY (State) REFERENCES States(StateId)
);
GO

CREATE TABLE SaleDetails (
    DetailId nvarchar(50) PRIMARY KEY,
    SaleId nvarchar(50),
    Product NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (SaleId) REFERENCES Sales(SaleId)
);
GO

CREATE LOGIN SecureUserLogin 
WITH PASSWORD = 'S3cureP@ssw0rd!2024'
GO
CREATE USER SecureUser FOR LOGIN SecureUserLogin;
GO

EXEC sp_addrolemember 'db_owner', 'SecureUser';



CREATE PROCEDURE SpGetUserById
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        UserId,
        Username,
        Password,
        Role,
        CommerceId,
        State
    FROM 
        Users
    WHERE 
        UserId = @UserId;
END;

create PROCEDURE SpUserLogin
    @Username NVARCHAR(50),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DecryptedPassword NVARCHAR(255);
    SELECT 
        @DecryptedPassword = CONVERT(NVARCHAR(255), DecryptByPassPhrase(@Password, Password))
    FROM 
        Users
    WHERE 
        Username = @Username;

    IF @DecryptedPassword is not null
    BEGIN
        SELECT 
            UserId,
            Username,
            Password,
            Role,
            CommerceId,
            State
        FROM 
            Users
        WHERE 
            Username = @Username ;
    END
    ELSE
    BEGIN
        SELECT NULL AS UserId;
    END
END;
GO

CREATE PROCEDURE SpCreateOrUpdateUser
    @UserId NVARCHAR(50),
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @Role NVARCHAR(50),
    @CommerceId UNIQUEIDENTIFIER,
    @State UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EncryptedPassword NVARCHAR(255);

    SET @EncryptedPassword = CONVERT(NVARCHAR(255), EncryptByPassPhrase(@Password, @Password));

    IF EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId)
    BEGIN

        UPDATE Users
        SET 
            Username = @Username,
            Password = @EncryptedPassword,  
            Role = @Role,
            CommerceId = @CommerceId,
            State = @State
        WHERE 
            UserId = @UserId;
        
        SELECT 'Usuario actualizado exitosamente' AS Message;
    END
    ELSE
    BEGIN

        INSERT INTO Users (UserId, Username, Password, Role, CommerceId, State)
        VALUES (CONVERT(NVARCHAR(50), NEWID()), @Username, @EncryptedPassword, @Role, @CommerceId, @State);
        
        SELECT 'Usuario creado exitosamente' AS Message;
    END
END;
GO


CREATE PROCEDURE SpGetAllUsers
    @OffSet INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, Username, Password, Role, CommerceId, State
    FROM Users
    ORDER BY Username  
    OFFSET @OffSet ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO

CREATE PROCEDURE SpCreateCommerceAndOwner
    @CommerceName NVARCHAR(100),
    @Address NVARCHAR(255),
    @RUC NVARCHAR(50),
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(50),
    @Role NVARCHAR(50) = 'Owner'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StateId nvarchar(50);
    DECLARE @CommerceId nvarchar(50);
    DECLARE @UserId nvarchar(50);
    DECLARE @EncryptedPassword NVARCHAR(255);

    SELECT @StateId = StateId FROM States WHERE StateName = 'Active';

    SET @CommerceId = CONVERT(NVARCHAR(50), NEWID());;
    INSERT INTO Commerce (CommerceId, CommerceName, Address, RUC, State)
    VALUES (@CommerceId, @CommerceName, @Address, @RUC, @StateId);

    SET @EncryptedPassword = CONVERT(NVARCHAR(255), EncryptByPassPhrase(@Password, @Password));

    SET @UserId = CONVERT(NVARCHAR(50), NEWID());;
    INSERT INTO Users (UserId, Username, Password, Role, CommerceId, State)
    VALUES (@UserId, @Username, @EncryptedPassword, @Role, @CommerceId, @StateId);

    SELECT 'Comercio y propietario creados exitosamente' AS Message;
END;
GO


Create PROCEDURE SpDeleteUserAndCommerce
    @UserId nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CommerceId UNIQUEIDENTIFIER;

    SELECT @CommerceId = CommerceId FROM Users WHERE UserId = @UserId AND Role = 'Owner';

    DELETE FROM Users WHERE UserId = @UserId;

    DELETE FROM Commerce WHERE CommerceId = @CommerceId;

    SELECT 'Usuario propietario y comercio eliminados exitosamente' AS Message;
END;
GO

CREATE PROCEDURE SpRegisterSale
    @SaleDate DATETIME,
    @UserId nvarchar(50),
    @CommerceId nvarchar(50),
    @Product NVARCHAR(100),
    @Quantity INT,
    @Price DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SaleId UNIQUEIDENTIFIER;

    SET @SaleId = CONVERT(NVARCHAR(50), NEWID());;
    INSERT INTO Sales (SaleId, SaleDate, UserId, CommerceId, State)
    VALUES (@SaleId, @SaleDate, @UserId, @CommerceId, (SELECT StateId FROM States WHERE StateName = 'Active'));

    INSERT INTO SaleDetails (DetailId, SaleId, Product, Quantity, Price)
    VALUES (CONVERT(NVARCHAR(50), NEWID()), @SaleId, @Product, @Quantity, @Price);

    SELECT 'Venta registrada exitosamente' AS Message;
END;
GO

CREATE PROCEDURE SpUpdateUserState
    @UserId nvarchar(50),
    @StateId nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET State = @StateId
    WHERE UserId = @UserId;

    -- Devolver el mensaje
    SELECT 'Estado del usuario actualizado exitosamente' AS Message;
END;
GO

CREATE PROCEDURE SpAddUserToCommerce
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @Role NVARCHAR(50),
    @CommerceId nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StateId NVARCHAR(50);
    DECLARE @UserId NVARCHAR(50);
    DECLARE @EncryptedPassword NVARCHAR(255);

    SELECT @StateId = StateId FROM States WHERE StateName = 'Active';

    SET @EncryptedPassword = CONVERT(NVARCHAR(255), EncryptByPassPhrase(@Password, @Password));

    SET @UserId = CONVERT(NVARCHAR(50), NEWID());

    INSERT INTO Users (UserId, Username, Password, Role, CommerceId, State)
    VALUES (@UserId, @Username, @EncryptedPassword, @Role, @CommerceId, @StateId);

    -- Devolver el mensaje
    SELECT 'Usuario creado exitosamente' AS Message;
END;
GO


-- Insertar datos iniciales
INSERT INTO States (StateId, StateName)
VALUES 
    (NEWID(), 'Active'),
    (NEWID(), 'Inactive'),
    (NEWID(), 'Pending');

	DECLARE @StateActive UNIQUEIDENTIFIER = (SELECT StateId FROM States WHERE StateName = 'Active'); 

	INSERT INTO Commerce (CommerceId, CommerceName, Address, RUC, State)
VALUES 
    (NEWID(), 'Comercio A', 'Calle Ficticia 123', '1234567890', @StateActive),
    (NEWID(), 'Comercio B', 'Avenida Principal 456', '0987654321', @StateActive);


DECLARE @StateActiveId UNIQUEIDENTIFIER = (SELECT StateId FROM States WHERE StateName = 'Active'); 
DECLARE @CommerceId UNIQUEIDENTIFIER = (SELECT Top 1 CommerceId FROM Commerce); 
DECLARE @KeyOneId UNIQUEIDENTIFIER = NEWID(); -- Simula un CommerceId
DECLARE @KeyTwoId UNIQUEIDENTIFIER = NEWID(); -- Simula un StateId

EXEC SpCreateOrUpdateUser 
    @UserId = @KeyOneId,
    @Username = 'usuario1',
    @Password = 'contraseña123',
    @Role = 'Owner',
    @CommerceId = @CommerceId,
    @State = @StateActiveId;

-- Usuario 2
EXEC SpCreateOrUpdateUser 
    @UserId = @KeyTwoId,
    @Username = 'usuario2',
    @Password = 'contraseña456',
    @Role = 'Employee',
    @CommerceId = @CommerceId,
    @State = @StateActiveId;

