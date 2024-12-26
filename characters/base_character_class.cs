using P_P.board;
using P_P.PrintingMethods;
using P_P;
using P_P.tramps;
using Spectre.Console;

namespace P_P.characters
{
    public class BaseCharacter
    {
        public string Icon;
        public string Ability;
        public int MovementCapacity;
        public int PlayerColumn;
        public int PlayerRow;
        public int Live = 100;
        public  PrintingMethods.PrintingMethods printingMethods = new PrintingMethods.PrintingMethods();
        public BaseCharacter(string name, string ability, int movementCapacity, int playerColumn, int playerRow)
        {
            this.Icon = name ?? throw new ArgumentNullException(nameof(name));
            this.Ability = ability ?? throw new ArgumentNullException(nameof(ability));
            this.MovementCapacity = movementCapacity;
            this.PlayerColumn = playerColumn;
            this.PlayerRow = playerRow;
        }

        public void Move(ref int playerRow, ref int playerColumn, ref int movementCapacity, Shell[,] gameBoard, BaseCharacter character , ConsoleKeyInfo key , List<BaseCharacter> characters , List<BaseTramp> tramps)
        {
            int newRow = playerRow;
            int newColumn = playerColumn;

            switch (key.Key)
            {
                case ConsoleKey.W:
                    newRow--;
                    break;
                case ConsoleKey.S:
                    newRow++;
                    break;
                case ConsoleKey.A:
                    newColumn--;
                    break;
                case ConsoleKey.D:
                    newColumn++;
                    break;
            }

            if (newRow >= 0 && newRow < gameBoard.GetLength(0) &&
                newColumn >= 0 && newColumn < gameBoard.GetLength(1) &&
                gameBoard[newRow, newColumn].GetType() != typeof(wall) &&
                !gameBoard[newRow, newColumn].HasCharacter)
            {
                // Limpiar posición anterior
                gameBoard[playerRow, playerColumn].HasCharacter = false;
                gameBoard[playerRow, playerColumn].CharacterIcon = null;
                gameBoard[playerRow, playerColumn] = new path("⬜️");

                // Actualizar nueva posición
                playerRow = newRow;
                playerColumn = newColumn;
                gameBoard[playerRow, playerColumn].HasCharacter = true;
                gameBoard[playerRow, playerColumn].CharacterIcon = this.Icon;
                // gameBoard[playerRow, playerColumn].IsPath = false;
                // Actualizar el tablero
                movementCapacity--;
                printingMethods.layout["Bottom"].Update(new Panel("Activa tu habilidad con H o muevete con W,A,S,D").Expand());
                printingMethods.PrintGameSpectre(gameBoard , character , characters , tramps);
                
            }
        }

        public void PlaceCharacter(Shell[,] gameBoard, BaseCharacter character)
        {
            gameBoard[character.PlayerRow, character.PlayerColumn].HasCharacter = true;
            gameBoard[character.PlayerRow, character.PlayerColumn].CharacterIcon = character.Icon;
        }

        public void TakeTurn(Shell[,] gameBoard, BaseCharacter character, List<BaseTramp> tramps , List<BaseCharacter> characters)
        {
           
            
            //revisar como hacer mejor esoo
            printingMethods.layout["Bottom"].Update(new Panel("Turno de " + character.Icon + "\nPulsa C para cambiar de personaje o cualquier otra tecla para moverte").Expand());
            printingMethods.PrintGameSpectre(gameBoard , character , characters , tramps);
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.C)
            {
                printingMethods.PrintGameSpectre(gameBoard , character , characters , tramps);
                // AnsiConsole.WriteLine("Introduce el personaje con el que quieres cambiar");
                int characterToChange = DisplayCharactersToChange(characters , character , gameBoard , tramps);
                character.ChangeWith(character , characters[characterToChange] , gameBoard);
                
                printingMethods.layout["Bottom"].Update(new Panel("Te has cambiado con el personaje " + characters[characterToChange].Icon).Expand());
                printingMethods.PrintGameSpectre(gameBoard , character , characters , tramps);
                //AnsiConsole.WriteLine("Te has cambiado con el personaje " + characters[characterToChange].Icon);
            }
            else{
                printingMethods.layout["Bottom"].Update(new Panel("Activa tu habilidad con H o muevete con W,A,S,D").Expand());
                printingMethods.PrintGameSpectre(gameBoard , character , characters , tramps);
                while (character.MovementCapacity != 0)
                {
                    ConsoleKeyInfo key2 = Console.ReadKey();
                    if (key2.Key == ConsoleKey.H)
                    {
                        character.UseAbility(gameBoard , character , tramps, characters);
                        printingMethods.layout["Bottom"].Update(new Panel("Habilidad usada").Expand());
                        printingMethods.PrintGameSpectre(gameBoard, character, characters , tramps);
                        //AnsiConsole.WriteLine("Habilidad usada");
                    }
                    else
                    {
                        character.Move(ref character.PlayerRow, ref character.PlayerColumn, ref character.MovementCapacity, gameBoard, character , key2 , characters , tramps);
                    }
                    if (gameBoard[character.PlayerRow, character.PlayerColumn].HasObject)
                    {
                        string? objectType = gameBoard[character.PlayerRow, character.PlayerColumn].ObjectType;
                        {
                            if (objectType == "tramp")
                            {
                                foreach (BaseTramp tramp in tramps)
                                {
                                    if (tramp.trampId == gameBoard[character.PlayerRow, character.PlayerColumn].ObjectId)
                                    {
                                        tramp.Interact(gameBoard, character , characters , tramps);
                                    }
                                }
                            }
                        }
                        printingMethods.PrintGameSpectre(gameBoard, character, characters , tramps);
                    }
                }
            }
            MovementCapacity = 5;
            printingMethods.layout["Bottom"].Update(new Panel("Turno finalizado . Toca enter para pasar al siguiente jugador").Expand());
            printingMethods.PrintGameSpectre(gameBoard, character, characters , tramps);
            //AnsiConsole.WriteLine("Turno finalizado . Toca enter para pasar al siguiente jugador");
            Console.ReadKey();
            printingMethods.PrintGameSpectre(gameBoard, character, characters , tramps);
        }

        public virtual void UseAbility(Shell[,] gameboard , BaseCharacter character , List<BaseTramp> tramps , List<BaseCharacter> characters)
        {
            
        }
        public void ChangeWith(BaseCharacter character , BaseCharacter charactertoChange , Shell[,] gameBoard)
        {
            gameBoard[character.PlayerRow, character.PlayerColumn].HasCharacter = false;
            gameBoard[character.PlayerRow, character.PlayerColumn].CharacterIcon = null;
            gameBoard[charactertoChange.PlayerRow, charactertoChange.PlayerColumn].HasCharacter = false;
            gameBoard[charactertoChange.PlayerRow, charactertoChange.PlayerColumn].CharacterIcon = null;
            
            gameBoard[character.PlayerRow, character.PlayerColumn].HasCharacter = true;
            gameBoard[character.PlayerRow, character.PlayerColumn].CharacterIcon = charactertoChange.Icon;
            gameBoard[charactertoChange.PlayerRow, charactertoChange.PlayerColumn].HasCharacter = true;
            gameBoard[charactertoChange.PlayerRow, charactertoChange.PlayerColumn].CharacterIcon = character.Icon;
            
            int tempRow = character.PlayerRow;
            int tempColumn = character.PlayerColumn;
           
            character.PlayerRow = charactertoChange.PlayerRow;
            character.PlayerColumn = charactertoChange.PlayerColumn;
            charactertoChange.PlayerRow = tempRow;
            charactertoChange.PlayerColumn = tempColumn;

            this.MovementCapacity = 0;
        }
       public virtual int DisplayCharactersToChange(List<BaseCharacter> characters, BaseCharacter character, Shell[,] gameBoard, List<BaseTramp> tramps)
        {
            // Crear las opciones de personajes
            var posibleChangeCharacters = new List<string>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != character)
                {
                    posibleChangeCharacters.Add($"Personaje {i} : {characters[i].Icon}");
                }
            }

            int selectedIndex = 0; // Índice del personaje seleccionado

            // Usar AnsiConsole.Live para manejar las actualizaciones dinámicas
            AnsiConsole.Live(printingMethods.layout).Start(ctx =>
            {
                bool selectionMade = false;

                while (!selectionMade)
                {
                    // Actualizar el layout["Bottom"] con las opciones del menú
                    var menuContent = new Panel(
                        $"Elige al jugador con el que quieres cambiar:\n\n" +
                        string.Join("\n", posibleChangeCharacters.Select((option, index) =>
                            index == selectedIndex
                                ? $"[green]> {option}[/]" // Opción seleccionada
                                : $"  {option}"          // Opciones no seleccionadas
                        ))
                    ).Expand();

                    printingMethods.layout["Bottom"].Update(menuContent);
                    ctx.Refresh();

                    // Capturar la entrada del usuario
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex = (selectedIndex - 1 + posibleChangeCharacters.Count) % posibleChangeCharacters.Count;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex = (selectedIndex + 1) % posibleChangeCharacters.Count;
                            break;
                        case ConsoleKey.Enter:
                            selectionMade = true;
                            break;
                    }
                }

                // Acción tras seleccionar un personaje
                var selectedCharacter = characters[selectedIndex];
                printingMethods.layout["Bottom"].Update(
                    new Panel($"Te has cambiado con el personaje {selectedCharacter.Icon}").Expand()
                );
                ctx.Refresh();

                // Volver a imprimir el juego con la selección hecha
                printingMethods.PrintGameSpectre(gameBoard, character, characters, tramps);
            });
            string selectedCharacter = posibleChangeCharacters[selectedIndex];
            int selectedCharacterIndex = int.Parse(selectedCharacter.Split(' ')[1]);
            return selectedCharacterIndex;
        }
    }
}
