using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoSingleton<MenuManager>
{
    public Canvas canvas;

    public void SetPauseVisibile(bool visible)
    {
        canvas.enabled = visible;
    }


}
