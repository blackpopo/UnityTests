using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class EmpathData
{
    [SerializeField] public int error;
    [SerializeField] public int calm;
    [SerializeField] public int anger;
    [SerializeField] public int joy;
    [SerializeField] public int sorrow;
    [SerializeField] public int energy;
}

public class EmpathError
{
    [SerializeField] public int error;
    [SerializeField] public string msg;
}

//Empath JSON Format 
//Succeed
// {
// "error":0, "calm":21, "anger":0, "joy":35,"sorrow":24, "energy":20
// }
//Failed
// {
// "error":1011, "msg": "wav format must be PCM_FLOAT, PCM_SIGNED, or PCM_UNSIGNED."
// }