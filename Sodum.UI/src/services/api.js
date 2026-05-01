import axios from "axios";

const api = axios.create({
  // Se estiver testando local: 'https://localhost:7200/api'
  // Se for a nuvem: 'https://sodum-api.onrender.com/api'
  baseURL: "https://sodum-api.onrender.com/api",
});

export default api;
