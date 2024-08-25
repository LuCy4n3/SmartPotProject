using SQLserverFinale.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SQLserverFinale
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            //This section below is for connection string 
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<PlantContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                    });
            });
            /*     builder.Services.AddDbContext<SamplePlantDbContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("optimumDB")));*/
            builder.Services.AddDbContext<PotContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                    });
            });
            builder.Services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                    });
            });
            /*builder.Services.AddDbContext<SamplePotDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                    });
            });*/

            //add or remove swagger 
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();
            /*
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }*/
          /*  var pots = new List<Pot>();

            void InitializePots() => pots = Enumerable.Range(1, 5)
                .Select(index => new Pot(index, "", 0, false, false, 0, 0, 0, 0, 0))
                .ToList();

            InitializePots();



            app.MapGet("/pots/{index}", (int index) =>
            {
                var ItemToFind = pots.Find(r => r.index == index);
                return Results.Ok(ItemToFind);

            });
            app.MapPost("/pots", () =>
            {
                return Results.Ok(pots);
            });
            app.MapPut("/pots", (Pot newPot) =>
            {
                var itemsToRemove = pots.Where(r => r.index == newPot.index).ToList();
                if (itemsToRemove != null)
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        pots.Remove(itemToRemove);
                    }

                pots.Add(newPot);
                return Results.Ok(newPot);
            });
            app.MapDelete("/pots/{index}", (int index) =>
            {
                var itemToRemove = pots.Single(r => r.index == index);
                pots.Remove(itemToRemove);
                if (itemToRemove != null)
                    pots.Remove(itemToRemove);
                return Results.NoContent();
            });
            app.MapDelete("/state", () =>
            {
                InitializePots();
                return Results.NoContent();
            });*/

            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseStaticFiles(); // Enable serving static files




            app.MapControllers();

            app.Run();
        }
        internal record Pot(int index, string plantName, int type, bool pompa, bool sera, double temp, double humidity, double potassium, double phosphor, double nitrogen)
        {

        }
    }
}
