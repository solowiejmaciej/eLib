import axios from "axios";

class ElibApiClient {
  constructor() {
    if (!import.meta.env.VITE_ELIB_NS_API_URL) {
      console.error(
        "VITE_ELIB_NS_API_URL is not defined in environment variables"
      );
    }

    console.log("Base ns URL:", import.meta.env.VITE_ELIB_NS_API_URL);

    this.client = axios.create({
      baseURL: import.meta.env.VITE_ELIB_NS_API_URL,
      headers: {
        "Content-Type": "application/json",
      },
    });

    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        console.error("API Error:", error.response || error);
        return Promise.reject(error);
      }
    );
  }

  async getNotifications() {
    try {
      const response = await this.client.get("/notifications");
      return response.data;
    } catch (error) {
      console.error("Error fetching notifications:", error);
      throw error;
    }
  }
}

const apiClient = new ElibApiClient();
export default apiClient;
