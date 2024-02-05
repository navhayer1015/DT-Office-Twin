using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Dropdown : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Dropdown, UxmlTraits>
    {
    }

    protected const string DROPDOWN_MENU_ITEM_TEXT_STYLE = "text-body2";
    protected const string DROPDOWN_MENU_ITEM_STYLE = "dropdown-item";
    protected const string DROPDOWN_MENU_STYLE = "dropdown-scroller";

    protected const float DROPDOWN_MENU_SPACE = 10f;
    protected const float MIN_EDGE_PADDING = 10f;

    protected Button _dropdownButton;
    protected Label _dropdownLabel;
    protected VisualElement _arrowIcon;

    protected ScrollView _dropdownMenu;

    protected readonly Dictionary<string, object> _choices = new();
    public Action<string, object> OnChoiceClicked;

    protected bool _hasMouse;
    protected VisualElement _root;

    protected void OnEnable()
    {
        // init button
        _dropdownButton = this.Q<Button>("button");
        _dropdownButton.RegisterCallback<FocusInEvent>(evt => OnDropdownFocusIn());
        _dropdownButton.RegisterCallback<FocusOutEvent>(evt => OnDropdownFocusOut());

        // init button label
        _dropdownLabel = this.Q<Label>("label");

        // init button icon
        _arrowIcon = this.Q<VisualElement>("icon");

        // init scroller
        _dropdownMenu = new ScrollView();
        _root = GetBaseVisualElement();
        _root.Add(_dropdownMenu);
        _dropdownMenu.AddToClassList(DROPDOWN_MENU_STYLE);
        _dropdownMenu.BringToFront();

        _dropdownMenu.RegisterCallback<MouseEnterEvent>(evt => OnMouseEnter());
        _dropdownMenu.RegisterCallback<MouseLeaveEvent>(evt => OnMouseExit());

        // Set dropdown menu position
        _dropdownMenu.style.left = _dropdownButton.contentRect.xMin;
        _dropdownMenu.style.top = _dropdownButton.worldBound.yMin;
        RefreshDropdownMenuChoices();

        TemplateContainer container = new TemplateContainer("test");
        var menuTest = new ScrollView();
        container.Add(menuTest);

        menuTest.Add(new Button());

        _dropdownMenu.visible = false;
    }

    public void InitDropdown(string startingLabel)
    {
        OnEnable();

        if (_dropdownLabel != null)
        {
            _dropdownLabel.text = startingLabel;
        }
    }

    public virtual void RefreshDropdownMenuChoices()
    {
        _dropdownMenu.Clear();

        foreach (var choice in _choices)
        {
            // create a button and add it to the dropdown menu
            var btn = new Button();
            btn.AddToClassList(DROPDOWN_MENU_ITEM_STYLE);

            var text = new Label();
            text.AddToClassList(DROPDOWN_MENU_ITEM_TEXT_STYLE);
            text.text = choice.Key;

            btn.Add(text);
            btn.RegisterCallback<ClickEvent>(evt => OnDropdownChoiceClicked(choice));
            _dropdownMenu.Add(btn);

            btn.focusable = false;
        }
    }

    private void OnDropdownFocusIn()
    {
        var w = _dropdownMenu.contentRect.width;
        var h = _dropdownMenu.contentRect.height;

        var leftBound = _root.resolvedStyle.width - w - MIN_EDGE_PADDING;
        var topBound = _root.resolvedStyle.height - h - MIN_EDGE_PADDING;
        _dropdownMenu.style.left = Mathf.Min(leftBound, _dropdownButton.worldBound.center.x - (w / 2f));
        _dropdownMenu.style.top = Mathf.Min(topBound, _dropdownButton.worldBound.yMax + DROPDOWN_MENU_SPACE);
        _dropdownMenu.visible = true;

        _dropdownMenu.BringToFront();
    }

    private void OnDropdownFocusOut()
    {
        if (!_hasMouse)
        {
            _dropdownMenu.visible = false;
        }
    }

    private void OnMouseEnter()
    {
        _hasMouse = true;
    }

    private void OnMouseExit()
    {
        _hasMouse = false;
    }

    private void OnDropdownChoiceClicked(KeyValuePair<string, object> choice)
    {
        if (_dropdownLabel != null)
        {
            _dropdownLabel.text = choice.Key;
        }

        OnChoiceClicked?.Invoke(choice.Key, choice.Value);

        _dropdownMenu.visible = false;
    }

    private VisualElement GetBaseVisualElement()
    {
        VisualElement element = this;
        while (element.parent != null)
        {
            element = element.parent;
        }

        return element;
    }

    public void AddChoices(IEnumerable<(string key, object userData)> choices)
    {
        foreach (var choice in choices)
        {
            AddChoice(choice.key, choice.userData);
        }
    }

    public void AddChoice(string choiceKey, object choiceUserData) => _choices.Add(choiceKey, choiceUserData);

    public virtual bool UpdateData(string choiceKey, object choiceUserData)
    {
        if (_choices.ContainsKey(choiceKey))
        {
            _choices[choiceKey] = choiceUserData;
            return true;
        }
        return false;
    }
    
    public void ClearChoices()
    {
        _choices.Clear();
        _dropdownMenu.Clear();
    }
}