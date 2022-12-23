using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType{LEFTGOAL, RIGHTGOAL}
public class Goal : MonoBehaviour
{
    [SerializeField] GoalType goalType = GoalType.LEFTGOAL;
    //private GameController gameController;

     public void OnGoalScored()
    {
        //GameController.instance.OnGoalScoredWithProperties(goalType);
        GameController.instance.OnGoalScored(goalType);
    }
}
