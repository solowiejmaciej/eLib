<template>
  <div class="w-full bg-gray-800 px-4 py-2">
    <div class="grid grid-cols-3 items-center">
      <div class="flex items-center gap-1">
        <Button
          v-for="item in visibleMenuItems"
          :key="item.label"
          :icon="item.icon"
          :label="item.label"
          :text="!isActiveRoute(item.route)"
          :outlined="isActiveRoute(item.route)"
          class="text-gray-200 !p-2"
          @click="item.command"
        />
      </div>

      <div class="flex justify-center">
        <BookSearchBar class="w-[30rem]" />
      </div>

      <div class="flex justify-end">
        <ProfileMenu @open-edit-user="onOpenEditUser" />
      </div>
    </div>
  </div>

  <Dialog
    modal
    dismissableMask
    :draggable="false"
    v-model:visible="showUserDataDialog"
    header="Edit user"
    @hide="handleDialogHide"
    :pt="{
      root: 'border-none',
      mask: {
        style: 'backdrop-filter: blur(2px)',
      },
    }"
  >
    <UserData @hide="handleDialogHide" />
  </Dialog>
</template>

<script setup>
import { ref, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useStore } from "vuex";
import BookSearchBar from "./BookSearchBar.vue";
import ProfileMenu from "./ProfileMenu.vue";
import UserData from "./UserData.vue";

const router = useRouter();
const route = useRoute();
const store = useStore();

const showUserDataDialog = ref(false);

// Podstawowe elementy menu dostępne dla wszystkich
const baseMenuItems = [
  {
    label: "Home",
    icon: "pi pi-home",
    route: "/",
    command: () => router.push("/"),
  },
  {
    label: "Books",
    icon: "pi pi-book",
    route: "/books",
    command: () => router.push("/books"),
  },
  {
    label: "New Arrivals",
    icon: "pi pi-sparkles",
    route: "/new-arrivals",
    command: () => router.push("/new-arrivals"),
  },
  {
    label: "Bestsellers",
    icon: "pi pi-crown",
    route: "/bestsellers",
    command: () => router.push("/bestsellers"),
  },
];

const userMenuItems = [
  {
    label: "Reading List",
    icon: "pi pi-list",
    route: "/reading-list",
    command: () => router.push("/reading-list"),
  },
];

const adminMenuItems = [
  {
    label: "Admin Panel",
    icon: "pi pi-cog",
    route: "/admin",
    command: () => router.push("/admin"),
  },
];

const visibleMenuItems = computed(() => {
  let items = [...baseMenuItems];

  if (store.getters.isAuthenticated) {
    items = [...items, ...userMenuItems];
  }

  if (store.getters.isAdmin) {
    items = [...items, ...adminMenuItems];
  }

  return items;
});

const isActiveRoute = (itemRoute) => {
  const currentPath = route.path;

  if (itemRoute === "/home") {
    return currentPath === "/" || currentPath === "/home";
  }

  if (itemRoute.includes(":")) {
    const baseRoute = itemRoute.split(":")[0];
    return currentPath.startsWith(baseRoute);
  }

  return currentPath === itemRoute;
};

function onOpenEditUser() {
  showUserDataDialog.value = true;
}

const handleDialogHide = () => {
  showUserDataDialog.value = false;
};
</script>

<style scoped>
:deep(.p-button.p-button-text) {
  color: #e5e7eb;
}

:deep(.p-button.p-button-text:hover) {
  background: rgba(255, 255, 255, 0.1);
}

:deep(.p-button.p-button-outlined) {
  background: rgba(255, 255, 255, 0.1);
  border-color: #e5e7eb;
  color: #e5e7eb;
}

:deep(.p-button.p-button-outlined:hover) {
  background: rgba(255, 255, 255, 0.2);
}
</style>
