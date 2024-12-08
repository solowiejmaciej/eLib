<template>
  <div class="space-y-6">
    <div class="bg-gray-800 rounded-lg p-4 border border-gray-700">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold">Recent Reservations</h2>
        <Button
          icon="pi pi-refresh"
          class="p-button-text p-button-sm text-white"
          @click="loadReservations"
          :loading="isLoading"
        />
      </div>

      <DataTable
        v-if="!isLoading"
        :value="reservations"
        :rows="pageSize"
        :totalRecords="totalRecords"
        :lazy="true"
        :paginator="true"
        :loading="isLoading"
        @page="onPageChange($event)"
        :first="(pageNumber - 1) * pageSize"
        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport"
        currentPageReportTemplate="{first} to {last} of {totalRecords}"
        class="p-datatable-sm"
        stripedRows
        removableSort
        :rowHover="true"
      >
        <Column field="bookId" header="Book" style="width: 250px">
          <template #body="slotProps">
            <div class="flex items-center gap-2">
              <Avatar
                :image="getBookPhotoUrl(slotProps.data.bookId)"
                size="normal"
                shape="circle"
                class="bg-indigo-500"
              />
              <span class="text-sm">{{
                getBookTitle(slotProps.data.bookId)
              }}</span>
            </div>
          </template>
        </Column>

        <Column field="status" header="Status" style="width: 150px">
          <template #body="slotProps">
            <Tag
              :value="getReservationStatus(slotProps.data)"
              :severity="getStatusSeverity(slotProps.data)"
              size="small"
            />
          </template>
        </Column>

        <Column field="dates" header="Dates" style="width: 200px">
          <template #body="slotProps">
            <div class="text-sm">
              <div class="text-gray-400">
                Start: {{ formatDate(slotProps.data.startDate) }}
              </div>
              <div class="text-gray-400">
                End: {{ formatDate(slotProps.data.endDate) }}
              </div>
            </div>
          </template>
        </Column>

        <Column
          header="Actions"
          style="width: 100px"
          v-if="store.getters.isAdmin"
        >
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

      <ProgressSpinner
        v-else
        style="width: 40px; height: 40px"
        class="flex justify-center p-4"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useToast } from "primevue/usetoast";
import store from "../store/store";

import apiClient from "../clients/eLibApiClient";

const props = defineProps({
  userId: {
    type: String,
    required: true,
  },
});

const toast = useToast();
const menu = ref();
const reservations = ref([]);
const isLoading = ref(false);
const searchPhrase = ref("");
const pageNumber = ref(1);
const pageSize = ref(10);
const totalRecords = ref(0);
const selectedReservation = ref(null);
const bookData = ref({});

const loadReservations = async () => {
  isLoading.value = true;
  try {
    const response = await apiClient.getReservationsByUserId(
      searchPhrase.value,
      pageNumber.value,
      pageSize.value,
      props.userId
    );
    reservations.value = response.items;
    totalRecords.value = response.totalCount;
    await loadBookData(response.items);
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load reservations",
      life: 3000,
    });
  } finally {
    isLoading.value = false;
  }
};

const formatDate = (date) => {
  return new Date(date).toLocaleDateString();
};

const getBookTitle = (bookId) => {
  return bookData.value[bookId].title || "Loading...";
};

const getBookPhotoUrl = (bookId) => {
  return bookData.value[bookId].details.coverUrl || "";
};

const getReservationStatus = (reservation) => {
  if (reservation.isReturned) return "Returned";
  if (reservation.isOverdue) return "Overdue";
  if (reservation.isExtended) return "Extended";
  if (reservation.canceledAt) return "Cancelled";
  return "Active";
};

const getStatusSeverity = (reservation) => {
  if (reservation.isOverdue) return "danger";
  if (reservation.isReturned) return "secondary";
  if (reservation.canceledAt) return "warning";
  if (reservation.isExtended) return "info";
  return "success";
};

const menuItems = [
  {
    label: "Return Book",
    icon: "pi pi-check",
    command: () => handleReturn(),
  },
  {
    label: "Extend Reservation",
    icon: "pi pi-calendar-plus",
    command: () => handleExtend(),
  },
  {
    label: "Cancel Reservation",
    icon: "pi pi-times",
    command: () => handleCancel(),
  },
];

const toggleMenu = (event, reservation) => {
  selectedReservation.value = reservation;
  menu.value.toggle(event);
};

const onPageChange = (event) => {
  pageNumber.value = event.page + 1;
  loadReservations();
};

// Action handlers
const handleReturn = async () => {
  try {
    await apiClient.returnReservation(selectedReservation.value.id);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Book returned successfully",
      life: 3000,
    });
    loadReservations();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to return book",
      life: 3000,
    });
  }
};

const handleExtend = async () => {
  try {
    const days = 7;
    const newEndDate = new Date(selectedReservation.value.endDate);
    await apiClient.extendReservation(
      selectedReservation.value.id,
      newEndDate,
      days
    );
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Reservation extended successfully",
      life: 3000,
    });
    loadReservations();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to extend reservation",
      life: 3000,
    });
  }
};

const handleCancel = async () => {
  try {
    await apiClient.cancelReservation(selectedReservation.value.id);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Reservation cancelled successfully",
      life: 3000,
    });
    loadReservations();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to cancel reservation",
      life: 3000,
    });
  }
};

const loadBookData = async (reservations) => {
  for (const reservation of reservations) {
    if (!bookData.value[reservation.bookId]) {
      try {
        const book = await apiClient.getBookById(reservation.bookId);
        bookData.value[reservation.bookId] = book;
      } catch (error) {
        console.error(
          `Failed to load book title for ${reservation.bookId}:`,
          error
        );
        bookData.value[reservation.bookId] = "Error loading title";
      }
    }
  }
};

onMounted(() => {
  loadReservations();
});
</script>
