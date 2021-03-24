using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONtest : MonoBehaviour
{
    string testJSON = "{\"status\":\"ok\",\"result\":[{\"age\":20.58,\"gender\":\"Female\",\"emotion\":\"neutral\",\"emotion_detail\":{\"anger\":0.08,\"happy\":0.14,\"neutral\":0.69,\"sad\":0.03,\"surprise\":0.03},\"head_pose\":{\"pitch\":0.23,\"roll\":4.15,\"yaw\":23.06},\"location\":{\"height\":455,\"width\":358,\"x1\":334,\"x2\":692,\"y1\":143,\"y2\":598}}],}";
    // string testJSON ="[{\"faceId\":\"f7eda569-4603-44b4-8add-cd73c6dec644\",\"faceRectangle\":{\"top\":131,\"left\":177,\"width\":162,\"height\":162},\"faceAttributes\":{\"smile\":0.0,\"headPose\":{\"pitch\":0.0,\"roll\":0.1,\"yaw\":-32.9},\"gender\":\"female\",\"age\":22.9,\"facialHair\":{\"moustache\":0.0,\"beard\":0.0,\"sideburns\":0.0},\"glasses\":\"NoGlasses\",\"emotion\":{\"anger\":0.0,\"contempt\":0.0,\"disgust\":0.0,\"fear\":0.0,\"happiness\":0.0,\"neutral\":0.986,\"sadness\":0.009,\"surprise\":0.005},\"blur\":{\"blurLevel\":\"low\",\"value\":0.06},\"exposure\":{\"exposureLevel\":\"goodExposure\",\"value\":0.67},\"noise\":{\"noiseLevel\":\"low\",\"value\":0.0},\"makeup\":{\"eyeMakeup\":true,\"lipMakeup\":true},\"accessories\":[],\"occlusion\":{\"foreheadOccluded\":false,\"eyeOccluded\":false,\"mouthOccluded\":false},\"hair\":{\"bald\":0.0,\"invisible\":false,\"hairColor\":[{\"color\":\"brown\",\"confidence\":1.0},{\"color\":\"black\",\"confidence\":0.87},{\"color\":\"other\",\"confidence\":0.51},{\"color\":\"blond\",\"confidence\":0.08},{\"color\":\"red\",\"confidence\":0.08},{\"color\":\"gray\",\"confidence\":0.02}]}}}]";    // Start is called before the first frame update
    void Start()
    {
        // FaceData faceData = ConvertJson(testJSON);
        // Debug.Log(faceData.persons[0].faceAttributes.smile);
        // Debug.Log(faceData.persons[0].faceAttributes.emotion.anger);
        UserLocalData userLocalData = ULConvertJson(testJSON);
        Debug.Log("This is Alive");
        Debug.Log(userLocalData);
        Debug.Log(userLocalData.result.emotion_detail.surprise);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private FaceData FaceConvertJson( string i_json )
    {
        Debug.AssertFormat( !string.IsNullOrEmpty( i_json ), "Json情報が設定されていません。" );

        // Jsonデータがエラーの場合(もっといいエラーの判定があればいいんだけど……)。
        if( i_json.IndexOf( "error" ) > 0 )
        {
            FaceError faceError = null;
            try
            {
                faceError   = JsonUtility.FromJson<FaceError>( i_json );
            }
            catch( System.Exception )
            {
                faceError   = new FaceError();
            }

            Debug.LogWarningFormat( "顔情報の取得に失敗しているJson情報です。code={0}, message={1}", faceError.error.code, faceError.error.message );

            return null;            
        }

        // 無理やりJsonUtilityで使える形に変更する。
        string json = string.Format( "{{\"persons\":{0}}}", i_json );

        FaceData faceData   = null;

        try
        {
            faceData    = JsonUtility.FromJson<FaceData>( json );
        }
        catch( System.Exception i_exception )
        {
            Debug.LogWarningFormat( "Json情報をクラス情報へ変換することに失敗しました。exception={0}", i_exception );
            faceData    = null;
        }

        return faceData;
    }

    private UserLocalData ULConvertJson(string i_json)
    {
        Debug.AssertFormat(!string.IsNullOrEmpty(i_json), "Json情報が設定されていません。");

        // Jsonデータがエラーの場合(もっといいエラーの判定があればいいんだけど……)。
        if (i_json.IndexOf("error") > 0)
        {
            FaceError faceError = null;
            try
            {
                faceError = JsonUtility.FromJson<FaceError>(i_json);
            }
            catch (System.Exception)
            {
                faceError = new FaceError();
            }

            Debug.LogWarningFormat("顔情報の取得に失敗しているJson情報です。code={0}, message={1}", faceError.error.code,
                faceError.error.message);

            return null;
        }

        string json = i_json.Replace("[", "").Replace("],", "");

        UserLocalData userLocalData = null;
        try
        {
            userLocalData    = JsonUtility.FromJson<UserLocalData>( json );
        }
        catch( System.Exception i_exception )
        {
            Debug.LogWarningFormat( "Json情報をクラス情報へ変換することに失敗しました。exception={0}", i_exception );
            userLocalData  = null;
        }

        return userLocalData;
        
    }

}
