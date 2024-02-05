using System.Collections.Generic;
using UnityEngine.UIElements;

public class ToggleGroup : VisualElement
{
    private const string ACTIVE_STYLE_CLASS_NAME = "togglegroup-button__active";
    private const string INACTIVE_STYLE_CLASS_NAME = "togglegroup-button__inactive";
    
    public new class UxmlFactory : UxmlFactory<ToggleGroup> { }

    private List<Button> _buttons;

    public void InitToggling(Button startingButton = null)
    {
        _buttons = this.Query<Button>().ToList();

        foreach (var btn in _buttons)
        {
            btn.RegisterCallback<ClickEvent>(evt => OnButtonSelected(btn));
        }

        OnButtonSelected(startingButton);
    }

    private void OnButtonSelected(Button btn)
    {
        if (btn == null || !_buttons.Contains(btn))
        {
            return;
        }
        
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].RemoveFromClassList(ACTIVE_STYLE_CLASS_NAME);
            _buttons[i].RemoveFromClassList(INACTIVE_STYLE_CLASS_NAME);
            
            _buttons[i].AddToClassList(_buttons[i] == btn? ACTIVE_STYLE_CLASS_NAME : INACTIVE_STYLE_CLASS_NAME);
        }
    }
}
