import axios from "axios";

class ElibNotificationServiceApiClient {
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

  async getNotificationsForUser(userId, pageNumber = 1, pageSize = 10) {
    try {
      const response = await this.client.get(`/notifications/${userId}`, {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching notifications:", error);
      throw error;
    }
  }

  async addNotification(message, userId, channel, title, phoneNumber, email) {
    try {
      const response = await this.client.post("/notifications", {
        message,
        userId,
        channel,
        title,
        phoneNumber,
        email,
      });
      return response.data;
    } catch (error) {
      console.error("Error adding notification:", error);
      throw error;
    }
  }
}

const notificationServiceApiClient = new ElibNotificationServiceApiClient();
export default notificationServiceApiClient;
