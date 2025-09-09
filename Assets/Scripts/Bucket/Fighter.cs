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

        public int BasicDamage
        {
            get { return basicDamage; }
        }
        [SerializeField]
        private int basicDamage = 0;

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

        [SerializeField] private Equipments equipments;

        public Fighter(string name, int basicDamage, Equipments equipments)
        {
            this.fighterName = name;
            this.basicDamage = basicDamage;
            this.equipments = equipments;
        }
    }

    public class FighterSortComparer : IComparer<Fighter>
    {
        public int Compare(Fighter fighter1, Fighter fighter2)
        {
            return fighter1.BasicDamage + fighter1.Equipments.GetPower() < fighter2.BasicDamage + fighter2.Equipments.GetPower() ? -1 : 1;
        }
    }
}