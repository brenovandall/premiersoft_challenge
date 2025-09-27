namespace PremiersoftChallenge.SharedKernel
{
    public interface IEntity<T> : IEntity
    {
        public T Id { get; set; }
    }

    public interface IEntity;
}
