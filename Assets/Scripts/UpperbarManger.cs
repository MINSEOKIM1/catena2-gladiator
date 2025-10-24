using System;
using Bucket.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpperbarManger : MonoBehaviour
{
    public TMP_Text date;
    public TMP_Text popularity;
    public TMP_Text money;
    public TMP_Text power;
    public TMP_Text time;

    public Slider hpSlider;

    private void Update()
    {
        date.text = $"{ScheduleManager.Instance.Date}일차";
        popularity.text = $"인기도 : {DataManager.Instance.popularity}%";
        money.text = $"돈 : {DataManager.Instance.money} gold";
        power.text = $"전투력 : {ManagementPhaseManager.Instance.fighters[0].GetTotalPower()}";
        string curtime;
        int curtimeint = DataManager.Instance.time; 
        if (curtimeint == 0)
        {
            curtime = "아침";
        } 
        else if (curtimeint == 1)
        {
            curtime = "낮";
        }
        else if (curtimeint == 2)
        {
            curtime = "저녁";
        }
        else if (curtimeint == 3)
        {
            curtime = "밤";
        }
        else
        {
            curtime = "그럴리가 이거 언젠데";
        }

        time.text = $"현재 시간 : {curtime}";

        hpSlider.value = DataManager.Instance.hp / 100f;
    }
}
