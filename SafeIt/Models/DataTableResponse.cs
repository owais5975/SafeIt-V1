namespace SafeIt.Models
{
    public class DataTableResponse
    {
        public string draw { get; set; } = String.Empty;
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public dynamic data { get; set; }
    }
}
