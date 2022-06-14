namespace maze_text_game
{
    public record Player
    {
        public string Name { get; set; }
        public bool HasVoted { get; set; }

        public Player(string name)
        {
            this.Name = name;
        }

        public Player()
        {
            this.HasVoted = false;
        }
    }
}
