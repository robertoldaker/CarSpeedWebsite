using System;
using System.Xml.Schema;
using HaloSoft.EventLogger;
using Microsoft.Extensions.FileProviders;
using CarSpeedWebsite.Data;

namespace CarSpeedWebsite.Models
{
    public class DataModel {

        public void Load() {
            using (var da = new DataAccess() ) {
                DataAccess.RunPostgreSQLQuery("SELECT pg_size_pretty(pg_database_size('car_speed'));",(row)=>{
                    this.Size = row[0].ToString();
                });
            }
            DiskUsage = new DiskUsage();
        }

        public string Size {get; private set;}
        public int DiskSpaceUsed {get; private set;}
        public DiskUsage DiskUsage {get; private set;}
    }

    public class DiskUsage {
        public DiskUsage() {
            var com = new Execute();
            int resp =com.Run("df","");
            Found = false;
            // Can generate an error if no sudo privilege but still outputs hat we need
            if ( com.StandardOutput!=null ) {
                var lines = com.StandardOutput.Split('\n');
                foreach( var line in lines) {
                    var cols = line.Split(new char[] {' ','\t'}, StringSplitOptions.RemoveEmptyEntries);
                    if ( cols.Length>=6) {
                        var mountedOn = cols[5];
                        if ( mountedOn=="/") {
                            try {
                                long total = long.Parse(cols[1]);
                                long used = long.Parse(cols[2]);
                                long avail = long.Parse(cols[3]);
                                Used = (int) (used/1048576);
                                Available = (int) (avail/1048576);
                                Total = (int) (total/1048576);
                                Found = true;
                            } catch (Exception e) {
                                Logger.Instance.LogErrorEvent("Problem obtaining disk space");
                                Logger.Instance.LogException(e);
                            }
                        }
                    }
                }
            }
        }
        public bool Found {get; private set;}
        public int Used {get; private set;}
        public int Available {get; private set;}
        public int Total {get; private set;}
    }     
}