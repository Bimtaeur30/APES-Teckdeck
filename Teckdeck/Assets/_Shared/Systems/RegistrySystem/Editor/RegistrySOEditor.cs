using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using _Shared.Systems.RegistrySystem.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace _Shared.Systems.RegistrySystem.Editor
{
    [CustomEditor(typeof(RegistrySO))]
    public class RegistrySOEditor : UnityEditor.Editor
    {
        enum ObjectFieldError { GuidIsNull, NoMeta, NoObject }

        private const string MissingTypeMsg = "타입을 찾을 수 없습니다. 클래스 이름을 바꿨다면 [MovedFrom]으로 이전 이름을 알려주세요";
        private const string NoInstanceMsg = "인스턴스가 없습니다. 인스펙터를 다시 열면 생성을 다시 시도합니다";

        [SerializeField] private VisualTreeAsset viewAsset = default;
        [SerializeField] private VisualTreeAsset rowAsset = default;

        private VisualElement _root;
        private RegistrySO _targetData;

        private TextField _baseTypeField;
        private Button _baseTypeBtn;
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

        private readonly Dictionary<RegistryEntry, string> _errorMsgDict = new();
        private RegistryEntry _openedItem;
        private Type _pendingItemType;
        
        private T Q<T>(string elemName) where T : VisualElement => _root.Q<T>(elemName);
        private bool SetErrorMsg(Label lbl, string msg = null)
        {
            bool hasError = !string.IsNullOrEmpty(msg);
            lbl.EnableInClassList("registry__error--visible", hasError);
            lbl.text = hasError ? msg : "";
            return hasError;
        }
        private void SetWarningMsg(Label lbl, string msg = null)
        {
            bool hasWarning = !string.IsNullOrEmpty(msg);
            lbl.EnableInClassList("registry__warning--visible", hasWarning);
            lbl.text = hasWarning ? msg : "";
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            viewAsset.CloneTree(_root);
            _targetData = (RegistrySO)target;
            
            //필요한 element 대입하기
            _baseTypeField = Q<TextField>("base-type-field");
            _baseTypeBtn = Q<Button>("base-type-btn");
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
            
            _baseTypeBtn.clicked += HandleBaseTypeBtn;
            _baseTypeField.RegisterCallback<ClickEvent>(_ => HandleBaseTypeBtn());
            _enumFolderField.RegisterValueChangedCallback(HandleEnumFolderObjectFieldChange);
            
            _entryList.bindItem += HandleBindItem;
            _entryList.unbindItem += HandleUnbindItem;
            _addBtn.clicked += HandleAddBtn;
            _removeBtn.clicked += HandleRemoveBtn;
            
            _pendingKeyField.RegisterCallback<KeyDownEvent>(HandlePendingKeyFieldKeyDown, TrickleDown.TrickleDown);
            _pendingKeyField.RegisterValueChangedCallback(HandlePendingKeyFieldValueChange);
            
            _inspectorCloseBtn.clicked += HandleInspectorCloseBtn;
            
            _generateBtn.clicked += HandleGenerateBtn;

            SyncEntryTypes();

            //값을 채우기
            FillValues();
            
            return _root;
        }
        
        private void OnDisable() => Undo.undoRedoPerformed -= UndoRedoPerformedHandle;

        private void FillValues()
        {
            FillBaseTypeField();

            CheckKeysValid();
            FillEntryList();
            
            FillObjectField(_enumFolderField, _targetData.enumFolderGuid);

            GenerateBtnDirtyCheck();
        }

        //Undo했을 시에 값만 바뀌고 에디터는 바뀌지 않는 문제를 해결하기 위해 Undo시에 발행되는 이벤트에 에디터에 값을 채우는 메서드 구독
        private void UndoRedoPerformedHandle()
        {
            FillValues();
            
            if (_openedItem != null && _targetData.entries.All(c => c != _openedItem))
                HandleInspectorCloseBtn();
        }

        private void HandleBindItem(VisualElement element, int index)
        {
            RegistryEntry item = _targetData.entries[index];
            Type itemType = GetEntryType(item);

            Label typeLabel = element.Q<Label>("type-label");
            typeLabel.userData = item;
            typeLabel.text = GetEntryTypeDisplayName(item, itemType);
            typeLabel.RegisterCallback<ClickEvent>(HandleItemTypeLblClick);

            Label itemError = element.Q<Label>("error");
            _errorMsgDict.TryGetValue(_targetData.entries[index], out string errorMsg);
            if (itemType == null && string.IsNullOrEmpty(errorMsg))
                errorMsg = MissingTypeMsg;
            SetErrorMsg(itemError, errorMsg);

            Label itemWarning = element.Q<Label>("warning");
            SetWarningMsg(itemWarning, itemType == null ? null : RegistryItemValidator.GetWarningMsg(itemType));
            
            TextField keyField = element.Q<TextField>("key-field");
            keyField.userData = item;
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
            
            TextField field = (TextField)evt.currentTarget;
            RegistryEntry item = (RegistryEntry)field.userData;
            item.enumKeyName = evt.newValue;

            CheckKeysValid();
            GenerateBtnDirtyCheck();
            _entryList.RefreshItems();
        }

        private void HandleItemTypeLblClick(ClickEvent evt)
        {
            RegistryEntry item = (RegistryEntry)((Label)evt.currentTarget).userData;
            OnEntryFocus(item);
        }

        private void OnEntryFocus(RegistryEntry item)
        {
            bool shouldOpen = item != _openedItem;
            _openedItem = shouldOpen ? item : null;
            string openClass = "registry__inspector--open";

            _inspectorPanel.EnableInClassList(openClass, shouldOpen);
            _entryList.RefreshItems();

            _inspectorBody.Clear();
            if (shouldOpen == false)
                return;
            
            if (_openedItem.registryItem == null)
            {
                Type itemType = GetEntryType(_openedItem);
                _inspectorTitle.text = GetEntryTypeDisplayName(_openedItem, itemType);
                string msg = itemType == null
                    ? MissingTypeMsg
                    : RegistryItemValidator.GetWarningMsg(itemType) ?? NoInstanceMsg;
                _inspectorBody.Add(new Label(msg));
                return;
            }

            _inspectorTitle.text = _openedItem.registryItem.GetType().Name;
            int idx = _targetData.entries.IndexOf(item);
            serializedObject.Update();
            SerializedProperty itemProp = serializedObject
                .FindProperty("entries")
                .GetArrayElementAtIndex(idx)
                .FindPropertyRelative("registryItem");
            
            var field = new PropertyField(itemProp);
            field.Bind(serializedObject);
            _inspectorBody.Add(field);
        }

        private void HandleAddBtn()
        {
            bool isOpened = _pendingRowContainer.ClassListContains("registry__pending--active");
            if (isOpened)
                return;

            Type baseType = GetBaseType();

            if (baseType == null || !IsBaseTypeSelectable(baseType))
                return;

            new RegistryTypeDropdown(new AdvancedDropdownState(), baseType, HandleTypeSelected)
                .Show(_addBtn.worldBound);
        }

        private void HandleTypeSelected(Type selectedType)
        {
            if (selectedType == null)
                return;
            
            _pendingItemType = selectedType;
            _pendingTypeLabel.text = selectedType.Name;
            _pendingRowContainer.AddToClassList("registry__pending--active");
            _pendingKeyField.value = selectedType.Name;
            _pendingKeyField.Focus();
        }

        private void ClosePending()
        {
            _pendingRowContainer.RemoveFromClassList("registry__pending--active");
            _pendingItemType = null;
            SetErrorMsg(_pendingError);
        }

        private bool CheckPendingKeyValid()
        {
            string errorMsg = GetKeyError(_pendingKeyField.text, false);
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = GetIdentifierError(_pendingKeyField.text, "Pending");
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
                if (TryAddEntry())
                    ClosePending();
            }
        }

        private void HandlePendingKeyFieldValueChange(ChangeEvent<string> evt)
            => CheckPendingKeyValid();

        private bool TryAddEntry()
        {
            string enumName = _pendingKeyField.text;
            if (!typeof(IRegistryItem).IsAssignableFrom(_pendingItemType))
            {
                SetErrorMsg(_pendingError, "IRegistryItem을 구현한 타입이 아닙니다");
                return false;
            }

            //[Serializable]이 없으면 SerializeReference가 저장하지 못하므로 null로 두고 typeName만 남긴다.
            IRegistryItem registryItem = null;
            if (_pendingItemType.IsSerializable)
            {
                try
                {
                    registryItem = CreateRegistryItem(_pendingItemType);
                }
                catch (Exception e)
                {
                    SetErrorMsg(_pendingError, $"인스턴스를 생성할 수 없습니다: {(e.InnerException ?? e).Message}");
                    return false;
                }
            }

            Undo.RecordObject(_targetData, "Add Registry Entry");

            RegistryEntry item = new RegistryEntry
            {
                enumValue = ++_targetData.lastEnumValue,
                enumKeyName = enumName,
                typeName = RegistryItemValidator.GetTypeName(_pendingItemType),
                registryItem = registryItem
            };
            
            _targetData.entries.Add(item);
            OnEntryFocus(item);
            GenerateBtnDirtyCheck();

            return true;
        }

        //기본 생성자(private 포함)가 있으면 필드 초기값이 적용되도록 생성자로 만들고, 없으면 생성자 없이 만든다.
        private static IRegistryItem CreateRegistryItem(Type type)
        {
            ConstructorInfo defaultCtor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            object instance = defaultCtor != null
                ? defaultCtor.Invoke(null)
                : RuntimeHelpers.GetUninitializedObject(type);
            return instance as IRegistryItem;
        }

        //인스펙터를 열 때 한 번. 비어 있거나 낡은 typeName(예: [MovedFrom]으로 이름 변경)을 실제 객체 기준으로 맞추고,
        //null로 저장된 항목 중 [Serializable]을 붙여 고친 타입은 인스턴스를 다시 만든다.
        private void SyncEntryTypes()
        {
            bool isChanged = false;

            foreach (RegistryEntry entry in _targetData.entries)
            {
                if (entry.registryItem != null)
                {
                    string typeName = RegistryItemValidator.GetTypeName(entry.registryItem.GetType());
                    if (entry.typeName != typeName)
                    {
                        entry.typeName = typeName;
                        isChanged = true;
                    }
                    continue;
                }

                Type type = RegistryItemValidator.ResolveType(entry.typeName);
                if (type == null || !type.IsSerializable)
                    continue;

                try
                {
                    entry.registryItem = CreateRegistryItem(type);
                    isChanged = true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Registry] {entry.enumKeyName}: 인스턴스를 다시 만들 수 없습니다: {(e.InnerException ?? e).Message}", _targetData);
                }
            }

            if (isChanged)
                EditorUtility.SetDirty(_targetData);
        }

        private static Type GetEntryType(RegistryEntry entry)
            => entry.registryItem != null
                ? entry.registryItem.GetType()
                : RegistryItemValidator.ResolveType(entry.typeName);

        //타입을 못 찾아도 저장된 이름이 있으면 그 이름(네임스페이스·어셈블리 제외)을 보여준다.
        private static string GetEntryTypeDisplayName(RegistryEntry entry, Type type)
        {
            if (type != null)
                return type.Name;
            if (string.IsNullOrEmpty(entry.typeName))
                return "Missing";

            string fullName = entry.typeName.Split(',')[0];
            return fullName.Substring(fullName.LastIndexOfAny(new[] { '.', '+' }) + 1);
        }

        private void HandleRemoveBtn()
        {
            var selectedItems = _entryList.selectedItems.ToList();
            if (!selectedItems.Any())
                return;
            
            Undo.RecordObject(_targetData, "Remove Registry Entry");
            
            _targetData.entries.RemoveAll(c => selectedItems.Contains(c));
            CheckKeysValid();
            GenerateBtnDirtyCheck();
            HandleInspectorCloseBtn();
        }

        private void HandleInspectorCloseBtn()
        {
            _inspectorPanel.RemoveFromClassList("registry__inspector--open");
            _inspectorBody.Clear();
            _openedItem = null;
            _entryList.RefreshItems();
        }

        private void HandleGenerateBtn()
        {
            if (string.IsNullOrEmpty(_targetData.enumName) || string.IsNullOrEmpty(_targetData.enumFolderGuid))
            {
                SetErrorMsg(_enumError, "Enum의 이름 또는 저장할 폴더가 존재하지 않습니다.");
                return;
            }
            if (_targetData.entries.Count == 0)
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

            string enumString = string.Join(",", _targetData.entries.Select(c 
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
            
            _targetData.lastEnums = GetEntryEnum().ToList();
            GenerateBtnDirtyCheck();
            EditorUtility.SetDirty(_targetData);
        }
        
        private void HandleEnumFolderObjectFieldChange(ChangeEvent<Object> evt)
        {
            Undo.RecordObject(_targetData, "Change Enum Folder");

            string errorMsg = null;
            
            if (evt.newValue == null)
                errorMsg = GetObjectFieldErrorMsg(ObjectFieldError.NoObject);
            
            string guid = GetGuid(evt.newValue);
            if (string.IsNullOrEmpty(guid))
                errorMsg = string.IsNullOrEmpty(errorMsg) ? GetObjectFieldErrorMsg(ObjectFieldError.NoMeta) : errorMsg;
  
            _targetData.enumFolderGuid = guid;
            SetErrorMsg(_enumError, errorMsg);
        }

        private void HandleBaseTypeBtn()
        {
            new RegistryTypeDropdown(new AdvancedDropdownState(), typeof(IRegistryItem), HandleBaseTypeSelected
                    , "베이스 타입", IsBaseTypeSelectable)
                .Show(_baseTypeField.worldBound);
        }

        private void HandleBaseTypeSelected(Type selectedType)
        {
            if (selectedType == null)
                return;

            string typeName = RegistryItemValidator.GetTypeName(selectedType);
            if (typeName == _targetData.baseTypeName)
                return;

            int itemCount = _targetData.entries.Count;
            if (itemCount > 0 && !EditorUtility.DisplayDialog("베이스 타입 변경",
                    $"베이스 타입을 {selectedType.Name}(으)로 바꾸면 항목 {itemCount}개가 모두 삭제됩니다.\n" +
                    "Ctrl+Z로 되돌릴 수 있습니다. 계속할까요?", "변경", "취소"))
                return;

            Undo.RecordObject(_targetData, "Change Base Type");
            _targetData.baseTypeName = typeName;
            _targetData.entries.Clear();

            //이전 베이스 기준으로 열려 있던 입력 줄, 인스펙터, 키 오류, enum 버튼 상태를 정리
            ClosePending();
            HandleInspectorCloseBtn();
            GenerateBtnDirtyCheck();
            FillBaseTypeField();
        }

        //베이스 타입은 IRegistryItem 또는 IInitRegistryItem을 직접 구현한 타입만 고를 수 있다. 추상 클래스와 인터페이스도 된다.
        private static bool IsBaseTypeSelectable(Type type)
        {
            if (type == typeof(IRegistryItem) || type == typeof(IInitRegistryItem))
                return false;
            if (type.ContainsGenericParameters || typeof(Object).IsAssignableFrom(type))
                return false;

            return GetDirectInterfaces(type).Any(i => i == typeof(IRegistryItem) || i == typeof(IInitRegistryItem));
        }

        //부모 클래스나 다른 인터페이스를 거쳐 들어온 인터페이스를 빼고, 이 타입이 직접 선언한 인터페이스만 남긴다.
        private static IEnumerable<Type> GetDirectInterfaces(Type type)
        {
            //부모한테서 온 인터페이스는 빠지고, 인터페이스를 통해 따라온 인터페이스가 남는다.
            Type[] inherited = type.BaseType?.GetInterfaces() ?? Type.EmptyTypes;
            Type[] introduced = type.GetInterfaces().Except(inherited).ToArray();
            //other가 i를 구현하고 있다? 그럼 i를 내쫓기
            return introduced.Where(i => !introduced.Any(other => other != i && i.IsAssignableFrom(other)));
        }

        private Type GetBaseType() => RegistryItemValidator.ResolveType(_targetData.baseTypeName);

        private void FillBaseTypeField()
        {
            Type baseType = GetBaseType();

            string errorMsg = null;
            if (string.IsNullOrEmpty(_targetData.baseTypeName))
                errorMsg = "베이스 타입을 지정하세요";
            else if (baseType == null)
                errorMsg = $"타입을 찾을 수 없습니다: {_targetData.baseTypeName}";
            else if (!IsBaseTypeSelectable(baseType))
                errorMsg = $"IRegistryItem 또는 IInitRegistryItem을 직접 구현한 타입이어야 합니다: {baseType.FullName}";

            //Undo 후 이 메서드가 호출될 때 값이 다시 기록되지 않도록 WithoutNotify
            _baseTypeField.SetValueWithoutNotify(baseType == null ? _targetData.baseTypeName : baseType.Name);
            _baseTypeField.tooltip = baseType?.FullName ?? "";
            SetErrorMsg(_settingError, errorMsg);
        }

        private string GetGuid(Object newObj)
        {
            string assetPath = AssetDatabase.GetAssetPath(newObj);
            return AssetDatabase.AssetPathToGUID(assetPath);
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
            
            _entryList.makeItem = () => rowAsset.CloneTree();
            _entryList.itemsSource = _targetData.entries;
            _entryList.RefreshItems();
        }

        private string GetKeyError(string key, bool isInList)
        {
            string errorMsg = GetIdentifierError(key, "Key");
            if (!string.IsNullOrEmpty(errorMsg))
                return errorMsg;
            
            if (isInList)
                return null;

            HashSet<string> enumNames = _targetData.entries
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
            
            foreach (RegistryEntry entry in _targetData.entries)
            {
                var errorMsg = GetKeyError(entry.enumKeyName, true);
                if (!enumNames.Add(entry.enumKeyName))
                    errorMsg = "키가 중복되었습니다";
                
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    successed = false;
                    _errorMsgDict[entry] = errorMsg;
                }
            }

            return successed;
        }

        //원래 저장된 enumName들과 컴포넌트들의 enumName이 일치하지 않는다면 Generate를 통해 enum을 만들어야 하기 때문에 버튼을 강조
        private void GenerateBtnDirtyCheck()
        {
            HashSet<string> entriesEnum = GetEntryEnum();
            HashSet<string> realEnum = _targetData.lastEnums.ToHashSet();
            
            bool isDirty = !entriesEnum.SetEquals(realEnum);
            _generateBtn.EnableInClassList("registry__generate--dirty", isDirty);
        }

        private HashSet<string> GetEntryEnum()
            => _targetData.entries
                .Select(c => $"{c.enumValue} = {c.enumKeyName}")
                .ToHashSet();
    }
}