# SQLServerListener
Receive Notifications when a Table Record Changes with C#

Receive Notifications on Record Change with SQLTableDependency
SqlTableDependency is a C# class used to receive notifications containing the modified record values when the content of a specified database table change.

Step0: Enable broker of database
```sql
ALTER DATABASE MyDatabase SET ENABLE_BROKER
```
IF the Alter Database takes long time to process, try below statement
```sql
USE master;
GO
ALTER DATABASE Collateral
    SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
GO
USE Collateral;
GO
```
Check if is_broker_enabled = 1 is success
```sql
SELECT
    name, database_id, is_broker_enabled
FROM sys.databases
```

Step1: Create a table name emp in your database.
```sql
CREATE TABLE [dbo].[Emp](
       [Id] [int] IDENTITY(1,1) NOT NULL,
       [FirstName] [nvarchar](50) NOT NULL,
       [LastName] [nvarchar](50) NOT NULL

) ON [PRIMARY]
```

------------------------------------------------
Step 2: Create one Customer class.
```c#
class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
```
Step 3: Add SqlTableDependency page in your project.
```npm
PM>  Install-Package SqlTableDependency
```
Step 4: Download and run project

Step 5: Insert,Update and Delete emp table by following query and you will see the changes on your output window.
```sql
INSERT INTO Emp ([FirstName], [LastName])
VALUES ('Ashok', 'Kumar')
go
DELETE FROM Emp WHERE ID =3 
go
UPDATE Emp SET [FirstName] = 'xyz' WHERE ID = 4
```
reference: https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency
