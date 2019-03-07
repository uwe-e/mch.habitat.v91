using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Providers.Sc.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Diagnostics;
using Sitecore.Services.Core.Model;

namespace MCH.Feature.DEF.Processors.PipelineSteps
{

    [RequiredPipelineStepPlugins(new Type[]    {
    typeof(ResolveSitecoreItemSettings)
    })]
    [RequiredEndpointPlugins(new Type[]
    {
    typeof(ItemModelRepositorySettings)
    })]
    [RequiredPipelineContextPlugins(new Type[]
    {
    typeof(SelectedLanguagesSettings)
    })]
    public class ResolveMultilanguageSitecoreItemDictionaryPipelineStep : Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.ResolveMultilanguageSitecoreItemDictionaryPipelineStep
    {
        protected override ItemModel CreateItemModel(object identifierObject, IItemModelRepository repository, ResolveSitecoreItemSettings settings, PipelineContext pipelineContext, ILogger logger, string language = null, int version = 0)
        {
            var itemModel = base.CreateItemModel(identifierObject, repository, settings, pipelineContext, logger, language, version);
            return itemModel;
        }
    }
}