using Dapper.Contrib.Extensions;
using DNA.Domain.Extentions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DNA.Domain.Utils {
    public class ScreenModelCollection : List<ScreenModel> {
        private readonly string _tablePrefix;
        public ScreenModelCollection(string tablePrefix) {
            this._tablePrefix = tablePrefix;
        }

        public new void Add(ScreenModel screenModel) {
            TryAdd(screenModel);
        }

        public bool TryAdd(ScreenModel screenModel) {
            if (screenModel == null)
                return false;
            screenModel.Index = this.Count;
            if (!this.Any(_ => _.Name == screenModel.Name)) {
                if (!string.IsNullOrWhiteSpace(screenModel.TableName))
                    screenModel.TableName.Replace("{TablePrefix}", _tablePrefix);
                base.Add(screenModel);
                return true;
            }
            return false;
        }

        public void TryAddRange(List<ScreenModel> screenModels) {
            if (screenModels == null)
                return;
            foreach (var item in screenModels) {
                TryAdd(item);
            }
        }
    }

    public class ScreenModel {

        [JsonIgnore] public int Index { get; set; }
        [JsonIgnore] public string TableName { get; set; }
        [JsonIgnore] public Type Type { get; set; }
        [JsonIgnore] public bool AllowEditing { get; set; }
        [JsonIgnore] public bool IsCalendarActive { get; set; }
        [JsonIgnore] public bool HasIdentityIncrement { get; set; }
        [JsonIgnore] public List<ScreenColumn> Columns { get; set; }
        [JsonIgnore] public List<ScreenSubModel> SubModels { get; set; }
        [JsonIgnore] public List<ScreenSubMenu> SubMenus { get; set; }


        [JsonProperty("keyFieldName")]
        public string KeyFieldName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; } = false;

        /// <summary>
        /// null, gallery, calendar
        /// </summary>
        [JsonProperty("viewType")]
        public string ViewType { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("dxIcon")]
        public string DxIcon { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("hasInSidebar")]
        public bool HasInSidebar { get; set; } = false;

        [JsonProperty("hideDetails")]
        public bool? HideDetails { get; set; } = false;

        [JsonProperty("isDefinitionModel")]
        public bool IsDefinitionModel { get; set; } = false;

        [JsonProperty("editing")]
        public ScreenEditing Editing { get; set; }

        [JsonProperty("calendar")]
        public ScreenCalendar Calendar { get; set; }

        [JsonProperty("assembly")]
        public string Assembly { get; set; }

        [JsonProperty("dataSource")]
        public ScreenDataSource DataSource { get; set; }

        /// <summary>
        /// </summary>
        /// <example>
        /// "grid": {
        ///   "allowFilter": true,
        ///   "columnAutoWidth": true,
        ///   "columnResizingMode": "widget",
        ///   "disabled": false,
        ///   "hoverStateEnabled": true,
        ///   "noDataText": "No data",
        ///   "pager": {
        ///     "visible": true,
        ///     "allowedPageSizes": null,
        ///     "infoText": "Sayfa {0}/{1} ({2} kayıt)",
        ///     "showInfo": true,
        ///     "showNavigationButtons": true,
        ///     "showPageSizeSelector": true
        ///   },
        ///   "paging": {
        ///     "enabled": true,
        ///     "defaultPageSize": 10
        ///   },
        ///   "rowAlternationEnabled": false,
        ///   "scrolling": {
        ///     "mode": "standard",
        ///     "columnRenderingMode": "standard",
        ///     "preloadEnabled": false,
        ///     "rowRenderingMode": "standard",
        ///     "scrollByContent": false,
        ///     "scrollByThumb": false,
        ///     "showScrollbar": "onHover",
        ///     "useNative": "auto"
        ///   },
        ///   "searchPanel": {
        ///     "visible": true
        ///   },
        ///   "selection": null,
        ///   "showBorders": false,
        ///   "showColumnHeaders": true,
        ///   "showColumnLines": true,
        ///   "showRowLines": false,
        ///   "sorting": {
        ///     "enabled": true,
        ///     "mode": "multiple",
        ///     "showSortIndexes": true,
        ///     "default": {
        ///       "selector": "Id",
        ///       "desc": true
        ///     }
        ///   },
        ///   "wordWrapEnabled": false
        /// </example>
        [JsonProperty("grid")]
        public Dictionary<string, object> Grid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// "actions": [
        ///   {
        ///     "text": "Goto the License View",
        ///     "icon": "security",
        ///     "route": "/panel/screen/license/{Id}"
        ///   },
        ///   {
        ///     "text": "Copy License Public Key",
        ///     "dxIcon": "copy",
        ///     "type": "dx",
        ///     "eval": "copy('Muşteri: {CustomerId} | Program: {ProgramId} | Anahtar: {PublicKey}');",
        ///     "dependsOnSelected": true,
        ///     "onSuccess": {
        ///       "text": "Kopyalandı."
        ///     }
        ///   },
        ///   {
        ///     "text": "Reset License to default",
        ///     "dxIcon": "revert",
        ///     "dependsOnSelected": true,
        ///     "request": {
        ///       "method": "POST",
        ///       "url": "api/license/reset/{Id}",
        ///       "refreshAfterSuccess": true,
        ///       "onError": {
        ///         "text": "İşlem başarısız."
        ///       },
        ///       "onSuccess": {
        ///         "text": "İşlem Başarılı."
        ///       }
        ///     },
        ///     "confirmation": {
        ///       "message": "Are you sure you want to reset License.MachineKey to null?"
        ///     },
        ///     "executeWhen": {
        ///       "condition": [
        ///         "Id",
        ///         ">",
        ///         "0"
        ///       ],
        ///       "eval": "1==1"
        ///     }
        ///   }
        /// ],
        /// </example>
        [JsonProperty("actions")]
        public object Actions { get; set; }

        public ScreenModel(Type type) {
            Name = type.Name;
            Type = type;
            HasIdentityIncrement = false;
            HasInSidebar = false;
            TableName = type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>()?.Name;
            Assembly = type.FullName;
            Route = Regex.Replace(Name, @"[A-Z]", (m) => $"-{m.Value}").Trim('-').ToLower();
            Title = Name + " Screen";
        }

        // public ScreenModel(Type type, string tableName) : this(type) {
        //     if (!string.IsNullOrWhiteSpace(tableName))
        //         TableName = tableName;
        // }

        public ScreenModel(Type type, bool hasIdentityIncrement) : this(type) {
            HasIdentityIncrement = hasIdentityIncrement;
        }

        public ScreenModel(string tableName, Type type, bool hasIdentityIncrement) : this(type) {
            HasIdentityIncrement = hasIdentityIncrement;
            if (!string.IsNullOrWhiteSpace(tableName))
                TableName = tableName;
        }

        public ScreenModel Visibility(bool visible = true) {
            this.Visible = visible;
            return this;
        }

        public ScreenModel Editable(bool editable) {
            this.AllowEditing = editable;
            return this;
        }

        public ScreenModel Definition() {
            this.IsDefinitionModel = true;
            return this;
        }

        public ScreenModel HiddenInSidebar() {
            this.HasInSidebar = true;
            return this;
        }

        public ScreenModel Emblem(string icon) {
            this.Icon = icon;
            return this;
        }

        public ScreenModel CalendarView(string textExpr, string allDayExpr, string startDateExpr, string endDateExpr, string descriptionExpr, params ScreenCalendarResource[] resources) {
            this.ViewType = "calendar";
            IsCalendarActive = true;
            this.Calendar = new ScreenCalendar() {
                startDateExpr = startDateExpr,
                endDateExpr = endDateExpr,
                allDayExpr = allDayExpr,
                textExpr = textExpr,
                descriptionExpr = descriptionExpr,
                resources = resources.ToList()
            };
            return this;
        }

        public ScreenModel GalleryView() {
            this.ViewType = "gallery";
            return this;
        }

        public void GenerateDataSource(ScreenDataSource existing) {
            DataSource = existing;
        }

        public void GenerateGrid(Dictionary<string, object> existing) {
            Grid = existing;
        }

        public void GenerateActions(object existing) {
            Actions = existing;
        }

        public void GenerateEditing(ScreenEditing existingEditing) {
            Editing = existingEditing;
            if (AllowEditing) {
                if (Editing == null)
                    Editing = new ScreenEditing() {
                        enabled = true,
                        allowAdding = true,
                        allowDeleting = true,
                        allowUpdating = true,
                        mode = "popup"
                    };
            }
        }

        public void GenerateCalendar(ScreenCalendar existingCalendar) {
            if (IsCalendarActive) {
                Calendar.startDateExpr = string.IsNullOrWhiteSpace(existingCalendar.startDateExpr) ? Calendar.startDateExpr : existingCalendar.startDateExpr;
                Calendar.endDateExpr = string.IsNullOrWhiteSpace(existingCalendar.endDateExpr) ? Calendar.endDateExpr : existingCalendar.endDateExpr;
                Calendar.textExpr = string.IsNullOrWhiteSpace(existingCalendar.textExpr) ? Calendar.textExpr : existingCalendar.textExpr;
                Calendar.allDayExpr = string.IsNullOrWhiteSpace(existingCalendar.allDayExpr) ? Calendar.allDayExpr : existingCalendar.allDayExpr;
                Calendar.descriptionExpr = string.IsNullOrWhiteSpace(existingCalendar.descriptionExpr) ? Calendar.descriptionExpr : existingCalendar.descriptionExpr;
                Calendar.editing ??= Editing != null
                    ? new ScreenCalendarEditing {
                        enabled = Editing.enabled,
                        allowAdding = Editing.allowAdding,
                        allowDeleting = Editing.allowDeleting,
                        allowDragging = Editing.allowUpdating,
                        allowResizing = Editing.allowUpdating,
                        allowUpdating = Editing.allowUpdating,
                        roles = Editing.roles
                    }
                    : null;
                Calendar.resources ??= existingCalendar.resources;
            }
            else {
                Calendar = existingCalendar;
            }
        }

        public void GenerateSubMenus(List<ScreenSubMenu> existing, ScreenModelCollection parent) {
            if (existing != null)
                SubMenus = existing;
        }

        public void GenerateSubModels(List<ScreenSubModel> existingSubModels, ScreenModelCollection parent) {
            SubModels = existingSubModels ?? new List<ScreenSubModel>();
            foreach (var item in Type.GetProperties()) {

                //var jsonIgnorAttr = item.GetCustomAttribute<JsonIgnoreAttribute>();
                //if (jsonIgnorAttr != null)
                //    continue;
                if (!item.PropertyType.IsPublic)
                    continue;
                if (item.PropertyType == typeof(string))
                    continue;
                if (item.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))) {
                    var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (argumentType == null)
                        continue;
                    var refScreen = parent.Find(_ => _.Name == argumentType.Name);
                    var subModel = SubModels.Find(_ => _.name == refScreen.Name);
                    if (subModel == null) {
                        subModel = new ScreenSubModel() { name = argumentType.Name };
                        SubModels.Add(subModel);
                    }
                    subModel.title ??= $"{refScreen.Name} Screen";
                    subModel.type ??= refScreen.ViewType;
                    subModel.icon ??= refScreen.Icon;
                    subModel.dxIcon ??= refScreen.DxIcon;
                    subModel.showIn ??= new string[] { "tab" };
                    subModel.route ??= refScreen.Route;
                    subModel.relationFieldNames ??= new object[2] { refScreen.KeyFieldName, "?" };
                }
            }
        }

        public void GenerateColumns(List<ScreenColumn> existingColumns, ScreenModelCollection parent) {
            Columns = existingColumns ?? new List<ScreenColumn>();
            foreach (var item in Type.GetProperties()) {
                var columnsAttr = item.GetCustomAttribute<ColumnAttribute>();
                if (!item.PropertyType.IsPublic)
                    continue;
                // remove ignored column
                if (item.GetCustomAttribute<JsonIgnoreAttribute>() != null) {
                    var ignored = Columns.Find(_ => _.name == item.Name);
                    if (ignored != null)
                        Columns.Remove(ignored);
                    continue;
                }

                #region xxx
                if (item.PropertyType == typeof(string)) { }
                else if (item.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
                    //var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();

                    //var tableAttr = argumentType.GetCustomAttribute<TableAttribute>();
                    //if (tableAttr != null) {
                    //    var dataColumn = Columns.Find(_ => _.name == item.Name);
                    //    var model = parent.Find(_ => _.Name == argumentType.Name);
                    //    columnData = dataColumn?.data != null ? new ScreenColumnData {
                    //        type = "customeStore",
                    //        name = model.Name,
                    //    } : null;
                    //}
                    continue;
                }
                else if (item.PropertyType.IsClass) {
                    continue;
                }
                #endregion

                var col = Columns.Find(_ => _.name == item.Name);
                if (col == null) {
                    col = new ScreenColumn(item.Name);
                    Columns.Add(col);
                }

                col.type = GetColType(col, item);

                var lookupTable = parent.Find(_ => _.Name == columnsAttr?.LookupType?.Name);
                if (lookupTable != null) {
                    if (col.data == null)
                        col.data = new ScreenColumnData();
                    col.data.name ??= lookupTable.Name;
                    // key = lookupTable.KeyFieldName,
                    col.data.type ??= "customStore";
                    col.data.valueExpr ??= columnsAttr.LookupType.GetProperties().Where(_ => _.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault()?.Name;
                    col.data.displayExpr ??= columnsAttr.LookupType.GetProperties().Where(_ => (_.GetCustomAttribute<ColumnAttribute>()?.DisplayExpr).GetValueOrDefault(false)).Select(_ => _.Name).ToArray();
                }
                // hidden
                if (col.hidden == null) {
                    if (columnsAttr?.Hidden == true)
                        col.hidden = true;
                }
                // roles
                if (col.roles == null) {
                    var restrictedRoleAttr = item.GetCustomAttribute<JsonRestrictedRoleAttribute>();
                    if (restrictedRoleAttr != null)
                        col.roles = restrictedRoleAttr.Roles.ToArray();
                }
                // required
                var required = item.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>() != null;
                if (col.required == null) {
                    if (required)
                        col.required = true;
                }
                if (required) {
                    if (col.allowEditing == null)
                        col.allowEditing = true;
                }
            }
        }

        string GetColType(ScreenColumn col, PropertyInfo item) {
            bool isEnum = false;
            Type secondType = null;
            if (item.PropertyType.IsValueType) {
                if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                    secondType = argumentType;
                    isEnum = secondType.IsEnum;
                }
            }
            string val;
            var columnAttribute = item.GetCustomAttribute<ColumnAttribute>();
            if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?)) {
                val = col.type ?? "check";
            }
            else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?)) {
                val = col.type ?? "datetime";
                col.format ??= "LLL";
            }
            else if (item.PropertyType.IsEnum || isEnum) {
                val = col.type ?? "numeric";
                col.autoComplete ??= item.PropertyType.Name;
            }
            else if (item.PropertyType.IsNumeric() || (secondType != null && secondType.IsNumeric())) {
                val = col.type ?? "numeric";
            }
            else if (columnAttribute?.Image == true) {
                val = col.type ?? "image";
            }
            else if (columnAttribute?.Color == true) {
                val = col.type ?? "color";
            }
            else {
                var stringLength = item.GetCustomAttribute<System.ComponentModel.DataAnnotations.StringLengthAttribute>()?.MaximumLength ?? 0;
                val = col.type ?? (stringLength > 100 ? "textArea" : "text");
                if (stringLength > 250)
                    col.colSpan ??= 2;
                if (stringLength > 0 && stringLength < 4000)
                    col.stringLength ??= stringLength;
            }
            return val;
        }
    }

    public class ScreenEditing {
        public bool enabled { get; set; }
        /// <summary>
        /// form, row, popup
        /// </summary>
        public string mode { get; set; }
        public bool? allowUpdating { get; set; }
        public bool? allowAdding { get; set; }
        public bool? allowDeleting { get; set; }
        public string[] roles { get; set; }
    }

    public class ScreenDataSource {

        public string name { get; set; }

        /// <summary>
        /// simpleArray, customStore
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string load { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string insert { get; set; }

        /// <summary>
        /// `url/${key}`
        /// </summary>
        public string update { get; set; }

        /// <summary>
        /// `url/${key}`
        /// </summary>
        public string delete { get; set; }
    }

    public class ScreenSubMenu {
        public string name { get; set; }
        public string type { get; set; }
        public string[] defaultFilter { get; set; }
        public bool showInSidebar { get; set; }
    }

    public class ScreenSubModel {
        public string name { get; set; }
        public string title { get; set; }
        public bool? visible { get; set; }
        public bool? dependsOnSelected { get; set; }

        /// <summary>
        /// list, property, gallery, files (not iplemented yet)
        /// </summary>
        public string type { get; set; }
        public string icon { get; set; }
        public string dxIcon { get; set; }
        public string uploadUrl { get; set; }

        /// <summary>
        /// toolbar, popup, tab  (multiple)
        /// </summary>
        public object showIn { get; set; }
        public string route { get; set; }
        public object[] relationFieldNames { get; set; }
        public string[] roles { get; set; }
    }

    public class ScreenColumn {
        public int? index { get; set; }
        public int? editIndex { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string caption { get; set; }

        /// <summary>
        /// , , , , currency
        /// </summary>
        public string type { get; set; }
        public string format { get; set; }
        public string currency { get; set; }
        public string autoComplete { get; set; }
        public bool? withTimeEdit { get; set; }
        public int? colSpan { get; set; }
        public bool? hidden { get; set; }
        public bool? translate { get; set; }
        public int? width { get; set; }
        public string[] roles { get; set; }
        public bool? required { get; set; }
        public int? stringLength { get; set; }
        public bool? allowEditing { get; set; }
        public bool? allowFiltering { get; set; }
        public string helpText { get; set; }
        public ScreenColumnData data { get; set; }
        public ScreenColumnEditWith editWith { get; set; }

        public ScreenColumn() {

        }

        public ScreenColumn(string name_) {
            name = name_;
        }
    }

    public class ScreenColumnData {
        /// <summary>
        /// customStore, simpleStore
        /// </summary>
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string valueExpr { get; set; }
        public object displayExpr { get; set; }
        public string key { get; set; }
        public string loadMode { get; set; }
        public bool? cacheRawData { get; set; }
        public string[] filter { get; set; }

        public List<ScreenColumn> columns { get; set; }

    }

    public class ScreenColumnEditWith {
        public string type { get; set; }
        public string key { get; set; }
        public string valueExpr { get; set; }
        public string displayExpr { get; set; }
        public List<ScreenColumn> columns { get; set; }
    }

    public class ScreenCalendar {
        public string[] dateRageFields { get; set; }
        public ScreenCalendarEditing editing { get; set; }
        public string startDateExpr { get; set; }
        public string endDateExpr { get; set; }
        public string allDayExpr { get; set; }
        public string textExpr { get; set; }
        public string descriptionExpr { get; set; }
        public List<ScreenCalendarResource> resources { get; set; }
    }


    public class ScreenCalendarEditing {
        public bool enabled { get; set; }
        public bool? allowUpdating { get; set; }
        public bool? allowAdding { get; set; }
        public bool? allowDeleting { get; set; }
        public bool? allowResizing { get; set; }
        public bool? allowDragging { get; set; }
        public string[] roles { get; set; }
    }

    public class ScreenCalendarResource {
        public string fieldExpr { get; set; }
        public string displayExpr { get; set; }
        public string valueExpr { get; set; }
        public string colorExpr { get; set; }
        public bool? allowMultiple { get; set; }
        public string label { get; set; }
        public bool? useColorAsDefault { get; set; }
        public ScreenDataSource dataSource { get; set; }
        public ScreenCalendarResource() {

        }
        public ScreenCalendarResource(Type type, string fieldExpr, string valueExpr = "Id", string displayExpr = "Name", string colorExpr = "Color") {
            this.label = type.Name;
            this.dataSource = new ScreenDataSource { type = "customStore", name = type.Name };
            this.fieldExpr = fieldExpr;
            this.displayExpr = displayExpr;
            this.valueExpr = valueExpr;
            this.colorExpr = colorExpr;
        }
    }
}
