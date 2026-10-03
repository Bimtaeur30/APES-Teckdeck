using System.Linq;
using _Shared.Systems.FsmSystem.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Shared.Systems.FsmSystem.Editor
{
    [CustomEditor(typeof(StateSO))]
    public class StateSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset editorView = default;
        
        private StateSO _targetData;
        
        public override VisualElement CreateInspectorGUI()
        {
            _targetData = (StateSO)target; //target은 Editor의 내부 변수이다.
            
            VisualElement root = new VisualElement();
            editorView.CloneTree(root);

            FillDropdownField(root);
            return root;
        }

        private void FillDropdownField(VisualElement root)
        {
            DropdownField field = root.Q<DropdownField>("ClassNameDropdown");

            var choices = TypeCache.GetTypesDerivedFrom<AbstractState>()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Select(type => $"{type.FullName}, {type.Assembly.GetName().Name}");
            
            field.choices.AddRange(choices);

            if (_targetData != null && !string.IsNullOrEmpty(_targetData.className)
                                    && field.choices.Contains(_targetData.className))
            {
                field.value = _targetData.className;
            }
            else if (_targetData != null && field.choices.Count > 0)
            {
                _targetData.className = field.choices.First();
                EditorUtility.SetDirty(_targetData);
            }
            
            AssetDatabase.SaveAssetIfDirty(_targetData);
        }
    }
}