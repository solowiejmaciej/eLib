<template>
  <div class="w-full bg-gray-800 px-4 py-2">
    <div class="grid grid-cols-3 items-center">
      <div class="flex items-center gap-1">
        <Button
          v-for="item in items"
          :key="item.label"
          :icon="item.icon"
          :label="item.label"
          text
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
import { ref } from "vue";
import { useRouter } from "vue-router";
import BookSearchBar from "./BookSearchBar.vue";
import ProfileMenu from "./ProfileMenu.vue";
import UserData from "./UserData.vue";

const router = useRouter();

let showUserDataDialog = ref(false);

const items = ref([
  {
    label: "Home",
    icon: "pi pi-home",
    command: () => {
      onHomeClick();
    },
  },
  {
    label: "Books",
    icon: "pi pi-book",
    command: () => {
      onBooksClick();
    },
  },
  {
    label: "New Arrivals",
    icon: "pi pi-sparkles",
    command: () => {
      onNewArrivalsClick();
    },
  },
  {
    label: "Bestsellers",
    icon: "pi pi-crown",
    command: () => {
      onBestsellersClick();
    },
  },
]);

function onHomeClick() {
  router.push("/");
}

function onBooksClick() {
  router.push("/books");
}

function onNewArrivalsClick() {
  router.push("/new-arrivals");
}

function onBestsellersClick() {
  router.push("/bestsellers");
}

function onOpenEditUser() {
  showUserDataDialog.value = true;
}

const handleDialogHide = () => {
  showDialog.value = false;
  showUserData.value = false;
};
</script>

<style scoped>
:deep(.p-button.p-button-text) {
  color: #e5e7eb;
}

:deep(.p-button.p-button-text:hover) {
  background: rgba(255, 255, 255, 0.1);
}
</style>
