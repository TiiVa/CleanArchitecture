using CleanArchitecture.Application.Interfaces.ServiceInterfaces;

namespace CleanArchitecture.Infrastructure.ExternalServices;

public class PasswordGeneratorService : IPasswordGeneratorService
{

    


    private readonly string[] Words =
    [
        "eventSphere",
        "1",
        "bok",
        "2",
        "ek",
        "ask",
        "3",
        "al",
        "gran",
        "4",
        "tall",
        "plum",
        "orange",
        "?",
        "citron",
        "lime",
        "strawberry",
        "raspberry",
        "!",
        "lemon",
        "apple",
        "wood",
        "mushroom",
        "fisk",
        "5",
        "hund",
        "cat",
        "orm",
        "spider",
        "scorpion",
        "wolf",
        "lion",
        "6",
        "tiger",
        "elephant",
        "7",
        "crocodile",
        "penguin",
        "zebra",
        "giraph",
        "winter",
        "summer",
        "flower",
        "bumblebee",
        "bi",
        "bird",
        "myra",
        "butterfly",
        "dragonfly",
        "korp",
        "duva",
        "sparv",
        "svala",
        "kaja",
        "falk",
        "8",
        "uggla",
        "kattuggla",
        "domkraft",
        "skruvmejsel",
        "spik",
        "skiftnyckel",
        "tumstock",
        "vattenpass",
        "hammare",
        "borr",
        "vanilj",
        "choklad",
        "honung",
        "eld",
        "M90",
        "9",
        "@",
        "regemente",
        "bataljon",
        "kompani",
        "pluton",
        "grupp",
        "gripen",
        "paj",
        "många",
        "kung",
        "drottning",
        "prins",
        "prinsessa",
    ];


    public string Generate()
    {
      
            const int minCharLength = 16;
            const int amountOfIterations = 4;
            var password = string.Empty;
            var random = new Random();

            for (var i = 0; i < amountOfIterations; i++)
            {
                var word = Words[random.Next(0, Words.Length)];

                if (password.ToLower().Contains(word.ToLower()))
                {
                    i--;
                    continue;
                }

                if (random.Next(0, 2) == 1)
                {
                    word = word.ToUpper();
                }

                if (i > 0 && i < amountOfIterations)
                {
                    password += "-";
                }

                password += word;

                if (i == amountOfIterations - 1 && password.Length < minCharLength)
                {
                    i--;
                }
            }

            if (!password.Any(char.IsDigit))
            {
                password += $"-{random.Next(0, 10)}";
            }

            
            return password;
        
      

       
    }
}