<template>
  <div class="space-y-6">
    <div class="bg-gray-800 rounded-lg p-4 border border-gray-700">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold">Notifications List</h2>
        <div class="flex gap-2">
          <Button
            icon="pi pi-plus"
            class="p-button-text p-button-sm text-white"
            @click="openSendDialog"
            tooltip="Send New Notification"
          />
          <Button
            icon="pi pi-refresh"
            class="p-button-text p-button-sm text-white"
            @click="loadNotifications"
            :loading="loading"
          />
        </div>
      </div>

      <DataTable
        v-if="!loading"
        :value="notifications"
        :rows="paginationState.pageSize"
        :totalRecords="paginationState.totalCount"
        :lazy="true"
        :paginator="true"
        :loading="loading"
        @page="onPageChange($event)"
        :first="(paginationState.pageNumber - 1) * paginationState.pageSize"
        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport"
        currentPageReportTemplate="{first} to {last} of {totalRecords}"
        class="p-datatable-sm"
        stripedRows
        removableSort
        :rowHover="true"
      >
        <Column field="title" header="Title" style="width: 250px">
          <template #body="slotProps">
            <div class="flex items-center gap-2">
              <i
                class="pi text-xl"
                :class="getNotificationIcon(slotProps.data.channel)"
              ></i>
              <span class="text-sm">{{ slotProps.data.title }}</span>
            </div>
          </template>
        </Column>

        <Column field="type" header="Type" style="width: 150px">
          <template #body="slotProps">
            <Tag
              :value="getChannelName(slotProps.data.channel)"
              severity="info"
              size="small"
            />
          </template>
        </Column>

        <Column field="createdAt" header="Date" style="width: 200px">
          <template #body="slotProps">
            <div class="text-sm text-gray-400">
              {{ formatDate(slotProps.data.createdAt) }}
            </div>
          </template>
        </Column>

        <Column header="Actions" style="width: 100px">
          <template #body="slotProps">
            <Menu ref="menu" :model="menuItems" :popup="true" />
            <Button
              icon="pi pi-ellipsis-v"
              class="p-button-text p-button-sm text-white"
              @click="toggleMenu($event, slotProps.data)"
            />
          </template>
        </Column>
      </DataTable>

      <div v-if="loading" class="flex justify-center p-4">
        <ProgressSpinner />
      </div>
    </div>

    <!-- Dialog do wyświetlania szczegółów powiadomienia -->
    <Dialog
      :maximizable="true"
      v-model:visible="displayDialog"
      :header="selectedNotification?.title || ''"
      :modal="true"
      :style="{ width: '50rem' }"
      :breakpoints="{ '960px': '75vw', '641px': '90vw' }"
      :draggable="false"
    >
      <div v-if="selectedNotification" class="p-4">
        <div
          v-if="!isHtmlMessage(selectedNotification.message)"
          class="whitespace-pre-line"
        >
          {{ selectedNotification.message }}
        </div>
        <iframe
          v-else
          ref="htmlFrame"
          :srcdoc="selectedNotification.message"
          class="w-full min-h-[800px] border-0"
          frameborder="0"
        ></iframe>
      </div>
      <template #footer>
        <Button
          label="Close"
          icon="pi pi-times"
          @click="closeDialog"
          class="p-button-text"
        />
      </template>
    </Dialog>

    <Dialog
      v-model:visible="displaySendDialog"
      header="Send New Notification"
      :modal="true"
      :style="{ width: '40rem' }"
      :breakpoints="{ '960px': '75vw', '641px': '90vw' }"
    >
      <div class="p-fluid">
        <div class="field mb-4">
          <label for="channel" class="block mb-2">Channel</label>
          <Dropdown
            id="channel"
            v-model="newNotification.channel"
            :options="channelOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="Select a channel"
            class="w-full"
            @change="onChannelChange"
          />
        </div>

        <div v-if="newNotification.channel !== 2" class="field mb-4">
          <label for="title" class="block mb-2">Title</label>
          <InputText
            id="title"
            v-model="newNotification.title"
            required
            class="w-full"
          />
        </div>

        <div class="field mb-4">
          <label for="message" class="block mb-2">Message</label>
          <Textarea
            id="message"
            v-model="newNotification.message"
            rows="5"
            required
            class="w-full"
          />
        </div>
      </div>

      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          @click="closeSendDialog"
          class="p-button-text"
        />
        <Button
          label="Send"
          icon="pi pi-send"
          @click="sendNotification"
          :loading="sending"
          class="p-button-primary"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import notificationServiceApiClient from "../clients/eLibNotificationServiceApiClient";
import apiClient from "../clients/eLibApiClient";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Tag from "primevue/tag";
import Button from "primevue/button";
import Dialog from "primevue/dialog";
import ProgressSpinner from "primevue/progressspinner";
import Menu from "primevue/menu";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import Textarea from "primevue/textarea";

const notifications = ref([]);
const loading = ref(true);
const error = ref(null);
const displayDialog = ref(false);
const selectedNotification = ref(null);
const menu = ref();
const displaySendDialog = ref(false);
const sending = ref(false);

const newNotification = ref({
  title: "",
  message: "",
  channel: null,
});

const paginationState = ref({
  pageNumber: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 1,
});

const channelOptions = [
  { label: "Email", value: 1 },
  { label: "SMS", value: 2 },
  { label: "System", value: 3 },
];

const props = defineProps({
  userId: {
    type: String,
    required: true,
  },
});

const onChannelChange = (event) => {
  if (event.value === 2) {
    newNotification.value.title = "";
  }
};

const loadNotifications = async () => {
  loading.value = true;
  try {
    const response = await notificationServiceApiClient.getNotificationsForUser(
      props.userId,
      paginationState.value.pageNumber,
      paginationState.value.pageSize
    );
    notifications.value = response.items;
    paginationState.value = {
      pageNumber: response.pageNumber,
      pageSize: response.pageSize,
      totalCount: response.totalCount,
      totalPages: response.totalPages,
    };
  } catch (err) {
    error.value = "Failed to load notifications";
    console.error(err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadNotifications();
});

const onPageChange = (event) => {
  paginationState.value.pageNumber = event.page + 1;
  loadNotifications();
};

const menuItems = [
  {
    label: "View Details",
    icon: "pi pi-external-link",
    command: () => showNotificationDetails(),
  },
];

const toggleMenu = (event, notification) => {
  selectedNotification.value = notification;
  menu.value?.toggle(event);
};

const showNotificationDetails = () => {
  displayDialog.value = true;
};

const closeDialog = () => {
  displayDialog.value = false;
  selectedNotification.value = null;
};

const openSendDialog = () => {
  newNotification.value = {
    title: "",
    message: "",
    channel: null,
  };
  displaySendDialog.value = true;
};

const closeSendDialog = () => {
  displaySendDialog.value = false;
};

const sendNotification = async () => {
  if (!newNotification.value.message || !newNotification.value.channel) {
    return;
  }

  if (newNotification.value.channel !== 2 && !newNotification.value.title) {
    return;
  }

  sending.value = true;
  try {
    var userToSendNotification = await apiClient.getUserById(props.userId);
    var email = userToSendNotification.email;
    var phoneNumber = userToSendNotification.phoneNumber;
    await notificationServiceApiClient.addNotification(
      newNotification.value.message,
      props.userId,
      newNotification.value.channel,
      newNotification.value.title,
      phoneNumber,
      email
    );
    closeSendDialog();
    await loadNotifications();
  } catch (err) {
    console.error("Failed to send notification:", err);
  } finally {
    sending.value = false;
  }
};

const getNotificationIcon = (channel) => {
  switch (channel) {
    case 1:
      return "pi-envelope";
    case 2:
      return "pi-phone";
    case 3:
      return "pi-bell";
    default:
      return "pi-question";
  }
};

const getChannelName = (channel) => {
  switch (channel) {
    case 1:
      return "Email";
    case 2:
      return "SMS";
    case 3:
      return "System";
    default:
      return "Unknown";
  }
};

const isHtmlMessage = (message) => {
  return message && message.trim().startsWith("<!DOCTYPE html>");
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString();
};
</script>

<style scoped>
:deep(.p-datatable) {
  background-color: transparent;
}

:deep(.p-datatable .p-datatable-header) {
  background-color: transparent;
  border: none;
}

:deep(.p-datatable .p-datatable-thead > tr > th) {
  background-color: transparent;
  border-width: 0 0 1px 0;
  color: #94a3b8;
  padding: 0.5rem 1rem;
}

:deep(.p-datatable .p-datatable-tbody > tr) {
  background-color: transparent;
  color: #e2e8f0;
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
  border-width: 0 0 1px 0;
  border-color: #1f2937;
  padding: 0.5rem 1rem;
}

:deep(.p-datatable .p-datatable-tbody > tr:hover) {
  background-color: rgba(255, 255, 255, 0.05);
}

:deep(.p-paginator) {
  background-color: transparent;
  border: none;
  padding: 1rem 0;
}

:deep(.p-paginator .p-paginator-pages .p-paginator-page) {
  color: #e2e8f0;
}

:deep(.p-paginator .p-paginator-first),
:deep(.p-paginator .p-paginator-prev),
:deep(.p-paginator .p-paginator-next),
:deep(.p-paginator .p-paginator-last) {
  color: #e2e8f0;
}

:deep(.p-dropdown) {
  background-color: #1f2937;
  border-color: #374151;
}

:deep(.p-dropdown:not(.p-disabled):hover) {
  border-color: #4b5563;
}

:deep(.p-inputtext),
:deep(.p-textarea) {
  background-color: #1f2937;
  border-color: #374151;
  color: #e2e8f0;
}

:deep(.p-inputtext:enabled:focus),
:deep(.p-textarea:enabled:focus) {
  border-color: #3b82f6;
  box-shadow: 0 0 0 1px #3b82f6;
}

:deep(.p-tag) {
  @apply text-sm;
}

:deep(.p-dialog-content) {
  @apply max-h-[70vh] overflow-y-auto;
}
</style>
