import Vuex from "vuex";
import apiClient from "../clients/eLibApiClient";

const store = new Vuex.Store({
  state: {
    status: "",
    user: JSON.parse(localStorage.getItem("user")) || null,
    token: localStorage.getItem("token") || null,
  },

  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.details?.isAdmin || false,
    hasEmailVerified: (state) => state.user?.details?.hasEmailVerified || false,
    hasPhoneVerified: (state) =>
      state.user?.details?.hasPhoneNumberVerified || false,
    currentUser: (state) => state.user,
    authStatus: (state) => state.status,
  },

  mutations: {
    auth_request(state) {
      state.status = "loading";
    },

    auth_success(state, { token, user }) {
      state.status = "success";
      state.token = token;
      state.user = user;

      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(user));
      apiClient.client.defaults.headers.common[
        "Authorization"
      ] = `Bearer ${token}`;
    },

    auth_error(state) {
      state.status = "error";
    },

    logout(state) {
      state.status = "";
      state.token = null;
      state.user = null;

      localStorage.removeItem("token");
      localStorage.removeItem("user");
      delete apiClient.client.defaults.headers.common["Authorization"];
    },
  },

  actions: {
    async loginWithEmail({ commit }, { email, password }) {
      try {
        commit("auth_request");
        const response = await apiClient.loginWithEmailAndPassword(
          email,
          password
        );
        const { accessToken, user } = response;
        commit("auth_success", {
          token: accessToken.token,
          user,
        });
        return response;
      } catch (error) {
        commit("auth_error");
        throw error;
      }
    },

    async loginWithPhone({ commit }, { phoneNumber, password }) {
      try {
        commit("auth_request");
        const response = await apiClient.loginWithPhoneNumber(
          phoneNumber,
          password
        );
        const { accessToken, user } = response;
        commit("auth_success", {
          token: accessToken.token,
          user,
        });
        return response;
      } catch (error) {
        commit("auth_error");
        throw error;
      }
    },

    logout({ commit }) {
      return new Promise((resolve) => {
        commit("logout");
        resolve();
      });
    },
  },
});

const token = localStorage.getItem("token");
if (token) {
  apiClient.client.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

export default store;
