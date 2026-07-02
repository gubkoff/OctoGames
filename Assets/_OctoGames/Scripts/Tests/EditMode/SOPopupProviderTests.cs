using System;
using System.Collections.Generic;
using NUnit.Framework;
using OctoGames.Popups;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace OctoGames.Tests
{
    public sealed class SOPopupProviderTests
    {
        private GameObject _prefab;
        private IPopupProvider _provider;
        private SOPopupProvider _providerAsset;

        [SetUp]
        public void SetUp()
        {
            _prefab = new GameObject("TestPopupPrefab");
            _prefab.AddComponent<TestPopup>();
            _providerAsset = ScriptableObject.CreateInstance<SOPopupProvider>();
            _provider = _providerAsset;
            AddEntry(_prefab);
        }

        [TearDown]
        public void TearDown()
        {
            if (_providerAsset != null)
                Object.DestroyImmediate(_providerAsset);

            if (_prefab != null)
                Object.DestroyImmediate(_prefab);
        }

        [Test]
        public void GetPrefab_ReturnsRegisteredPrefab()
        {
            var result = _provider.GetPrefab<TestPopup>();

            Assert.AreEqual(_prefab, result);
        }

        [Test]
        public void GetPrefab_Throws_WhenMissing()
        {
            Assert.Throws<KeyNotFoundException>(() => _provider.GetPrefab<MissingPopup>());
        }

        [Test]
        public void GetPrefab_Throws_WhenPrefabHasNoView()
        {
            var prefabWithoutView = new GameObject("NoViewPrefab");
            var provider = ScriptableObject.CreateInstance<SOPopupProvider>();

            LogAssert.Expect(
                LogType.Error,
                "Prefab 'NoViewPrefab' in provider '' is missing PopupBaseView component.");

            AddEntry(provider, prefabWithoutView);

            Assert.Throws<KeyNotFoundException>(() => provider.GetPrefab<TestPopup>());

            Object.DestroyImmediate(provider);
            Object.DestroyImmediate(prefabWithoutView);
        }

        [Test]
        public void GetShowPolicy_ReturnsConfiguredPolicy()
        {
            var policy = _provider.GetShowPolicy(typeof(TestPopup));

            Assert.AreEqual(PopupShowPolicy.Queue, policy);
        }

        [Test]
        public void GetShowPolicy_Throws_WhenMissing()
        {
            Assert.Throws<KeyNotFoundException>(() => _provider.GetShowPolicy(typeof(MissingPopup)));
        }

        private void AddEntry(GameObject prefab) => AddEntry(_providerAsset, prefab);

        private static void AddEntry(SOPopupProvider provider, GameObject prefab)
        {
            var serializedObject = new SerializedObject(provider);
            var entries = serializedObject.FindProperty("_items");
            var index = entries.arraySize;
            entries.InsertArrayElementAtIndex(index);
            entries.GetArrayElementAtIndex(index).FindPropertyRelative("_prefab").objectReferenceValue = prefab;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private sealed class TestPopup : PopupBaseView
        {
        }

        private sealed class MissingPopup : PopupBaseView
        {
        }
    }
}
