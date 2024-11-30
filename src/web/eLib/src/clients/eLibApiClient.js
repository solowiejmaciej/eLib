import axios from "axios";

class ElibApiClient {
  constructor() {
    if (!import.meta.env.VITE_ELIB_API_URL) {
      console.error(
        "VITE_ELIB_API_URL is not defined in environment variables"
      );
    }

    console.log("Base URL:", import.meta.env.VITE_ELIB_API_URL);

    this.client = axios.create({
      baseURL: import.meta.env.VITE_ELIB_API_URL,
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

  async getBooks(searchPhrase = "", pageNumber = 1, pageSize = 10) {
    try {
      const response = await this.client.get("/books", {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchFraze: searchPhrase,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching books:", error);
      throw error;
    }
  }

  async getBookById(id) {
    try {
      const response = await this.client.get(`/books/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching book:", error);
      throw error;
    }
  }

  async getAuthorById(id) {
    try {
      const response = await this.client.get(`/authors/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching author:", error);
      throw error;
    }
  }
}

const apiClient = new ElibApiClient();
export default apiClient;
