using Godot;
using System;

public interface IWindow
{
    public string ID { get; }

    public void SetActive(bool flag);

    public void SetContentsActive(bool flag);
}
