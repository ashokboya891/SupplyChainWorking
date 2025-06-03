@echo off
sqlcmd -S Ashok\SQLEXPRESS -d Chain -E -Q "EXEC dbo.AutoFulfillPendingRestocks" 
