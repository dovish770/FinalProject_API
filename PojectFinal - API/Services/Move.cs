namespace PojectFinal___API.Services
{
    public class Move
    {
        public static Dictionary<string, (int x, int y)> Moves = new Dictionary<string, (int x, int y)>
        {
            {"n", (0, 1)},
            {"s", (0, -1)},
            {"w", (-1, 0)},
            {"e", (1, 0)},
            {"nw", (-1, 1)},
            {"ne", (1, 1)},
            {"sw", (-1, -1)},
            {"se", (1, -1)}
        };

    }
}
