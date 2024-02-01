using Godot;
using System;

public interface ISoundEffect
{
    public string ID { get; }
    public bool IsActivate { get; set; }


    public void Activate();

    public void Disable();

}
