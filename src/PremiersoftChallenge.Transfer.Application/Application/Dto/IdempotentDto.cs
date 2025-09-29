namespace Application.Dto
{
    public class IdempotentDto
    {
        public string Key { get; set; } = default!;
        public string Request { get; set; } = default!;
        public string Response { get; set; } = default!;
    }
}
