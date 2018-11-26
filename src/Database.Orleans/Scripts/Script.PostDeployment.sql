/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

WITH Query AS
(
	SELECT QueryKey, QueryText
	FROM
	(
		VALUES

		/* Cluster Membership */
		(
			'UpdateIAmAlivetimeKey','
			-- This is expected to never fail by Orleans, so return value
			-- is not needed nor is it checked.
			SET NOCOUNT ON;
			UPDATE OrleansMembershipTable
			SET
				IAmAliveTime = @IAmAliveTime
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Address = @Address AND @Address IS NOT NULL
				AND Port = @Port AND @Port IS NOT NULL
				AND Generation = @Generation AND @Generation IS NOT NULL;
		'),
		(
			'InsertMembershipVersionKey','
			SET NOCOUNT ON;
			INSERT INTO OrleansMembershipVersionTable
			(
				DeploymentId
			)
			SELECT @DeploymentId
			WHERE NOT EXISTS
			(
			SELECT 1
			FROM
				OrleansMembershipVersionTable
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
			);

			SELECT @@ROWCOUNT;
		'),
		(
			'InsertMembershipKey','
			SET XACT_ABORT, NOCOUNT ON;
			DECLARE @ROWCOUNT AS INT;
			BEGIN TRANSACTION;
			INSERT INTO OrleansMembershipTable
			(
				DeploymentId,
				Address,
				Port,
				Generation,
				SiloName,
				HostName,
				Status,
				ProxyPort,
				StartTime,
				IAmAliveTime
			)
			SELECT
				@DeploymentId,
				@Address,
				@Port,
				@Generation,
				@SiloName,
				@HostName,
				@Status,
				@ProxyPort,
				@StartTime,
				@IAmAliveTime
			WHERE NOT EXISTS
			(
			SELECT 1
			FROM
				OrleansMembershipTable
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Address = @Address AND @Address IS NOT NULL
				AND Port = @Port AND @Port IS NOT NULL
				AND Generation = @Generation AND @Generation IS NOT NULL
			);

			UPDATE OrleansMembershipVersionTable
			SET
				Timestamp = GETUTCDATE(),
				Version = Version + 1
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Version = @Version AND @Version IS NOT NULL
				AND @@ROWCOUNT > 0;

			SET @ROWCOUNT = @@ROWCOUNT;

			IF @ROWCOUNT = 0
				ROLLBACK TRANSACTION
			ELSE
				COMMIT TRANSACTION
			SELECT @ROWCOUNT;
		'),
		(
			'UpdateMembershipKey','
			SET XACT_ABORT, NOCOUNT ON;
			BEGIN TRANSACTION;

			UPDATE OrleansMembershipVersionTable
			SET
				Timestamp = GETUTCDATE(),
				Version = Version + 1
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Version = @Version AND @Version IS NOT NULL;

			UPDATE OrleansMembershipTable
			SET
				Status = @Status,
				SuspectTimes = @SuspectTimes,
				IAmAliveTime = @IAmAliveTime
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Address = @Address AND @Address IS NOT NULL
				AND Port = @Port AND @Port IS NOT NULL
				AND Generation = @Generation AND @Generation IS NOT NULL
				AND @@ROWCOUNT > 0;

			SELECT @@ROWCOUNT;
			COMMIT TRANSACTION;
		'),
		(
			'GatewaysQueryKey','
			SELECT
				Address,
				ProxyPort,
				Generation
			FROM
				OrleansMembershipTable
			WHERE
				DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
				AND Status = @Status AND @Status IS NOT NULL
				AND ProxyPort > 0;
		'),
		(
			'MembershipReadRowKey','
			SELECT
				v.DeploymentId,
				m.Address,
				m.Port,
				m.Generation,
				m.SiloName,
				m.HostName,
				m.Status,
				m.ProxyPort,
				m.SuspectTimes,
				m.StartTime,
				m.IAmAliveTime,
				v.Version
			FROM
				OrleansMembershipVersionTable v
				-- This ensures the version table will returned even if there is no matching membership row.
				LEFT OUTER JOIN OrleansMembershipTable m ON v.DeploymentId = m.DeploymentId
				AND Address = @Address AND @Address IS NOT NULL
				AND Port = @Port AND @Port IS NOT NULL
				AND Generation = @Generation AND @Generation IS NOT NULL
			WHERE
				v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
		'),
		(
			'MembershipReadAllKey','
			SELECT
				v.DeploymentId,
				m.Address,
				m.Port,
				m.Generation,
				m.SiloName,
				m.HostName,
				m.Status,
				m.ProxyPort,
				m.SuspectTimes,
				m.StartTime,
				m.IAmAliveTime,
				v.Version
			FROM
				OrleansMembershipVersionTable v LEFT OUTER JOIN OrleansMembershipTable m
				ON v.DeploymentId = m.DeploymentId
			WHERE
				v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
		'),
		(
			'DeleteMembershipTableEntriesKey','
			DELETE FROM OrleansMembershipTable
			WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
			DELETE FROM OrleansMembershipVersionTable
			WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
		'),

		/* Reminders */
		(
			'UpsertReminderRowKey','
			DECLARE @Version AS INT = 0;
			SET XACT_ABORT, NOCOUNT ON;
			BEGIN TRANSACTION;
			UPDATE OrleansRemindersTable WITH(UPDLOCK, ROWLOCK, HOLDLOCK)
			SET
				StartTime = @StartTime,
				Period = @Period,
				GrainHash = @GrainHash,
				@Version = Version = Version + 1
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND GrainId = @GrainId AND @GrainId IS NOT NULL
				AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL;

			INSERT INTO OrleansRemindersTable
			(
				ServiceId,
				GrainId,
				ReminderName,
				StartTime,
				Period,
				GrainHash,
				Version
			)
			SELECT
				@ServiceId,
				@GrainId,
				@ReminderName,
				@StartTime,
				@Period,
				@GrainHash,
				0
			WHERE
				@@ROWCOUNT=0;
			SELECT @Version AS Version;
			COMMIT TRANSACTION;
		'),
		(
			'ReadReminderRowsKey','
			SELECT
				GrainId,
				ReminderName,
				StartTime,
				Period,
				Version
			FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND GrainId = @GrainId AND @GrainId IS NOT NULL;
		'),
		(
			'ReadReminderRowKey','
			SELECT
				GrainId,
				ReminderName,
				StartTime,
				Period,
				Version
			FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND GrainId = @GrainId AND @GrainId IS NOT NULL
				AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL;
		'),
		(
			'ReadRangeRows1Key','
			SELECT
				GrainId,
				ReminderName,
				StartTime,
				Period,
				Version
			FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND GrainHash > @BeginHash AND @BeginHash IS NOT NULL
				AND GrainHash <= @EndHash AND @EndHash IS NOT NULL;
		'),
		(
			'ReadRangeRows2Key','
			SELECT
				GrainId,
				ReminderName,
				StartTime,
				Period,
				Version
			FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND ((GrainHash > @BeginHash AND @BeginHash IS NOT NULL)
				OR (GrainHash <= @EndHash AND @EndHash IS NOT NULL));
		'),
		(
			'DeleteReminderRowKey','
			DELETE FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND GrainId = @GrainId AND @GrainId IS NOT NULL
				AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL
				AND Version = @Version AND @Version IS NOT NULL;
			SELECT @@ROWCOUNT;
		'),
		(
			'DeleteReminderRowsKey','
			DELETE FROM OrleansRemindersTable
			WHERE
				ServiceId = @ServiceId AND @ServiceId IS NOT NULL;
		')

	) AS Q (QueryKey, QueryText)
)
MERGE INTO OrleansQuery AS T
USING Query AS S
ON T.QueryKey = S.QueryKey
WHEN MATCHED AND T.QueryText <> S.QueryText THEN
UPDATE SET T.QueryText = S.QueryText
WHEN NOT MATCHED BY TARGET THEN
INSERT (QueryKey, QueryText)
VALUES (QueryKey, QueryText)
WHEN NOT MATCHED BY SOURCE THEN
DELETE;
GO
