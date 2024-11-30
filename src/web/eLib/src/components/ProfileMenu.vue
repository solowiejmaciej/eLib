<template>
  <div>
    <Button
      type="button"
      label="Login"
      icon="pi pi-sign-in"
      @click="onLoginClick"
      v-if="!isUserLoggedIn"
    />
    <div v-else class="flex items-center space-x-4" @click="toggle">
      <h3>Maciej Solowiej</h3>
      <Avatar label="MS" size="xlarge" shape="circle" />
      <Menu ref="menu" id="overlay_menu" :model="menuItems" :popup="true" />
    </div>
  </div>
</template>

<script setup>
import Avatar from "primevue/avatar";
import Button from "primevue/button";
import { useRouter } from "vue-router";
import { ref, defineEmits } from "vue";

var router = useRouter();

let menu = ref();
const emit = defineEmits(["open-edit-user"]);

let isUserLoggedIn = ref(true);

const toggle = (event) => {
  menu.value.toggle(event);
};

const menuItems = [
  {
    label: "Logout",
    icon: "pi pi-sign-out",
    command: () => {
      onLogoutClick();
    },
  },
  {
    label: "Edit profile",
    icon: "pi pi-user-edit",
    command: () => {
      onEditProfileClick();
    },
  },
];

function onLoginClick() {
  router.push("/login");
}

function onLogoutClick() {}

function onEditProfileClick() {
  emit("open-edit-user");
}
</script>
