using System;


namespace BotExample.Model
{
    [Serializable]
    public class Character
    {
        public Character() { }

        public Character(string name, string profession, string information, string imageurl)
        {
            Name = name;
            Profession = profession;
            Information = information;
            Imageurl = imageurl;
        }

        public string Name { get; set; }

        public string Profession { get; set; }

        public string Information { get; set; }

        public string Imageurl { get; set; }
    }
}