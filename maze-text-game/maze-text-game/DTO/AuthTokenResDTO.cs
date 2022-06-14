namespace maze_text_game.DTO
{
    public record AuthTokenResDTO
    {
        public string Token { get; init; }

        public AuthTokenResDTO(string token)
        {
            this.Token = token;
        }
    }
}