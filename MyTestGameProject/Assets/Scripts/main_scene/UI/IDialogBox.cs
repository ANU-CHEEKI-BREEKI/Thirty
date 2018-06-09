using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IDialogBox
{
    void Show(object owner = null);
    void Show(Vector2 positionOnScreen, object owner = null);
    void Hide();
    IDialogBox SetSize(float? width = null, float? height = null);
    IDialogBox SetTitle(string title);
    IDialogBox SetTitleColor(Color color);
    IDialogBox SetText(string text, bool isScrolable = false);
    IDialogBox SetTextColor(Color color);
    IDialogBox AddButton(string text, Action action);
    IDialogBox AddCancelButton(string text);
    IDialogBox SetPrefButtonHeight(float prefHeight);
    IDialogBox SetIcon(Sprite sprite);
}
