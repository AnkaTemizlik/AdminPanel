using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models {
    public class KeyValue : KeyValue<int> {
        public KeyValue() {
        }
        public KeyValue(string value) : base(0, value) {
        }
        public KeyValue(int key, string value) : base(key, value) {
        }
    }

    public class KeyValue<T> {
        public T Key { get; set; }
        public string Value { get; set; }
        public KeyValue() {
        }

        public KeyValue(T key, string value) {
            Key = key;
            Value = value;
        }
    }
    public class KeyValue<T, K> {
        public T Key { get; set; }
        public K Value { get; set; }
        public KeyValue() {
        }

        public KeyValue(T key, K value) {
            Key = key;
            Value = value;
        }
    }

    public class IdValue {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public IdValue() {

        }
        public IdValue(int id, string value) {
            Id = id;
            Value = value;
        }
    }


    public class IdValue<T> {

        [JsonProperty("id")]
        public T Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public IdValue() {

        }
        public IdValue(T id, string value) {
            Id = id;
            Value = value;
        }
    }
}
