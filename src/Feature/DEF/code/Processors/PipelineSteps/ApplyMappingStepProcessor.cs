using Sitecore.DataExchange;
using Sitecore.DataExchange.ApplyMapping;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.DataAccess;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.Services.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCH.Feature.DEF.Processors.PipelineSteps
{
    [RequiredPipelineStepPlugins(new Type[]
            {
    typeof(MappingSettings)
            })]
    [RequiredPipelineContextPlugins(new Type[]
            {
    typeof(SynchronizationSettings)
            })]
    public class ApplyMappingStepProcessor : Sitecore.DataExchange.ApplyMapping.ApplyMappingStepProcessor
    {
        protected override void ProcessPipelineStep(PipelineStep pipelineStep, PipelineContext pipelineContext, ILogger logger)
        {
            MappingSettings mappingSettings = pipelineStep.GetMappingSettings();
            if (mappingSettings == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because the pipeline step is missing a plugin.", $"plugin: {typeof(MappingSettings).FullName}");
                return;
            }
            IMappingSet mappingSet = mappingSettings.MappingSet;
            if (mappingSet == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because the pipeline step has no mapping set assigned.", $"plugin: {typeof(MappingSettings).FullName}", string.Format("property: {0}", "MappingSet"));
                return;
            }
            if (mappingSet.Mappings == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because the pipeline step has no mappings assigned.", $"plugin: {typeof(MappingSettings).FullName}", string.Format("property: {0}", "MappingSet"));
                return;
            }
            object sourceObject = GetSourceObject(mappingSettings, pipelineContext, logger);
            if (sourceObject == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because no source object could be resolved from the pipeline context.", $"source location: {mappingSettings.SourceObjectLocation}");
                return;
            }
            object targetObject = GetTargetObject(mappingSettings, pipelineContext, logger);
            if (targetObject == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because no target object could be resolved from the pipeline context.", $"target location: {mappingSettings.TargetObjectLocation}");
                return;
            }
            MappingContext mappingContext = new MappingContext
            {
                Source = sourceObject,
                Target = targetObject
            };
            if (!mappingSet.Run(mappingContext))
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because mapping set failed.", $"mappings that succeeded: {mappingContext.RunSuccess.Count}", $"mappings that were not attempted: {mappingContext.RunIgnore.Count}", $"mappings that failed: {mappingContext.RunFail.Count}");
                Log(logger.Error, pipelineContext, "At least one required value mapping failed.", $"mappings that failed: {mappingContext.RunFail.Count}", string.Format("value mapping ids: {0}", string.Join(",", (from x in mappingContext.RunFail
                                                                                                                                                                                                                     select x.Identifier).ToArray())));
                return;
            }
            if (mappingContext.RunFail.Any())
            {
                Log(logger.Debug, pipelineContext, "At least one value mapping failed.", $"mappings that failed: {mappingContext.RunFail.Count}", string.Format("value mapping ids: {0}", string.Join(",", (from x in mappingContext.RunFail
                                                                                                                                                                                                            select x.Identifier).ToArray())));
            }
            pipelineContext.GetSynchronizationSettings().IsTargetDirty = IsTargetDirty(mappingContext, mappingSettings, pipelineContext, logger);
            if (ShouldRunMappingsAppliedActions(mappingContext, mappingSettings, pipelineContext, logger))
            {
                RunMappingsAppliedActions(mappingContext, mappingSettings, pipelineContext, logger);
            }
        }

        protected override void RunMappingsAppliedActions(MappingContext mappingContext, MappingSettings mappingSettings, PipelineContext pipelineContext, ILogger logger)
        {
            if (mappingSettings.MappingsAppliedActions == null || !mappingSettings.MappingsAppliedActions.Any() || !mappingContext.RunSuccess.Any())
            {
                return;
            }
            IMappingsAppliedActionSet mappingsAppliedActionSet = GetMappingsAppliedActionSet(mappingSettings);
            if (mappingsAppliedActionSet == null)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because no mappings applied action set was resolved.");
                return;
            }
            MappingsAppliedContext context = new MappingsAppliedContext
            {
                MappingContext = mappingContext,
                MappingSet = mappingSettings.MappingSet
            };
            bool num = mappingsAppliedActionSet.Run(context);
            string actionSetResultMessage = GetActionSetResultMessage(context);
            if (!num)
            {
                Log(logger.Error, pipelineContext, "Pipeline step processing will abort because at least one mappings applied actions set failed.", actionSetResultMessage);
            }
            else
            {
                Log(logger.Debug, pipelineContext, "Mappings applied actions were applied successfully.", actionSetResultMessage);
            }
        }

        private string GetActionSetResultMessage(MappingsAppliedContext context)
        {
            List<string> list = new List<string>();
            if (context.ActionSuccess != null && context.ActionSuccess.Any())
            {
                list.Add(string.Format("actions that succeeded: {0}", string.Join(", ", (from x in context.ActionSuccess
                                                                                         select x.GetType().FullName).ToArray())));
            }
            if (context.ActionFail != null && context.ActionFail.Any())
            {
                list.Add(string.Format("actions that failed: {0}", string.Join(", ", (from x in context.ActionFail
                                                                                      select x.GetType().FullName).ToArray())));
            }
            if (context.ActionNotAttempted != null && context.ActionNotAttempted.Any())
            {
                list.Add(string.Format("actions that were not attempted: {0}", string.Join(", ", (from x in context.ActionNotAttempted
                                                                                                  select x.GetType().FullName).ToArray())));
            }
            return string.Join(", ", list.ToArray());
        }

        public override object GetObjectFromPipelineContext(Guid location, PipelineContext pipelineContext, ILogger logger)
        {
            if (location == ItemIDs.PipelineContextStorageLocationSource)
            {
                return GetSynchronizationSettings(pipelineContext, useParentPipelineContext: false)?.Source;
            }
            if (location == ItemIDs.PipelineContextStorageLocationTarget)
            {
                return GetSynchronizationSettings(pipelineContext, useParentPipelineContext: false)?.Target;
            }
            if (location == ItemIDs.PipelineContextStorageLocationParentSource)
            {
                return GetSynchronizationSettings(pipelineContext, useParentPipelineContext: true)?.Source;
            }
            if (location == ItemIDs.PipelineContextStorageLocationParentTarget)
            {
                return GetSynchronizationSettings(pipelineContext, useParentPipelineContext: true)?.Target;
            }
            if (location == ItemIDs.PipelineContextStorageLocationTempStorage)
            {
                return GetPipelineContextPlugin<TemporaryStoragePlugin>(pipelineContext, useParentPipelineContext: false)?.Object;
            }
            if (location == ItemIDs.PipelineContextStorageLocationParentTempStorage)
            {
                return GetPipelineContextPlugin<TemporaryStoragePlugin>(pipelineContext, useParentPipelineContext: true)?.Object;
            }
            if (location == ItemIDs.PipelineContextIterableData)
            {
                return GetPipelineContextPlugin<IterableDataSettings>(pipelineContext, useParentPipelineContext: false)?.Data;
            }
            return null;
        }

        protected override T GetPipelineContextPlugin<T>(PipelineContext pipelineContext, bool useParentPipelineContext)
        {
            PipelineContext pipelineContext2 = GetPipelineContext(pipelineContext, useParentPipelineContext);
            if (pipelineContext2 != null)
            {
                return pipelineContext2.GetPlugin<T>();
            }
            return default(T);
        }

        private PipelineContext GetPipelineContext(PipelineContext pipelineContext, bool useParentPipelineContext)
        {
            if (!useParentPipelineContext)
            {
                return pipelineContext;
            }
            return pipelineContext.GetPlugin<ParentPipelineContextSettings>()?.ParentPipelineContext;
        }
    }
}