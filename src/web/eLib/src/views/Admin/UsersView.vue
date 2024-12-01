<template>
  <div class="container mx-auto py-8 px-4">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-xl font-semibold text-white">Users Collection</h2>
      <div class="flex gap-2">
        <span class="p-input-icon-left flex-1">
          <IconField class="w-full">
            <InputIcon class="pi pi-search" />
            <InputText
              v-model="filters.searchPhrase"
              placeholder="Search users..."
              class="w-full bg-slate-800 border-slate-700 text-white"
              @input="debounceSearch"
            />
          </IconField>
        </span>
      </div>
    </div>

    <DataTable
      :value="users"
      :loading="loading"
      :paginator="true"
      v-model:rows="filters.pageSize"
      :totalRecords="totalCount"
      :lazy="true"
      @page="onPage"
      stripedRows
      class="p-datatable-sm"
      :rowsPerPageOptions="[5, 10, 25, 50]"
    >
      <Column>
        <template #body="slotProps">
          <Avatar
            :label="getInitials(slotProps.data)"
            size="large"
            shape="circle"
            :class="{
              'bg-purple-500': !slotProps.data?.details?.isAdmin,
              'bg-red-500': slotProps.data?.details?.isAdmin,
            }"
          />
        </template>
      </Column>

      <Column field="name" header="Name" class="text-white">
        <template #body="slotProps">
          <span class="text-white"
            >{{ slotProps.data.name }} {{ slotProps.data.surname }}</span
          >
        </template>
      </Column>

      <Column field="email" header="Email" class="text-white">
        <template #body="slotProps">
          <span class="text-white">{{ slotProps.data.email }}</span>
        </template>
      </Column>

      <Column :exportable="false">
        <template #body="slotProps">
          <div class="flex gap-2 justify-end">
            <Button
              icon="pi pi-user"
              text
              rounded
              severity="info"
              @click="navigateToProfile(slotProps.data.id)"
            />
            <Button
              icon="pi pi-trash"
              text
              rounded
              severity="danger"
              @click="confirmDelete(slotProps.data)"
            />
          </div>
        </template>
      </Column>
    </DataTable>

    <Dialog
      v-model:visible="deleteDialogVisible"
      header="Confirm Delete"
      :style="{ width: '450px' }"
      :modal="true"
    >
      <div class="confirmation-content">
        <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem" />
        <span
          >Are you sure you want to delete {{ selectedUser?.name }}
          {{ selectedUser?.surname }}?</span
        >
      </div>
      <template #footer>
        <Button
          label="No"
          icon="pi pi-times"
          text
          @click="deleteDialogVisible = false"
        />
        <Button
          label="Yes"
          icon="pi pi-check"
          severity="danger"
          @click="deleteUser"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useToast } from "primevue/usetoast";
import debounce from "lodash/debounce";
import apiClient from "../../clients/eLibApiClient";

const router = useRouter();
const toast = useToast();

const users = ref([]);
const loading = ref(false);
const totalCount = ref(0);
const first = ref(0);
const deleteDialogVisible = ref(false);
const selectedUser = ref(null);

const filters = ref({
  pageSize: 10,
  pageNumber: 1,
  searchPhrase: "",
});

const getUsers = async () => {
  try {
    loading.value = true;
    const response = await apiClient.getUsers(
      filters.value.searchPhrase,
      filters.value.pageNumber,
      filters.value.pageSize
    );
    users.value = response.items;
    totalCount.value = response.totalCount;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to fetch users",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const debounceSearch = debounce(() => {
  filters.value.pageNumber = 1;
  first.value = 0;
  getUsers();
}, 300);

const getInitials = (user) => {
  return `${user.name[0]}${user.surname[0]}`;
};

const onPage = (event) => {
  filters.value.pageNumber = Math.floor(event.first / event.rows) + 1;
  first.value = event.first;
  getUsers();
};

const navigateToProfile = (userId) => {
  router.push(`/admin/users/${userId}`);
};

const confirmDelete = (user) => {
  selectedUser.value = user;
  deleteDialogVisible.value = true;
};

const deleteUser = async () => {
  try {
    await apiClient.deleteUser(selectedUser.value.id);
    deleteDialogVisible.value = false;
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "User deleted successfully",
      life: 3000,
    });
    getUsers();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to delete user",
      life: 3000,
    });
  }
};

onMounted(() => {
  getUsers();
});
</script>

<style scoped>
:deep(.p-datatable) {
  @apply rounded-lg overflow-hidden shadow-lg;
}

:deep(.p-datatable .p-datatable-header) {
  @apply bg-slate-800 border-slate-700 text-white;
}

:deep(.p-datatable .p-datatable-thead > tr > th) {
  @apply bg-slate-800 border-slate-700 text-white;
}

:deep(.p-datatable .p-datatable-tbody > tr) {
  @apply bg-slate-800 border-slate-700;
}

:deep(.p-datatable .p-paginator) {
  @apply bg-slate-800 border-slate-700;
}

:deep(.p-datatable .p-paginator .p-paginator-pages .p-paginator-page) {
  @apply text-white;
}

:deep(.p-dialog) {
  @apply bg-slate-800 text-white;
}

:deep(.p-dialog .p-dialog-header) {
  @apply bg-slate-800 border-slate-700 text-white;
}

:deep(.p-dialog .p-dialog-content) {
  @apply bg-slate-800 border-slate-700 text-white;
}

:deep(.p-dialog .p-dialog-footer) {
  @apply bg-slate-800 border-slate-700;
}
</style>
