using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace _Shared.Systems.RegistrySystem.Editor
{
    public class ComponentTypeDropdown : AdvancedDropdown
    {
        //AdvancedDropdownItem을 상속받아 ComponentTypeDropdown에 추가할 수 있도록 한다.
        private class TypeItem : AdvancedDropdownItem
        {
            public readonly Type Type;

            public TypeItem(string displayName, Type type) : base(displayName)
            {
                Type = type;
            }
        }

        private readonly Type _baseType;
        private readonly Action<Type> _onSelected;
        private readonly string _title;
        private readonly Func<Type, bool> _isAddable;

        //기본적인 드롭다운을 구성하는 state 외에 baseType과 선택된 타입을 전달하기 위해 외부에서 콜백 함수를 onSelected에 담는다.
        
        //state는 전에 열렸던 상태를 기억하고 다시 열었을 때 스크롤 위치를 미리 변경함. 여기선 매번 가장 위에서 열려도 상관 없기 때문에 
        //매번 new()를 통해 만들어도 상관 없다.
        //title과 isAddable을 넘기지 않으면 항목 추가용(기본 제목, IsAddable)으로 동작한다.
        public ComponentTypeDropdown(AdvancedDropdownState state, Type baseType, Action<Type> onSelected
            , string title = "컴포넌트 타입", Func<Type, bool> isAddable = null)
            : base(state)
        {
            _baseType = baseType;
            _onSelected = onSelected;
            _title = title;
            _isAddable = isAddable ?? IsAddable;

            minimumSize = new Vector2(250f, 300f);
        }

        //Show() 호출될 때 호출되는 함수.
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(_title);

            //baseType이 null인 상태로 생성된 경우 return
            if (_baseType == null)
            {
                //누를 수 없는 Item을 하나 생성하여 경고
                root.AddChild(new AdvancedDropdownItem("베이스 타입을 먼저 지정하세요") { enabled = false });
                return root;
            }

            //유니티가 컴파일 할 때 만들어 놓은 표를 조회하여 타입들을 받은 후 _baseType까지 추가
            IEnumerable<Type> candidates = TypeCache.GetTypesDerivedFrom(_baseType).Append(_baseType);

            //IsAddable에서 _baseType이 abstract인 경우 걸러짐. 다른 타입도 검사하여 거른 후 이름 순서로 정렬
            List<Type> types = candidates
                .Where(_isAddable)
                .OrderBy(t => t.Name)
                .ToList();

            if (types.Count == 0)
            {
                root.AddChild(new AdvancedDropdownItem("추가할 수 있는 타입이 없습니다") { enabled = false });
                return root;
            }

            //네임스페이스가 다르고 이름이 같은 클래스의 이름들을 하나로 묶어 묶인 이름들만 HashSet에 저장
            HashSet<string> duplicatedNames = types
                .GroupBy(t => t.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            //실제 이름은 이름이 겹치는 경우(duplicatedNames에 포함된 경우) 네임스페이스까지, 그렇지 않으면 클래스 이름만
            foreach (Type type in types)
            {
                string displayName = duplicatedNames.Contains(type.Name)
                    ? $"{type.Name} ({type.Namespace})"
                    : type.Name;
                root.AddChild(new TypeItem(displayName, type));
            }

            return root;
        }

        //추상 클래스 또는 인터페이스인 경우, 제네릭이 포함된 경우, UnityEngine.Object인 경우, [Serializable]이 없는 경우 false
        private bool IsAddable(Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type.ContainsGenericParameters)
                return false;

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return false;

            //SerializeReference는 [Serializable]이 붙은 타입만 저장한다.
            if (!type.IsSerializable)
                return false;

            return true;
        }

        //아이템이 선택되었을 때 _onSelected 델리게이트 호출.
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is TypeItem typeItem)
                _onSelected?.Invoke(typeItem.Type);
        }
    }
}
