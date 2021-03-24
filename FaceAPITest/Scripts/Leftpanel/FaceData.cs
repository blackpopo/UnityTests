using UnityEngine;

[System.Serializable]
public class FaceData
{
    [SerializeField]
    public FacePersonData[] persons = null;

} // class FaceData

[System.Serializable]
public class FacePersonData
{
    [SerializeField]
    public string           faceId          = null;
    [SerializeField]
    public FaceRectangle    faceRectangle   = null;
    [SerializeField]
    public FaceLandmarks    faceLandmarks   = null;
    [SerializeField]
    public FaceAttributes   faceAttributes  = null;

} // class FacePersonData

[System.Serializable]
public class FaceRectangle
{
    [SerializeField]
    public int  top     = 0;
    [SerializeField]
    public int  left    = 0;
    [SerializeField]
    public int  width   = 0;
    [SerializeField]
    public int  height  = 0;

} // class FaceRectangle

[System.Serializable]
public class FaceLandmarks
{
    [SerializeField]
    public Vector2  pupilLeft           = Vector2.zero;
    [SerializeField]
    public Vector2  pupilRight          = Vector2.zero;
    [SerializeField]
    public Vector2  noseTip             = Vector2.zero;
    [SerializeField]
    public Vector2  mouthLeft           = Vector2.zero;
    [SerializeField]
    public Vector2  mouthRight          = Vector2.zero;
    [SerializeField]
    public Vector2  eyebrowLeftOuter    = Vector2.zero;
    [SerializeField]
    public Vector2  eyebrowLeftInner    = Vector2.zero;
    [SerializeField]
    public Vector2  eyeLeftOuter        = Vector2.zero;
    [SerializeField]
    public Vector2  eyeLeftTop          = Vector2.zero;
    [SerializeField]
    public Vector2  eyeLeftBottom       = Vector2.zero;
    [SerializeField]
    public Vector2  eyeLeftInner        = Vector2.zero;
    [SerializeField]
    public Vector2  eyebrowRightInner   = Vector2.zero;
    [SerializeField]
    public Vector2  eyebrowRightOuter   = Vector2.zero;
    [SerializeField]
    public Vector2  eyeRightInner       = Vector2.zero;
    [SerializeField]
    public Vector2  eyeRightTop         = Vector2.zero;
    [SerializeField]
    public Vector2  eyeRightBottom      = Vector2.zero;
    [SerializeField]
    public Vector2  eyeRightOuter       = Vector2.zero;
    [SerializeField]
    public Vector2  noseRootLeft        = Vector2.zero;
    [SerializeField]
    public Vector2  noseRootRight       = Vector2.zero;
    [SerializeField]
    public Vector2  noseLeftAlarTop     = Vector2.zero;
    [SerializeField]
    public Vector2  noseRightAlarTop    = Vector2.zero;
    [SerializeField]
    public Vector2  noseLeftAlarOutTip  = Vector2.zero;
    [SerializeField]
    public Vector2  noseRightAlarOutTip = Vector2.zero;
    [SerializeField]
    public Vector2  upperLipTop         = Vector2.zero;
    [SerializeField]
    public Vector2  upperLipBottom      = Vector2.zero;
    [SerializeField]
    public Vector2  underLipTop         = Vector2.zero;
    [SerializeField]
    public Vector2  underLipBottom      = Vector2.zero;

} // class FaceLandmarks

[System.Serializable]
public class FaceAttributes
{
    [SerializeField]
    public float            age         = 0.0f;
    [SerializeField]
    public string           gender      = null;
    [SerializeField]
    public float            smile       = 0.0f;
    [SerializeField]
    public FacialHair       facialHair  = null;
    [SerializeField]
    public HeadPose         headPose    = null;
    [SerializeField]
    public string           glasses     = null;
    [SerializeField]
    public Emotion          emotion     = null;
    [SerializeField]
    public Hair             hair        = null;
    [SerializeField]
    public Makeup           makeup      = null;
    [SerializeField]
    public Accessories[]    accessories = null;
    [SerializeField]
    public Occlusion        occlusion   = null;
    [SerializeField]
    public Blur             blur        = null;
    [SerializeField]
    public Exposure         exposure    = null;
    [SerializeField]
    public Noise            noise       = null;

} // class FaceAttributes

[System.Serializable]
public class FacialHair
{
    [SerializeField]
    public float    moustache   = 0.0f;
    [SerializeField]
    public float    beard       = 0.0f;
    [SerializeField]
    public float    sideburns   = 0.0f;

} // class FacialHair

[System.Serializable]
public class HeadPose
{
    [SerializeField]
    public float    pitch   = 0.0f;
    [SerializeField]
    public float    roll    = 0.0f;
    [SerializeField]
    public float    yaw     = 0.0f;

} // class HeadPose

[System.Serializable]
public class Emotion
{
    [SerializeField]
    public float    anger       = 0.0f;
    [SerializeField]
    public float    contempt    = 0.0f;
    [SerializeField]
    public float    disgust     = 0.0f;
    [SerializeField]
    public float    fear        = 0.0f;
    [SerializeField]
    public float    happiness   = 0.0f;
    [SerializeField]
    public float    neutral     = 0.0f;
    [SerializeField]
    public float    sadness     = 0.0f;
    [SerializeField]
    public float    surprise    = 0.0f;

} // class Emotion

[System.Serializable]
public class Hair
{
    [SerializeField]
    public float        bald        = 0.0f;
    [SerializeField]
    public bool         invisible   = false;
    [SerializeField]
    public HairColor[]  hairColor   = null;

} // class Hair

[System.Serializable]
public class HairColor
{
    [SerializeField]
    public string   color       = null;
    [SerializeField]
    public float    confidence  = 0.0f;

} // class HairColor

[System.Serializable]
public class Makeup
{
    [SerializeField]
    public bool eyeMakeup   = false;
    [SerializeField]
    public bool lipMakeup   = false;

} // class Makeup

[System.Serializable]
public class Accessories
{
    [SerializeField]
    public string   type       = null;
    [SerializeField]
    public float    confidence  = 0.0f;

} // class Accessories

[System.Serializable]
public class Occlusion
{
    [SerializeField]
    public bool foreheadOccluded    = false;
    [SerializeField]
    public bool eyeOccluded         = false;
    [SerializeField]
    public bool mouthOccluded       = false;

} // class Occlusion

[System.Serializable]
public class Blur
{
    [SerializeField]
    public string   blurLevel   = null;
    [SerializeField]
    public float    value       = 0.0f;

} // class Blur

[System.Serializable]
public class Exposure
{
    [SerializeField]
    public string   exposureLevel   = null;
    [SerializeField]
    public float    value           = 0.0f;

} // class Exposure

[System.Serializable]
public class Noise
{
    [SerializeField]
    public string   noiseLevel  = null;
    [SerializeField]
    public float    value       = 0.0f;

} // class Noise


[System.Serializable]
public class FaceError
{
    [SerializeField]
    public FaceErrorDetail  error   = null;

} // class FaceError

[System.Serializable]
public class FaceErrorDetail
{
    [SerializeField]
    public string   code        = null;
    [SerializeField]
    public string   message     = null;

} // class FaceErrorDetail