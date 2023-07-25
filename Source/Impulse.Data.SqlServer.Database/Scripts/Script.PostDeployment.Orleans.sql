:r .\Script.PostDeployment.Orleans.Clustering.sql
:r .\Script.PostDeployment.Orleans.Persistence.sql
:r .\Script.PostDeployment.Orleans.Reminders.sql
GO

ALTER TABLE OrleansMembershipTable REBUILD;
ALTER TABLE OrleansMembershipVersionTable REBUILD;
ALTER TABLE OrleansQuery REBUILD;
ALTER TABLE OrleansRemindersTable REBUILD;
ALTER TABLE OrleansStorage REBUILD;
GO