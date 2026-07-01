using System;
using System.Collections.Generic;

namespace OctoGames.Repository
{
    public interface IRepository<T>
        where T : class, IRepositoryEntity
    {
        void Add(T item);
        void Remove(T item);
        bool TryGet(Guid id, out T item);
        IReadOnlyList<T> GetAll();
        void Clear();
    }
}
