using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXRButton_Radio : TXRButton_Toggle
{
    public Action ButtonSelected;
    public Action ButtonDeselected;

    protected override void Init()
    {
        base.Init();
        ToggleOn.AddListener(OnButtonSelected);
        ToggleOff.AddListener(OnButtonDeselected);
    }

    private void OnButtonSelected()
    {
        ButtonSelected?.Invoke();
    }

    private void OnButtonDeselected()
    {
        ButtonDeselected?.Invoke();
    }

    public void ManuallyDeselect()
    {
        TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
    }
}