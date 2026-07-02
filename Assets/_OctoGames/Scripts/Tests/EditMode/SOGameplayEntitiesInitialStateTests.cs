using NUnit.Framework;
using OctoGames.App.Features.Entities;
using UnityEngine;

namespace OctoGames.Tests
{
    public sealed class SOGameplayEntitiesInitialStateTests
    {
        [Test]
        public void Entities_ExposesConfiguredEntries()
        {
            var asset = ScriptableObject.CreateInstance<SOGameplayEntitiesInitialState>();
            var entities = asset.Entities;

            Assert.AreEqual(3, entities.Count);
            Assert.AreEqual(GameplayEntityType.Enemy, entities[0].Type);
            Assert.AreEqual(GameplayEntityState.Active, entities[0].State);

            Object.DestroyImmediate(asset);
        }
    }
}
