using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

 public class ExtendedStandaloneInputModule : StandaloneInputModule
{
    private static ExtendedStandaloneInputModule singleton;

    protected override void Awake()
    {
        base.Awake();
        singleton = this;
    }

    public static PointerEventData GetPointerEventData(int pointerId = -1)
    {
        PointerEventData eventData;
        singleton.GetPointerData(pointerId, out eventData, true);

        return eventData;
    }

    public static BaseEventData GetBaseData()
    {
        return singleton.GetBaseEventData();
    }
}
