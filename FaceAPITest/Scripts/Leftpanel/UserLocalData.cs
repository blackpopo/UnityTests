using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class UserLocalData
{
    [SerializeField] public string status = null;
    [SerializeField] public Result result = null;
}

[System.Serializable]
public class Result
{
    [SerializeField] public float age = 0.0f;
    [SerializeField] public string gender = null;
    [SerializeField] public string emotion = null;
    [SerializeField] public EmotionDetail emotion_detail = null;
    [SerializeField] public HeadPose head_pose = null;
    [SerializeField] public Location location = null;
}

[System.Serializable]
public class EmotionDetail
{
    [SerializeField] public float anger = 0.0f;
    [SerializeField] public float sad = 0.0f;
    [SerializeField] public float neutral = 0.0f;
    [SerializeField] public float happy = 0.0f;
    [SerializeField] public float surprise = 0.0f;
}

// [System.Serializable]
// public class HeadPose
// {
//     [SerializeField] public float pitch;
//     [SerializeField] public float yaw;
//     [SerializeField] public float roll;
// }

[System.Serializable]
public class Location
{
    [SerializeField] public int height = 0;
    [SerializeField] public int width = 0;
    [SerializeField] public int x1 = 0;
    [SerializeField] public int x2 = 0;
    [SerializeField] public int y1 = 0;
    [SerializeField] public int y2 = 0;
}

[System.Serializable]
public class UserLocalError
{
    [SerializeField] public string status = null;
    [SerializeField] public string error_type = null;
    [SerializeField] public string error_message_detail = null;
}
// //JSON Status OK
// {
// "status": "ok",
// "result": [
// {
//     "age": 20.58,
//     "gender": "Female",
//     "emotion": "neutral",
//     "emotion_detail": {
//         "anger": 0.08,
//         "happy": 0.14,
//         "neutral": 0.69,
//         "sad": 0.03,
//         "surprise": 0.03
//     },
//     "head_pose": {
//         "pitch": 0.23,
//         "roll": 4.15,
//         "yaw": 23.06
//     },
//     "location": {
//         "height": 455,
//         "width": 358,
//         "x1": 334,
//         "x2": 692,
//         "y1": 143,
//         "y2": 598
//     }
// }
// ],
// }

//Json Status Error 
// {
// status: "error",
// error_type: "Bad Request",
// error_message_detail: "File is not image"
// }