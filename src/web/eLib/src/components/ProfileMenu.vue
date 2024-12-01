<template>
  <div>
    <Button
      type="button"
      label="Login"
      icon="pi pi-sign-in"
      @click="onLoginClick"
      v-if="!isAuthenticated"
    />
    <div v-else class="flex items-center space-x-4" @click="toggle">
      <div class="flex flex-col">
        <h3 class="font-medium">{{ userFullName }}</h3>
      </div>
      <Avatar
        :label="userInitials"
        size="large"
        shape="circle"
        :class="{
          'bg-purple-500': !user?.details?.isAdmin,
          'bg-red-500': user?.details?.isAdmin,
        }"
      />
      <Menu ref="menu" id="overlay_menu" :model="getMenuItems" :popup="true" />
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import Avatar from "primevue/avatar";
import Button from "primevue/button";
import Menu from "primevue/menu";

const store = useStore();
const router = useRouter();
const menu = ref();
const emit = defineEmits(["open-edit-user"]);

const isAuthenticated = computed(() => store.getters.isAuthenticated);
const user = computed(() => store.getters.currentUser);
const isAdmin = computed(() => store.getters.isAdmin);

const userFullName = computed(() => {
  if (!user.value) return "";
  return `${user.value.name} ${user.value.surname}`;
});

const userInitials = computed(() => {
  if (!user.value) return "";
  return `${user.value.name.charAt(0)}${user.value.surname.charAt(0)}`;
});

const getMenuItems = computed(() => {
  const baseItems = [
    {
      label: "My Profile",
      icon: "pi pi-user",
      command: () => {
        onEditProfileClick();
      },
    },
    {
      separator: true,
    },
  ];

  if (isAdmin.value) {
    baseItems.push({
      label: "Manage Users",
      icon: "pi pi-users",
      command: () => {
        router.push("/admin/users");
      },
    });
  }

  baseItems.push(
    {
      separator: true,
    },
    {
      label: "Logout",
      icon: "pi pi-sign-out",
      command: () => {
        onLogoutClick();
      },
    }
  );

  return baseItems;
});

// Event handlers
const toggle = (event) => {
  menu.value.toggle(event);
};

function onLoginClick() {
  router.push("/login");
}

async function onLogoutClick() {
  try {
    await store.dispatch("logout");
    router.push("/login");
  } catch (error) {
    console.error("Logout failed:", error);
  }
}

function onEditProfileClick() {
  emit("open-edit-user");
}
</script>

<style scoped>
:deep(.p-avatar) {
  color: white;
}
</style>
