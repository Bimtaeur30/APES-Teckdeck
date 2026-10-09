using _Shared.Systems.InspectorSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace _Shared.Systems.InspectorSystem.Editor
{
    //ReadOnlyFieldAttribute을 리플렉션으로 찾은 후 Drawer을 찾는다. ~Drawer 네이밍은 관례. Drawer가 없어도 오류가 나지는 않는다
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public class ReadOnlyFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Play 중이 아니고 EditableInEditMode가 true라면 ReadOnly는 false, 그 외는 true
            var readOnlyField = (ReadOnlyFieldAttribute)attribute;
            bool isReadOnly = !readOnlyField.EditableInEditMode || EditorApplication.isPlaying;

            //EditorGUI.DisabledScope는 이름대로 스코프를 생성하여 using이 끝나 Dispose가 호출될 때 까지 isReadOnly가 true라면
            //그려지는 필드를 꺼버린다(값 변경 불가능, 회색으로 처리)
            using (new EditorGUI.DisabledScope(isReadOnly))
            {
                //마지막 true는 List에 요소, +버튼, -버튼 등의 자식까지 같이 그리라는 뜻.
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        //필드가 그려지기 전 높이를 받기 위해 호출됨. override를 하지 않으면 자식(위에서 설명한 요소들)은 고려하지 않은 높이를 반환한다
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
    }
}
