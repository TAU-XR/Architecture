using UnityEngine;

public class TXRRadioButtonGroup : MonoBehaviour
{
    public TXRButton_Toggle[] Buttons => _buttons;
    [SerializeField] private TXRButton_Toggle[] _buttons;

    public TXRButton_Toggle SelectedButton => _selectedButton;
    private TXRButton_Toggle _selectedButton;

    public void Reset()
    {
        if (_selectedButton != null)
        {
            _selectedButton.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
            _selectedButton = null;
        }
    }

    private void OnEnable()
    {
        RegisterButtonEvents();
    }

    private void OnDisable()
    {
        UnregisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            button.ButtonSelected += () => OnButtonSelected(button);
            button.ButtonDeselected += () => OnButtonDeselected(button);
        }
    }

    private void UnregisterButtonEvents()
    {
        foreach (TXRButton_Toggle button in _buttons)
        {
            button.ButtonSelected -= () => OnButtonSelected(button);
            button.ButtonDeselected -= () => OnButtonDeselected(button);
        }
    }

    private void OnButtonSelected(TXRButton_Toggle button)
    {
        if (_selectedButton == button)
        {
            return;
        }

        if (_selectedButton != null)
        {
            _selectedButton.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
        }

        _selectedButton = button;
    }

    private void OnButtonDeselected(TXRButton_Toggle button)
    {
        if (_selectedButton == button)
        {
            _selectedButton = null;
        }
    }

    public void SelectButton(TXRButton_Toggle button)
    {
        button.TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
    }
}