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
            </div>
          </div>
        </div>
      </div>

      <!-- Menu -->
      <Menubar :model="menuItems" class="mb-6 bg-gray-800 border-gray-700" />

      <!-- Content Area -->
      <div class="bg-gray-800 rounded-xl p-6">
        <!-- Profile Tab -->
        <div v-if="activeTab === 'profile'" class="space-y-6">
          <form @submit.prevent="updateProfile" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-300">
                  First Name
                </label>
                <InputText
                  v-model="user.name"
                  class="w-full bg-gray-700 border-gray-600"
                  placeholder="Enter your first name"
                />
              </div>
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-300">
                  Last Name
                </label>
                <InputText
                  v-model="user.surname"
                  class="w-full bg-gray-700 border-gray-600"
                  placeholder="Enter your last name"
                />
              </div>
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-300">
                  Email
                </label>
                <div class="flex gap-3">
                  <InputText
                    v-model="user.email"
                    class="w-full bg-gray-700 border-gray-600"
                    placeholder="Enter your email"
                  />
                </div>
              </div>
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-300">
                  Phone
                </label>
                <div class="flex gap-3">
                  <InputText
                    v-model="user.phoneNumber"
                    class="w-full bg-gray-700 border-gray-600"
                    placeholder="Enter your phone number"
                  />
                </div>
              </div>
            </div>
            <div class="flex justify-end">
              <Button
                type="submit"
                label="Save Changes"
                :loading="saving"
                class="w-32"
              />
            </div>
          </form>
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
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useToast } from "primevue/usetoast";
import { useRoute, useRouter } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import ReadingList from "../../components/ReadingList.vue";
import ReservationList from "../../components/ReservationList.vue";
import NotificationsList from "../../components/NotificationsList.vue";

const toast = useToast();
const route = useRoute();
const router = useRouter();

const isLoading = ref(true);
const saving = ref(false);
const activeTab = ref("profile");

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
    notificationChannel: 1,
  },
});

const menuItems = computed(() => [
  {
    label: "Profile",
    icon: "pi pi-user",
    command: () => navigateToTab("profile"),
  },
  {
    label: "Reservations",
    icon: "pi pi-book",
    command: () => navigateToTab("reservations"),
  },
  {
    label: "Reading List",
    icon: "pi pi-list",
    command: () => navigateToTab("reading-list"),
  },
  {
    label: "Notifications",
    icon: "pi pi-bell",
    command: () => navigateToTab("notifications"),
  },
]);

const navigateToTab = (tab) => {
  activeTab.value = tab;
};

const getNotificationChannelLabel = computed(() => {
  const channel = user.value.details.notificationChannel;
  if (channel & 1) return "Email Notifications";
  if (channel & 2) return "SMS Notifications";
  return "No Notifications";
});

const getNotificationChannelIcon = computed(() => {
  const channel = user.value.details.notificationChannel;
  if (channel & 1) return "pi pi-envelope";
  if (channel & 2) return "pi pi-phone";
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
    console.log(userData);
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

const updateProfile = async () => {
  if (!user.value.id) return;

  saving.value = true;
  try {
    await apiClient.updateUser(user.value.id, user.value);
    await loadUserData();
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Profile updated successfully",
      life: 3000,
    });
  } catch (error) {
    console.error("Error updating profile:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to update profile",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
};

const getInitials = (firstName, lastName) => {
  return `${firstName?.[0] || ""}${lastName?.[0] || ""}`.toUpperCase();
};

// Initial data loading
onMounted(async () => {
  if (user.value.id) {
    await loadUserData();
  }
});
</script>

<style scoped>
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
</style>
