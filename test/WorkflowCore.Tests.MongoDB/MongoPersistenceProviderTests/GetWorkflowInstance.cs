﻿using Machine.Specifications;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.MongoDB.Services;
using WorkflowCore.Services;
using WorkflowCore.TestAssets;
using WorkflowCore.TestAssets.Persistence;

namespace WorkflowCore.Tests.MongoDB.MongoPersistenceProviderTests
{    
    [Subject(typeof(MongoPersistenceProvider))]    
    public class Mongo_GetWorkflowInstance : GetWorkflowInstance
    {
        protected override IPersistenceProvider Provider
        {
            get
            {
                var client = new MongoClient("mongodb://localhost:" + DockerSetup.Port);
                var db = client.GetDatabase("workflow-tests");
                return new MongoPersistenceProvider(db);
            }
        }

        Behaves_like<GetWorkflowInstanceBehaviors> get_workflow_instance;        
    }
}
