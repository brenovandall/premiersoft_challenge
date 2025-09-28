namespace Application.Dto
{
    public class TransactionDto
    {
        public string Id { get; set; } = default!;
        public string CheckingAccountId { get; set; } = default!;
        public string TransactionDate { get; set; } = default!;
        public string TransactionFlow { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}
