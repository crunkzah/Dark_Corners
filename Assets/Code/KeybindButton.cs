using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class KeybindButton : MonoBehaviour
{
    public TextMeshProUGUI label_tmp;
    public TextMeshProUGUI key_tmp;
    public Button button;
    [HideInInspector] public GameInput input;

    public void SetDisplayValues() {
        label_tmp.text = SeparateString(input.EntryKey);
        key_tmp.text = input.Key.ToString();
    }

    private string SeparateString(string input) {
        string separatedString = Regex.Replace(input, @"([A-Z])", " $1").Trim();
        separatedString = Regex.Replace(separatedString, @"(\d+)", " $1 ");
        separatedString = Regex.Replace(separatedString, @"\s+", " ");
        return separatedString;
    }
}
