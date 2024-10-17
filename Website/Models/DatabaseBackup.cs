using System.Diagnostics;
using HaloSoft.EventLogger;

namespace CarSpeedWebsite.Models
{
    public class DatabaseBackup
    {
        private const string DB_NAME = "car_speed";

        public DatabaseBackup() {
        }

        public StreamReader BackupToStream(out string filename) {

            var dbName = DB_NAME;
            var args = $"--clean --if-exists {dbName}";
            var now = DateTime.Now;
            var ts = now.ToString("yyyy-MMM-dd-HH-mm-ss");
            filename = $"{dbName}-{ts}.sql";

            // explicitly using /usr/bin to ensure it picks up v14.
            // installing gdal brings in postgres12 which then means "pg_dump" is v12
            var exe = "/usr/bin/pg_dump";
            ProcessStartInfo oInfo = new ProcessStartInfo(exe, args);
			oInfo.UseShellExecute = false;
			oInfo.CreateNoWindow = true;

			oInfo.RedirectStandardOutput = true;
			oInfo.RedirectStandardError = true;

			StreamReader srOutput = null;
			StreamReader srError = null;

			Process proc = System.Diagnostics.Process.Start(oInfo);
			srOutput = proc.StandardOutput;
			return srOutput;            
        }


    }
}