using PremiersoftChallenge.BuildingBlocks.Results;

namespace PremiersoftChallenge.BuildingBlocks.Errors
{
    public static class TransactionErrors
    {
        public static readonly Error InvalidValue = Error.Problem("INVALID_VALUE", "O valor da transação deve ser maior do que 0 (zero).");
        public static readonly Error InvalidFlowType = Error.Problem("INVALID_TYPE", "Para este tipo de operação, a conta corrente deve ser a mesma logada.");
        public static readonly Error RefundFailed = Error.Problem("REFUND_FAILED", "Ocorreu um erro ao processar a requisição, e também processar o reembolso, contate o banco.");
    }
}
