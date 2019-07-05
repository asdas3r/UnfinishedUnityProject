using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Dropdown))]
public class DropdownCancelEventHandle : EventTrigger
{
    private bool dropdownIsOpen;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        dropdownIsOpen = false;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        dropdownIsOpen = true;
    }

    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);

        /*if (!dropdownIsOpen)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.cancelHandler);
        }*/
    }
}
