namespace P_P.characters
{
    public class YellowSquareCharacter : BaseCharacter
    {
        public new string icon;
        public YellowSquareCharacter(string icon, string ability, ref int movementCapacity, ref int playerRow, ref int playerColumn)
            : base(icon, ability, movementCapacity, playerColumn, playerRow)
        {
            this.icon = icon ?? throw new ArgumentNullException(nameof(icon));
        }
    }
}