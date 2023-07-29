using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class InteractableTarget : MonoBehaviour, Interactable
{
    public List<DelayedUltEventHolder> ultEvents;
    public void Interact()
    {
        for(int i = 0; i < ultEvents.Count; i++)
            ultEvents[i].Invoke();
    }
}
