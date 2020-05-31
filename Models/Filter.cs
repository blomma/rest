using System;
using Newtonsoft.Json;

namespace rest.Models {
    public class Filter : ICloneable {
        public string OrderBy { get; set; }
        public string[] ThenBy { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }

        public object Clone() {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }
    }
}