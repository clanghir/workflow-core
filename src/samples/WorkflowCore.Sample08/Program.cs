﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample08
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<HumanWorkflow>();
            host.Start();


            Console.WriteLine("Starting workflow...");
            string workflowId = host.StartWorkflow("HumanWorkflow").Result;

            Thread.Sleep(1000);
            Console.WriteLine();
            Console.WriteLine("Open user actions are");
            var openItems = host.GetOpenUserActions(workflowId);
            foreach (var item in openItems)
            {
                Console.WriteLine(item.Prompt + ", Assigned to " + item.AssignedPrincipal);
                Console.WriteLine("Options are ");
                foreach (var option in item.Options)
                {
                    Console.WriteLine(" - " + option.Key + " : " + option.Value + ", ");
                }

                Thread.Sleep(500);
                Console.WriteLine();
                Console.WriteLine("Choosing " + item.Options.Last().Key);

                host.PublishUserAction(openItems.First().Key, "MYDOMAIN\\me", item.Options.Last().Value).Wait();
            }

            Console.ReadLine();
            host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseMongoDB(@"mongodb://localhost:27017", "workflow3"));
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.;Database=WorkflowCore3;Trusted_Connection=True;", true, true));            
            //services.AddWorkflow(x => x.UseSqlite(@"Data Source=database2.db;", true));            


            var serviceProvider = services.BuildServiceProvider();

            //config logging
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddDebug(LogLevel.Debug);
            return serviceProvider;
        }

        
    }
}
