namespace Infrastructure.Http
{
    public sealed class ApiErrorResponse
    {
        public string Title { get; set; } = default!;
        public string Detail { get; set; } = default!;
        public string Type { get; set; } = default!;
        public int Status { get; set; }
        public Dictionary<string, object>? Extensions { get; set; }
    }
}
