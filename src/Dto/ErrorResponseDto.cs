using System.Collections.Generic;

namespace IATec.Shared.HttpClient.Dto
{
    public class ErrorResponseDto
    {
        public List<string> Messages { get; set; } = new List<string>();
    }
}