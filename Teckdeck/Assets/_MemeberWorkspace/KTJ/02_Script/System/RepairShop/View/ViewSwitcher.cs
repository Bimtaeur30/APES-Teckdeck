using System;
using System.Linq;
using CoreSystem;
using Unity.Cinemachine;
using UnityEngine;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private EventChannelSO repairshopChannel;
    [SerializeField] private ViewDefinition[] viewDefinitions;
    private ViewDefinition currentView;

    private void Awake()
    {
        currentView = viewDefinitions[0];
    }

    private void OnEnable()
    {
        repairshopChannel.AddListener<ChangeViewEvent>(HandleChangeViewEvent);
    }

    private void OnDisable()
    {
        repairshopChannel.RemoveListener<ChangeViewEvent>(HandleChangeViewEvent);
    }

    private void Update()
    {
        currentView?.Interaction.HandleInput();
    }

    private void HandleChangeViewEvent(ChangeViewEvent obj)
    {
        ViewId viewId = obj.ViewId;
        ViewDefinition targetView = viewDefinitions.Where(x => x.Id == viewId).FirstOrDefault();
        
        if (targetView == default)
            Debug.LogAssertion("ViewDefinitions 에 " + viewId + "가 등록되지 않았습니다. 인스펙터를 확인하세요.");
        else // 성공시
        {
            currentView =  targetView;
            foreach (ViewDefinition view in viewDefinitions)
            {
                int priority = (targetView == view) ? 10 : 0;
                view.Camera.Priority = priority;
            }
        }
    }

}
