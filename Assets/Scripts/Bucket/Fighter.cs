using System;
using System.Collections.Generic;
using Bucket;
using UnityEngine;

namespace Bucket
{
    [Serializable]
    public class Fighter
    {
        public string FighterName
        {
            get { return fighterName; }
        }
        [SerializeField]
        private string fighterName;

        public int CurrentRank
        {
            get { return currentRank; }
            set => currentRank = value;
        }

        [SerializeField]
        private int currentRank;

        public int Tiredness
        {
            get { return tiredness; }
            set { tiredness = Mathf.Clamp(value, 0, 100); }
        }
        [SerializeField]
        private int tiredness;
        
        public float MatchmakingRate
        {
            get { return matchmakingRate; }
        }
        [SerializeField]
        private float matchmakingRate = 100;

        private const float mmrConstant = 20;

        public int BasicDamage
        {
            get { return basicDamage; }
        }
        [SerializeField]
        private int basicDamage = 0;

        public int BasicDefense
        {
            get { return basicDefense; }
        }
        [SerializeField]
        private int basicDefense = 0;
        
        public int Popularity
        {
            get { return popularity; }
        }
        [SerializeField]
        private int popularity = 0;

        public Equipments Equipments
        {
            get { return equipments; }
        }

        public Sprite fighterSprite;

        [SerializeField] private Equipments equipments;

        public Fighter(string name, int basicDamage, Equipments equipments, Sprite fighterSprite = null)
        {
            this.fighterName = name;
            this.basicDamage = basicDamage;
            this.equipments = equipments;
            this.fighterSprite = fighterSprite;
        }

        public int GetTotalPower()
        {
            return GetTotalAttackPower() + GetTotalDefensePower();
        }

        public int GetTotalAttackPower()
        {
            return basicDamage + Equipments.GetAttackPower();
        }

        public int GetTotalDefensePower()
        {
            return basicDefense + Equipments.GetDefensePower();
        }

        public void AddBasicDamage(int damage)
        {
            basicDamage += damage;
        }

        public void AddBasicDefense(int defense)
        {
            basicDefense += defense;
        }
        
        public void WinMatch(Fighter enemy)
        {
            float winrate = CaculateWinRate(this, enemy);
            matchmakingRate += mmrConstant * (1 - winrate);
        }
        
        public void LoseMatch(Fighter enemy)
        {
            float winrate = CaculateWinRate(this, enemy);
            matchmakingRate -= mmrConstant * winrate;
        }

        public static float CaculateWinRate(Fighter fighter1, Fighter fighter2)
        {
            float f = fighter2.matchmakingRate + fighter2.GetTotalPower() - fighter1.matchmakingRate -
                      fighter1.GetTotalPower();
            return f >= 0 ? Mathf.Clamp01(0.5f * (Mathf.Exp(-0.025f * (int)(f)))) 
                : Mathf.Clamp01(1 - 0.5f * (Mathf.Exp(-0.025f * (int)(f))));
        }
    }

    public class FighterSortComparer : IComparer<Fighter>
    {
        public int Compare(Fighter fighter1, Fighter fighter2)
        {
            return fighter1.GetTotalPower() + fighter1.MatchmakingRate < fighter2.GetTotalPower() + fighter2.MatchmakingRate ? -1 : 1;
        }
    }
}