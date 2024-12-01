import { createRouter, createWebHistory } from "vue-router";
import DefaultLayout from "../layouts/DefaultLayout.vue";
import { compile } from "vue";

const routes = [
  {
    path: "/",
    component: DefaultLayout,
    children: [
      {
        path: "/",
        redirect: "/home",
      },
      {
        path: "/home",
        name: "home",
        component: () => import("../views/Home/HomeView.vue"),
      },
      {
        path: "/books",
        name: "books",
        component: () => import("../views/Books/BooksView.vue"),
      },
      {
        path: "/books/:id",
        name: "book",
        component: () => import("../views/Books/BookView.vue"),
      },
      {
        path: "/new-arrivals",
        name: "new-arrivals",
        component: () => import("../views/Books/NewBooksView.vue"),
      },
      {
        path: "/bestsellers",
        name: "bestsellers",
        component: () => import("../views/Books/BestsellersBooksView.vue"),
      },
      {
        path: "/reading-list",
        name: "reading-list",
        component: () => import("../views/Books/ReadingListView.vue"),
      },
    ],
  },
  {
    path: "/admin",
    component: DefaultLayout,
    children: [
      {
        path: "/admin",
        name: "admin-panel",
        component: () => import("../views/Admin/AdminPanel.vue"),
      },
      {
        path: "/admin/users",
        name: "manage-users",
        component: () => import("../views/Admin/UsersView.vue"),
      },
      {
        path: "/admin/users/:id",
        name: "user-profile",
        component: () => import("../views/Admin/UserProfileView.vue"),
      }
    ],
  },
  {
    path: "/:catchAll(.*)",
    name: "notFound",
    component: () => import("../views/Errors/NotFoundView.vue"),
  },
  {
    path: "/login",
    name: "login",
    component: () => import("../views/LoginRegister/LoginView.vue"),
  },
];

const Router = createRouter({
  history: createWebHistory(),
  routes,
});

export default Router;
