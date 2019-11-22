using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RealEstate_Web_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OnlineDB test = new OnlineDB();
            // OnlineDB.ReadAssetsTable();
            //OnlineDB.getTable(1);
            //OnlineDB.InsertNewEntryToOpenContractsTable("1115", "0x939xxc", "0x8887", "0x22222887");
            OnlineDB.InsertNewEntryToAssetsTable("ce155c9664386764ee49f72aa0e5d2820c7dee301154b545e26e69f6408f4d34", "2222", "Karkum 9 Ako", "100" , "3", "www.walla.co.il", "0x123");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
