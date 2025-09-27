﻿namespace PremiersoftChallenge.SharedKernel
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}
