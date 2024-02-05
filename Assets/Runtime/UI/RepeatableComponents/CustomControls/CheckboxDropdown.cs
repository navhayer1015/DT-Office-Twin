
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class CheckboxDropdown : Dropdown
{
    [Preserve]
    public new class UxmlFactory : UxmlFactory<CheckboxDropdown, UxmlTraits>
    {
    }

    public override void RefreshDropdownMenuChoices()
    {
        _dropdownMenu.Clear();

        foreach (var choice in _choices)
        {
            // create a button and add it to the dropdown menu
            Checkbox btn = new Checkbox();
            btn.label = choice.Key;
            btn.size = Size.M;
            btn.RegisterValueChangedCallback((x) => { OnDropdownChoiceClicked(choice.Key, x.newValue); });
            _dropdownMenu.Add(btn);
            btn.focusable = false;
            if (choice.Value is bool activeValue)
            {
                //tn.value = activeValue?CheckboxState.Checked:CheckboxState.Unchecked;
            }
        }
    }

    // private void OnDropdownChoiceClicked(string name, CheckboxState state)
    // {
    //     OnChoiceClicked?.Invoke(name, state == CheckboxState.Checked);
    //     _dropdownMenu.visible = false;
    // }
    public override bool UpdateData(string choiceKey, object choiceUserData)
    {
        if (_choices.ContainsKey(choiceKey) && choiceUserData is bool newValue)
        {
            _choices[choiceKey] = choiceUserData;
            foreach (var child in _dropdownMenu.Children())
            {
                if (child is Checkbox checkbox && checkbox.label == choiceKey)
                {
                    checkbox.SetValueWithoutNotify(newValue?CheckboxState.Checked:CheckboxState.Unchecked);
                }
            }
            return true;
        }
        return false;
    }
}