/*using System;
using System.Collections.Generic;

public class PlayerState
{
    public int UID { get; private set; }
    public string ID { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; } = 10;
    public List<Card> Cards { get; private set; }

    public PlayerState(int uid)
    {
        UID = uid;
        Mana = MaxMana;
        Cards = new List<Card>();
    }

    public void RegenerateMana()
    {
        Mana = Math.Min(Mana + 1, MaxMana);
    }

    public void ReplaceCard(int cardIndex)
    {
        Cards[cardIndex] = CardFactory.DrawNewCard();
    }

    public bool CanSummon(int cost)
    {
        return Mana >= cost;
    }
}
*/