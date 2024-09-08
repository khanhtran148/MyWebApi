using System;

namespace MyWebApi.Domain.Entities
{
    public class Language
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int LanguageId { get; set; }
    }
}
