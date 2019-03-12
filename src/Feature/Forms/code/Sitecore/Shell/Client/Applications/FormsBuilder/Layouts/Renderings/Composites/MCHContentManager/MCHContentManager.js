(function (speak) {
    speak.component([], function () {
        return {
            initialized: function () {
                this.CompItems = [];

                this.defineComputedProperty("InnerDatasourceComponent", function () {
                    return this.DynamicDatasource;
                });

                this.on({
                    "change:CompItems": this.updatedCompItems,
                    "change:CompDatasourceGuid": this.updatedCompDatasourceGuid,
                    "change:CompDisplayFieldName": this.updatedCompDisplayFieldName,
                    "change:CompValueFieldName": this.updatedCompValueFieldName
                }, this);

                this.DynamicDatasource.on({
                    "change:Items": this.updatedInnerItems,
                    "change:CurrentDisplayFieldName": this.updatedInnerDisplayFieldName,
                    "change:CurrentValueFieldName": this.updatedInnerValueFieldName,
                    "change:DataSourceGuid": this.updatedInnerDatasourceGuid
                }, this);
            },

            reset: function () {
                if (this.DynamicDatasource.Items.length) {
                    this.DynamicDatasource.Items = [];
                }
                this.DynamicDatasource.CurrentDisplayFieldName = "";
                this.DynamicDatasource.CurrentValueFieldName = "";
                this.DynamicDatasource.DataSourceGuid = "";
            },

            updatedCompDatasourceGuid: function () {
                this.InnerDatasourceComponent.DataSourceGuid = this.CompDatasourceGuid;
            },

            updatedCompDisplayFieldName: function () {
                this.InnerDatasourceComponent.CurrentDisplayFieldName = this.CompDisplayFieldName;
            },

            updatedCompValueFieldName: function () {
                this.InnerDatasourceComponent.CurrentValueFieldName = this.CompValueFieldName;
            },

            updatedCompItems: function (items) {
                if (items !== this.InnerDatasourceComponent.Items) {
                    this.InnerDatasourceComponent.reset(items);
                }
            },

            updatedInnerDatasourceGuid: function () {
                this.CompDatasourceGuid = this.InnerDatasourceComponent.DataSourceGuid;
            },

            updatedInnerDisplayFieldName: function () {
                this.CompDisplayFieldName = this.InnerDatasourceComponent.CurrentDisplayFieldName;
            },

            updatedInnerValueFieldName: function () {
                this.CompValueFieldName = this.InnerDatasourceComponent.CurrentValueFieldName;
            },

            updatedInnerItems: function () {
                var items = this.InnerDatasourceComponent.Items;
                if (items !== this.CompItems) {
                    this.CompItems = items;
                }
            }
        };
    }, "MCHContentManager");
})(Sitecore.Speak);