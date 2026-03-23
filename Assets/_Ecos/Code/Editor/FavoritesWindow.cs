#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gaton.Favorites
{
    public class FavoritesWindow : EditorWindow
    {
        const string EDITOR_PREFS_SUFIX = "_FavoritesWindow_Save_Data";


        #region Icons

        readonly static string[] starIcons = new[] { "d_Favorite Icon", "Favorite", "Favorite Icon", "d_Favorite", "sv_icon_dot", "sv_label_0" };
        readonly static string[] reorderIcons = new[] { "d_align_vertically_center", "d_align_vertically_center_active" };
        readonly static string[] removeIcons = new[] { "CrossIcon" };
        readonly static string[] dragIcon = new[] { "d_scenepicking_pickable-mixed_hover@2x" };
        readonly static string[] folderIcons = new[] { "Folder Icon", "d_Folder Icon" };
        //readonly static string[] starIcons = new[] { "d_Favorite Icon", "Favorite", "Favorite Icon", "d_Favorite", "sv_icon_dot", "sv_label_0" };

        #endregion
        #region Visuals

        const int tabButtonMaxHeight = 20;

        #endregion

        #region Custom Classes
        [Serializable]
        class FavoriteTab
        {
            public string name = "New Tab";
            public List<string> assetGUIDs = new List<string>();
        }

        [Serializable]
        class SaveData
        {
            public List<FavoriteTab> tabs = new List<FavoriteTab>();
            public int selectedTab = 0;
        }
        #endregion

        #region Member Variables
        SaveData data;
        string editorPrefsKey;

        VisualElement tabsBar;
        ListView listView;
        #endregion

        [MenuItem("Window/Favorites")]
        public static void ShowWindow()
        {
            var window = GetWindow<FavoritesWindow>();
            UpdateTitleContent(window);
            window.minSize = new Vector2(300, 250);
            window.Show();
        }

        private static void UpdateTitleContent(FavoritesWindow window)
        {
            window.titleContent = new GUIContent("Favorites", TryToFindTexture(starIcons), "Easily track your favorite objects!");
        }

        private void OnEnable()
        {
            UpdateTitleContent(this);

            // We can only access the product name after "OnEnable"
            editorPrefsKey = $"{Application.productName}{EDITOR_PREFS_SUFIX}";

            LoadData();
            ConstructUI();
            RebuildTabsUI();
            ShowTabContent(data.selectedTab);
        }

        private void OnDisable() => SaveDataToPrefs();

        #region Data Persistence
        private void LoadData()
        {
            var json = EditorPrefs.GetString(editorPrefsKey, string.Empty);

            if (string.IsNullOrEmpty(json))
            {
                data = new SaveData();
                data.tabs.Add(new FavoriteTab() { name = "Default" });
                data.selectedTab = 0;
                SaveDataToPrefs();
                return;
            }

            try
            {
                data = JsonUtility.FromJson<SaveData>(json);
                if (data == null || data.tabs == null || data.tabs.Count == 0)
                {
                    data = new SaveData();
                    data.tabs.Add(new FavoriteTab() { name = "Default" });
                }
            }
            catch
            {
                data = new SaveData();
                data.tabs.Add(new FavoriteTab() { name = "Default" });
            }
        }

        private void SaveDataToPrefs()
        {
            var json = JsonUtility.ToJson(data);
            EditorPrefs.SetString(editorPrefsKey, json);
        }
        #endregion

        #region UI Construction
        private void ConstructUI()
        {
            var root = rootVisualElement;
            root.Clear();
            root.style.flexDirection = FlexDirection.Column;

            // Tabs bar
            var tabsScroll = new ScrollView(ScrollViewMode.Horizontal)
            {
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                style =
            {
                height = 40,
                marginLeft = 5,
            }
            };


            tabsBar = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            tabsScroll.Add(tabsBar);
            root.Add(tabsScroll);

            root.Add(new VisualElement()
            {
                style = { maxHeight = 1, backgroundColor = Color.gray, flexGrow = 1, marginBottom = 3 }
            });

            // ListView area
            listView = new ListView
            {
                selectionType = SelectionType.None,
                reorderable = true,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                style = { flexGrow = 1 }
            };

            listView.makeItem = MakeListItem;
            listView.bindItem = BindListItem;
            listView.unbindItem = (_, __) => { };
            listView.itemsSource = new List<string>(); // replaced when switching tabs
            listView.itemsAdded += _ => SaveDataToPrefs();
            listView.itemsRemoved += _ => SaveDataToPrefs();
            listView.itemIndexChanged += (_, __) => SaveDataToPrefs();
            listView.makeNoneElement = MakeNoneElement;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            root.Add(listView);

            RegisterDragHandling(root);
        }

        private void RegisterDragHandling(VisualElement element)
        {
            element.pickingMode = PickingMode.Position;

            element.RegisterCallback<DragEnterEvent>(evt =>
            {
                if (!HasObjectSelected())
                {
                    return;
                }

                if (listView.worldBound.Contains(evt.mousePosition))
                {
                    HighlightList(true);
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.StopPropagation();
                }
            }, TrickleDown.TrickleDown);

            element.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                if (!HasObjectSelected())
                {
                    return;
                }

                if (listView.worldBound.Contains(evt.mousePosition))
                {
                    HighlightList(true);
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.StopPropagation();
                }
            }, TrickleDown.TrickleDown);

            element.RegisterCallback<DragLeaveEvent>(evt => { HighlightList(false); }, TrickleDown.TrickleDown);
            element.RegisterCallback<DragExitedEvent>(evt => { HighlightList(false); }, TrickleDown.TrickleDown);

            element.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (!HasObjectSelected())
                {
                    return;
                }

                if (listView.worldBound.Contains(evt.mousePosition))
                {
                    DragAndDrop.AcceptDrag();

                    var objectReferences = DragAndDrop.objectReferences;

                    if (objectReferences == null || objectReferences.Length == 0)
                    {
                        return;
                    }

                    int addedCount = 0;
                    int invalidCount = 0;

                    var currentTab = GetCurrentTab();

                    foreach (var obj in objectReferences)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);

                        if (string.IsNullOrEmpty(path))
                        {
                            invalidCount++;
                            continue;
                        }

                        string guid = AssetDatabase.AssetPathToGUID(path);
                        if (!string.IsNullOrEmpty(guid) && !currentTab.assetGUIDs.Contains(guid))
                        {
                            currentTab.assetGUIDs.Add(guid);
                            addedCount++;
                        }
                    }

                    HighlightList(false);
                    SaveDataToPrefs();
                    ShowTabContent(data.selectedTab);

                    evt.StopPropagation();
                }
            }, TrickleDown.TrickleDown);

            bool HasObjectSelected()
            {
                return DragAndDrop.objectReferences.Length > 0;
            }

            void HighlightList(bool highlight)
            {
                //TODO: make if dragging an element that is already on the list, highlight this element.
                if (highlight)
                {
                    listView.style.backgroundColor = new StyleColor(new Color(0.12f, 0.5f, 0.95f, 0.08f)); // blue
                                                                                                           //listView.style.borderLeftWidth = 1;
                                                                                                           //listView.style.borderRightWidth = 1;
                                                                                                           //listView.style.borderTopWidth = 1;
                                                                                                           //listView.style.borderBottomWidth = 1;
                                                                                                           //listView.style.borderLeftColor = new StyleColor(new Color(0.12f, 0.5f, 0.95f, 0.35f));
                                                                                                           //listView.style.borderRightColor = new StyleColor(new Color(0.12f, 0.5f, 0.95f, 0.35f));
                                                                                                           //listView.style.borderTopColor = new StyleColor(new Color(0.12f, 0.5f, 0.95f, 0.35f));
                                                                                                           //listView.style.borderBottomColor = new StyleColor(new Color(0.12f, 0.5f, 0.95f, 0.35f));
                }
                else
                {
                    listView.style.backgroundColor = StyleKeyword.Null;
                    listView.style.borderLeftWidth = StyleKeyword.Null;
                    listView.style.borderRightWidth = StyleKeyword.Null;
                    listView.style.borderTopWidth = StyleKeyword.Null;
                    listView.style.borderBottomWidth = StyleKeyword.Null;
                }
            }
        }

        private VisualElement MakeNoneElement()
        {
            VisualElement root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;
            root.style.marginTop = 10;
            root.style.marginLeft = 5;
            root.Add(new Label("Drag any asset here!") { style = { unityFontStyleAndWeight = FontStyle.Italic } });

            Image img = CreateImageFromIconName(dragIcon);
            img.style.alignSelf = Align.FlexStart;
            img.style.width = 16;
            img.style.height = 16;
            root.Add(img);

            return root;
        }

        VisualElement MakeListItem()
        {
            // Row container
            var row = new VisualElement
            {
                style =
            {
                flexDirection = FlexDirection.Row,
                alignItems = Align.FlexStart,
                alignContent = Align.FlexStart,
                justifyContent = Justify.FlexStart,
                paddingLeft = 4,
                paddingRight = 4,
                marginBottom = 2,
                minHeight = 22
            }
            };

            var reorderHandle = new VisualElement
            {
                name = "ReorderHandle",
                pickingMode = PickingMode.Ignore,
            };

            reorderHandle.style.width = 20;
            reorderHandle.style.height = 16;
            reorderHandle.style.marginRight = 6;
            reorderHandle.style.alignItems = Align.Center;
            reorderHandle.style.justifyContent = Justify.Center;

            var img = CreateImageFromIconName(reorderIcons);
            img.style.width = 12;
            img.style.height = 12;
            img.style.alignSelf = Align.Center;
            img.style.justifyContent = Justify.Center;

            reorderHandle.Add(img);

            row.Add(reorderHandle);

            var objField = new ObjectField { objectType = typeof(UnityEngine.Object), allowSceneObjects = false };
            objField.style.width = 200;
            objField.style.flexShrink = 1;
            objField.style.marginRight = 4;
            objField.Q<Label>().style.textOverflow = TextOverflow.Ellipsis;
            row.Add(objField);
            //objField.style.alignSelf = Align.FlexEnd;

            objField.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Show In Explorer", (action) => EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(objField.value)));
            }));

            var folderBtn = new Button(() =>
            {
                EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(objField.value));
            })
            {
                tooltip = "Show in Explorer"
            };

            folderBtn.style.marginLeft = 4;
            folderBtn.style.paddingLeft = 2;
            folderBtn.style.paddingRight = 2;
            folderBtn.style.width = 20;
            folderBtn.style.height = 20;
            folderBtn.style.alignSelf = Align.Center;

            var imgFolder = CreateImageFromIconName(folderIcons);
            folderBtn.Add(imgFolder);

            row.Add(folderBtn);
            Button removeBtn = CreateRemoveButton(row);

            row.Add(removeBtn);


            return row;
        }

        private Button CreateRemoveButton(VisualElement row)
        {
            Button removeBtn = new Button(() =>
            {
                var tab = GetCurrentTab();
                string guid = row.userData as string;

                if (tab != null)
                {
                    tab.assetGUIDs.Remove(guid);
                    SaveDataToPrefs();
                    ShowTabContent(data.selectedTab);
                }
            });

            removeBtn.tooltip = "Remove";

            removeBtn.style.marginLeft = 4;
            removeBtn.style.paddingLeft = 2;
            removeBtn.style.paddingRight = 2;
            removeBtn.style.width = 20;
            removeBtn.style.height = 20;
            removeBtn.style.alignSelf = Align.Center;

            var imgTrash = CreateImageFromIconName(removeIcons);
            removeBtn.Add(imgTrash);
            return removeBtn;
        }

        void BindListItem(VisualElement element, int index)
        {
            var curTab = GetCurrentTab();
            if (index < 0 || index >= curTab.assetGUIDs.Count) return;
            string guid = curTab.assetGUIDs[index];
            element.userData = guid;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            var objField = element.Q<ObjectField>();
            objField.SetValueWithoutNotify(asset);

            var reorderHandle = element.Q<VisualElement>("ReorderHandle");
            if (reorderHandle != null)
            {
                if (asset != null)
                {
                    reorderHandle.tooltip = $"Drag to reorder\n{asset.name}";
                }
            }
        }
        #endregion

        #region Tabs
        void RebuildTabsUI()
        {
            tabsBar.Clear();

            for (int i = 0; i < data.tabs.Count; i++)
            {
                // capture the index
                int tabIndex = i;
                var tab = data.tabs[tabIndex];

                // create container for tab so layout stays steady
                var tabContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, alignSelf = Align.Center } };
                tabContainer.style.marginRight = 4;

                // button displays name and receives clicks and context menu
                var btn = new Button(() => SelectTab(tabIndex)) { text = tab.name };
                btn.style.minWidth = 40;
                btn.style.maxHeight = tabButtonMaxHeight;
                btn.style.paddingLeft = 6;
                btn.style.paddingRight = 6;
                btn.style.alignSelf = Align.Center;

                if (tabIndex == data.selectedTab)
                    btn.style.backgroundColor = new Color(0.24f, 0.49f, 0.9f, 0.9f);

                // right click menu
                btn.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    // use the captured tabIndex for all menu actions/statuses
                    evt.menu.AppendAction("Rename", _ => BeginRenameTab(tabIndex));

                    evt.menu.AppendAction("Clear", (callback) => ClearCurrentTab());

                    evt.menu.AppendAction("Delete", _ => DeleteTab(tabIndex),
                        data.tabs.Count > 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                    evt.menu.AppendAction("Move Left", _ => MoveTabLeft(tabIndex),
                        tabIndex > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                    evt.menu.AppendAction("Move Right", _ => MoveTabRight(tabIndex),
                        tabIndex < data.tabs.Count - 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                }));

                // Prevent right-click from also triggering a normal click
                btn.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (evt.button == (int)MouseButton.RightMouse)
                    {
                        evt.StopPropagation();
                    }
                });

                tabContainer.Add(btn);
                tabsBar.Add(tabContainer);
            }

            // Add button appended AFTER the last tab (inside tabsBar)
            var addBtn = new Button(AddNewTab) { text = "+", tooltip = "Add Tab" };
            addBtn.style.minWidth = 14;
            addBtn.style.maxHeight = 14;
            addBtn.style.width = 14;
            addBtn.style.marginLeft = 6;
            addBtn.style.marginTop = 4;
            //addBtn.style.flexShrink = 0;
            addBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            addBtn.style.fontSize = 10;
            addBtn.style.alignSelf = Align.Center;
            tabsBar.Add(addBtn);
        }

        void MoveTabLeft(int index) => MoveTab(index, -1);
        void MoveTabRight(int index) => MoveTab(index, +1);

        void MoveTab(int sourceIndex, int delta)
        {
            if (sourceIndex < 0 || sourceIndex >= data.tabs.Count) return;

            // desired final index in the list (before modification)
            int destIndex = Mathf.Clamp(sourceIndex + delta, 0, data.tabs.Count - 1);
            if (destIndex == sourceIndex) return;

            var moving = data.tabs[sourceIndex];

            // remove the element first
            data.tabs.RemoveAt(sourceIndex);

            // insert at the desired final index (this is the correct index in the list AFTER removal)
            // because 'destIndex' was computed as the final desired position.
            data.tabs.Insert(destIndex, moving);

            // Update selectedTab to preserve user context.
            if (data.selectedTab == sourceIndex)
            {
                // If the moved tab was selected, it should stay selected at the new index.
                data.selectedTab = destIndex;
            }
            else if (sourceIndex < destIndex)
            {
                if (data.selectedTab > sourceIndex && data.selectedTab <= destIndex)
                    data.selectedTab--;
            }
            else
            {
                if (data.selectedTab >= destIndex && data.selectedTab < sourceIndex)
                    data.selectedTab++;
            }

            SaveDataToPrefs();
            RebuildTabsUI();
            ShowTabContent(data.selectedTab);
        }


        void SelectTab(int index)
        {
            data.selectedTab = Mathf.Clamp(index, 0, data.tabs.Count - 1);
            SaveDataToPrefs();
            RebuildTabsUI();
            ShowTabContent(data.selectedTab);
        }

        void AddNewTab()
        {
            data.tabs.Add(new FavoriteTab() { name = $"Tab {data.tabs.Count + 1}" });
            data.selectedTab = data.tabs.Count - 1;
            SaveDataToPrefs();
            RebuildTabsUI();
            ShowTabContent(data.selectedTab);
        }

        void DeleteTab(int index)
        {
            if (data.tabs.Count <= 1) return;

            if (data.tabs[index].assetGUIDs.Count <= 0 || EditorUtility.DisplayDialog("Delete Tab", $"Do you want to delete the '{data.tabs[index].name}' tab?", "Delete", "Cancel"))
            {
                data.tabs.RemoveAt(index);
                data.selectedTab = Mathf.Clamp(data.selectedTab, 0, data.tabs.Count - 1);
                SaveDataToPrefs();
                RebuildTabsUI();
                ShowTabContent(data.selectedTab);
            }
        }

        void BeginRenameTab(int index)
        {
            tabsBar.Clear();
            for (int i = 0; i < data.tabs.Count; i++)
            {
                if (i == index)
                {
                    var tf = new TextField { value = data.tabs[i].name };
                    tf.style.width = 160;
                    tf.RegisterCallback<KeyDownEvent>(evt =>
                    {
                        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                            FinishRenameTab(index, tf.value);
                        else if (evt.keyCode == KeyCode.Escape)
                            RebuildTabsUI();
                    });
                    tf.RegisterCallback<FocusOutEvent>(_ => FinishRenameTab(index, tf.value));
                    tabsBar.Add(tf);
                    tf.Focus();
                }
                else
                {
                    var b = new Button(() => SelectTab(i)) { text = data.tabs[i].name };
                    tabsBar.Add(b);
                }
            }

            // keep the add button available when renaming
            var addBtn = new Button(AddNewTab) { text = "+", tooltip = "Add Tab" };
            addBtn.style.minWidth = 28;
            addBtn.style.width = 28;
            addBtn.style.marginLeft = 6;
            addBtn.style.flexShrink = 0;
            addBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            tabsBar.Add(addBtn);
        }

        void FinishRenameTab(int index, string newName)
        {
            if (index >= 0 && index < data.tabs.Count && !string.IsNullOrWhiteSpace(newName))
                data.tabs[index].name = newName.Trim();
            SaveDataToPrefs();
            RebuildTabsUI();
        }

        void ClearCurrentTab()
        {
            var cur = GetCurrentTab();
            if (cur == null) return;
            if (EditorUtility.DisplayDialog("Clear Tab", $"Clear all favorites from '{cur.name}'?", "Yes", "No"))
            {
                cur.assetGUIDs.Clear();
                SaveDataToPrefs();
                ShowTabContent(data.selectedTab);
            }
        }
        #endregion

        #region ListView Content
        void ShowTabContent(int index)
        {
            var cur = GetCurrentTab();
            if (cur == null) return;

            listView.itemsSource = cur.assetGUIDs;
            listView.Rebuild();
        }

        FavoriteTab GetCurrentTab()
        {
            if (data == null || data.tabs.Count == 0) return null;
            int idx = Mathf.Clamp(data.selectedTab, 0, data.tabs.Count - 1);
            return data.tabs[idx];
        }
        #endregion

        static Image CreateImageFromIconName(string[] options)
        {
            var img = new Image
            {
                image = TryToFindTexture(options),
                scaleMode = ScaleMode.ScaleToFit,
            };

            return img;
        }

        static Texture TryToFindTexture(string[] options)
        {
            foreach (var name in options)
            {
                try
                {
                    var content = EditorGUIUtility.IconContent(name);
                    if (content != null && content.image != null)
                        return content.image;
                }
                catch { }
            }

            return EditorGUIUtility.whiteTexture;
        }


    }
}

#endif