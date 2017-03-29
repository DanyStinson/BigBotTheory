using System.Collections.Generic;

namespace BotExample.Model
{
    public class BigBangTheoryClient
    {
        private Dictionary<string, Character> characters = new Dictionary<string, Character>()
        {
            {"leonard", new Character("Leonard Hofstadter", "Experimental Physicist" ,"insert information here" ,"insert image url here")},
            {"penny", new Character("Penny", "Aspiring Actress" ,"insert information here" ,"insert image url here")},
            {"raj", new Character("Rajesh Koothrappali", "Particle Astrophysicist" ,"insert information here" ,"insert image url here")},
            {"howard", new Character("Howard Wolowitz", "Aerospace Engineer" ,"insert information here" ,"insert image url here")}
        };

        private Dictionary<string, string> plans = new Dictionary<string, string>()
        {
            { "monday", "get a Thai takeout!" },
            { "tuesday", "have a cheeseburger at the Cheesecake Factory!" },
            { "wednesday", "play Halo with your friends!" },
            { "thursday", "have a nice slice of pizza!" },
            { "friday", "get a chinese takeaway!" },
            { "saturday", "do some of your laundry!" },
            { "sunday", "relax at home and do some physics!" }
        };

        public Character GetCharacter(string name)
            => characters.ContainsKey(name.ToLower()) ? characters[name.ToLower()] : null;

        public string GetPlan(string dayOfWeek)
            => plans[dayOfWeek.ToLower()];
    }
}
