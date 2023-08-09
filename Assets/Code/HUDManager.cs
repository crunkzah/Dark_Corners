using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoSingleton<HUDManager>
{
    public TextMeshProUGUI hp_tmp;
    public TextMeshProUGUI label_tmp;
    public TextMeshProUGUI score_tmp;
    

    void Update()
    {
        hp_tmp.SetText(PlayerController.Instance.Health.ToString());
        score_tmp.SetText(ArenaManager.Score.ToString());
    }

    public static void SetLabelText(string text)
    {
        Instance.label_tmp.SetText(text);
        Instance.label_tmp.enabled = true;
    }

    public static void HideLabelText()
    {
        Instance.label_tmp.enabled = false;
    }
}
