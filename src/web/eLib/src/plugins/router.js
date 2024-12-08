import { createRouter, createWebHistory } from "vue-router";
import DefaultLayout from "@/Layouts/DefaultLayout.vue";
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
      {
        path: "/users/:id",
        name: "user-profile",
        component: () => import("../views/Users/UserProfileView.vue"),
      },
      {
        path: "/authors/:id",
        name: "author",
        component: () => import("../views/Authors/AuthorView.vue"),
      },
      {
        path: "/authors/:id/books",
        name: "author-books",
        component: () => import("../views/Authors/BooksByAuthorView.vue"),
      },
      {
        path: "/authors",
        name: "authors",
        component: () => import("../views/Authors/AuthorsView.vue"),
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
