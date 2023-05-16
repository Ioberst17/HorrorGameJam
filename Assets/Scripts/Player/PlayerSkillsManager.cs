using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsManager : MonoBehaviour
{
    private DataManager dataManager;
    [SerializeField] private PlayerSkills skills;

    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.Instance;
        skills = new PlayerSkills();
        LoadPlayerSkills();
        skills.UnlockAllSkills(); // remove as demo narrative progress, have all skills at beginning for testing
    }

    private void UnlockSkill(PlayerSkills.SkillType skill) { skills.UnlockSkill(skill); }

    private void SavePlayerSkills()
    {
        for (int i = 0; i < skills.unlockedSkillsList.Count; i++)
        {
            if (!dataManager.sessionData.playerSkills.unlockedSkillsList.Contains(skills.unlockedSkillsList[i]))
            { dataManager.sessionData.playerSkills.unlockedSkillsList.Add(skills.unlockedSkillsList[i]); }
        }
    }

    private void LoadPlayerSkills()
    {
        for (int i = 0; i < dataManager.sessionData.playerSkills.unlockedSkillsList.Count; i++)
        {
            skills.unlockedSkillsList.Add(dataManager.sessionData.playerSkills.unlockedSkillsList[i]);
        }
    }

    public bool hasJump() { return skills.IsSkillUnlocked(PlayerSkills.SkillType.Jump); }
    public bool hasDash() { return skills.IsSkillUnlocked(PlayerSkills.SkillType.Dash); }
    public bool hasFire() { return skills.IsSkillUnlocked(PlayerSkills.SkillType.Fire); }
    public bool hasBlock() { return skills.IsSkillUnlocked(PlayerSkills.SkillType.Block); }

    private void OnDestroy()
    {
        SavePlayerSkills();
    }
}
