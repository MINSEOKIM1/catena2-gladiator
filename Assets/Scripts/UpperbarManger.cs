using System;
using Bucket.Manager;
using TMPro;
using UnityEngine;

public class UpperbarManger : MonoBehaviour
{
    public TMP_Text date;
    public TMP_Text popularity;
    public TMP_Text money;
    public TMP_Text power;
    public TMP_Text time;

    private void Update()
    {
        date.text = $"{ScheduleManager.Instance.Date}일차";
        popularity.text = $"인기도 : {DataManager.Instance.popularity}%";
        money.text = $"돈 : {DataManager.Instance.money} gold";
        power.text = $"전투력 : {ManagementPhaseManager.Instance.fighters[0].GetTotalPower()}";
        time.text = $"현재 시간 : {ScheduleManager.Instance.Time}";
    }
}
