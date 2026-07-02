using System;
using NUnit.Framework;
using OctoGames.Repository;

namespace OctoGames.Tests
{
    public sealed class RepositoryTests
    {
        private Repository<StubItem> _repository;

        [SetUp]
        public void SetUp() => _repository = new Repository<StubItem>();

        [Test]
        public void Add_TryGet_ReturnsItem()
        {
            var item = new StubItem(Guid.NewGuid());

            _repository.Add(item);

            Assert.IsTrue(_repository.TryGet(item.Id, out var found));
            Assert.AreSame(item, found);
        }

        [Test]
        public void Remove_RemovesItem()
        {
            var item = new StubItem(Guid.NewGuid());
            _repository.Add(item);

            _repository.Remove(item);

            Assert.IsFalse(_repository.TryGet(item.Id, out _));
        }

        [Test]
        public void GetAll_ReturnsSnapshot()
        {
            var first = new StubItem(Guid.NewGuid());
            var second = new StubItem(Guid.NewGuid());
            _repository.Add(first);
            _repository.Add(second);

            var all = _repository.GetAll();

            Assert.AreEqual(2, all.Count);
            CollectionAssert.Contains(all, first);
            CollectionAssert.Contains(all, second);
        }

        [Test]
        public void GetAll_ReturnsCopy_NotLiveView()
        {
            var item = new StubItem(Guid.NewGuid());
            _repository.Add(item);

            var snapshot = _repository.GetAll();
            _repository.Remove(item);

            Assert.AreEqual(1, snapshot.Count);
            Assert.AreEqual(0, _repository.GetAll().Count);
        }

        [Test]
        public void Add_SameItemTwice_IsIdempotent()
        {
            var item = new StubItem(Guid.NewGuid());

            _repository.Add(item);
            _repository.Add(item);

            Assert.AreEqual(1, _repository.GetAll().Count);
        }

        [Test]
        public void Add_DuplicateIdDifferentItem_Throws()
        {
            var id = Guid.NewGuid();
            _repository.Add(new StubItem(id));

            Assert.Throws<ArgumentException>(() => _repository.Add(new StubItem(id)));
        }

        [Test]
        public void Add_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
        }

        [Test]
        public void Remove_UnknownItem_IsNoOp()
        {
            Assert.DoesNotThrow(() => _repository.Remove(new StubItem(Guid.NewGuid())));
        }

        [Test]
        public void Remove_Null_IsNoOp()
        {
            Assert.DoesNotThrow(() => _repository.Remove(null));
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            _repository.Add(new StubItem(Guid.NewGuid()));
            _repository.Add(new StubItem(Guid.NewGuid()));

            _repository.Clear();

            Assert.AreEqual(0, _repository.GetAll().Count);
        }

        private sealed class StubItem : IRepositoryEntity
        {
            public StubItem(Guid id) => Id = id;

            public Guid Id { get; }
        }
    }
}
