namespace maze_text_game.DTO
{
    public record GameCreateResDTO
    {
        public string GameGuid { get; init; }

        public GameCreateResDTO(string gameGuid)
        {
            this.GameGuid = gameGuid;
        }
    }
}
