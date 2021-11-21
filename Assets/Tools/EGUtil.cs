using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class EGUtil
{
    public static void AddListener(this Component component, EventTriggerType eventTriggerType,
        UnityAction<BaseEventData> callback)
    {
        EventTrigger eventTrigger = component.GetComponent<EventTrigger>();
        eventTrigger = eventTrigger ? eventTrigger : component.gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers = eventTrigger.triggers ?? new List<EventTrigger.Entry>();
        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == eventTriggerType);
        if (entry == null)
        {
            entry = new EventTrigger.Entry();
            eventTrigger.triggers.Add(entry);
        }

        entry.callback.AddListener(callback);
    }
}