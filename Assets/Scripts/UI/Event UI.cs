using TMPro;
using UnityEngine;

public class EventUI : MonoBehaviour
{
    private EventBase activeEvent;

    public TextMeshProUGUI title;

    public void Bind(EventBase currentEvent)
    {
        activeEvent = currentEvent;
        title.text = currentEvent.eventTitle;
    }

    public void OnConfirm()
    {
        if (activeEvent != null)
        {
            EventManager.instance.currentActiveEvent = activeEvent;
            activeEvent.ChangeButtonText();
            activeEvent.ShowUI();
        }
        else
        {
            Debug.Log("Event data is NULL!");
        }
    }
    public EventBase GetActiveEvent()
    {
        return activeEvent;
    }
}
