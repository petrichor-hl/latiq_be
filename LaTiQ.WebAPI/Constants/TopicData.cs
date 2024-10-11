using LaTiQ.Core.Entities.Topic;
using System.Collections.Generic;

namespace LaTiQ.WebAPI.Constants
{
    public class TopicData
    {
        private static readonly List<string> fruits = new() { "Orange", "Banana", "Mango", "Kiwi", "Apple", "Grapes", "Pineapple", "Strawberry", "Watermelon", "Peach" };
        private static readonly List<string> nationalFlag = new() { "VietNam", "Germany", "Mexico", "Japan", "Canada", "Australia", "Brazil" };
        private static readonly List<string> animals = new() { "Dog", "Cat", "Mouse", "Chicken", "Cow", "Pig", "Goat", "Duck", "Rabbit", "Horse", "Parrot" };

        private static readonly List<string> programmingLanguages = new()
            {
                    "Java",
                    "Python",
                    "C++",
                    "JavaScript",
                    "C#",
                    "PHP",
                    "Ruby",
                    "Swift",
                    "Go",
                    "Kotlin"
            };

        private static readonly List<string> anime = new() {
                    "Naruto",
                    "One Piece",
                    "Dragon Ball",
                    "Attack on Titan",
                    "My Hero Academia",
                    "Death Note",
                    "Fullmetal Alchemist",
                    "Sword Art Online",
                    "Demon Slayer",
                    "Tokyo Ghoul"
            };

        private static readonly List<string> pokemon = new() {
                    "Pikachu",
                    "Charizard",
                    "Bulbasaur",
                    "Squirtle",
                    "Jigglypuff",
                    "Meowth",
                    "Eevee",
                    "Mewtwo",
                    "Snorlax",
                    "Gengar"
            };

        private static readonly Topic fruitTopic = new Topic
                .Builder()
                .SetId(Guid.NewGuid())
                .SetName("Fruits")
                .SetImageUrl("https://www.mooringspark.org/hs-fs/hubfs/bigstock-Fresh-Fruits-assorted-Fruits-C-365480089%20Large.jpeg")
                .SetWords(fruits)
                .Build();

        private static readonly Topic nationalFlagTopic = new Topic
                    .Builder()
                    .SetId(Guid.NewGuid())
                    .SetName("National Flag")
                    .SetImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/2/21/Flag_of_Vietnam.svg/2560px-Flag_of_Vietnam.svg.png")
                    .SetWords(nationalFlag)
                    .Build();

        private static readonly Topic animalTopic = new Topic
                .Builder()
                .SetId(Guid.NewGuid())
                .SetName("Animals")
                .SetImageUrl("https://media.newyorker.com/photos/62c4511e47222e61f46c2daa/4:3/w_2663,h_1997,c_limit/shouts-animals-watch-baby-hemingway.jpg")
                .SetWords(animals)
                .Build();

        private static readonly Topic programmingLanguagesTopic = new Topic
                .Builder()
                .SetId(Guid.NewGuid())
                .SetName("Programming Languages")
                .SetImageUrl("https://i.pinimg.com/originals/8e/23/1e/8e231e0aa5c7acb23e299ae2f4889fbe.png")
                .SetWords(programmingLanguages)
                .Build();

        private static readonly Topic animeTopic = new Topic
                .Builder()
                .SetId(Guid.NewGuid())
                .SetName("Anime")
                .SetImageUrl("https://i.ytimg.com/vi/xXmXM0qRMbo/hq720.jpg")
                .SetWords(anime)
                .Build();

        private static readonly Topic pokemonTopic = new Topic
                .Builder()
                .SetId(Guid.NewGuid())
                .SetName("Pokemon")
                .SetImageUrl("https://yt3.googleusercontent.com/wzEypbVsmY9BI-IbLwVius4UvC2rejtJB_PTXAdPpYXQ07EIjl5Ms55NCFq_dILwONpxrzE2xA=s900-c-k-c0x00ffffff-no-rj")
                .SetWords(pokemon)
                .Build();

        public static List<Topic> Topics = new List<Topic> { fruitTopic, nationalFlagTopic, animalTopic, programmingLanguagesTopic, animeTopic, pokemonTopic };

    }

}
