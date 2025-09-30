using Domain;

namespace Application.Data.Repository
{
    public interface ITransferRepository
    {
        Task Add(ITransfer transfer);
    }
}
