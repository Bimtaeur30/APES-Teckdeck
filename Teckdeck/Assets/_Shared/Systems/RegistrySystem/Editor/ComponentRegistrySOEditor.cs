using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Shared.Systems.RegistrySystem.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace _Shared.Systems.RegistrySystem.Editor
{
    [CustomEditor(typeof(ComponentRegistrySO))]
    public class ComponentRegistrySOEditor : UnityEditor.Editor
    {
        enum ObjectFieldError { GuidIsNull, NoMeta, NoObject }
        
        [SerializeField] private VisualTreeAsset viewAsset = default;
        [SerializeField] private VisualTreeAsset rowAsset = default;

        private VisualElement _root;
        private ComponentRegistrySO _targetData;

        private ObjectField _baseScriptField;
        private Label _settingError;
        
        private ListView _entryList;
        private VisualElement _pendingRowContainer;
        private TextField _pendingKeyField;
        private Label _pendingTypeLabel;
        private Label _pendingError;
        private Button _addBtn;
        private Button _removeBtn;

        private VisualElement _inspectorPanel;
        private Label _inspectorTitle;
        private Button _inspectorCloseBtn;
        private VisualElement _inspectorBody;

        private ObjectField _enumFolderField;
        private Button _generateBtn;
        private Label _enumError;

        private readonly Dictionary<ComponentListItem, string> _errorMsgDict = new();
        private ComponentListItem _openedItem;
        private Type _pendingComponentType;
        
        private T Q<T>(string elemName) where T : VisualElement => _root.Q<T>(elemName);
        private bool SetErrorMsg(Label lbl, string msg = null)
        {
            bool hasError = !string.IsNullOrEmpty(msg);
            lbl.EnableInClassList("registry__error--visible", hasError);
            lbl.text = hasError ? msg : "";
            return hasError;
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            viewAsset.CloneTree(_root);
            _targetData = (ComponentRegistrySO)target;
            
            //필요한 에셋 대입하기
            _baseScriptField = Q<ObjectField>("base-script-field");
            _settingError = Q<Label>("settings-error");
            
            _entryList = Q<ListView>("entry-list");
            _pendingRowContainer = Q<VisualElement>("pending-row-container");
            _pendingKeyField = Q<TextField>("pending-key-field");
            _pendingTypeLabel = Q<Label>("pending-type-label");
            _pendingError = Q<Label>("pending-error");
            _addBtn = Q<Button>("add-btn");
            _removeBtn = Q<Button>("remove-btn");
            
            _inspectorPanel = Q<VisualElement>("inspector-panel");
            _inspectorTitle = Q<Label>("inspector-title");
            _inspectorCloseBtn = Q<Button>("inspector-close-btn");
            _inspectorBody = Q<VisualElement>("inspector-body");
            
            _enumFolderField = Q<ObjectField>("enum-folder-field");
            _generateBtn = Q<Button>("generate-btn");
            _enumError = Q<Label>("enum-error");

            _entryList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            
            //구독하기
            Undo.undoRedoPerformed -= UndoRedoPerformedHandle;
            Undo.undoRedoPerformed += UndoRedoPerformedHandle;
            
            _entryList.bindItem += HandleBindItem;
            _entryList.unbindItem += HandleUnbindItem;
            _addBtn.clicked += HandleAddBtn;
            _removeBtn.clicked += HandleRemoveBtn;
            _inspectorCloseBtn.clicked += HandleInspectorCloseBtn;
            _generateBtn.clicked += HandleGenerateBtn;
            _pendingKeyField.RegisterCallback<KeyDownEvent>(HandlePendingKeyFieldKeyDown, TrickleDown.TrickleDown);
            _pendingKeyField.RegisterValueChangedCallback(HandlePendingKeyFieldValueChange);

            _baseScriptField.RegisterValueChangedCallback(evt 
                => HandleObjectFieldChange(evt, true, _settingError, "Change BaseScript"));
            _enumFolderField.RegisterValueChangedCallback(evt 
                => HandleObjectFieldChange(evt, false, _enumError, "Change Enum Folder"));
            
            //값을 채우기
            FillValues();

            if (AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(_targetData.prefabFolderGuid)))
            {
                HashSet<string> prefabsInComponents = _targetData.components
                    .Select(c => AssetDatabase.GetAssetPath(c.component))
                    .ToHashSet();
                IEnumerable<string> prefabsInFolder = AssetDatabase.FindAssets("t:Prefab"
                        , new[] { AssetDatabase.GUIDToAssetPath(_targetData.prefabFolderGuid) })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => !string.IsNullOrEmpty(path) && !prefabsInComponents.Contains(path));
                foreach (string path in prefabsInFolder)
                    AssetDatabase.DeleteAsset(path);
            }
            Undo.ClearUndo(_targetData);
            
            return _root;
        }
        
        private void OnDisable() => Undo.undoRedoPerformed -= UndoRedoPerformedHandle;

        private void FillValues()
        {
            FillObjectField(_baseScriptField, _targetData.baseScriptGuid, _settingError);

            CheckKeysValid();
            FillEntryList();
            
            FillObjectField(_enumFolderField, _targetData.enumFolderGuid);

            GenerateBtnDirtyCheck();
        }

        //Undo했을 시에 값만 바뀌고 에디터는 바뀌지 않는 문제를 해결하기 위해 Undo시에 발행되는 이벤트에 에디터에 값을 채우는 메서드 구독
        private void UndoRedoPerformedHandle()
        {
            FillValues();
            
            if (_openedItem != null && _targetData.components.All(c => c != _openedItem))
                HandleInspectorCloseBtn();
        }

        private void HandleBindItem(VisualElement element, int index)
        {
            ComponentListItem item = _targetData.components[index];
            
            Label typeLabel = element.Q<Label>("type-label");
            typeLabel.userData = item;
            typeLabel.text = item.component == null ? null : item.component.name;
            typeLabel.RegisterCallback<ClickEvent>(HandleItemTypeLblClick);

            Label error = element.Q<Label>("error");
            _errorMsgDict.TryGetValue(_targetData.components[index], out string errorMsg);
            SetErrorMsg(error, errorMsg);
            
            TextField keyField = element.Q<TextField>("key-field");
            keyField.userData = index;
            keyField.SetValueWithoutNotify(item.enumKeyName);
            keyField.RegisterValueChangedCallback(HandleKeyFieldValueChange);
            
            //인스펙터를 연 Item이라면 파란 줄 표시
            VisualElement row = element.Q<VisualElement>("row");
            row.EnableInClassList("registry-row--open", item == _openedItem);
        }

        private void HandleUnbindItem(VisualElement element, int index)
        {
            Label typeLabel = element.Q<Label>("type-label");
            typeLabel.UnregisterCallback<ClickEvent>(HandleItemTypeLblClick);
            
            TextField keyField = element.Q<TextField>("key-field");
            keyField.UnregisterValueChangedCallback(HandleKeyFieldValueChange);
        }

        private void HandleKeyFieldValueChange(ChangeEvent<string> evt)
        {
            Undo.RecordObject(_targetData, "Change Key Field");
            
            TextField field = evt.currentTarget as TextField;
            int idx = (int)field!.userData;
            _targetData.components[idx].enumKeyName = evt.newValue;

            CheckKeysValid();
            GenerateBtnDirtyCheck();
            _entryList.RefreshItems();
            EditorUtility.SetDirty(_targetData);
        }

        private void HandleItemTypeLblClick(ClickEvent evt)
        {
            ComponentListItem item = (ComponentListItem)((Label)evt.currentTarget).userData;
            OnComponentItemFocus(item);
        }

        private void OnComponentItemFocus(ComponentListItem item)
        {
            bool shouldOpen = item != _openedItem;
            _openedItem = shouldOpen ? item : null;
            string openClass = "registry__inspector--open";

            _inspectorPanel.EnableInClassList(openClass, shouldOpen);

            _inspectorBody.Clear();
            if (shouldOpen == false)
                return;

            if (_openedItem.component == null)
            {
                _inspectorPanel.RemoveFromClassList(openClass);
                CheckKeysValid();
                return;
            }
            
            _inspectorTitle.text = _openedItem.component.name;
            _inspectorBody.Add(new InspectorElement(_openedItem.component));
            _entryList.RefreshItems();
        }

        private void HandleAddBtn()
        {
            bool isOpened = _pendingRowContainer.ClassListContains("registry__pending--active");
            if (isOpened)
                return;

            Type baseType = (_baseScriptField.value as MonoScript)?.GetClass();
            SetErrorMsg(_settingError, baseType == null ? "baseScript가 존재하지 않습니다." : null);
            
            new ComponentTypeDropdown(new AdvancedDropdownState(), baseType, HandleTypeSelected)
                .Show(_addBtn.worldBound);
        }

        private void HandleTypeSelected(Type selectedType)
        {
            if (selectedType == null)
                return;
            
            _pendingComponentType = selectedType;
            _pendingTypeLabel.text = selectedType.Name;
            _pendingRowContainer.AddToClassList("registry__pending--active");
            _pendingKeyField.value = selectedType.Name;
            _pendingKeyField.Focus();
        }

        private void ClosePending()
        {
            _pendingRowContainer.RemoveFromClassList("registry__pending--active");
            _pendingComponentType = null;
            SetErrorMsg(_pendingError);
        }

        private bool CheckPendingKeyValid()
        {
            string errorMsg = GetKeyError(_pendingKeyField.text, true);
            return !SetErrorMsg(_pendingError, errorMsg);
        }
        
        private void HandlePendingKeyFieldKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                evt.StopPropagation();
                ClosePending();
                return;
            }
                
            if (evt.keyCode is KeyCode.Return or KeyCode.KeypadEnter && CheckPendingKeyValid())
            {
                evt.StopPropagation();
                if (TryAddToComponentList())
                    ClosePending();
            }
        }

        private void HandlePendingKeyFieldValueChange(ChangeEvent<string> evt)
            => CheckPendingKeyValid();

        private bool TryAddToComponentList()
        {
            string enumName = _pendingKeyField.text;
            
            GameObject go = new GameObject(enumName);
            try
            {
                //GO 생성
                go.AddComponent(_pendingComponentType);
                bool isValidFolder =
                    AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(_targetData.prefabFolderGuid));
            
                //만약 처음 추가한다면 SO와 같은 레벨에 폴더 추가하기
                if (isValidFolder == false)
                {
                    string soPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_targetData));
                    soPath = soPath?.Replace('\\', '/');
                    string prefabFolderPath = soPath + '/' + $"{_targetData.name}'s prefabs";
                    _targetData.prefabFolderGuid = AssetDatabase.IsValidFolder(prefabFolderPath)
                        ? AssetDatabase.AssetPathToGUID(prefabFolderPath) 
                        : AssetDatabase.CreateFolder(soPath, $"{_targetData.name}'s prefabs");
                    
                    if (string.IsNullOrEmpty(_targetData.prefabFolderGuid))
                    {
                        SetErrorMsg(_pendingError, "폴더 생성에 실패하였습니다.");
                        return false;
                    }
                }
            
                string guidPath = AssetDatabase.GUIDToAssetPath(_targetData.prefabFolderGuid);
                string savePath = AssetDatabase.GenerateUniqueAssetPath($"{guidPath}/{go.name}.prefab");
                //프리팹으로 저장
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, savePath);
                if (prefab == null)
                {
                    SetErrorMsg(_pendingError, $"프리팹 생성에 실패했습니다. " +
                                               $"guidPath: {string.IsNullOrEmpty(guidPath)}" +
                                               $", savePath = {string.IsNullOrEmpty(savePath)}");
                    return false;
                }
            
                ComponentListItem item = new ComponentListItem
                {
                    enumValue = ++_targetData.lastEnumValue,
                    enumKeyName = enumName,
                    component = prefab.GetComponent(_pendingComponentType) as MonoBehaviour
                };
            
                _targetData.components.Add(item);
                OnComponentItemFocus(item);
                return true;
            }
            finally
            {
                DestroyImmediate(go);
                _entryList.RefreshItems();
                GenerateBtnDirtyCheck();
                EditorUtility.SetDirty(_targetData);
                Undo.ClearUndo(_targetData);
            }
        }

        private void HandleRemoveBtn()
        {
            var selectedItems = _entryList.selectedItems;
            if (!selectedItems.Any())
                return;
            
            Undo.RecordObject(_targetData, "Remove Component");
            
            _targetData.components.RemoveAll(c => selectedItems.Contains(c));
            CheckKeysValid();
            
            HandleInspectorCloseBtn();
            EditorUtility.SetDirty(_targetData);
        }

        private void HandleInspectorCloseBtn()
        {
            _inspectorPanel.RemoveFromClassList("registry__inspector--open");
            _inspectorBody.Clear();
            _openedItem = null;
            _entryList.RefreshItems();
            GenerateBtnDirtyCheck();
        }

        private void HandleGenerateBtn()
        {
            if (string.IsNullOrEmpty(_targetData.enumName) || string.IsNullOrEmpty(_targetData.enumFolderGuid))
            {
                SetErrorMsg(_enumError, "폴더명 또는 부모 폴더가 존재하지 않습니다.");
                return;
            }
            if (_targetData.components.Count == 0)
            {
                SetErrorMsg(_enumError, "컴포넌트의 수가 충분하지 않습니다(1개 이상)");
                return;
            }
            if (CheckKeysValid() == false)
            {
                SetErrorMsg(_enumError, "잘못된 키가 존재합니다");
                return;
            }
            string identifierError = GetIdentifierError(_targetData.enumName, "EnumName");
            if (SetErrorMsg(_enumError, identifierError))
                return;

            
            string forderPath = AssetDatabase.GUIDToAssetPath(_targetData.enumFolderGuid);
            if (string.IsNullOrEmpty(forderPath) || !AssetDatabase.IsValidFolder(forderPath))
            {
                SetErrorMsg(_enumError, "존재하지 않는 폴더입니다");
                return;
            }

            string enumString = string.Join(",", _targetData.components.Select(c 
                => $"{c.enumKeyName} = {c.enumValue}"));

            string nameSpace = forderPath;
            if (nameSpace.StartsWith("Assets/"))
                nameSpace = nameSpace.Substring("Assets/".Length);
            
            nameSpace = string.Join('.', nameSpace.Split('/')
                .ToList()
                .Where(str => !_targetData.skipNamespaces.Contains(str)));
            if (string.IsNullOrEmpty(nameSpace))
                nameSpace = "None";

            string code = string.Format(CodeFormat.EnumFormat, nameSpace, _targetData.enumName, enumString);
            File.WriteAllText($"{forderPath}/{_targetData.enumName}.cs", code);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); //이걸 해줘야 컴파일이 새로 들어간다.
            
            _targetData.lastEnums = GetComponentEnum().ToList();
            GenerateBtnDirtyCheck();
            EditorUtility.SetDirty(_targetData);
        }
        
        private void HandleObjectFieldChange(ChangeEvent<Object> evt
            , bool isBaseScript, Label errorLbl = null, string undoName = null)
        {
            Undo.RecordObject(_targetData, undoName);

            string errorMsg = null;
            
            if (evt.newValue == null)
                errorMsg = GetObjectFieldErrorMsg(ObjectFieldError.NoObject);
            
            string assetPath = AssetDatabase.GetAssetPath(evt.newValue);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                errorMsg = string.IsNullOrEmpty(errorMsg) ? GetObjectFieldErrorMsg(ObjectFieldError.NoMeta) : errorMsg;
  
            if (isBaseScript) _targetData.baseScriptGuid = guid;
            else _targetData.enumFolderGuid = guid;
            EditorUtility.SetDirty(_targetData);
            SetErrorMsg(errorLbl, errorMsg);
        }

        private void FillObjectField(ObjectField field, string guid, Label errorMsgLbl = null)
        {
            string errorMsg = null;
            
            if (string.IsNullOrEmpty(guid))
                errorMsg = GetObjectFieldErrorMsg(ObjectFieldError.GuidIsNull);
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
                errorMsg = string.IsNullOrEmpty(errorMsg) ? GetObjectFieldErrorMsg(ObjectFieldError.NoMeta) : errorMsg;
            
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null)
                errorMsg = string.IsNullOrEmpty(errorMsg) ? GetObjectFieldErrorMsg(ObjectFieldError.NoObject) : errorMsg;
            
            //Undo를 한 후에 이 메서드가 호출되어 value가 바뀌면 다시 So에 값이 할당되는데, 그럼 Redo가 지워지기 때문에 WithoutNotify
            field.SetValueWithoutNotify(asset);

            if (errorMsgLbl != null)
                SetErrorMsg(errorMsgLbl, errorMsg);
        }

        private string GetObjectFieldErrorMsg(ObjectFieldError error) => error switch
        {
            ObjectFieldError.GuidIsNull => "GUID가 존재하지 않습니다",
            ObjectFieldError.NoMeta => "GUID에 해당하는 .meta가 존재하지 않습니다",
            ObjectFieldError.NoObject => "Object가 존재하지 않습니다",
            _ => ""
        };

        private void FillEntryList()
        {
            if (_entryList == null)
                return;
            
            //모든 요소를 돌면서 키가 중복되어 있으면 아래쪽에 있는 요소한테 경고 붙이기.
            //프리팹을 검색할 수 없다면 해당 요소를 지우고 Ctrl Z를 못하게 기록 지우기.
            HashSet<int> enumValues = new HashSet<int>();
            HashSet<MonoBehaviour> components = new HashSet<MonoBehaviour>();

            bool Predicator(ComponentListItem c) =>
                !enumValues.Add(c.enumValue)
                || !c.component
                || !components.Add(c.component);

            int removedCount = _targetData.components.RemoveAll(Predicator);
            _entryList.makeItem = () => rowAsset.CloneTree();
            _entryList.itemsSource = _targetData.components;
            _entryList.RefreshItems();

            if (removedCount > 0)
            {
                EditorUtility.SetDirty(_targetData);
                Undo.ClearUndo(_targetData);
            }
        }

        private string GetKeyError(string key, bool newKey)
        {
            string errorMsg = GetIdentifierError(key, "Key");
            if (!string.IsNullOrEmpty(errorMsg))
                return errorMsg;
            
            if (newKey == false)
                return null;

            HashSet<string> enumNames = _targetData.components
                .Select(c => c.enumKeyName)
                .ToHashSet();
            if (!enumNames.Add(key))
                return "중복되었습니다";
            
            return null;
        }
        
        private string GetIdentifierError(string identifier, string identifierName)
        {
            if (string.IsNullOrEmpty(identifier))
                return $"{identifierName}: 한 글자 이상이여야 합니다";
            for (int i = 0; i < identifier.Length; i++)
            {
                if (i == 0 && !char.IsLetter(identifier[i]) && identifier[i] != '_')
                    return $"{identifierName}: 식별자로 사용할 수 없습니다";
                if (!char.IsLetter(identifier[i]) && identifier[i] != '_' && !char.IsDigit(identifier[i]))
                    return $"{identifierName}: 식별자로 사용할 수 없습니다";
            }
        
            return null;
        }
        
        private bool CheckKeysValid()
        {
            _errorMsgDict.Clear();
            bool successed = true;
            HashSet<string> enumNames = new HashSet<string>();
            
            foreach (ComponentListItem compoItem in _targetData.components)
            {
                var errorMsg = GetKeyError(compoItem.enumKeyName, false);
                if (!enumNames.Add(compoItem.enumKeyName))
                    errorMsg = "키가 중복되었습니다";
                
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    successed = false;
                    _errorMsgDict[compoItem] = errorMsg;
                }
            }

            return successed;
        }

        //원래 저장된 enumName들과 컴포넌트들의 enumName이 일치하지 않는다면 Generate를 통해 enum을 만들어야 하기 때문에 버튼을 강조
        private void GenerateBtnDirtyCheck()
        {
            HashSet<string> componentsEnum = GetComponentEnum();
            HashSet<string> realEnum = _targetData.lastEnums.ToHashSet();
            
            bool isDirty = !componentsEnum.SetEquals(realEnum);
            _generateBtn.EnableInClassList("registry__generate--dirty", isDirty);
        }

        private HashSet<string> GetComponentEnum()
            => _targetData.components
                .Select(c => $"{c.enumValue} = {c.enumKeyName}")
                .ToHashSet();
    }
}