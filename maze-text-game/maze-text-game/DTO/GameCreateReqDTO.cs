namespace maze_text_game.DTO
{
    public record GameCreateReqDTO
    {
        public int PlayerLimit { get; init; }
        public int MapWidth { get; init; }
        public int MapHeight { get; init; }
    }
}