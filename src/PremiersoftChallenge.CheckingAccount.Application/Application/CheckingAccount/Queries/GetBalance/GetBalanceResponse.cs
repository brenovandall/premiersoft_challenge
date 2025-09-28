namespace Application.CheckingAccount.Queries.GetBalance
{
    public sealed class GetBalanceResponse
    {
        public long AccountNumber { get; set; }
        public string Name { get; set; } = default!;
        public DateTime ResponseDate { get; set; }
        public double Balance { get; set; }
    }
}
