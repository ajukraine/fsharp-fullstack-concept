namespace Feed.AspNetApi

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open System.Data.SqlClient
open Persistance

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        
        let hz = fun (sp: IServiceProvider) -> Persistance.Sql.getAllFeeds (sp.GetService<SqlConnection>())
        services
            .AddTransient<SqlConnection>(fun (sp: IServiceProvider) -> 
                let conneciton = new SqlConnection (this.Configuration.GetConnectionString("FeedDb"))
                conneciton.Open()
                conneciton)
            .AddTransient<GetAllFeeds>(Func<IServiceProvider, GetAllFeeds>(fun (sp: IServiceProvider) -> Persistance.Sql.getAllFeeds (sp.GetService<SqlConnection>())))
            .AddMvc(fun opt -> opt.EnableEndpointRouting <- false) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseMvc() |> ignore

    member val Configuration : IConfiguration = null with get, set