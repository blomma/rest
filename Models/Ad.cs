using System;

namespace rest.Models {
    public class Ad {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public double? Price { get; set; }
        public string Email { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}