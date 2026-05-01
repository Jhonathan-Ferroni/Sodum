import { useState } from "react";
import api from "./services/api";
import "./App.css";

function App() {
  const [query, setQuery] = useState("");
  const [searchType, setSearchType] = useState("Game"); // 'Game' ou 'Music'
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState("");

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!query) return;

    setLoading(true);
    setError("");
    setResult(null);

    try {
      // Monta a rota dinamicamente dependendo do que o usuário escolheu
      const endpoint =
        searchType === "Game"
          ? `/Game/recommend?gameName=${query}`
          : `/Music/recommend?trackName=${query}`;

      const response = await api.get(endpoint);
      setResult(response.data);
    } catch (err) {
      setError("Não foi possível encontrar recomendações. Tente outro termo.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app-container">
      <header className="header">
        <h1>Sodum</h1>
        <p>----------------------</p>
        <p>A sintonia perfeita entre seus jogos e músicas.</p>
      </header>

      <main className="main-content">
        <form onSubmit={handleSearch} className="search-form">
          <div className="toggle-container">
            <button
              type="button"
              className={searchType === "Game" ? "active" : ""}
              onClick={() => setSearchType("Game")}
            >
              Buscar Jogo
            </button>
            <button
              type="button"
              className={searchType === "Music" ? "active" : ""}
              onClick={() => setSearchType("Music")}
            >
              Buscar Música
            </button>
          </div>

          <div className="input-group">
            <input
              type="text"
              placeholder={
                searchType === "Game"
                  ? "Ex: Resident Evil, Skyrim..."
                  : "Ex: System of a Down, DaBaby..."
              }
              value={query}
              onChange={(e) => setQuery(e.target.value)}
            />
            <button type="submit" disabled={loading}>
              {loading ? "Pensando..." : "Recomendar"}
            </button>
          </div>
        </form>

        {error && <div className="error-message">{error}</div>}

        {result && (
          <div className="results-container">
            {/* Bloco do Item Original (Jogo ou Música pesquisada) */}
            <div className="original-item">
              <h2>
                Baseado em:{" "}
                {searchType === "Game" ? result.game?.name : result.music?.name}
              </h2>
              {searchType === "Game" && result.game?.headerImage && (
                <img
                  src={result.game.headerImage}
                  alt="Capa do Jogo"
                  className="cover-image"
                />
              )}
            </div>

            {/* Bloco das Recomendações da IA */}
            <div className="recommendations-grid">
              <h3>Recomendações da IA:</h3>
              {result.recommendations.map((rec, index) => (
                <div key={index} className="recommendation-card">
                  <h4>
                    {searchType === "Game" ? rec.trackName : rec.gameName}
                  </h4>
                  <h5>
                    {searchType === "Game" ? rec.artistName : rec.developer}
                  </h5>
                  <p>
                    <strong>Por que combina?</strong> {rec.matchReason}
                  </p>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;
