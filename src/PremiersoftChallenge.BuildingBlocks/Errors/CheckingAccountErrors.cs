using PremiersoftChallenge.BuildingBlocks.Results;

namespace PremiersoftChallenge.BuildingBlocks.Errors
{
    public static class CheckingAccountErrors
    {
        public static readonly Error InvalidCpf = Error.Problem("INVALID_DOCUMENT", "O CPF informado é inválido.");
        public static readonly Error InvalidCredentials = Error.Problem("USER_UNAUTHORIZED", "Credenciais incorretas.");
        public static readonly Error InvalidAccount = Error.Problem("INVALID_ACCOUNT", "Conta corrente não encontrada.");
        public static readonly Error InactiveAccount = Error.Problem(
            "INACTIVE_ACCOUNT", "A conta corrente não pode realizar movimentações, pois a mesma encontra-se inativa.");
    }
}
