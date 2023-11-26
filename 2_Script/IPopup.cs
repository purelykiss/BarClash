using Godot;
using System;

public interface IPopup
{
    public string ID { get; }

    public void SetActive(bool flag);
}
