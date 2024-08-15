using UnityEngine;

public class ToggleTry : MonoBehaviour
{
    TXRButton _button;
    TXRButtonToggleState _state;
    void Start()
    {
        _button = GetComponent<TXRButton>();
        _button.Released.AddListener(OnReleased);
        _button.HoverEnter.AddListener(OnHoverEnter);
        _button.HoverExit.AddListener(OnHoverExit);
        _state = TXRButtonToggleState.Off;
    }

    private void OnHoverEnter()
    {
        // skipping the set color to active that happens on the txrButton internal hover. (OnHoverEnterInternal) 
        _button.SetState(TXRButtonState.Hover);
    }

    private void OnHoverExit()
    {
        if (_state == TXRButtonToggleState.On)
        {
            _button.SetState(TXRButtonState.Pressed);
        }
        else
        {
            _button.SetState(TXRButtonState.Active);
        }
    }

    private void OnReleased()
    {
        bool wasButtonAlreadyPressed = _button.State == TXRButtonState.Pressed;
        if (_state == TXRButtonToggleState.Off)
        {
            _button.SetState(TXRButtonState.Pressed);
            Debug.Log("Toggle ON");
        }
        else
        {
            _button.SetState(TXRButtonState.Active);
            Debug.Log("Toggle OFF");
        }
    }

    void Update()
    {

    }
}
