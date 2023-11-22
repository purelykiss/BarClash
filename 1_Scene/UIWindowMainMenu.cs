using Godot;
using System;

public partial class UIWindowMainMenu : ColorRect, IWindow
{
    UIPauseMenu _UIPauseMenu;
    bool _isActive;
    [Export] Button _GoMainButton;
    [Export] Button _OptionButton;
    [Export] Button _ContinueButton;

	public override void _Ready()
	{
        _UIPauseMenu = GetParent<UIPauseMenu>();
        _isActive = false;
        Visible = false;
        DisableFeature();
    }

	public override void _Process(double delta)
	{

	}

    public void SetActive(bool flag)
    {

    }

    public void ActivateFeature()
    {
        _GoMainButton.Disabled = false;
        _OptionButton.Disabled = false;
        _ContinueButton.Disabled = false;
    }

    public void DisableFeature()
    {
        _GoMainButton.Disabled = true;
        _OptionButton.Disabled = true;
        _ContinueButton.Disabled = true;
    }

    public void GoMainButtonPressed()
    {

    }

    public void OptionButtonPressed()
    {

    }

    public void ContinueButtonPressed()
    {

    }
}
