using UnityEngine;

namespace Bucket
{
    public class Player : Fighter
    {
        /// <summary>
        /// min 1 max 10
        /// </summary>
        public int AttackDifficulty
        {
            get => attackDifficulty;
            set => attackDifficulty = Mathf.Clamp(value, 1, 10);
        }
        private int attackDifficulty = 1;

        /// <summary>
        /// max combo number should be bigger than 1
        /// </summary>
        public int MaxComboNumber
        {
            get => maxComboNumber;
            set => maxComboNumber = Mathf.Max(value, 1);
        }
        private int maxComboNumber = 3;
    
        public Player(string name, int basicDamage, Equipments equipments) : base(name, basicDamage, equipments)
        {
        }

        public void InitPlayer(int attackDifficulty, int maxComboNumber)
        {
            this.attackDifficulty = attackDifficulty;
            this.maxComboNumber = maxComboNumber;
        }
    }
}
