using System.ComponentModel.DataAnnotations;

namespace ZetesAPI_2.Data
{
    public class CsvResponses
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Search { get; set; }
        public string LibraryFilter { get; set; }
        public string Visible { get; set; }
    }
}
