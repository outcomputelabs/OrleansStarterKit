WITH Query AS
(
	SELECT QueryKey, QueryText
	FROM
	(
		VALUES

		-- Cluster Membership
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
		'),

		/* Storage */
		(
			'WriteToStorageKey',
			'-- When Orleans is running in normal, non-split state, there will
			-- be only one grain with the given ID and type combination only. This
			-- grain saves states mostly serially if Orleans guarantees are upheld. Even
			-- if not, the updates should work correctly due to version number.
			--
			-- In split brain situations there can be a situation where there are two or more
			-- grains with the given ID and type combination. When they try to INSERT
			-- concurrently, the table needs to be locked pessimistically before one of
			-- the grains gets @GrainStateVersion = 1 in return and the other grains will fail
			-- to update storage. The following arrangement is made to reduce locking in normal operation.
			--
			-- If the version number explicitly returned is still the same, Orleans interprets it so the update did not succeed
			-- and throws an InconsistentStateException.
			--
			-- See further information at https://dotnet.github.io/orleans/Documentation/Core-Features/Grain-Persistence.html.
			BEGIN TRANSACTION;
			SET XACT_ABORT, NOCOUNT ON;

			DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;


			-- If the @GrainStateVersion is not zero, this branch assumes it exists in this database.
			-- The NULL value is supplied by Orleans when the state is new.
			IF @GrainStateVersion IS NOT NULL
			BEGIN
				UPDATE Storage
				SET
					PayloadBinary = @PayloadBinary,
					PayloadJson = @PayloadJson,
					PayloadXml = @PayloadXml,
					ModifiedOn = GETUTCDATE(),
					Version = Version + 1,
					@NewGrainStateVersion = Version + 1,
					@GrainStateVersion = Version + 1
				WHERE
					GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
					AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
					AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
					AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
					AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
					AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
					AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
					AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
					OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));
			END

			-- The grain state has not been read. The following locks rather pessimistically
			-- to ensure only one INSERT succeeds.
			IF @GrainStateVersion IS NULL
			BEGIN
				INSERT INTO Storage
				(
					GrainIdHash,
					GrainIdN0,
					GrainIdN1,
					GrainTypeHash,
					GrainTypeString,
					GrainIdExtensionString,
					ServiceId,
					PayloadBinary,
					PayloadJson,
					PayloadXml,
					ModifiedOn,
					Version
				)
				SELECT
					@GrainIdHash,
					@GrainIdN0,
					@GrainIdN1,
					@GrainTypeHash,
					@GrainTypeString,
					@GrainIdExtensionString,
					@ServiceId,
					@PayloadBinary,
					@PayloadJson,
					@PayloadXml,
					GETUTCDATE(),
					1
				 WHERE NOT EXISTS
				 (
					-- There should not be any version of this grain state.
					SELECT 1
					FROM Storage WITH(XLOCK, ROWLOCK, HOLDLOCK, INDEX(IX_Storage))
					WHERE
						GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
						AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
						AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
						AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
						AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
						AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
						AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				 ) OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

				IF @@ROWCOUNT > 0
				BEGIN
					SET @NewGrainStateVersion = 1;
				END
			END

			SELECT @NewGrainStateVersion AS NewGrainStateVersion;
			COMMIT TRANSACTION;'
		),
		(
			'ClearStorageKey',
			'BEGIN TRANSACTION;
			SET XACT_ABORT, NOCOUNT ON;
			DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;
			UPDATE Storage
			SET
				PayloadBinary = NULL,
				PayloadJson = NULL,
				PayloadXml = NULL,
				ModifiedOn = GETUTCDATE(),
				Version = Version + 1,
				@NewGrainStateVersion = Version + 1
			WHERE
				GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
				AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
				AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
				AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
				AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
				AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
				AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
				OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

			SELECT @NewGrainStateVersion;
			COMMIT TRANSACTION;'
		),
		(
			'ReadFromStorageKey',
			'-- The application code will deserialize the relevant result. Not that the query optimizer
			-- estimates the result of rows based on its knowledge on the index. It does not know there
			-- will be only one row returned. Forcing the optimizer to process the first found row quickly
			-- creates an estimate for a one-row result and makes a difference on multi-million row tables.
			-- Also the optimizer is instructed to always use the same plan via index using the OPTIMIZE
			-- FOR UNKNOWN flags. These hints are only available in SQL Server 2008 and later. They
			-- should guarantee the execution time is robustly basically the same from query-to-query.
			SELECT
				PayloadBinary,
				PayloadXml,
				PayloadJson,
				Version
			FROM
				Storage
			WHERE
				GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
				AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
				AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
				AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
				AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
				AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
				AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
				OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));'
		)

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
