using PremiersoftChallenge.BuildingBlocks.Results;

namespace PremiersoftChallenge.BuildingBlocks.Errors
{
    public static class CheckingAccountErrors
    {
        public static readonly Error InvalidCpf = Error.Problem("INVALID_DOCUMENT", "O CPF informado é inválido.");
        public static readonly Error InvalidCredentials = Error.Problem("USER_UNAUTHORIZED", "Credenciais incorretas.");
    }
}
