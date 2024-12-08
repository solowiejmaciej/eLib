<template>
  <div class="container mx-auto py-8 px-4">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-semibold">Authors Collection</h1>
      <div class="flex gap-2">
        <span class="p-input-icon-left flex-1">
          <IconField class="w-full">
            <InputIcon class="pi pi-search" />
            <InputText
              v-model="filters.searchPhrase"
              placeholder="Search authors..."
              class="w-full"
              @input="debounceSearch"
            />
          </IconField>
        </span>
        <Button
          v-if="isAdmin"
          icon="pi pi-plus"
          label="Add Author"
          @click="openNewAuthorDialog"
        />
        <Button
          icon="pi pi-book"
          label="Books"
          @click="$router.push('/books')"
          text
          raised
        />
      </div>
    </div>

    <DataTable
      :value="authors"
      :loading="loading"
      :paginator="true"
      :rows="filters.pageSize"
      :totalRecords="totalCount"
      :lazy="true"
      @page="onPage($event)"
      stripedRows
      class="p-datatable-lg"
      v-model:first="first"
      :rowsPerPageOptions="[10, 25, 50]"
    >
      <Column>
        <template #header>Photo</template>
        <template #body="{ data }">
          <img
            :src="data.details.photoUrl"
            :alt="`${data.name} ${data.surname}`"
            class="w-12 h-12 object-cover rounded-full shadow"
          />
        </template>
      </Column>

      <Column field="name" header="Name">
        <template #body="{ data }">
          <router-link
            :to="`/authors/${data.id}`"
            class="text-primary-600 hover:text-primary-700 hover:underline"
          >
            {{ data.name }} {{ data.surname }}
          </router-link>
        </template>
      </Column>

      <Column field="birthday" header="Born">
        <template #body="{ data }">
          {{ new Date(data.birthday).toLocaleDateString() }}
        </template>
      </Column>

      <Column v-if="isAdmin" :exportable="false">
        <template #body="{ data }">
          <div class="flex gap-2 justify-end">
            <Button
              icon="pi pi-pencil"
              @click="editAuthor(data)"
              text
              rounded
            />
            <Button
              icon="pi pi-trash"
              @click="confirmDelete(data)"
              severity="danger"
              text
              rounded
            />
          </div>
        </template>
      </Column>
    </DataTable>

    <AuthorForm
      :visible="authorDialog"
      @update:visible="(val) => (authorDialog = val)"
      :author-to-edit="editingAuthor"
      @author-saved="loadAuthors"
    />

    <ConfirmDialog />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useStore } from "vuex";
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import debounce from "lodash/debounce";
import apiClient from "../../clients/eLibApiClient";
import AuthorForm from "../../components/AuthorForm.vue";

const store = useStore();
const confirm = useConfirm();
const toast = useToast();

const authors = ref([]);
const loading = ref(false);
const totalCount = ref(0);
const first = ref(0);
const authorDialog = ref(false);
const editingAuthor = ref(null);

const filters = ref({
  pageSize: 50,
  pageNumber: 1,
  searchPhrase: "",
});

const isAdmin = computed(() => store.getters.isAdmin);

const loadAuthors = async () => {
  try {
    loading.value = true;
    const response = await apiClient.getAuthors(
      filters.value.searchPhrase,
      filters.value.pageNumber,
      filters.value.pageSize
    );
    authors.value = response.items;
    totalCount.value = response.totalCount;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load authors",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const debounceSearch = debounce(() => {
  filters.value.pageNumber = 1;
  first.value = 0;
  loadAuthors();
}, 300);

const onPage = (event) => {
  filters.value.pageNumber = event.page + 1;
  filters.value.pageSize = event.rows;
  loadAuthors();
};

const openNewAuthorDialog = () => {
  editingAuthor.value = null;
  authorDialog.value = true;
};

const editAuthor = (data) => {
  editingAuthor.value = data;
  authorDialog.value = true;
};

const confirmDelete = (data) => {
  confirm.require({
    message: "Are you sure you want to delete this author?",
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: () => deleteAuthor(data.id),
  });
};

const deleteAuthor = async (id) => {
  try {
    await apiClient.deleteAuthor(id);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Author deleted successfully",
      life: 3000,
    });
    loadAuthors();
  } catch (error) {
    if (error.response?.status === 400) {
      toast.add({
        severity: "info",
        summary: "Invalid operation",
        detail: "Authors with books cannot be deleted",
        life: 3000,
      });
      return;
    }
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to delete author",
      life: 3000,
    });
  }
};

onMounted(() => {
  loadAuthors();
});
</script>

<style scoped>
:deep(.p-datatable) {
  @apply shadow-lg rounded-lg;
}
</style>
