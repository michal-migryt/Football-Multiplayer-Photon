using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType{LEFTGOAL, RIGHTGOAL}
public class Goal : MonoBehaviour
{
    [SerializeField] GoalType goalType = GoalType.LEFTGOAL;
    AudioSource audioSource;
    //private GameController gameController;
    private void Start() {
        audioSource = GetComponent<AudioSource>();
        UpdateAudioSource();
        SettingsManager.instance.updateSettingsDelegate += UpdateAudioSource;
    }
     public void OnGoalScored()
    {
        GameController.instance.OnGoalScored(goalType);
        audioSource.Play();
    }
    private void UpdateAudioSource()
    {
        audioSource.volume  = SettingsManager.instance.volume;
    }
}
