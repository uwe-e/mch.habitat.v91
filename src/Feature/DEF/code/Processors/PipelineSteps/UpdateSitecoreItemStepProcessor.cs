using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Diagnostics;
using Sitecore.Services.Core.Model;

namespace MCH.Feature.DEF.Processors.PipelineSteps
{
    [RequiredPipelineStepPlugins(new Type[]
{
    typeof(DataLocationSettings),
    typeof(EndpointSettings)
})]

    public class UpdateSitecoreItemStepProcessor : Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.UpdateSitecoreItemStepProcessor
    {
        protected override void ProcessPipelineStep(PipelineStep pipelineStep, PipelineContext pipelineContext, ILogger logger)
        {
            IItemModelRepository itemModelRepository = GetItemModelRepository();
            if (itemModelRepository != null)
            {
                IEnumerable<ItemModel> targetObjectAsItemModels = GetTargetObjectAsItemModels(pipelineStep, pipelineContext, logger);
                if (targetObjectAsItemModels != null)
                {
                    Guid guid = Guid.Empty;
                    foreach (ItemModel item in targetObjectAsItemModels)
                    {
                        FixItemModel(item);
                        string text = item.ContainsKey("ItemLanguage") ? item["ItemLanguage"].ToString() : string.Empty;
                        if (guid == Guid.Empty)
                        {
                            guid = item.GetItemId();
                        }
                        bool flag = false;
                        if (guid == Guid.Empty)
                        {
                            guid = itemModelRepository.Create(item);
                            if (guid != Guid.Empty)
                            {
                                flag = true;
                                item["ItemID"] = guid;
                            }
                        }
                        else
                        {
                            flag = itemModelRepository.Update(guid, item, text);
                        }
                        if (!flag)
                        {
                            logger.Error("Item was not saved. (id: {0}, language: {1})", guid, text);
                            break;
                        }
                        logger.Debug("Item was saved. (id: {0}, language: {1})", guid, text);
                    }
                }
            }
        }

        protected override IEnumerable<ItemModel> GetTargetObjectAsItemModels(PipelineStep pipelineStep, PipelineContext pipelineContext, ILogger logger)
        {
            DataLocationSettings dataLocationSettings = pipelineStep.GetDataLocationSettings();
            object objectFromPipelineContext = GetObjectFromPipelineContext(dataLocationSettings.DataLocation, pipelineContext, logger);
            if (objectFromPipelineContext == null)
            {
                return null;
            }
            List<ItemModel> list = new List<ItemModel>();
            ItemModel itemModel = objectFromPipelineContext as ItemModel;
            if (itemModel != null)
            {
                list.Add(itemModel);
            }
            else
            {
                IDictionary<string, ItemModel> dictionary = objectFromPipelineContext as IDictionary<string, ItemModel>;
                if (dictionary != null)
                {
                    foreach (string key in dictionary.Keys)
                    {
                        list.Add(dictionary[key]);
                    }
                }
            }
            if (list.Count == 0)
            {
                Log(logger.Error, pipelineContext, "The object from the data source location is not compatible with the pipeline step processor.", $"data source location: {dataLocationSettings.DataLocation}");
            }
            return list;
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
            //return base.GetPipelineContextPlugin<T>(pipelineContext, useParentPipelineContext);
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