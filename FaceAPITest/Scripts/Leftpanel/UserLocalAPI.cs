using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UserLocalAPI

{
    static string endpoint = "https://ai-api.userlocal.jp/face ";

    public async UniTask GetUnityRequest(byte[] bytes)
    {
        // string image = System.Convert.ToBase64String(bytes);
        // Debug.Log(image);
        // List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        // formData.Add( new MultipartFormFileSection(image, "image_data") );
        WWWForm formData = new WWWForm();
        formData.AddBinaryData("image_data", bytes);
        UnityWebRequest www = UnityWebRequest.Post(endpoint, formData);
        await www.SendWebRequest();
        string responseText = ResponseAsync(www); 
        Debug.Log("Response");
        Debug.Log(JsonPrettyPrint(responseText));
    }

    private static string ResponseAsync(UnityWebRequest request)
    {
        string responseText = "";
        // レスポンス文字列を取得
        if (request.isHttpError || request.isNetworkError)
        {
            int responseCode = (int)request.responseCode;
            string responseContentType = "";
            // レスポンスヘッダ取得
            string responseHeader = "";

            Dictionary<string, string> responseHeaders = request.GetResponseHeaders();
            responseHeaders = responseHeaders ?? new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in responseHeaders)
            {
                string text;
                if (pair.Key == null)
                {
                    text = pair.Value;
                }
                else
                {
                    text = pair.Key + "：" + pair.Value;
                    switch (pair.Key.ToLower())
                    {
                        case "content-type":
                            string[] types = pair.Value.Split(';', ' ');
                            if (types.Length != 0)
                            {
                                responseContentType = types[0];
                            }
                            break;
                    }
                }
                responseHeader += text + "\n";
            }
            Debug.Log(String.Format("Error Occured...\n request Error {0} \n response Code : {1} \n response Content Type : {2} \n response Header : {3}", request.error, responseCode, responseContentType, responseHeader)); ;
        }
        else
        {
            responseText = request.downloadHandler.text;
        }

        return responseText;
    }

    static string JsonPrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
            return string.Empty;

        json = json.Replace(Environment.NewLine, "").Replace("\t", "");

        StringBuilder sb = new StringBuilder();
        bool quote = false;
        bool ignore = false;
        int offset = 0;
        int indentLength = 3;

        foreach (char ch in json)
        {
            switch (ch)
            {
                case '"':
                    if (!ignore) quote = !quote;
                    break;
                case '\'':
                    if (quote) ignore = !ignore;
                    break;
            }

            if (quote)
                sb.Append(ch);
            else
            {
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', ++offset * indentLength));
                        break;
                    case '}':
                    case ']':
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', --offset * indentLength));
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', offset * indentLength));
                        break;
                    case ':':
                        sb.Append(ch);
                        sb.Append(' ');
                        break;
                    default:
                        if (ch != ' ') sb.Append(ch);
                        break;
                }
            }
        }

        return sb.ToString().Trim();
    }
}
