using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Shared.Systems.InspectorSystem.Runtime;
using _Shared.Systems.RegistrySystem.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Shared.Systems.RegistrySystem.Editor
{
    [CustomEditor(typeof(RegistryRuntime))]
    public class RegistryRuntimeEditor : UnityEditor.Editor
    {
        private const string NotPlayingMsg = "플레이 중에만 사본이 만들어집니다";
        private const string NoEntryMsg = "만들어진 사본이 없습니다";
        private const string NoFieldMsg = "표시할 필드가 없습니다";

        private RegistryRuntime _targetRuntime;
        private VisualElement _entryContainer;
        private Button _applyAllBtn;

        public override VisualElement CreateInspectorGUI()
        {
            _targetRuntime = (RegistryRuntime)target;
            var root = new VisualElement();

            //플레이 중에 SO를 바꿔도 사본은 다시 만들어지지 않으므로 편집 모드에서만 바꿀 수 있다.
            var registrySOField = new PropertyField(serializedObject.FindProperty("registrySO"));
            registrySOField.SetEnabled(!EditorApplication.isPlaying);
            root.Add(registrySOField);

            _applyAllBtn = new Button(HandleApplyAllBtn)
            {
                text = "전체 적용",
                style =
                {
                    marginTop = 4
                }
            };
            root.Add(_applyAllBtn);

            _entryContainer = new VisualElement
            {
                style =
                {
                    marginTop = 4
                }
            };
            root.Add(_entryContainer);

            //다른 스크립트가 늦게 조회해 사본이 나중에 만들어지는 경우를 위해 개수가 바뀌면 다시 그린다.
            root.TrackPropertyValue(serializedObject.FindProperty("runtimeEntries.Array.size"), _ => FillEntries());

            FillEntries();
            return root;
        }

        private void FillEntries()
        {
            _entryContainer.Clear();
            serializedObject.Update();
            SerializedProperty entriesProp = serializedObject.FindProperty("runtimeEntries");

            bool hasEntry = EditorApplication.isPlaying && entriesProp.arraySize > 0;
            _applyAllBtn.style.display = hasEntry ? DisplayStyle.Flex : DisplayStyle.None;
            if (!hasEntry)
            {
                string msg = EditorApplication.isPlaying ? NoEntryMsg : NotPlayingMsg;
                _entryContainer.Add(new HelpBox(msg, HelpBoxMessageType.Info));
                return;
            }

            for (int i = 0; i < entriesProp.arraySize; i++)
                _entryContainer.Add(CreateEntryElement(entriesProp.GetArrayElementAtIndex(i), i));
        }

        private VisualElement CreateEntryElement(SerializedProperty entryProp, int index)
        {
            SerializedProperty itemProp = entryProp.FindPropertyRelative("registryItem");
            string keyName = entryProp.FindPropertyRelative("enumKeyName").stringValue;
            string typeName = itemProp.managedReferenceValue?.GetType().Name ?? "Missing";

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.FlexStart;

            //다시 그려도 펼침 상태가 유지되도록 고정 번호로 viewDataKey를 준다.
            var foldout = new Foldout
            {
                text = $"{keyName} ({typeName})",
                value = false,
                viewDataKey = $"registry-runtime-{entryProp.FindPropertyRelative("enumValue").intValue}",
                style =
                {
                    flexGrow = 1
                }
            };
            row.Add(foldout);

            var applyBtn = new Button(() => ApplyEntries(new[] { index })) { text = "적용" };
            row.Add(applyBtn);

            SerializedProperty end = itemProp.GetEndProperty();
            SerializedProperty child = itemProp.Copy();
            bool hasField = false;
            for (bool enterChildren = true;
                 child.NextVisible(enterChildren) && !SerializedProperty.EqualContents(child, end);
                 enterChildren = false)
            {
                var field = new PropertyField(child.Copy());
                field.Bind(serializedObject);
                foldout.Add(field);
                hasField = true;
            }

            if (!hasField)
                foldout.Add(new Label(NoFieldMsg));

            return row;
        }

        private void HandleApplyAllBtn()
        {
            serializedObject.Update();
            int count = serializedObject.FindProperty("runtimeEntries").arraySize;
            ApplyEntries(Enumerable.Range(0, count));
        }

        //한 번 누른 버튼의 변경은 Undo 한 번으로 되돌아간다.
        private void ApplyEntries(IEnumerable<int> indices)
        {
            RegistrySO registrySO = _targetRuntime.RegistrySO;
            if (registrySO == null)
                return;

            serializedObject.Update();
            SerializedProperty entriesProp = serializedObject.FindProperty("runtimeEntries");

            Undo.RecordObject(registrySO, "Apply Registry Runtime");
            bool isChanged = false;

            foreach (int index in indices)
            {
                SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(index);
                int enumValue = entryProp.FindPropertyRelative("enumValue").intValue;
                string keyName = entryProp.FindPropertyRelative("enumKeyName").stringValue;
                object runtimeItem = entryProp.FindPropertyRelative("registryItem").managedReferenceValue;

                RegistryEntry soEntry = registrySO.entries.Find(e => e.enumValue == enumValue);
                string errorMsg = GetApplyError(runtimeItem, soEntry);
                if (errorMsg != null)
                {
                    Debug.LogWarning($"[Registry] {registrySO.name}/{keyName}: {errorMsg}", registrySO);
                    continue;
                }

                CopyWithoutReadOnly(runtimeItem, soEntry.registryItem);
                isChanged = true;
            }

            if (isChanged)
                EditorUtility.SetDirty(registrySO);
        }

        private static string GetApplyError(object runtimeItem, RegistryEntry soEntry)
        {
            if (runtimeItem == null)
                return "사본이 없어 건너뜁니다";
            if (soEntry == null)
                return "SO에 같은 번호의 항목이 없어 건너뜁니다";
            if (soEntry.registryItem == null)
                return "SO 항목에 인스턴스가 없어 건너뜁니다";
            if (soEntry.registryItem.GetType() != runtimeItem.GetType())
                return "SO 항목과 타입이 달라 건너뜁니다";
            return null;
        }

        //사본 값을 통째로 덮어쓴 뒤 [ReadOnlyField] 필드만 원래 값으로 되돌린다.
        //원래 값은 덮어쓰기 전에 JSON으로 따로 복사해 두어야 List 같은 참조 필드도 덮어쓰기에 휩쓸리지 않는다.
        private static void CopyWithoutReadOnly(object source, object destination)
        {
            Type type = destination.GetType();
            List<FieldInfo> readOnlyFields = GetReadOnlyFields(type);
            object backup = readOnlyFields.Count > 0
                ? JsonUtility.FromJson(JsonUtility.ToJson(destination), type)
                : null;

            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), destination);

            foreach (FieldInfo field in readOnlyFields)
                field.SetValue(destination, field.GetValue(backup));
        }

        //부모 클래스의 private 필드는 자식 타입의 GetFields로 보이지 않으므로 부모까지 올라가며 모은다.
        private static List<FieldInfo> GetReadOnlyFields(Type type)
        {
            var fields = new List<FieldInfo>();
            for (Type t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                fields.AddRange(t
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(f => f.IsDefined(typeof(ReadOnlyFieldAttribute), false)));
            }
            return fields;
        }
    }
}
