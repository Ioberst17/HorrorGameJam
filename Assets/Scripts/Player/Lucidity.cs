using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lucidity : MonoBehaviour
{
    [SerializeField] public float level;
    [SerializeField] private float lucidity_MAX = 100f;
    [SerializeField] private float lucidityReducationRate = 0.0001f;
    public float narrativeItemRateChange;
    public GameController gameController;

    [SerializeField] LucidityData.LevelData variablesToChange;

    // Parameters to change as lucidity changes
    [SerializeField] Light2D globalLight;
    PlayerHealth playerHealth;
    [SerializeField] AudioManager audioManager;

    void Start()
    {
        level = lucidity_MAX;
        gameController = FindObjectOfType<GameController>();
        globalLight = GameObject.Find("Light 2D_global").GetComponent<Light2D>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if(gameController.GameState != "isPaused") { level -= lucidityReducationRate * narrativeItemRateChange; }
        else { audioManager.UpdateThemePitch(1); }

        if(level > lucidity_MAX) { level = lucidity_MAX; }

        if(level < 0) { EndRun(); }

        HandleLucidityChanges();
    }

    public void Increase(string itemName)
    {
        if(itemName == "Hourglass") { level += 10; }
        else { Debug.LogFormat("Itemname {0} could not be found when picked up to reduce lucidity; check what string is being used to reduce it"); }
    }

    private void HandleLucidityChanges()
    {
        if (level >= 75 && level < lucidity_MAX) {  variablesToChange = LucidityData.GetValue("Base"); PassUpdates(variablesToChange); }
        else if(level >= 50 && level < 75) {  variablesToChange = LucidityData.GetValue("High"); PassUpdates(variablesToChange); }
        else if(level >= 25 && level < 50) { variablesToChange = LucidityData.GetValue("Med"); PassUpdates(variablesToChange); }
        else if (level < 25) { variablesToChange = LucidityData.GetValue("Low"); PassUpdates(variablesToChange); }
    }

    private void PassUpdates(LucidityData.LevelData update)
    {
        globalLight.intensity = update.lightValue;
        playerHealth.lucidityDamageModifier = update.increaseDamageMod;
        audioManager.UpdateThemePitch(update.musicToPlay);
    }

    private void EndRun() { level = 100; /*reset player transform*/ }
}
