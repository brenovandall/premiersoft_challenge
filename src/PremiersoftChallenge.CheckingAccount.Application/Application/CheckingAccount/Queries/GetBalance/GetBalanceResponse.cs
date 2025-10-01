namespace Application.CheckingAccount.Queries.GetBalance
{
    public sealed class GetBalanceResponse
    {
        public long AccountNumber { get; set; }
        public string Name { get; set; } = default!;
        public string ResponseDate { get; set; } = default!;
        public double Balance { get; set; }

        public GetBalanceResponse()
        {
            ResponseDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
