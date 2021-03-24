using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace SpeachToText
{
    public class Speech2Text
    {
        public static readonly string TokenAccessUri = "https://japanwest.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
        public static readonly string EndpointAccessUri = "https://japanwest.tts.speech.microsoft.com/cognitiveservices/v1";
        private string apiKey = "c5a88b17adcf42c28a0948019a7d2d29";
        private string accessToken;
        private Timer accessTokenRenewer;
 
        private const int RefreshTokenDuration = 9;
 
        public Speech2Text()
        {
            HttpPost(TokenAccessUri, this.apiKey);
 
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }
 
 
        // 信頼できないSSL証明書を「問題なし」にするメソッド
        private bool OnRemoteCertificateValidationCallback(
          System.Object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            return true;  // 「SSL証明書の使用は問題なし」と示す
        }
 
        public string GetAccessToken()
        {
            return this.accessToken;
        }
 
        private void RenewAccessToken()
        {
            HttpPost(TokenAccessUri, this.apiKey);
        }
 
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception)
            {
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception)
                {
                }
            }
        }
 
        private void HttpPost(string accessUri, string apiKey)
        {
            Debug.Log("HttpPost apiKey=" + apiKey);
 
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
 
            WebRequest webRequest = WebRequest.Create(accessUri);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            webRequest.Headers["Ocp-Apim-Subscription-Key"] = apiKey;
 
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] waveBytes = null;
                        int count = 0;
                        do
                        {
                            byte[] buf = new byte[1024];
                            count = stream.Read(buf, 0, 1024);
                            ms.Write(buf, 0, count);
                        } while (stream.CanRead && count > 0);
 
                        waveBytes = ms.ToArray();
 
                        this.accessToken = Encoding.UTF8.GetString(waveBytes);
                    }
                }
            }
            Debug.Log("accessToken=" + this.accessToken);
        }
 
        public string Listen(string audioFile)
        {
            string requestUri = "https://japaneast.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=ja-JP";
            FileStream fs;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
            HttpWebRequest request = null;
            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            request.SendChunked = true;
            request.Accept = @"application/json;text/xml";
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentType = @"audio/wav; codec=audio/pcm; samplerate=16000";
            request.Headers["Authorization"] = "Bearer " + this.accessToken;
            request.Headers["Ocp-Apim-Subscription-Key"] = this.apiKey;
 
            Debug.Log("accessToken=" + this.accessToken);
 
            using (fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = null;
                int bytesRead = 0;
                using (Stream requestStream = request.GetRequestStream())
                {
                    buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                    requestStream.Flush();
                }
            }
 
            string text = "";
 
            using (WebResponse response = request.GetResponse())
            {
                Debug.Log(((HttpWebResponse)response).StatusCode);
 
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    // {"RecognitionStatus":"Success","DisplayText":"こんにちは","Offset":7700000,"Duration":10500000}
                    string responseString = sr.ReadToEnd();
                    STTResponse res = JsonUtility.FromJson<STTResponse>(responseString);
                    text = res.DisplayText;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "";
                    }
                }
            }
            Debug.Log("response=" + text);
            return text;
        }
    }
 
    [Serializable]
    public class STTResponse
    {
        public string RecognitionStatus;
        public string DisplayText;
        public int Offset;
        public int Duration;
    }
}