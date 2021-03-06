﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Models;

namespace WorkflowCore.Persistence.EntityFramework
{
    internal static class ExtensionMethods
    {
        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        internal static PersistedWorkflow ToPersistable(this WorkflowInstance instance, PersistedWorkflow persistable = null)
        {
            if (persistable == null)            
                persistable = new PersistedWorkflow();                        

            persistable.Data = JsonConvert.SerializeObject(instance.Data, SerializerSettings);
            persistable.Description = instance.Description;
            persistable.InstanceId = new Guid(instance.Id);
            persistable.NextExecution = instance.NextExecution;
            persistable.Version = instance.Version;
            persistable.WorkflowDefinitionId = instance.WorkflowDefinitionId;
            persistable.Status = instance.Status;
            persistable.CreateTime = instance.CreateTime;
            persistable.CompleteTime = instance.CompleteTime;
            
            foreach (var ep in instance.ExecutionPointers)
            {
                var persistedEP = persistable.ExecutionPointers.FirstOrDefault(x => x.Id == ep.Id);
                
                if (persistedEP == null)
                {
                    persistedEP = new PersistedExecutionPointer();
                    persistable.ExecutionPointers.Add(persistedEP);
                }
                 
                persistedEP.Id = ep.Id ?? Guid.NewGuid().ToString(); 
                persistedEP.StepId = ep.StepId;
                persistedEP.Active = ep.Active;
                persistedEP.SleepUntil = ep.SleepUntil;
                persistedEP.PersistenceData = JsonConvert.SerializeObject(ep.PersistenceData, SerializerSettings);
                persistedEP.StartTime = ep.StartTime;
                persistedEP.EndTime = ep.EndTime;
                persistedEP.StepName = ep.StepName;
                persistedEP.EventName = ep.EventName;
                persistedEP.EventKey = ep.EventKey;
                persistedEP.EventPublished = ep.EventPublished;
                persistedEP.ConcurrentFork = ep.ConcurrentFork;
                persistedEP.PathTerminator = ep.PathTerminator;
                persistedEP.EventData = JsonConvert.SerializeObject(ep.EventData, SerializerSettings);

                foreach (var attr in ep.ExtensionAttributes)
                {
                    var persistedAttr = persistedEP.ExtensionAttributes.FirstOrDefault(x => x.AttributeKey == attr.Key);
                    if (persistedAttr == null)
                    {
                        persistedAttr = new PersistedExtensionAttribute();
                        persistedEP.ExtensionAttributes.Add(persistedAttr);
                    }

                    persistedAttr.AttributeKey = attr.Key;
                    persistedAttr.AttributeValue = JsonConvert.SerializeObject(attr.Value, SerializerSettings);
                }

                foreach (var err in ep.Errors)
                {
                    var persistedErr = persistedEP.Errors.FirstOrDefault(x => x.Id == err.Id);
                    if (persistedErr == null)
                    {
                        persistedErr = new PersistedExecutionError();
                        persistedErr.Id = err.Id ?? Guid.NewGuid().ToString();
                        persistedErr.ErrorTime = err.ErrorTime;
                        persistedErr.Message = err.Message;
                        persistedEP.Errors.Add(persistedErr);
                    }
                }

            }

            return persistable;
        }

        internal static PersistedSubscription ToPersistable(this EventSubscription instance)
        {
            PersistedSubscription result = new PersistedSubscription();            
            result.SubscriptionId = new Guid(instance.Id);
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.StepId = instance.StepId;
            result.WorkflowId = instance.WorkflowId;
            result.SubscribeAsOf = instance.SubscribeAsOf;

            return result;
        }

        internal static PersistedEvent ToPersistable(this Event instance)
        {
            PersistedEvent result = new PersistedEvent();
            result.EventId = new Guid(instance.Id);
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.EventTime = instance.EventTime;
            result.IsProcessed = instance.IsProcessed;
            result.EventData = JsonConvert.SerializeObject(instance.EventData, SerializerSettings);

            return result;
        }

        internal static WorkflowInstance ToWorkflowInstance(this PersistedWorkflow instance)
        {
            WorkflowInstance result = new WorkflowInstance();
            result.Data = JsonConvert.DeserializeObject(instance.Data, SerializerSettings);
            result.Description = instance.Description;
            result.Id = instance.InstanceId.ToString();
            result.NextExecution = instance.NextExecution;
            result.Version = instance.Version;
            result.WorkflowDefinitionId = instance.WorkflowDefinitionId;
            result.Status = instance.Status;
            result.CreateTime = instance.CreateTime;
            result.CompleteTime = instance.CompleteTime;
            
            foreach (var ep in instance.ExecutionPointers)
            {
                var pointer = new ExecutionPointer();
                result.ExecutionPointers.Add(pointer);

                pointer.Id = ep.Id;
                pointer.StepId = ep.StepId;
                pointer.Active = ep.Active;
                pointer.SleepUntil = ep.SleepUntil;
                pointer.PersistenceData = JsonConvert.DeserializeObject(ep.PersistenceData, SerializerSettings);
                pointer.StartTime = ep.StartTime;
                pointer.EndTime = ep.EndTime;
                pointer.StepName = ep.StepName;
                pointer.EventName = ep.EventName;
                pointer.EventKey = ep.EventKey;
                pointer.EventPublished = ep.EventPublished;
                pointer.ConcurrentFork = ep.ConcurrentFork;
                pointer.PathTerminator = ep.PathTerminator;
                pointer.EventData = JsonConvert.DeserializeObject(ep.EventData, SerializerSettings);

                foreach (var attr in ep.ExtensionAttributes)
                {
                    pointer.ExtensionAttributes[attr.AttributeKey] = JsonConvert.DeserializeObject(attr.AttributeValue, SerializerSettings);
                }

                foreach (var err in ep.Errors)
                {
                    var execErr = new ExecutionError();
                    execErr.Id = err.Id;
                    execErr.ErrorTime = err.ErrorTime;
                    execErr.Message = err.Message;
                    pointer.Errors.Add(execErr);                    
                }

            }


            return result;
        }

        internal static EventSubscription ToEventSubscription(this PersistedSubscription instance)
        {
            EventSubscription result = new EventSubscription();
            result.Id = instance.SubscriptionId.ToString();
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.StepId = instance.StepId;
            result.WorkflowId = instance.WorkflowId;
            result.SubscribeAsOf = instance.SubscribeAsOf;

            return result;
        }

        internal static Event ToEvent(this PersistedEvent instance)
        {
            Event result = new Event();
            result.Id = instance.EventId.ToString();
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.EventTime = instance.EventTime;
            result.IsProcessed = instance.IsProcessed;
            result.EventData = JsonConvert.DeserializeObject(instance.EventData, SerializerSettings);

            return result;
        }
    }
}
