using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public int Index { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type Type { get; set; }
        public bool HasIdentityIncrement { get; set; }
        public bool HasInSidebar { get; set; } = false;
        public bool Editable { get; set; } = false;
        public bool Visible { get; set; } = false;
        public bool IsDefinitionModel { get; set; } = false;
        //public List<ScreenModel> SubModels { get; set; } = new List<ScreenModel>();

        public ScreenModel(Type type) {
            Name = type.Name;
            Type = type;
            HasIdentityIncrement = false;
            HasInSidebar = false;
        }

        public ScreenModel(Type type, string tableName) {
            TableName = tableName;
            Name = type.Name;
            Type = type;
        }

        public ScreenModel(string tableName, Type type, bool hasIdentityIncrement) {
            TableName = tableName ?? type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>()?.Name;
            Name = type.Name;
            Type = type;
            HasIdentityIncrement = hasIdentityIncrement;
        }

        public ScreenModel Visibility(bool visible = true) {
            this.Visible = visible;
            return this;
        }

        public ScreenModel Editing(bool editable) {
            this.Editable = editable;
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

    }
}
