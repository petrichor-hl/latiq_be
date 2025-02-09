using LaTiQ.Core.Entities;

namespace LaTiQ.WebAPI.Constants
{
    public class TopicData
    {
        private static readonly List<string> fruits = new() { "Orange", "Banana", "Mango", "Kiwi", "Apple", "Grapes", "Pineapple", "Strawberry", "Watermelon", "Peach" };
        private static readonly List<string> nationalFlag = new() { "Mexico", "Germany", "VietNam", "Japan", "Canada", "Australia", "Brazil" };
        private static readonly List<string> animals = new() { "Dog", "Cat", "Mouse", "Chicken", "Cow", "Pig", "Goat", "Duck", "Rabbit", "Horse", "Parrot", "Sheep", "Tiger", "Lion", "Fox", "Monkey", "Elephant" };

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
                .SetId(Guid.Parse("d712e3d0-7594-4e93-93a0-44ce8a2a5755"))
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
                .SetId(Guid.Parse("bcb9d8f5-6d25-4792-a815-eff00c92071d"))
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
                .SetImageUrl("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ32HfdVY5nMrFgAb4dNa5QniBkK2ZK76PUTQ&s")
                .SetWords(pokemon)
                .Build();

        public static readonly List<Topic> Topics = new List<Topic>
        {
                animeTopic, 
                pokemonTopic, 
                fruitTopic, 
                nationalFlagTopic, 
                animalTopic, 
                programmingLanguagesTopic
        };

    }

}
