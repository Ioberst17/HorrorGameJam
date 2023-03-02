using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkills
{
    [System.Serializable]
    public enum SkillType { 
        None, 
        Jump, Dash, Fire, Block,
        HealthMax_1, HealthMax_2,
        MoveSpeed_1, MoveSpeed_2
    }

    [SerializeField]public List<SkillType> unlockedSkillsList;

    public PlayerSkills() { unlockedSkillsList = new List<SkillType>(); }

    public List<PlayerSkills.SkillType> GetPlayerSkills() { return unlockedSkillsList; }

    public void UnlockSkill(SkillType skill)
    {
        if (!IsSkillUnlocked(skill))
        {
            unlockedSkillsList.Add(skill);
            EventSystem.current.SkillUnlockTrigger(skill);
        }
    }

    public void UnlockAllSkills()
    {
        foreach(PlayerSkills.SkillType skill in Enum.GetValues(typeof(PlayerSkills.SkillType))) { UnlockSkill(skill); }
    }

    public bool IsSkillUnlocked(SkillType skill) { return unlockedSkillsList.Contains(skill); }

    public SkillType GetSkillRequirement(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.HealthMax_2: return SkillType.HealthMax_1;
            case SkillType.MoveSpeed_2: return SkillType.MoveSpeed_1;
        }
        return SkillType.None;
    }

    public bool TryUnlockSkill(SkillType skill)
    {
        if (CanUnlock(skill)) { UnlockSkill(skill); return true; }
        else { return false; }
    }

    public bool CanUnlock(SkillType skill)
    {
        SkillType skillReq = GetSkillRequirement(skill);

        if (skillReq != SkillType.None)
        {
            if (IsSkillUnlocked(skillReq)) { UnlockSkill(skill); return true; }
            else { return false; }
        }
        else { UnlockSkill(skill); return true; }
    }
}
