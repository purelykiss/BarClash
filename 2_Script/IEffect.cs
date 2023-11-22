using Godot;
using System;

public interface IEffect
{
    public string ID { get; }

    public bool IsActivate { get; set; }


    public void Activate();

    public void Disable();

}
