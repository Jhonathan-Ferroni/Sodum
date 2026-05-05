using System;
using System.Collections.Generic;
using System.Linq;


namespace Sodum.API.DTOs
{
    public class SuggestionDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; } // Opcional: para identificar se é "Jogo" ou "Música"
    }
}