import axios from "axios";

class ElibApiClient {
  constructor() {
    if (!import.meta.env.VITE_ELIB_API_URL) {
      console.error(
        "VITE_ELIB_API_URL is not defined in environment variables"
      );
    }

    console.log("Base main URL:", import.meta.env.VITE_ELIB_API_URL);

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

  async deleteBook(id) {
    try {
      const response = await this.client.delete(`/books/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error deleting book:", error);
      throw error;
    }
  }

  async createBook(book) {
    try {
      const response = await this.client.post("/books", book);
      return response.data;
    } catch (error) {
      console.error("Error creating book:", error);
      throw error;
    }
  }

  async updateBook(id, book) {
    try {
      const response = await this.client.put(`/books/${id}`, book);
      return response.data;
    } catch (error) {
      console.error("Error updating book:", error);
      throw error;
    }
  }

  async getAuthors(searchPhrase = "", pageNumber = 1, pageSize = 10) {
    try {
      const response = await this.client.get("/authors", {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchFraze: searchPhrase,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching authors:", error);
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

  async loginWithEmailAndPassword(email, password) {
    try {
      const response = await this.client.post("/tokens/email", {
        email,
        password,
      });
      return response.data;
    } catch (error) {
      console.error("Error logging in:", error);
      throw error;
    }
  }

  async loginWithPhoneNumber(phoneNumber, password) {
    try {
      const response = await this.client.post("/tokens/phone", {
        phoneNumber,
        password,
      });
      return response.data;
    } catch (error) {
      console.error("Error logging in:", error);
      throw error;
    }
  }

  async registerUser(user) {
    try {
      const response = await this.client.post("/users", user);
      return response.data;
    } catch (error) {
      console.error("Error registering user:", error);
      throw error;
    }
  }

  async getUserById(id) {
    try {
      const response = await this.client.get(`/users/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching user:", error);
      throw error;
    }
  }

  async updateUser(id, user) {
    try {
      const response = await this.client.put(`/users/${id}`, {
        name: user.name,
        surname: user.surname,
        email: user.email,
        phoneNumber: user.phoneNumber,
      });
      return response.data;
    } catch (error) {
      console.error("Error updating user:", error);
      throw error;
    }
  }

  async deleteUser(id) {
    try {
      const response = await this.client.delete(`/users/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error deleting user:", error);
      throw error;
    }
  }

  async getUsers(searchPhrase = "", pageNumber = 1, pageSize = 10) {
    try {
      const response = await this.client.get("/users", {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchFraze: searchPhrase,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching users:", error);
      throw error;
    }
  }

  async addToReadingList(bookId) {
    // try {
    //   const response = await this.client.post(`/reading-list/${bookId}`);
    //   return response.data;
    // } catch (error) {
    //   console.error("Error adding to reading list:", error);
    //   throw error;
    // }
  }

  async removeFromReadingList(bookId) {
    // try {
    //   const response = await this.client.delete(`/reading-list/${bookId}`);
    //   return response.data;
    // } catch (error) {
    //   console.error("Error removing from reading list:", error);
    //   throw error;
    // }
  }

  async getReadingList() {
    // try {
    //   const response = await this.client.get("/reading-list");
    //   return response.data;
    // } catch (error) {
    //   console.error("Error fetching reading list:", error);
    //   throw error;
    // }
  }

  async getReservations(searchPhrase = "", pageNumber = 1, pageSize = 10) {
    try {
      const response = await this.client.get("/reservations", {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchFraze: searchPhrase,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching reservations:", error);
      throw error;
    }
  }

  async getReservationsByUserId(
    searchPhrase = "",
    pageNumber = 1,
    pageSize = 10,
    userId
  ) {
    try {
      const response = await this.client.get(`/reservations/user/${userId}`, {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchFraze: searchPhrase,
        },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching reservations:", error);
      throw error;
    }
  }

  async returnReservation(reservationId) {
    try {
      const response = await this.client.post(
        `/reservations/${reservationId}/return`
      );
      return response.data;
    } catch (error) {
      console.error("Error returning reservation:", error);
      throw error;
    }
  }

  async createReservation(reservation) {
    try {
      const response = await this.client.post("/reservations", reservation);
      return response.data;
    } catch (error) {
      console.error("Error creating reservation:", error);
      throw error;
    }
  }

  async cancelReservation(reservationId) {
    try {
      const response = await this.client.post(
        `/reservations/${reservationId}/cancel`
      );
      return response.data;
    } catch (error) {
      console.error("Error canceling reservation:", error);
      throw error;
    }
  }

  async extendReservation(reservationId, endDate, numerOfDays) {
    const newEndDate = new Date(
      endDate.getTime() + numerOfDays * 24 * 60 * 60 * 1000
    ).toISOString();
    try {
      const response = await this.client.post(
        `/reservations/${reservationId}/extend`,
        { newEndDate } // Przekazujemy obiekt z kluczem newEndDate
      );
      return response.data;
    } catch (error) {
      console.error("Error extending reservation:", error);
      throw error;
    }
  }

  transformDate = (date) => {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(
      2,
      "0"
    )}-${String(date.getDate()).padStart(2, "0")}T${String(
      date.getHours()
    ).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}:${String(
      date.getSeconds()
    ).padStart(2, "0")}.${String(date.getMilliseconds()).padStart(3, "0")}Z`;
  };

  async addNotification(notification) {
    try {
      const response = await this.client.post("/notifications", notification);
      return response.data;
    } catch (error) {
      console.error("Error adding notification:", error);
      throw error;
    }
  }
}

const apiClient = new ElibApiClient();
export default apiClient;
