using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditorInternal;

public static class GenDirector
{
    public static int credits;
    private class SpawnCard
    {
        public string name;
        public List<string> conditions;
        public int cost;
        public int weight;
        public SpawnCard(int _cost, int _weight, string _name, List<string> _condition)
        {
            name = _name;
            conditions = _condition;
            cost = _cost;
            weight = _weight;
        }
    }
    private class Deck
    {
        public List<SpawnCard> cards;
        public Deck(List<SpawnCard> spawnCards)
        {
            cards = spawnCards;
        }
        public int UseCard(string name)
        {
            SpawnCard used = null;
            foreach (var card in cards)
                if (card.name == name) used = card;
            if (used != null)
            {
                used.weight--;
                if (used.weight == 0) cards.Remove(used);
                return used.cost;
            }
            else return 0;
        }
        public bool Contains(string name)
        {
            foreach (var card in cards)
                if (card.name == name) return true;
            return false;
        }
    }

    private static Deck deck;
    private static Deck hand;

    public static string Generate(int amountOfCredits, List<string> options, Pole pole)
    {
        deck = new Deck(new List<SpawnCard>()
            {
                new SpawnCard(100, 1, "UseOfPoints", new List<string>()),
                new SpawnCard(50, pole.systemPath.dots.Count / 4, "AddPoint", new List<string>(){"UseOfPoints"}),

                new SpawnCard(100, 1, "UseOfColors", new List<string>()),
                new SpawnCard(40, pole.zone.Count - 1, "AddColor", new List<string>(){"UseOfColors"}),
                new SpawnCard(50, 1, "UseOfClrRings", new List<string>(){"UseOfColors"}),
                new SpawnCard(50, (pole.height - 1) * (pole.width - 1),"ClrRing", new List<string>(){"UseOfColors", "UseOfClrRings"}),
                new SpawnCard(50, 1, "UseOfClrStars", new List<string>(){"UseOfColors"}),
                new SpawnCard(50, (pole.height - 1) * (pole.width - 1), "ClrStar", new List<string>(){"UseOfColors", "UseOfClrStars"}),

                new SpawnCard(100, 1, "UseOfShapes", new List<string>()),
                new SpawnCard(50, pole.zone.Count, "SplitZoneOnShapes", new List<string>(){"UseOfShapes"}),
                new SpawnCard(50, (pole.height - 1) * (pole.width - 1) / 4, "AddShape", new List<string>(){"UseOfShapes", "SplitZoneOnShapes"}),
            });
        hand = new Deck(new List<SpawnCard>());
        credits = amountOfCredits;
        foreach(string opt in options)
        {
            credits -= deck.UseCard(opt);
        }
        while (credits > 0)
        {
            List<SpawnCard> spawns = new List<SpawnCard>();
            foreach(var opt in deck.cards)
            {
                bool req = true;
                foreach (var cond in opt.conditions)
                    if (!hand.Contains(cond)) req = false;
                if (!req) continue;
                for (int i = 0; i < opt.weight; ++i)
                    spawns.Add(opt);
            }
            var choice = spawns[Core.PolePreferences.MyRandom.GetRandom() % spawns.Count];
            hand.cards.Add(choice);
            credits -= deck.UseCard(choice.name);
        }
        int pt = 0, color = 1, cr = 0, cs = 0, zs = 0, sh = 0;
        foreach (var opt in hand.cards) 
        {
            switch(opt.name)
            {
                case "AddPoint":
                    pt++;
                    break;
                case "AddColor":
                    color++;
                    break;
                case "ClrRing":
                    cr++;
                    break;
                case "ClrStar":
                    cs++;
                    break;
                case "SplitZoneOnShapes":
                    zs++;
                    break;
                case "AddShape":
                    sh++;
                    break;
                default:
                    break;
            }
        }
        string debug = "pt:" + pt + " color:" + color + " cr:" + cr + " cs:" + cr + " zs:" + zs + " sh:" + sh;
        return debug;
    }
}

