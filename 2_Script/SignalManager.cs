using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 시그널을 받고 필요한 곳으로 보내주기만 함
/// </summary>
public partial class SignalManager : Node
{
    public static SignalManager instance { get; private set; }

    [Signal] public delegate void FocusEventHandler(Vector2 globalPos);
    [Signal] public delegate void BGPosEventHandler(Vector2 globalPos);
    [Signal] public delegate void PauseBlindEventHandler(bool flagIsPause);

    public override void _Ready()
    {
        if(instance == null)
        {
            instance = this;
            ProcessMode = ProcessModeEnum.Always;
        }
        else if(instance != this)
        {
            QueueFree();
        }
    }
    public void ConnectNodes(string fromPath, string signalName, string toPath, string methodName)
    {
        
    }
}
