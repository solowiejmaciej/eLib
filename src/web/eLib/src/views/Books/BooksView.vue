<template>
  <div class="container mx-auto py-8 px-4">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-semibold">Books Collection</h1>
      <div class="flex gap-2">
        <span class="p-input-icon-left flex-1">
          <IconField class="w-full">
            <InputIcon class="pi pi-search" />
            <InputText
              v-model="filters.searchPhrase"
              placeholder="Search books..."
              class="w-full"
              @input="debounceSearch"
            />
          </IconField>
        </span>
        <Button
          v-if="isAdmin"
          icon="pi pi-plus"
          label="Add Book"
          @click="openNewBookDialog"
        />
      </div>
    </div>

    <DataTable
      :value="books"
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
        <template #header>Cover</template>
        <template #body="{ data }">
          <img
            :src="data.details.coverUrl"
            :alt="data.title"
            class="w-16 h-24 object-cover rounded shadow"
          />
        </template>
      </Column>

      <Column field="title" header="Title">
        <template #body="{ data }">
          <router-link
            :to="`/books/${data.id}`"
            class="text-primary-600 hover:text-primary-700 hover:underline"
          >
            {{ data.title }}
          </router-link>
        </template>
      </Column>

      <Column field="details.quantity" header="Available" />

      <Column v-if="isAdmin" :exportable="false">
        <template #body="{ data }">
          <div class="flex gap-2 justify-end">
            <Button icon="pi pi-pencil" @click="editBook(data)" text rounded />
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

    <BookForm
      :visible="bookDialog"
      @update:visible="(val) => (bookDialog = val)"
      :book-to-edit="editingBook"
      @book-saved="loadBooks"
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
import BookForm from "../../components/BookForm.vue";

const store = useStore();
const confirm = useConfirm();
const toast = useToast();

const books = ref([]);
const loading = ref(false);
const totalCount = ref(0);
const first = ref(0);
const bookDialog = ref(false);
const editingBook = ref(null);

const filters = ref({
  pageSize: 50,
  pageNumber: 1,
  searchPhrase: "",
});

const isAdmin = computed(() => store.getters.isAdmin);

const loadBooks = async () => {
  try {
    loading.value = true;
    const response = await apiClient.getBooks(
      filters.value.searchPhrase,
      filters.value.pageNumber,
      filters.value.pageSize
    );
    books.value = response.items;
    totalCount.value = response.totalCount;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load books",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const debounceSearch = debounce(() => {
  filters.value.pageNumber = 1;
  first.value = 0;
  loadBooks();
}, 300);

const onPage = (event) => {
  filters.value.pageNumber = event.page + 1;
  filters.value.pageSize = event.rows; // Dodaj tę linię
  loadBooks();
};

const openNewBookDialog = () => {
  editingBook.value = null;
  bookDialog.value = true;
};

const editBook = (data) => {
  editingBook.value = data;
  bookDialog.value = true;
};

const confirmDelete = (data) => {
  confirm.require({
    message: "Are you sure you want to delete this book?",
    header: "Confirm Delete",
    icon: "pi pi-exclamation-triangle",
    acceptClass: "p-button-danger",
    accept: () => deleteBook(data.id),
  });
};

const deleteBook = async (id) => {
  try {
    await apiClient.deleteBook(id);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Book deleted successfully",
      life: 3000,
    });
    loadBooks();
  } catch (error) {
    if (error.response?.status === 400) {
      toast.add({
        severity: "info",
        summary: "Invalid operation",
        detail: "Already borrowed books cannot be deleted",
        life: 3000,
      });
      return;
    }
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to delete book",
      life: 3000,
    });
  }
};

onMounted(() => {
  loadBooks();
});
</script>

<style scoped>
:deep(.p-datatable) {
  @apply shadow-lg rounded-lg;
}
</style>
