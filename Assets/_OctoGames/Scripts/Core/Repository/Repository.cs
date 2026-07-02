using System;
using System.Collections.Generic;

namespace OctoGames.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class, IRepositoryEntity
    {
        private readonly Dictionary<Guid, T> _items = new();

        public event Action Changed;

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.TryGetValue(item.Id, out var existing))
            {
                if (ReferenceEquals(existing, item))
                    return;

                throw new ArgumentException(
                    $"Item with id '{item.Id}' is already in the repository.",
                    nameof(item));
            }

            _items[item.Id] = item;
            OnChanged();
        }

        public void Remove(T item)
        {
            if (item == null)
                return;

            if (!_items.Remove(item.Id))
                return;

            OnChanged();
        }

        public bool TryGet(Guid id, out T item) => _items.TryGetValue(id, out item);

        public IReadOnlyList<T> GetAll()
        {
            var result = new T[_items.Count];
            _items.Values.CopyTo(result, 0);
            return result;
        }

        public void Clear() => _items.Clear();

        private void OnChanged() => Changed?.Invoke();
    }
}
