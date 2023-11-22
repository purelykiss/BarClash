using Godot;
using System;
using System.Collections.Generic;

public partial class FiniteStateMachine : Node
{
    IUnitMove _baseMove;
	List<IState> _states = new List<IState>();
	IState _CurrentState;

	public IState CurrentState => _CurrentState;

    public List<IState> States => _states;


	public override void _Ready()
	{
        _baseMove = GetParent<IUnitMove>();

        foreach (var item in GetChildren())
		{
			if(item is IState)
			{
				IState tmp = (IState)item;
				_states.Add(tmp);
			}
		}

		_CurrentState = _states[0];
	}

	public void Activate()
	{
		_CurrentState.Move();
		_CurrentState.Animation();
		Change();
	}

	void Change()
	{
        string changeTo = CheckAndComfirmHit();
        if(changeTo == null)
            changeTo = _CurrentState.Change();

        if (changeTo != null)
		{
			foreach(var item in _states)
			{
				if (item.ID == changeTo) 
				{
					_CurrentState.OnFinish();
                    _CurrentState = item;
                    break;
				}
			}
		}
    }

    string CheckAndComfirmHit()
    {
        string result = null;
        if(_baseMove.HitList.Count > 0)
        {
            foreach (var item in _states)
            {
                if (item is IStagger)
                {
                    if(CheckCondition(item.ID))
                    {
                        result = item.ID;
                        break;
                    }
                }
            }

            _baseMove.HitList.RemoveAt(0);
        }

        return result;
    }

	public bool CheckCondition(string id)
	{
		IState state = null;
		bool flag = false;

		foreach(var item in _states)
		{
			if (item.ID == id)
			{
				state = item;
				break;
			}
		}

		if (state != null)
			flag = state.Condition();

        return flag;
	}

	public bool CheckCancelable(IState state) 
	{
		bool flag = false;

		switch(_CurrentState.CurCancelState)
		{
			case IState.Cancelable.Always:
				flag = true;
				break;
			case IState.Cancelable.SameLevel:
				if (state.PriorityLevel >= _CurrentState.PriorityLevel)
					flag = true;
				break;
			case IState.Cancelable.HigherLevel:
                if (state.PriorityLevel > _CurrentState.PriorityLevel)
                    flag = true;
                break;
			case IState.Cancelable.Never:
				flag = false;
				break;
        }

		return flag;
	}
}
