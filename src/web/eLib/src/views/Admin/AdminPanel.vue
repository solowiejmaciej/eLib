<template>
  <div class="min-h-screen bg-gray-900 text-white p-6">
    <div class="mb-6">
      <div class="flex justify-between items-center mb-4">
        <h1 class="text-2xl font-bold">Admin Dashboard</h1>
        <div class="flex gap-2">
          <Button
            label="New Book"
            icon="pi pi-plus"
            class="p-button-sm p-button-success"
            @click="handleNewBook"
          />
          <Button
            label="Users"
            icon="pi pi-users"
            class="p-button-sm p-button-info"
            @click="$router.push('/admin/users')"
          />
        </div>
      </div>
    </div>

    <!-- Statistics Grid -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <div
        v-for="stat in statistics"
        :key="stat.title"
        class="bg-gray-800 rounded-lg p-4 border border-gray-700"
      >
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm">{{ stat.title }}</p>
            <h3 class="text-xl font-bold mt-1">
              <ProgressSpinner
                v-if="loading"
                style="width: 20px; height: 20px"
              />
              <span v-else>{{ stat.value }}</span>
            </h3>
          </div>
          <i
            :class="stat.icon"
            class="text-xl"
            :style="{ color: stat.color }"
          ></i>
        </div>
      </div>
    </div>

    <!-- Recent Reservations -->
    <div class="bg-gray-800 rounded-lg p-4 border border-gray-700">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold">Recent Reservations</h2>
        <Button
          icon="pi pi-refresh"
          class="p-button-text p-button-sm text-white"
          @click="loadReservations"
          :loading="loadingReservations"
        />
      </div>

      <DataTable
        v-if="!loadingReservations"
        :value="recentReservations"
        :rows="pageSize"
        :totalRecords="totalRecords"
        :lazy="true"
        :paginator="true"
        :loading="loadingReservations"
        @page="onPageChange($event)"
        :first="(currentPage - 1) * pageSize"
        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport"
        currentPageReportTemplate="{first} to {last} of {totalRecords}"
        class="p-datatable-sm"
        stripedRows
      >
        <Column header="User" style="width: 250px">
          <template #body="slotProps">
            <div class="flex items-center gap-2">
              <Avatar
                :label="getUserInitials(slotProps.data.userId)"
                size="normal"
                shape="circle"
                class="bg-indigo-500"
              />
              <span class="text-sm">{{
                getUserFullName(slotProps.data.userId)
              }}</span>
            </div>
          </template>
        </Column>
        <Column header="Status" style="width: 150px">
          <template #body="slotProps">
            <Tag
              :value="getReservationStatus(slotProps.data)"
              :severity="getStatusSeverity(slotProps.data)"
              size="small"
            />
          </template>
        </Column>
        <Column header="Dates" style="width: 200px">
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
        <Column header="Actions" style="width: 100px">
          <template #body="slotProps">
            <Button
              icon="pi pi-ellipsis-v"
              class="p-button-text p-button-sm text-white"
              @click="(event) => showMenu(event, slotProps.data)"
            />
          </template>
        </Column>
      </DataTable>

      <div v-else class="flex justify-center p-4">
        <ProgressSpinner style="width: 40px; height: 40px" />
      </div>

      <Menu id="reservation_menu" ref="menu" :model="menuItems" :popup="true" />
    </div>
  </div>

  <BookForm
    :visible="bookDialog"
    @update:visible="(val) => (bookDialog = val)"
    :book-to-edit="null"
    @book-saved="loadData"
  />
</template>

<script setup>
import { ref, onMounted } from "vue";
import apiClient from "../../clients/eLibApiClient";
import { useToast } from "primevue/usetoast";
import BookForm from "../../components/BookForm.vue";

const toast = useToast();
const loading = ref(true);
const loadingReservations = ref(true);
const pageSize = ref(10);
const currentPage = ref(1);
const totalRecords = ref(0);
const users = ref(new Map());
const selectedReservation = ref(null);
const bookDialog = ref(false);

const statistics = ref([
  {
    title: "Total Books",
    value: 0,
    icon: "pi pi-book",
    color: "#4ade80", // green
  },
  {
    title: "Active Users",
    value: 0,
    icon: "pi pi-users",
    color: "#60a5fa", // blue
  },
  {
    title: "Active Reservations",
    value: 0,
    icon: "pi pi-calendar",
    color: "#f472b6", // pink
  },
]);

const recentReservations = ref([]);

const showMenu = (event, data) => {
  selectedReservation.value = data;
  menu.value.toggle(event);
};

const formatDate = (date) => {
  return new Date(date).toLocaleDateString("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
};

const getUserInitials = (userId) => {
  const user = users.value.get(userId);
  if (!user) return "U";
  return `${user.name[0]}${user.surname[0]}`;
};

const getUserFullName = (userId) => {
  const user = users.value.get(userId);
  if (!user) return "Loading...";
  return `${user.name} ${user.surname}`;
};

const getReservationStatus = (reservation) => {
  if (reservation.isOverdue) return "Overdue";
  if (reservation.isReturned) return "Returned";
  if (reservation.canceledAt) return "Canceled";
  if (reservation.isExtended) return "Extended";
  return "Active";
};

const getStatusSeverity = (reservation) => {
  if (reservation.isOverdue) return "danger";
  if (reservation.isReturned) return "success";
  if (reservation.canceledAt) return "warning";
  if (reservation.isExtended) return "info";
  return "success";
};

const onPageChange = async (event) => {
  currentPage.value = event.page + 1;
  pageSize.value = event.rows;
  await loadReservations();
};

const loadReservations = async () => {
  loadingReservations.value = true;
  try {
    const response = await apiClient.getReservations(
      "",
      currentPage.value,
      pageSize.value
    );
    recentReservations.value = response.items;
    totalRecords.value = response.totalCount;

    const userIds = new Set(response.items.map((r) => r.userId));
    for (const userId of userIds) {
      if (!users.value.has(userId)) {
        const userDetails = await apiClient.getUserById(userId);
        users.value.set(userId, userDetails);
      }
    }
  } catch (error) {
    console.error("Error loading reservations:", error);
  } finally {
    loadingReservations.value = false;
  }
};

onMounted(async () => {
  loadData();
});

const loadData = async () => {
  loading.value = true;
  try {
    const booksResponse = await apiClient.getBooks("", 1, 1000);
    statistics.value[0].value = booksResponse.totalCount || 0;

    const usersResponse = await apiClient.getUsers("", 1, 1000);
    statistics.value[1].value = usersResponse.totalCount || 0;

    await loadReservations();
    statistics.value[2].value = totalRecords.value;
  } catch (error) {
    console.error("Error loading dashboard data:", error);
  } finally {
    loading.value = false;
  }
};

const menu = ref();
const menuItems = ref([
  {
    label: "Return Book",
    icon: "pi pi-check",
    command: async () => {
      if (selectedReservation.value) {
        try {
          await apiClient.returnReservation(selectedReservation.value.id);
          loadReservations();
          toast.add({
            severity: "success",
            summary: "Success",
            detail: "Book returned successfully",
            life: 3000,
          });
        } catch (error) {
          console.error("Error returning book:", error);
          toast.add({
            severity: "error",
            summary: "Error",
            detail: "Failed to return book",
            life: 3000,
          });
        }
      }
    },
  },
  {
    label: "Extend Reservation",
    icon: "pi pi-calendar-plus",
    command: async () => {
      await handleExtendReservation();
    },
  },
  {
    label: "Cancel Reservation",
    icon: "pi pi-times",
    command: async () => {
      if (selectedReservation.value) {
        try {
          await apiClient.cancelReservation(selectedReservation.value.id);
          loadReservations();
          toast.add({
            severity: "success",
            summary: "Success",
            detail: "Reservation canceled successfully",
            life: 3000,
          });
        } catch (error) {
          console.error("Error canceling reservation:", error);
          toast.add({
            severity: "error",
            summary: "Error",
            detail: "Failed to cancel reservation",
            life: 3000,
          });
        }
      }
    },
  },
]);

const handleNewBook = () => {
  bookDialog.value = true;
};

const handleExtendReservation = async () => {
  if (selectedReservation.value) {
    try {
      const days = 7;
      const newEndDate = new Date(selectedReservation.value.endDate);
      await apiClient.extendReservation(
        selectedReservation.value.id,
        newEndDate,
        days
      );
      loadReservations();
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Reservation extended successfully",
        life: 3000,
      });
    } catch (error) {
      console.error("Error extending reservation:", error);
      toast.add({
        severity: "error",
        summary: "Error",
        detail: "Failed to extend reservation",
        life: 3000,
      });
    }
  }
};
</script>

<style>
.p-datatable {
  background: transparent !important;
}

.p-datatable .p-datatable-thead > tr > th {
  background: transparent !important;
  color: #ffffff !important;
  border-color: #374151 !important;
  padding: 0.5rem !important;
}

.p-datatable .p-datatable-tbody > tr {
  background: transparent !important;
  color: #ffffff !important;
}

.p-datatable .p-datatable-tbody > tr > td {
  border-color: #374151 !important;
  padding: 0.5rem !important;
}

.p-datatable.p-datatable-striped .p-datatable-tbody > tr:nth-child(even) {
  background: rgba(255, 255, 255, 0.02) !important;
}

.p-paginator {
  background: transparent !important;
  color: #ffffff !important;
  padding: 0.5rem !important;
}

.p-paginator .p-paginator-page,
.p-paginator .p-paginator-next,
.p-paginator .p-paginator-last,
.p-paginator .p-paginator-first,
.p-paginator .p-paginator-prev {
  color: #ffffff !important;
  min-width: 2rem !important;
  height: 2rem !important;
}

.p-paginator .p-paginator-page.p-highlight {
  background: #4f46e5 !important;
}

.p-tag {
  padding: 0.15rem 0.4rem !important;
}

.p-button.p-button-sm {
  padding: 0.4rem 1rem;
  font-size: 0.875rem;
}

.p-button.p-button-sm .p-button-icon {
  font-size: 0.875rem;
}

.p-menu {
  background: #1f2937 !important;
  border-color: #374151 !important;
}

.p-menu .p-menuitem-link {
  color: #ffffff !important;
  padding: 0.5rem 1rem !important;
}

.p-menu .p-menuitem-link:hover {
  background: #374151 !important;
}

.p-menu .p-menuitem-link .p-menuitem-icon {
  color: #9ca3af !important;
}
</style>
