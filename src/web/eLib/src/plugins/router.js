import { createRouter, createWebHistory } from "vue-router";
import DefaultLayout from "../layouts/DefaultLayout.vue";

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
