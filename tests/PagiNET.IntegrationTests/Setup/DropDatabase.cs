﻿using System.IO;
using System.Linq;
using PagiNET.IntegrationTests.SqlCommands;

namespace PagiNET.IntegrationTests.Setup
{
    public class DropDatabase
    {
        private readonly DatabaseConfig _cfg;

        public DropDatabase(DatabaseConfig cfg)
        {
            _cfg = cfg;
        }

        public void Go()
        {
            var fileNames = SqlCommandHelpers.ExecuteSqlQuery(_cfg.MasterConnString, $@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{_cfg.DatabaseName}')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                SqlCommandHelpers.ExecuteSqlCommand(_cfg.MasterConnString, $@"
                    ALTER DATABASE [{_cfg.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{_cfg.DatabaseName}'");

                fileNames.ForEach(File.Delete);
            }
        }
    }
}