using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class judgeVowel: MonoBehaviour
{
    public string vowel=null;

    public void Vowel(float[] folmants)
    {
        // Debug.Log("Vowelling");
        vowel = null;
        var ma_first_cnt = 0;
        var ma_second_cnt = 0;
        var mi_first_cnt = 0;
        var mi_second_cnt = 0;
        var mu_first_cnt = 0;
        var mu_second_cnt = 0;
        var me_first_cnt = 0;
        var me_second_cnt = 0;
        var mo_first_cnt = 0;
        var mo_second_cnt = 0;
        var mn_first_cnt = 0;
        var mn_second_cnt = 0;
        var fa_first_cnt = 0;
        var fa_second_cnt = 0;
        var fi_first_cnt = 0;
        var fi_second_cnt = 0;
        var fu_first_cnt = 0;
        var fu_second_cnt = 0;
        var fe_first_cnt = 0;
        var fe_second_cnt = 0;
        var fo_first_cnt = 0;
        var fo_second_cnt = 0;
        var fn_first_cnt = 0;
        var fn_second_cnt = 0;

        var vowel_count = 0;

        // Debug.Log("folmants[0] :" + folmants[0]);
        if ( 100.0f <=folmants[0] && folmants[0] < 1400.0f)
        {
            for (int i = 0; i < folmants.Length; i++)
            {
                //Male
                if (650.0f < folmants[i] && folmants[i] < 1400.0f)
                {
                    ma_first_cnt += 1;
                }

                if (900.0f < folmants[i] && folmants[i] < 1600.0f)
                {
                    ma_second_cnt += 1;
                }

                if (100.0f < folmants[i] && folmants[i] < 350.0f)
                {
                    mi_first_cnt += 1;
                }

                if (1900.0f < folmants[i] && folmants[i] < 3100.0f)
                {
                    mi_second_cnt += 1;
                }

                if (100.0f < folmants[i] && folmants[i] < 500.0f)
                {
                    mu_first_cnt += 1;
                }

                if (1100.0f < folmants[i] && folmants[i] < 1800.0f)
                {
                    mu_second_cnt += 1;
                }

                if (350.0f < folmants[i] && folmants[i] < 650.0f)
                {
                    me_first_cnt += 1;
                }

                if (1800.0f < folmants[i] && folmants[i] < 2600.0f)
                {
                    me_second_cnt += 1;
                }

                if (300.0f < folmants[i] && folmants[i] < 650.0f)
                {
                    mo_first_cnt += 1;
                }

                if (500.0f < folmants[i] && folmants[i] < 1100.0f)
                {
                    mo_second_cnt += 1;
                }

                //Female
                if (700.0f < folmants[i] && folmants[i] < 1400.0f)
                {
                    fa_first_cnt += 1;
                }

                if (1250.0f < folmants[i] && folmants[i] < 2000.0f)
                {
                    fa_second_cnt += 1;
                }

                if (150.0f < folmants[i] && folmants[i] < 600.0f)
                {
                    fi_first_cnt += 1;
                }

                if (2300.0f < folmants[i] && folmants[i] < 3600.0f)
                {
                    fi_second_cnt += 1;
                }

                if (250.0f < folmants[i] && folmants[i] < 600.0f)
                {
                    fu_first_cnt += 1;
                }

                if (1250.0f < folmants[i] && folmants[i] < 2000.0f)
                {
                    fu_second_cnt += 1;
                }

                if (250.0f < folmants[i] && folmants[i] < 750.0f)
                {
                    fe_first_cnt += 1;
                }

                if (2000.0f < folmants[i] && folmants[i] < 3000.0f)
                {
                    fe_second_cnt += 1;
                }

                if (400.0f < folmants[i] && folmants[i] < 900.0f)
                {
                    fo_first_cnt += 1;
                }

                if (500.0f < folmants[i] && folmants[i] < 1250.0f)
                {
                    fo_second_cnt += 1;
                }
            }

            if ((mi_first_cnt >= 5 && mi_second_cnt >= 2) || (fi_first_cnt >= 5 && fi_second_cnt >= 2))
            {
                vowel = "あ";
            }
            else if ((mi_first_cnt >= 2 && mi_second_cnt >=5) || (fi_first_cnt >= 2 && fi_second_cnt >= 5))
            {
                vowel = "お";
            }
            else if ((me_first_cnt >= 3 && me_second_cnt >= 1) || (fe_first_cnt >= 3 && fe_second_cnt >= 1))
            {
                vowel = "え";
            }
            else if ((mi_first_cnt >= 3 && mi_second_cnt >= 2) || (fi_first_cnt >= 3 && fi_second_cnt >= 2))
            {
                vowel = "い";
            }
            else if ((mu_first_cnt >= 3 && mu_second_cnt >= 1) || (fu_first_cnt >= 3 && fu_second_cnt >= 1))
            {
                vowel = "う";
            }

            
            if (vowel != null)
            {
                // Debug.Log("あ: " + ma_first_cnt + " : " + ma_second_cnt);
                // Debug.Log("い: " + mi_first_cnt + " : " + mi_second_cnt);
                // Debug.Log("う: " + mu_first_cnt + " : " + mu_second_cnt);
                // Debug.Log("え: " + me_first_cnt + " : " + me_second_cnt);
                // Debug.Log("お: " + mo_first_cnt + " : " + mo_second_cnt);
                Debug.Log(vowel);
            }

        }
    }
}
