using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// player의 스킬 관련 속성을 담당<br/>
/// 나중에 스킬 추가와 스킬 슬롯기능 추가에 용이하도록 만듦
/// </summary>
public partial class SkillControl : Node
{
    private IUnitMove _baseMove;
    private List<SkillProperties> _skillPropertiesList = new List<SkillProperties>();

    [Export] private Godot.Collections.Array<SkillData> _skillDataList;
    [Export] private SkillSlotData _skillSlotData;
    /// <summary>
    /// 스킬슬롯 이름과 메서드 Input.IsActionPressed("player_skillA")과의 연결
    /// </summary>
    

    private Dictionary<string, SkillProperties> _slotSkillDict;
    private Dictionary<string, string> _slotKeyConvert;

    public override void _Ready()
    {
        _baseMove = GetParent<IUnitMove>();
        

        foreach (SkillData item in _skillDataList)
        {
            //SkillName을 통해 SkillData의 중복을 찾음
            bool flagIsSameSkill = false;

            foreach(var item2 in _skillPropertiesList)
            {
                if(item2.SkillName == item.SkillName)
                {
                    flagIsSameSkill = true;
                    break;
                }
            }

            //중복이 아닐 경우 _skillPropertiesList에 속성 추가
            if (!flagIsSameSkill)
            {
                SkillProperties tmp = new SkillProperties();
                tmp.SkillName = item.SkillName;
                tmp.Cooltime = item.Cooltime;
                _skillPropertiesList.Add(tmp);
            }
        }

        InitializeSlotAndSkill();
        InitializeSlotAndKey();
    }

	public override void _Process(double delta)
	{
        CheckButtonPress();
        CheckSkillFactor();
    }

    private void CheckButtonPress()
    {
        foreach(var item in _slotKeyConvert)
        {
            var tmpKeyName = item.Value;
            var tmpSkill = _slotSkillDict[item.Key];

            if (Input.IsActionPressed(tmpKeyName))
            {
                if (!tmpSkill.FlagSkillPress)
                {
                    tmpSkill.FlagSkillPress = true;
                    tmpSkill.CurSkillBuffer = tmpSkill.SkillBuffer;
                    tmpSkill.IsTryingSkill = true;
                }
            }
            else
            {
                if (tmpSkill.FlagSkillPress)
                {
                    tmpSkill.FlagSkillPress = false;
                    tmpSkill.CurSkillBuffer = 0;
                    tmpSkill.IsTryingSkill = false;
                }
            }
        }
    }

    private void CheckSkillFactor()
    {
        foreach(var item in _skillPropertiesList)
        {
            var tmpSkill = item;

            tmpSkill.CurSkillBuffer -= (float)GetPhysicsProcessDeltaTime();
            if (tmpSkill.CurSkillBuffer <= 0)
            {
                tmpSkill.CurSkillBuffer = 0;
                tmpSkill.IsTryingSkill = false;
            }

            if (!tmpSkill.HasSkill)
            {
                tmpSkill.CurCooltime -= (float)GetPhysicsProcessDeltaTime();
                if (tmpSkill.CurCooltime <= 0)
                {
                    tmpSkill.CurCooltime = tmpSkill.Cooltime;
                    tmpSkill.HasSkill = true;
                }
            }
        }
    }

    private void InitializeSlotAndSkill()
    {
        //_slotSkillDict 생성자 호출 안 했을 경우 실행
        if (_slotSkillDict == null)
            _slotSkillDict = new Dictionary<string, SkillProperties>();

        //기존에 스킬을 등록해뒀다면 해당 스킬 속성 FlagSkillPress, CurSkillBuffer, IsTryingSkill 초기화
        foreach (var item in _slotSkillDict)
        {
            var tmpSkill = item.Value;

            tmpSkill.FlagSkillPress = false;
            tmpSkill.CurSkillBuffer = 0f;
            tmpSkill.IsTryingSkill = false;
        }

        //기존 슬롯 스킬 초기화
        _slotSkillDict.Clear();

        //_skillSlotData에 연결된 스킬이름에 따라 _slotSkillDict 연결
        foreach (var item in _skillPropertiesList)
        {
            if (item.SkillName == _skillSlotData.SkillAName)
            {
                if (!_slotSkillDict.ContainsKey("skillA"))
                    _slotSkillDict.Add("skillA", item);
            }
            if (item.SkillName == _skillSlotData.SkillBName)
            {
                if (!_slotSkillDict.ContainsKey("skillB"))
                    _slotSkillDict.Add("skillB", item);
            }
            if (item.SkillName == _skillSlotData.SkillCName)
            {
                if (!_slotSkillDict.ContainsKey("skillC"))
                    _slotSkillDict.Add("skillC", item);
            }
            if (item.SkillName == _skillSlotData.SkillDName)
            {
                if (!_slotSkillDict.ContainsKey("skillD"))
                    _slotSkillDict.Add("skillD", item);
            }
        }
    }

    private void InitializeSlotAndKey()
    {
        if(_slotKeyConvert != null)
            _slotKeyConvert.Clear();

        _slotKeyConvert = new Dictionary<string, string>
        {
            { "skillA", "player_skillA"},
            { "skillB", "player_skillB"},
            { "skillC", "player_skillC"},
            { "skillD", "player_skillD"},
        };
    }

    public void UpdateSkillSlot()
    {

    }
}
