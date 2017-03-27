using System.Collections.Generic;
using System.Linq;

namespace BotExample.Model
{
    public class BigBangTheoryClient
    {
        private Dictionary<string, Character> characters = new Dictionary<string, Character>()
        {
            {"leonard", new Character("Leonard Hofstadter", "Experimental Physicist" ,"Leonard is my friend" ,"http://vignette3.wikia.nocookie.net/thebigbangtheory/images/b/bf/250px-Leonard.jpg/revision/latest?cb=20120917154638&path-prefix=es")},
            {"penny", new Character("Penny", "Aspiring Actress" ,"Penny is my friend" ,"https://upload.wikimedia.org/wikipedia/en/4/41/Penny_bigbangtheory.jpg")},
            {"raj", new Character("Rajesh Koothrappali", "Particle Astrophysicist" ,"Rajesh is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/9/97/Raj.jpg/revision/20110809180135")},
            {"howard", new Character("Howard Wolowitz", "Aerospace Engineer" ,"Howard is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/6/6a/Howardwolowitz.jpg/revision/latest/top-crop/width/240/height/240?cb=20100425200930")}
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

        public IEnumerable<Character> GetAllCharacters()
        => characters.Select(c => c.Value).ToList();

    }
}
