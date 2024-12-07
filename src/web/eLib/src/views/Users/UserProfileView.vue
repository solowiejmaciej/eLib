<template>
  <div class="min-h-screen bg-gray-900 text-gray-100">
    <div class="max-w-7xl mx-auto px-4 pt-8">
      <!-- Header section pozostaje bez zmian -->
      <div class="bg-gray-800 rounded-xl p-6 mb-6">
        <div class="flex items-center gap-6">
          <Avatar
            :label="getInitials(user.name, user.surname)"
            size="xlarge"
            shape="circle"
            class="bg-indigo-600 w-24 h-24 text-2xl"
          />
          <div class="space-y-3">
            <h1 class="text-3xl font-bold">
              {{ user.name }} {{ user.surname }}
            </h1>
            <div class="flex flex-wrap gap-3">
              <Tag
                v-if="user.details.hasEmailVerified"
                icon="pi pi-check"
                severity="success"
                value="Email Verified"
              />
              <Tag
                v-else
                icon="pi pi-times"
                severity="warning"
                value="Email Not Verified"
              />
              <Tag
                v-if="user.details.hasPhoneNumberVerified"
                icon="pi pi-phone"
                severity="success"
                value="Phone Verified"
              />
              <Tag
                v-else
                icon="pi pi-times"
                severity="warning"
                value="Phone Not Verified"
              />
              <Tag
                :icon="getNotificationChannelIcon"
                severity="info"
                :value="getNotificationChannelLabel"
              />
              <Tag icon="pi pi-sync" @click="handleChangeNotificationChannel" />
            </div>
          </div>
        </div>
      </div>

      <!-- Menu -->
      <Menubar :model="menuItems" class="mb-6 bg-gray-800 border-gray-700">
        <template #item="{ item }">
          <a
            v-ripple
            :class="[
              'p-menuitem-link',
              { 'active-menuitem': activeTab === item.id },
            ]"
            @click="item.command"
          >
            <span :class="item.icon" class="p-menuitem-icon"></span>
            <span class="p-menuitem-text">{{ item.label }}</span>
          </a>
        </template>
      </Menubar>

      <!-- Content Area -->
      <div class="bg-gray-800 rounded-xl p-6">
        <div v-if="activeTab === 'profile'" class="space-y-6">
          <UserProfile
            :user="user"
            @update="loadUserData"
            @verified="loadUserData"
          />
        </div>

        <div v-else-if="activeTab === 'reservations'" class="space-y-6">
          <ReservationList :userId="user.id" />
        </div>

        <div v-else-if="activeTab === 'reading-list'" class="space-y-6">
          <ReadingList :userId="user.id" />
        </div>

        <div v-else-if="activeTab === 'notifications'" class="space-y-6">
          <NotificationsList :userId="user.id" />
        </div>

        <div v-else-if="activeTab === 'reviews'" class="space-y-6">
          <ReviewsList :userId="user.id" />
        </div>
      </div>
    </div>
  </div>

  <Dialog
    v-model:visible="isNotificationChannelDialogVisible"
    header="Notification Channel"
    modal
    class="max-w-sm"
    :draggable="false"
  >
    <ChangeNotificationChannel
      :selected="user.details.notificationChannel"
      :userId="user.id"
      @update="loadUserData"
    ></ChangeNotificationChannel>
  </Dialog>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useToast } from "primevue/usetoast";
import { useRoute, useRouter } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import ReadingList from "../../components/ReadingList.vue";
import ReservationList from "../../components/ReservationList.vue";
import NotificationsList from "../../components/NotificationsList.vue";
import ReviewsList from "../../components/ReviewsList.vue";
import UserProfile from "../../components/UserProfile.vue";
import ChangeNotificationChannel from "../../components/ChangeNotificationChannel.vue";

const toast = useToast();
const route = useRoute();
const router = useRouter();

const isLoading = ref(true);
const isNotificationChannelDialogVisible = ref(false);
const activeTab = ref("profile");

// Lista dostępnych tabów
const availableTabs = [
  "profile",
  "reservations",
  "reading-list",
  "notifications",
  "reviews",
];

// Inicjalizacja aktywnego taba na podstawie query param
onMounted(() => {
  const tabFromQuery = route.query.tab;
  if (tabFromQuery && availableTabs.includes(tabFromQuery)) {
    activeTab.value = tabFromQuery;
  }
});

// Obserwuj zmiany w query params i aktualizuj aktywny tab
watch(
  () => route.query.tab,
  (newTab) => {
    if (newTab && availableTabs.includes(newTab)) {
      activeTab.value = newTab;
    }
  }
);

const user = ref({
  id: route.params.id || "",
  email: "",
  name: "",
  surname: "",
  phoneNumber: "",
  details: {
    hasEmailVerified: false,
    hasPhoneNumberVerified: false,
    isAdmin: false,
    notificationChannel: null,
  },
});

const menuItems = computed(() => [
  {
    id: "profile",
    label: "Profile",
    icon: "pi pi-user",
    command: () => navigateToTab("profile"),
  },
  {
    id: "reservations",
    label: "Reservations",
    icon: "pi pi-book",
    command: () => navigateToTab("reservations"),
  },
  {
    id: "reading-list",
    label: "Reading List",
    icon: "pi pi-list",
    command: () => navigateToTab("reading-list"),
  },
  {
    id: "notifications",
    label: "Notifications",
    icon: "pi pi-bell",
    command: () => navigateToTab("notifications"),
  },
  {
    id: "reviews",
    label: "Reviews",
    icon: "pi pi-star",
    command: () => navigateToTab("reviews"),
  },
]);

const navigateToTab = (tab) => {
  // Aktualizuj URL z nowym tabem
  router.replace({
    query: {
      ...route.query, // zachowaj inne query params jeśli istnieją
      tab: tab,
    },
  });
  activeTab.value = tab;
};

// Reszta kodu pozostaje bez zmian...
const getNotificationChannelLabel = computed(() => {
  const channel = user.value.details.notificationChannel;
  if (channel == 1) return "Email Notifications";
  if (channel == 2) return "SMS Notifications";
  if (channel == 3) return "System Notifications";
  return "No Notifications";
});

const getNotificationChannelIcon = computed(() => {
  const channel = user.value.details.notificationChannel;
  if (channel & 1) return "pi pi-envelope";
  if (channel & 2) return "pi pi-phone";
  if (channel & 3) return "pi pi-bell";
  return "pi pi-bell-slash";
});

const loadUserData = async () => {
  if (!user.value.id) {
    console.error("No user ID provided");
    return;
  }

  try {
    isLoading.value = true;
    const userData = await apiClient.getUserById(user.value.id);
    user.value = {
      ...userData,
      emailVerified: !!userData.emailVerified,
      phoneVerified: !!userData.phoneVerified,
    };
  } catch (error) {
    console.error("Error loading user data:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load user data",
      life: 3000,
    });
  } finally {
    isLoading.value = false;
  }
};

const getInitials = (firstName, lastName) => {
  return `${firstName?.[0] || ""}${lastName?.[0] || ""}`.toUpperCase();
};

onMounted(async () => {
  if (user.value.id) {
    await loadUserData();
  }
});

const handleChangeNotificationChannel = () => {
  isNotificationChannelDialogVisible.value = true;
};
</script>

<style scoped>
/* Style pozostają bez zmian */
.p-menubar {
  border-radius: 0.75rem;
  padding: 0.5rem;
}

.p-menubar .p-menubar-root-list {
  gap: 0.5rem;
}

.p-menubar .p-menuitem-link {
  padding: 0.75rem 1rem !important;
  border-radius: 0.5rem !important;
  color: #e5e7eb !important;
  transition: all 0.2s ease;
}

.p-menubar .p-menuitem-link:hover {
  background-color: rgba(255, 255, 255, 0.1) !important;
}

.p-menubar .p-menuitem-link .p-menuitem-icon,
.p-menubar .p-menuitem-link .p-menuitem-text {
  color: #e5e7eb !important;
}

.p-menubar .active-menuitem {
  background-color: #4f46e5 !important;
}

.p-menubar .active-menuitem:hover {
  background-color: #4338ca !important;
}

.p-menubar .active-menuitem .p-menuitem-icon,
.p-menubar .active-menuitem .p-menuitem-text {
  color: white !important;
}
</style>
