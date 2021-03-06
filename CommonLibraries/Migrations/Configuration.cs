namespace CommonLibraries.Migrations
{
    using Database;
    using Models;
    using Models.dbModels;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class Configuration : DbMigrationsConfiguration<sqlDbContextEF>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(sqlDbContextEF context)
        {
            //This method will be called after migrating to the latest version.

            //You can use the DbSet<T>.AddOrUpdate() helper extension method
            //to avoid creating duplicate seed data.E.g.

            context.User.AddOrUpdate(
              u => u.Name,
              new User { Name = "Milinda", Password = "cXdlcnR5", Group = "Administrator" },
              new User { Name = "Jeffery", Password = "cXdlcnR5", Group = "Administrator" },
              new User { Name = "MIS", Password = "MTIzNA== ", Group = "User" },
              new User { Name = "OP", Password = string.Empty, Group = "User" }
            );

            context.agvModel_Info.AddOrUpdate(u => u.Name,
                new agvModel_Info { Name = "DaVinci Super", Code = "3FSXX", Remark = "The Models Should be here." },
                new agvModel_Info { Name = "DaVinci Super Colour", Code = "3FCXX", Remark = "The Models Should be here." }
                );

            context.agvZone_Info.AddOrUpdate(u => u.Name,
                new agvZone_Info { Name = "Zone1", Remark = "Remarks go here." },
                new agvZone_Info { Name = "Zone2", Remark = "Remarks go here." },
                new agvZone_Info { Name = "Zone3", Remark = "Remarks go here." }
                );

            context.agvCommand_Type_Info.AddOrUpdate(x => new { x.Name, x.Byte, x.Argument },
                new agvCommand_Type_Info { Name = "Table_Move", Byte = 1, Type = "Table_Move", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "TableInitial", Byte = 2, Type = "TableInitial", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "TableFromIdle", Byte = 3, Type = "TableFromIdle", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "TableToIdle", Byte = 4, Type = "TableToIdle", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "TableFinish", Byte = 5, Type = "TableFinish", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "AGV_Request", Byte = 6, Type = "AGV_Request", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "ChargeAll", Byte = 7, Type = "ChargeAll", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "NewTable", Byte = 8, Type = "NewTable", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" },
                new agvCommand_Type_Info { Name = "TableRotate", Byte = 9, Type = "TableRotate", Argument = "Argument Desriptions go here", DataSource = "agvStationTestFlow", Remark = "Remarks Go here" }
                );

            context.agvStatus_Info.AddOrUpdate(u => u.Status,
                new agvStatus_Info { Status = "Inital", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "OK", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "NG", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "Idle", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "Busy", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "ChargeAll", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "Rotate", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "Finished", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" },
                new agvStatus_Info { Status = "New", Arg1 = string.Empty, Arg2 = string.Empty, Arg3 = string.Empty, Remark = "Remarks go here" }
                );

            var model = context.agvModel_Info.Where(x => x.Id < 100).FirstOrDefault();
            var zone = context.agvZone_Info.Where(x => x.Id < 100).FirstOrDefault();
            if (model != null && zone != null) AddStations(context);

            var stations = context.agvStation_Info.FirstOrDefault();
            if (model != null && zone != null && stations != null) AddStationTestFlows(context);
        }

        private static void AddStationTestFlows(sqlDbContextEF context)
        {
            //Add the initial test flow, requesting a new table. From Initial --> Idle
            //Add the second test flow, requesting a table from Idle --> S1
            //then s1--> s2 til s10.
            //Add a one to many flow, S10 to --> MechCal Group
            //Add Many to one flow, MechCal --> S15
            //Then S15--> S16 till s18
            //Then its S18 --> QC
            //Then Process Finished Flow, QC --> Idle.

            //All stations to Repair.
            //From Repair back to All Stations.
            //Finished repair to idle flow.

            //Mechanical stations rotate flows.
            //ChargeAll from all stations to ChargeArea.

            //Table Initializing Steps.
            CreateagvStationTestFlow(context, "NewTable", "Idle1", "New", "NewTable");
            CreateagvStationTestFlow(context, "Idle1", "S1", "Initial", "TableInitial");
            //Normal A->B Flow
            CreateagvStationTestFlow(context, "S1", "S2", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S2", "S3", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S3", "S4", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S4", "S5", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S5", "S6", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S6", "S7", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S7", "S8", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S8", "S9", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S9", "S10", "OK", "Table_Move");
            //One to Many Flow.
            CreateagvStationTestFlow(context, "S10", "M11", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S10", "M12", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S10", "M13", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S10", "M14", "OK", "Table_Move");
            //Many to One Flow.
            CreateagvStationTestFlow(context, "M11", "S15", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "M12", "S15", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "M13", "S15", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "M14", "S15", "OK", "Table_Move");
            //Normal A-B Flow
            CreateagvStationTestFlow(context, "S15", "S16", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S16", "S17", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S17", "S18", "OK", "Table_Move");
            CreateagvStationTestFlow(context, "S18", "QC1", "Finished", "Table_Move");
            //NG Flow
            CreateagvStationTestFlow(context, "S1", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S2", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S3", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S4", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S5", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S6", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S7", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S8", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S9", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S10", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "M11", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "M12", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "M13", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "M14", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S15", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S16", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S17", "Repair1", "NG", "Table_Move");
            CreateagvStationTestFlow(context, "S18", "Repair1", "NG", "Table_Move");
            //Rotate Flow
            CreateagvStationTestFlow(context, "M11", "M11", "Rotate", "TableRotate");
            CreateagvStationTestFlow(context, "M12", "M12", "Rotate", "TableRotate");
            CreateagvStationTestFlow(context, "M13", "M13", "Rotate", "TableRotate");
            CreateagvStationTestFlow(context, "M14", "M14", "Rotate", "TableRotate");


            //for (int i = 0; i < 3; i++)
            //{
            //    context.agvStationTestFlow.AddOrUpdate(x => new { x.Command_TypeId, x.Current_StationId, x.Dest_StationId },
            //    new agvStationTestFlow
            //    {
            //        Command_TypeId = context.agvCommand_Type_Info.Where(x => x.Name.StartsWith("Table_Move")).FirstOrDefault().Id,
            //        Current_StationId = context.agvStation_Info.Where(x => x.Name.StartsWith("S")).FirstOrDefault().Id + i,
            //        Dest_StationId = context.agvStation_Info.Where(x => x.Name.StartsWith("S")).FirstOrDefault().Id + i + 1,
            //        ZoneId = context.agvZone_Info.Where(x => x.Id < 100).FirstOrDefault().Id,
            //        StatusId = context.agvStatus_Info.Where(x => x.Id < 100).FirstOrDefault().Id,
            //        UpdateTime = DateTime.Now
            //    });
            //}
        }

        private static void CreateagvStationTestFlow(sqlDbContextEF context, string currentStationName, string destStationName, string status, string commandType)
        {
            context.agvStationTestFlow.AddOrUpdate(x => new { x.Command_TypeId, x.Current_StationId, x.Dest_StationId },
                new agvStationTestFlow
                {
                    Current_StationId = context.agvStation_Info.Where(s => s.Name.Equals(currentStationName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id,
                    Dest_StationId = context.agvStation_Info.Where(s => s.Name.Equals(destStationName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id,
                    StatusId = context.agvStatus_Info.Where(s => s.Status.Equals(status,StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id,
                    Command_TypeId = context.agvCommand_Type_Info.Where(c => c.Name.Equals(commandType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id,
                    UpdateTime = DateTime.Now
                });
        }

        public Expression<Func<agvStationTestFlow, object>> identifierExpression()
        {
            return x => x.Current_StationId;
        }
        private static void AddStations(sqlDbContextEF context)
        {
            string stationName = string.Empty;
            CreateStation(context, "NewTable");
            CreateStation(context, "Initial");

            for (int i = 1; i < 10 + 1; i++)
            {
                CreateStation(context, "S" + i.ToString());
            }

            for (int i = 15; i < 18 + 1; i++)
            {
                CreateStation(context, "S" + i.ToString());
            }

            for (int i = 11; i < 14 + 1; i++)
            {
                CreateStation(context, "M" + i.ToString());
            }

            //for (int i = 1; i < 30 + 1; i++)
            //{
            //    CreateStation(context, "P" + i.ToString());
            //}

            //for (int i = 1; i < 3 + 1; i++)
            //{
            //    CreateStation(context, "FDS" + i.ToString());
            //}

            //for (int i = 1; i < 15 + 1; i++)
            //{
            //    CreateStation(context, "CSA" + i.ToString());
            //}

            for (int i = 1; i < 1 + 1; i++)
            {
                CreateStation(context, "QC" + i.ToString());
            }

            for (int i = 1; i < 1 + 1; i++)
            {
                CreateStation(context, "Repair" + i.ToString());
            }

            for (int i = 1; i < 1 + 1; i++)
            {
                CreateStation(context, "Idle" + i.ToString());
            }

            for (int i = 1; i < 1 + 1; i++)
            {
                CreateStation(context, "Charge" + i.ToString());
            }

            for (int i = 1; i < 3 + 1; i++)
            {
                CreateStation(context, "Packing" + i.ToString());
            }
        }

        private static void CreateStation(sqlDbContextEF context, string stationName)
        {
            context.agvStation_Info.AddOrUpdate(u => u.Name,
                                new agvStation_Info
                                {
                                    Name = stationName,
                                    Face = "Up",
                                    Location = "x:001,y:002",
                                    Group = "None",
                                    ModelId = context.agvModel_Info.Where(x => x.Id < 100).FirstOrDefault().Id,
                                    Remark = "Remarks go here.",
                                    Status = "Default",
                                    UpdateTime = DateTime.Now,
                                    ZoneId = context.agvZone_Info.Where(x => x.Id < 100).FirstOrDefault().Id
                                });
        }
    }
}
