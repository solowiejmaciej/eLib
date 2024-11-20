import { createRouter, createWebHistory } from "vue-router";

const routes = [
  {
    path: "/",
    redirect: "/home",
  },
  {
    path: "/home",
    name: "home",
    component: () => import("../views/Home/HomeView.vue"),
  },
  //   {
  //     path: "/register",
  //     name: "register",
  //     component: () => import("../views/LoginView/RegisterView.vue"),
  //   },
  //   {
  //     path: "/login",
  //     name: "login",
  //     component: () => import("../views/LoginView/LoginView.vue"),
  //   },
  {
    path: "/:catchAll(.*)",
    name: "notFound",
    component: () => import("../views/Errors/NotFoundView.vue"),
  },
];

const Router = createRouter({
  history: createWebHistory(),
  routes,
});

export default Router;
