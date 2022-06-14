namespace maze_text_game
{
    public record Player
    {
        public string Name;
        public bool HasVoted = false;

        public Player(string name)
        {
            this.Name = name;
        }
    }
}
