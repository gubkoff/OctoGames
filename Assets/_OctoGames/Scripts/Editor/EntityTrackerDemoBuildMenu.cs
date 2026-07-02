#if UNITY_EDITOR
using System.IO;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Entities.Demo;
using OctoGames.App.Features.HUD;
using OctoGames.App.Features.Popups.ClassicConfirmPopup;
using ClassicConfirmPopupView = OctoGames.App.Features.Popups.ClassicConfirmPopup.ClassicConfirmPopup;
using OctoGames.App.Features.Popups.EntityDetailPopup;
using EntityDetailPopupView = OctoGames.App.Features.Popups.EntityDetailPopup.EntityDetailPopup;
using OctoGames.Composition;
using OctoGames.Popups;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OctoGames.Editor
{
    public static class EntityTrackerDemoBuildMenu
    {
        private const string Root = "Assets/_OctoGames";
        private const string GameplayPrefabFolder = Root + "/Content/Prefabs/Gameplay";
        private const string UiPrefabFolder = Root + "/Content/Prefabs/UI";
        private const string ConfigFolder = Root + "/Content/Configs";
        private const string ScenePath = Root + "/Content/Scenes/EntityTrackerDemo.unity";
        private const string CatalogPath = ConfigFolder + "/PopupsCatalog.asset";

        [MenuItem("OctoGames/Build Entity Tracker Demo")]
        public static void Build()
        {
            EnsureFolders();
            var enemyPrefab = CreateEntityPrefab<EnemyEntity>("EnemyEntity", PrimitiveType.Capsule, Color.red);
            var interactablePrefab = CreateEntityPrefab<InteractableEntity>("InteractableEntity", PrimitiveType.Cube, Color.yellow);
            var storyActorPrefab = CreateEntityPrefab<StoryActorEntity>("StoryActorEntity", PrimitiveType.Cylinder, Color.cyan);

            var confirmPrefab = CreateConfirmPopupPrefab();
            var detailPrefab = CreateEntityDetailPopupPrefab();

            var catalog = CreateOrLoadCatalog();
            var initialState = CreateOrLoadInitialState();
            var entityCatalog = CreateOrLoadEntityCatalog(enemyPrefab, interactablePrefab, storyActorPrefab);

            UpdatePopupCatalog(catalog, confirmPrefab, detailPrefab);
            CreateDemoScene(catalog, initialState, entityCatalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Entity Tracker Demo assets built successfully.");
        }

        private static void EnsureFolders()
        {
            CreateFolder("Assets/_OctoGames/Content/Prefabs/Gameplay");
            CreateFolder(ConfigFolder);
        }

        private static void CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var folderName = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                CreateFolder(parent);

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static GameObject CreateEntityPrefab<T>(
            string name,
            PrimitiveType primitiveType,
            Color color) where T : GameplayEntityBase
        {
            var path = $"{GameplayPrefabFolder}/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                return existing;

            var temp = GameObject.CreatePrimitive(primitiveType);
            temp.name = name;
            temp.AddComponent<T>();

            var renderer = temp.GetComponent<Renderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                renderer.sharedMaterial = new Material(shader) { color = color };
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            Object.DestroyImmediate(temp);
            return prefab;
        }

        private static GameObject CreateConfirmPopupPrefab()
        {
            var path = $"{UiPrefabFolder}/ClassicConfirmPopup.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                return existing;

            var root = CreatePopupRoot<ClassicConfirmPopupView, ClassicConfirmPopupBinder>("ClassicConfirmPopup");
            var panel = CreatePanel(root.transform, "Panel", new Vector2(420f, 220f));

            CreateText(panel.transform, "TitleText", "Confirm", 22, TextAnchor.UpperCenter,
                new Vector2(0f, -20f), new Vector2(380f, 40f));
            CreateText(panel.transform, "BodyText", "Are you sure?", 16, TextAnchor.UpperLeft,
                new Vector2(0f, -70f), new Vector2(380f, 70f));

            var confirmButton = CreateButton(panel.transform, "ConfirmButton", "Confirm",
                new Vector2(-90f, -170f), new Vector2(150f, 36f));
            var cancelButton = CreateButton(panel.transform, "CancelButton", "Cancel",
                new Vector2(90f, -170f), new Vector2(150f, 36f));

            var popup = root.GetComponent<ClassicConfirmPopupView>();
            var serialized = new SerializedObject(popup);
            serialized.FindProperty("_titleText").objectReferenceValue = panel.transform.Find("TitleText").GetComponent<Text>();
            serialized.FindProperty("_bodyText").objectReferenceValue = panel.transform.Find("BodyText").GetComponent<Text>();
            serialized.FindProperty("_confirmButton").objectReferenceValue = confirmButton;
            serialized.FindProperty("_cancelButton").objectReferenceValue = cancelButton;
            serialized.FindProperty("_confirmButtonLabel").objectReferenceValue = confirmButton.transform.Find("Label").GetComponent<Text>();
            serialized.FindProperty("_cancelButtonLabel").objectReferenceValue = cancelButton.transform.Find("Label").GetComponent<Text>();
            serialized.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject CreateEntityDetailPopupPrefab()
        {
            var path = $"{UiPrefabFolder}/EntityDetailPopup.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                return existing;

            var root = CreatePopupRoot<EntityDetailPopupView, EntityDetailPopupBinder>("EntityDetailPopup");
            var panel = CreatePanel(root.transform, "Panel", new Vector2(460f, 360f));

            CreateText(panel.transform, "TitleText", "Entity", 22, TextAnchor.UpperCenter,
                new Vector2(0f, -20f), new Vector2(420f, 36f));
            CreateText(panel.transform, "TypeText", "Type:", 16, TextAnchor.UpperLeft,
                new Vector2(0f, -70f), new Vector2(420f, 28f));
            CreateText(panel.transform, "StateText", "State:", 16, TextAnchor.UpperLeft,
                new Vector2(0f, -105f), new Vector2(420f, 28f));

            var disableButton = CreateButton(panel.transform, "DisableButton", "Disable",
                new Vector2(0f, -150f), new Vector2(200f, 32f));
            var enableButton = CreateButton(panel.transform, "EnableButton", "Enable",
                new Vector2(0f, -190f), new Vector2(200f, 32f));
            var completeButton = CreateButton(panel.transform, "CompleteButton", "Complete",
                new Vector2(0f, -230f), new Vector2(200f, 32f));
            var deleteButton = CreateButton(panel.transform, "DeleteButton", "Delete",
                new Vector2(0f, -270f), new Vector2(200f, 32f));
            var closeButton = CreateButton(panel.transform, "CloseButton", "Close",
                new Vector2(0f, -315f), new Vector2(200f, 32f));

            var popup = root.GetComponent<EntityDetailPopupView>();
            var serialized = new SerializedObject(popup);
            serialized.FindProperty("_titleText").objectReferenceValue = panel.transform.Find("TitleText").GetComponent<Text>();
            serialized.FindProperty("_typeText").objectReferenceValue = panel.transform.Find("TypeText").GetComponent<Text>();
            serialized.FindProperty("_stateText").objectReferenceValue = panel.transform.Find("StateText").GetComponent<Text>();
            serialized.FindProperty("_disableButton").objectReferenceValue = disableButton;
            serialized.FindProperty("_enableButton").objectReferenceValue = enableButton;
            serialized.FindProperty("_completeButton").objectReferenceValue = completeButton;
            serialized.FindProperty("_deleteButton").objectReferenceValue = deleteButton;
            serialized.FindProperty("_closeButton").objectReferenceValue = closeButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject CreatePopupRoot<TView, TBinder>(string name)
            where TView : PopupBaseView
            where TBinder : PopupBinderBase<TView>
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup), typeof(FadePopupAnimation), typeof(TBinder), typeof(TView));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var canvasGroup = root.GetComponent<CanvasGroup>();
            var animation = root.GetComponent<FadePopupAnimation>();
            var popup = root.GetComponent<TView>();

            var serialized = new SerializedObject(popup);
            serialized.FindProperty("_canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("_animation").objectReferenceValue = animation;
            serialized.FindProperty("_closeOnDimmerClick").boolValue = true;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            var dimmer = new GameObject("Dimmer", typeof(RectTransform), typeof(Image), typeof(Button));
            dimmer.transform.SetParent(root.transform, false);
            var dimmerRect = dimmer.GetComponent<RectTransform>();
            dimmerRect.anchorMin = Vector2.zero;
            dimmerRect.anchorMax = Vector2.one;
            dimmerRect.offsetMin = Vector2.zero;
            dimmerRect.offsetMax = Vector2.zero;
            var dimmerImage = dimmer.GetComponent<Image>();
            dimmerImage.color = new Color(0f, 0f, 0f, 0.55f);

            serialized = new SerializedObject(popup);
            serialized.FindProperty("_dimmerButton").objectReferenceValue = dimmer.GetComponent<Button>();
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 size)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
            return panel;
        }

        private static Text CreateText(
            Transform parent,
            string name,
            string text,
            int fontSize,
            TextAnchor alignment,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var label = go.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = Color.white;
            label.text = text;
            return label;
        }

        private static Button CreateButton(
            Transform parent,
            string name,
            string label,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            go.GetComponent<Image>().color = new Color(0.25f, 0.45f, 0.85f, 1f);

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelGo.GetComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 14;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labelText.text = label;

            return go.GetComponent<Button>();
        }

        private static SOPopupProvider CreateOrLoadCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<SOPopupProvider>(CatalogPath);
            if (catalog != null)
                return catalog;

            catalog = ScriptableObject.CreateInstance<SOPopupProvider>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
            return catalog;
        }

        private static SOGameplayEntitiesInitialState CreateOrLoadInitialState()
        {
            var path = $"{ConfigFolder}/SOGameplayEntitiesInitialState.asset";
            var asset = AssetDatabase.LoadAssetAtPath<SOGameplayEntitiesInitialState>(path);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<SOGameplayEntitiesInitialState>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static SOGameplayEntityCatalog CreateOrLoadEntityCatalog(
            GameObject enemyPrefab,
            GameObject interactablePrefab,
            GameObject storyActorPrefab)
        {
            var path = $"{ConfigFolder}/SOGameplayEntityCatalog.asset";
            var asset = AssetDatabase.LoadAssetAtPath<SOGameplayEntityCatalog>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<SOGameplayEntityCatalog>();
                AssetDatabase.CreateAsset(asset, path);
            }

            var serialized = new SerializedObject(asset);
            var entries = serialized.FindProperty("_entries");
            entries.arraySize = 3;
            SetCatalogEntry(entries, 0, GameplayEntityType.Enemy, enemyPrefab.GetComponent<GameplayEntityBase>());
            SetCatalogEntry(entries, 1, GameplayEntityType.Interactable, interactablePrefab.GetComponent<GameplayEntityBase>());
            SetCatalogEntry(entries, 2, GameplayEntityType.StoryActor, storyActorPrefab.GetComponent<GameplayEntityBase>());
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static void SetCatalogEntry(
            SerializedProperty entries,
            int index,
            GameplayEntityType type,
            GameplayEntityBase prefab)
        {
            var entry = entries.GetArrayElementAtIndex(index);
            entry.FindPropertyRelative("Type").enumValueIndex = (int)type;
            entry.FindPropertyRelative("Prefab").objectReferenceValue = prefab;
        }

        private static void UpdatePopupCatalog(
            SOPopupProvider catalog,
            GameObject confirmPrefab,
            GameObject detailPrefab)
        {
            var settingsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{UiPrefabFolder}/SettingsPopup.prefab");
            var serialized = new SerializedObject(catalog);
            var items = serialized.FindProperty("_items");
            items.ClearArray();

            if (settingsPrefab != null)
                AddPopupItem(items, settingsPrefab);

            AddPopupItem(items, confirmPrefab);
            AddPopupItem(items, detailPrefab);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void AddPopupItem(SerializedProperty items, GameObject prefab)
        {
            var index = items.arraySize;
            items.InsertArrayElementAtIndex(index);
            var item = items.GetArrayElementAtIndex(index);
            item.FindPropertyRelative("_prefab").objectReferenceValue = prefab;
            item.FindPropertyRelative("_scope").enumValueIndex = 0;
            item.FindPropertyRelative("_defaultShowPolicy").enumValueIndex = 1;
        }

        private static void CreateDemoScene(
            SOPopupProvider catalog,
            SOGameplayEntitiesInitialState initialState,
            SOGameplayEntityCatalog entityCatalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2f, 1f, 2f);

            var entitiesRoot = new GameObject("EntitiesRoot").transform;

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var eventSystemGo = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            var popupRootGo = new GameObject("PopupRoot", typeof(RectTransform), typeof(PopupRoot));
            popupRootGo.transform.SetParent(canvasGo.transform, false);
            var popupRootRect = popupRootGo.GetComponent<RectTransform>();
            popupRootRect.anchorMin = Vector2.zero;
            popupRootRect.anchorMax = Vector2.one;
            popupRootRect.offsetMin = Vector2.zero;
            popupRootRect.offsetMax = Vector2.zero;
            var popupRoot = popupRootGo.GetComponent<PopupRoot>();
            var popupRootSerialized = new SerializedObject(popupRoot);
            popupRootSerialized.FindProperty("_container").objectReferenceValue = popupRootRect;
            popupRootSerialized.ApplyModifiedPropertiesWithoutUndo();

            var hudGo = new GameObject("BaseHUD", typeof(RectTransform), typeof(BaseHUD));
            hudGo.transform.SetParent(canvasGo.transform, false);
            var hudRect = hudGo.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0f, 1f);
            hudRect.anchorMax = new Vector2(0f, 1f);
            hudRect.pivot = new Vector2(0f, 1f);
            hudRect.anchoredPosition = new Vector2(20f, -20f);
            hudRect.sizeDelta = new Vector2(360f, 220f);

            var activeCount = CreateText(hudGo.transform, "ActiveCountText", "Active: 0", 18, TextAnchor.UpperLeft,
                new Vector2(0f, 0f), new Vector2(320f, 28f));
            var activeRect = activeCount.GetComponent<RectTransform>();
            activeRect.anchorMin = new Vector2(0f, 1f);
            activeRect.anchorMax = new Vector2(0f, 1f);
            activeRect.pivot = new Vector2(0f, 1f);
            activeRect.anchoredPosition = Vector2.zero;

            var addEnemy = CreateHudButton(hudGo.transform, "AddEnemyButton", "Add Enemy", new Vector2(0f, -40f));
            var addInteractable = CreateHudButton(hudGo.transform, "AddInteractableButton", "Add Interactable", new Vector2(0f, -80f));
            var addStoryActor = CreateHudButton(hudGo.transform, "AddStoryActorButton", "Add Story Actor", new Vector2(0f, -120f));
            var restart = CreateHudButton(hudGo.transform, "RestartButton", "Restart", new Vector2(0f, -160f));

            var hud = hudGo.GetComponent<BaseHUD>();
            var hudSerialized = new SerializedObject(hud);
            hudSerialized.FindProperty("_activeCountText").objectReferenceValue = activeCount;
            hudSerialized.FindProperty("_addEnemyButton").objectReferenceValue = addEnemy;
            hudSerialized.FindProperty("_addInteractableButton").objectReferenceValue = addInteractable;
            hudSerialized.FindProperty("_addStoryActorButton").objectReferenceValue = addStoryActor;
            hudSerialized.FindProperty("_restartButton").objectReferenceValue = restart;
            hudSerialized.ApplyModifiedPropertiesWithoutUndo();

            var camera = Camera.main;
            if (camera != null)
            {
                camera.transform.position = new Vector3(0f, 6f, -8f);
                camera.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
                camera.gameObject.AddComponent<EntityPickController>();
            }

            var scopeGo = new GameObject("EntityTrackerLifetimeScope", typeof(EntityTrackerLifetimeScope));
            var scope = scopeGo.GetComponent<EntityTrackerLifetimeScope>();
            var scopeSerialized = new SerializedObject(scope);
            SetObjectReference(scopeSerialized, "_popupProvider", catalog);
            SetObjectReference(scopeSerialized, "_popupRoot", popupRoot);
            SetObjectReference(scopeSerialized, "_initialState", initialState);
            SetObjectReference(scopeSerialized, "_entityCatalog", entityCatalog);
            SetObjectReference(scopeSerialized, "_entitiesRoot", entitiesRoot);
            var autoInject = scopeSerialized.FindProperty("autoInjectGameObjects");
            if (autoInject == null)
                throw new System.InvalidOperationException("EntityTrackerLifetimeScope: property 'autoInjectGameObjects' not found.");

            autoInject.arraySize = 2;
            autoInject.GetArrayElementAtIndex(0).objectReferenceValue = hudGo;
            autoInject.GetArrayElementAtIndex(1).objectReferenceValue = camera != null ? camera.gameObject : null;
            scopeSerialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorSceneManager.OpenScene(ScenePath);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        }

        private static Button CreateHudButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            var button = CreateButton(parent, name, label, anchoredPosition, new Vector2(220f, 32f));
            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            return button;
        }

        private static void SetObjectReference(SerializedObject serializedObject, string propertyName, Object value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                throw new System.InvalidOperationException(
                    $"Property '{propertyName}' not found on {serializedObject.targetObject.GetType().Name}.");
            }

            property.objectReferenceValue = value;
        }
    }
}
#endif
