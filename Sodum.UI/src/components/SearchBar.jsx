import React, { useState, useEffect } from "react";

const SearchBar = ({ query, setQuery, searchType }) => {
  const [suggestions, setSuggestions] = useState([]);
  const [activeSuggestion, setActiveSuggestion] = useState(0);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      // Só busca se tiver 3 letras ou mais e se as sugestões estiverem ativas (evita buscar quando seleciona)
      if (query.trim().length >= 3 && showSuggestions) {
        setIsLoading(true);
        try {
          // Traduz o searchType do seu App.jsx para a sua API .NET
          const tipoApi = searchType === "Game" ? "jogo" : "musica";
          const API_URL = `http://localhost:5227/api/search/autocomplete?q=${encodeURIComponent(query)}&tipo=${tipoApi}`;

          const response = await fetch(API_URL);
          if (response.ok) {
            const data = await response.json();
            setSuggestions(data);
          }
        } catch (error) {
          console.error("Erro ao buscar sugestões:", error);
        } finally {
          setIsLoading(false);
        }
      } else {
        setSuggestions([]);
      }
    }, 300);

    return () => clearTimeout(delayDebounceFn);
  }, [query, searchType, showSuggestions]);

  // Quando o usuário digita algo
  const handleChange = (e) => {
    setQuery(e.target.value);
    setShowSuggestions(true); // Abre a caixa de sugestões
  };

  // Navegação por teclado
  const handleKeyDown = (e) => {
    if (!showSuggestions || suggestions.length === 0) return;

    if (e.key === "Enter") {
      e.preventDefault(); // Evita que o Enter envie o formulário principal imediatamente
      const selected = suggestions[activeSuggestion];
      setQuery(selected.nome);
      setShowSuggestions(false);
    } else if (e.key === "ArrowUp") {
      if (activeSuggestion === 0) return;
      setActiveSuggestion(activeSuggestion - 1);
    } else if (e.key === "ArrowDown") {
      if (activeSuggestion === suggestions.length - 1) return;
      setActiveSuggestion(activeSuggestion + 1);
    }
  };

  // Quando clica em uma sugestão
  const handleSelect = (nome) => {
    setQuery(nome);
    setShowSuggestions(false); // Fecha a lista
  };

  return (
    <div style={{ position: "relative", width: "100%", flex: 1 }}>
      <input
        type="text"
        value={query}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        placeholder={
          searchType === "Game"
            ? "Ex: Resident Evil, Skyrim..."
            : "Ex: System of a Down..."
        }
        // Aplica o estilo que o seu .input-group > input provavelmente já tem
        style={{
          width: "100%",
          padding: "10px",
          fontSize: "16px",
          boxSizing: "border-box",
        }}
        autoComplete="off"
      />

      {isLoading && (
        <div style={{ position: "absolute", right: "10px", top: "12px" }}>
          ⏳
        </div>
      )}

      {showSuggestions && query.length >= 3 && (
        <ul
          style={{
            position: "absolute",
            top: "100%",
            left: 0,
            right: 0,
            backgroundColor: "#1e1e1e", // Cor escura para combinar com seu layout
            color: "#fff",
            border: "1px solid #333",
            borderRadius: "4px",
            listStyle: "none",
            padding: 0,
            margin: 0,
            zIndex: 1000,
            maxHeight: "250px",
            overflowY: "auto",
            textAlign: "left",
          }}
        >
          {suggestions.length === 0 && !isLoading ? (
            <li style={{ padding: "10px", color: "#888" }}>
              Nenhum resultado encontrado
            </li>
          ) : (
            suggestions.map((suggestion, index) => {
              const isActive = index === activeSuggestion;
              return (
                <li
                  key={suggestion.id}
                  onClick={() => handleSelect(suggestion.nome)}
                  onMouseEnter={() => setActiveSuggestion(index)}
                  style={{
                    padding: "12px 10px",
                    cursor: "pointer",
                    backgroundColor: isActive ? "#333" : "transparent", // Destaque ao passar o mouse
                    display: "flex",
                    justifyContent: "space-between",
                    borderBottom: "1px solid #2a2a2a",
                  }}
                >
                  <span>{suggestion.nome}</span>
                  <span style={{ fontSize: "0.8em", color: "#aaa" }}>
                    {suggestion.tipo === "Jogo" ? "🎮" : "🎵"}
                  </span>
                </li>
              );
            })
          )}
        </ul>
      )}
    </div>
  );
};

export default SearchBar;
