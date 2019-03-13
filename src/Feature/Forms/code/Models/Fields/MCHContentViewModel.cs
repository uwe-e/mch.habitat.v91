using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ExperienceForms.Mvc.Models;

namespace MCH.Feature.Forms.Models.Fields
{
    public class MCHContentViewModel : Sitecore.ExperienceForms.Mvc.Models.Fields.DropDownListViewModel
    {
        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            DataSource = item.Fields["Datasource"]?.Value;
            IsDynamic = MainUtil.GetBool(item.Fields["Is Dynamic"]?.Value, defaultValue: false);
            ShowEmptyItem = MainUtil.GetBool(item.Fields[Templates.MCHCountries.Fields.ShowEmptyItem]?.Value, defaultValue: false);
        }
        protected override void UpdateItemFields(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            base.UpdateItemFields(item);
            item.Fields[Templates.MCHCountries.Fields.ShowEmptyItem]?.SetValue(ShowEmptyItem ? "1" : string.Empty, force: true);
        }

        protected override void InitializeDataSourceSettings(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            ListFieldItemCollection settings = DataSourceSettingsManager.GetSettings(item);
            if (settings != null)
            {
                var fields = settings.OrderBy(c => c.Text);
                Items.Clear();
                Items.AddRange(fields);
            }
        }
    }
}