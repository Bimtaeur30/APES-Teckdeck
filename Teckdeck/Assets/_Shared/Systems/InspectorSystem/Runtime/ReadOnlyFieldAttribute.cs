using System;
using UnityEngine;

namespace _Shared.Systems.InspectorSystem.Runtime
{
    //필드에만 사용 가능한 Attribute임을 명시
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyFieldAttribute : PropertyAttribute
    {
        //플레이 중이 아닐 때 Edit할 수 있도록 선택지를 주기 위해 만든 필드.
        //Attribute를 추가할 때 (true)를 붙여 값 변경 가능(기본은 false)
        public readonly bool EditableInEditMode;

        //Attribute를 추가할 때 명시하는 생성자. 리플렉션으로 호출된다.
        //applyToCollection는 List 같은 컬렉션 자체에 적용할지 그 요소에 적용할지 정하는 것. true로 하면 컬렉션에 적용
        public ReadOnlyFieldAttribute(bool editableInEditMode = false) : base(applyToCollection: true)
        {
            EditableInEditMode = editableInEditMode;
        }
    }
}
