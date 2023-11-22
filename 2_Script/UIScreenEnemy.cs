using Godot;
using System;

public partial class UIScreenEnemy : Control
{
    [Export] WarriorMove _warriorMove;
    [Export] float _delay = 3f;
    float _curDelay;
    bool flagDead;
    bool flagDisableVisible;


    public override void _Ready()
    {
        flagDead = false;
        flagDisableVisible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(_warriorMove.IsDead)
        {
            if(!flagDead)
            {
                flagDead = true;
                _curDelay = _delay;
            }
        }



        if(flagDead)
        {
            _curDelay -= (float)delta;
            if(_curDelay <= 0)
            {
                _curDelay = 0;
                if(!flagDisableVisible)
                {
                    flagDisableVisible = true;
                    Visible = false;
                }
            }
        }
    }


}
