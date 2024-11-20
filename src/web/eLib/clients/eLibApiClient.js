import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.ELIB_API_URL,
});

export default apiClient;
