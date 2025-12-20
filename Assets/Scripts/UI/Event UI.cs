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
        //method to display event
        //activeEvent.Execute();

        activeEvent.hasBeenSeen = true;
        EventManager.instance.RemoveActiveEvent(activeEvent.eventTitle);

        UIManager.instance.RemoveEvent(this);
    }
}
