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

    <Dialog
      v-model:visible="bookDialog"
      :header="editingBook ? 'Edit Book' : 'New Book'"
      modal
      class="p-fluid"
      :style="{ width: '50rem' }"
    >
      <div class="grid grid-cols-1 gap-4">
        <div class="field">
          <label for="title">Title</label>
          <InputText
            id="title"
            v-model="book.title"
            required="true"
            autofocus
          />
        </div>

        <div class="field">
          <label for="author">Author</label>
          <div class="p-input-icon-right w-full">
            <i v-if="loadingAuthors" class="pi pi-spin pi-spinner" />
            <AutoComplete
              id="author"
              v-model="selectedAuthor"
              :suggestions="authorSuggestions"
              @complete="searchAuthors"
              optionLabel="fullName"
              class="w-full"
              forceSelection
              placeholder="Search for an author"
            >
              <template #option="slotProps">
                <div class="flex align-items-center">
                  <img
                    :src="slotProps.option.details.photoUrl"
                    :alt="slotProps.option.fullName"
                    class="w-8 h-8 rounded-full mr-2"
                  />
                  <div>
                    <div>{{ slotProps.option.fullName }}</div>
                    <small
                      >Born:
                      {{
                        new Date(slotProps.option.birthday).toLocaleDateString()
                      }}</small
                    >
                  </div>
                </div>
              </template>
            </AutoComplete>
          </div>
        </div>

        <div class="field">
          <label for="description">Description</label>
          <Textarea
            id="description"
            v-model="book.description"
            rows="3"
            autoResize
          />
        </div>

        <div class="field">
          <label for="quantity">Quantity</label>
          <InputNumber id="quantity" v-model="book.quantity" :min="0" />
        </div>

        <div class="field">
          <label for="coverImageUrl">Cover URL</label>
          <InputText id="coverImageUrl" v-model="book.coverImageUrl" />
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end gap-2">
          <Button label="Cancel" icon="pi pi-times" @click="hideDialog" text />
          <Button
            label="Save"
            icon="pi pi-check"
            @click="saveBook"
            :loading="saving"
          />
        </div>
      </template>
    </Dialog>

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

const store = useStore();
const confirm = useConfirm();
const toast = useToast();

const books = ref([]);
const loading = ref(false);
const totalCount = ref(0);
const first = ref(0);
const bookDialog = ref(false);
const saving = ref(false);
const editingBook = ref(false);
const loadingAuthors = ref(false);
const authorSuggestions = ref([]);
const selectedAuthor = ref(null);

const filters = ref({
  pageSize: 50,
  pageNumber: 1,
  searchPhrase: "",
});

const book = ref({
  title: "",
  authorId: null,
  description: "",
  coverImageUrl: "",
  quantity: 0,
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
  filters.value.pageNumber = Math.floor(event.first / event.rows) + 1;
  first.value = event.first;
  loadBooks();
};

const searchAuthors = async (event) => {
  try {
    loadingAuthors.value = true;
    const response = await apiClient.getAuthors(event.query, 1, 10);
    authorSuggestions.value = response.items.map((author) => ({
      ...author,
      fullName: `${author.name} ${author.surname}`,
    }));
  } catch (error) {
    console.error("Failed to search authors:", error);
  } finally {
    loadingAuthors.value = false;
  }
};

const openNewBookDialog = () => {
  book.value = {
    title: "",
    authorId: null,
    description: "",
    coverImageUrl: "",
    quantity: 0,
  };
  selectedAuthor.value = null;
  editingBook.value = false;
  bookDialog.value = true;
};

const editBook = (data) => {
  book.value = {
    id: data.id,
    title: data.title,
    authorId: data.authorId,
    description: data.details.description,
    coverImageUrl: data.details.coverUrl,
    quantity: data.details.quantity,
  };
  selectedAuthor.value = null;
  editingBook.value = true;
  bookDialog.value = true;

  loadAuthorDetails(data.authorId);
};

const loadAuthorDetails = async (authorId) => {
  try {
    const author = await apiClient.getAuthorById(authorId);
    selectedAuthor.value = {
      ...author,
      fullName: `${author.name} ${author.surname}`,
    };
  } catch (error) {
    console.error("Failed to load author details:", error);
  }
};

const hideDialog = () => {
  bookDialog.value = false;
  editingBook.value = false;
  selectedAuthor.value = null;
};

const saveBook = async () => {
  if (!selectedAuthor.value) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Please select an author",
      life: 3000,
    });
    return;
  }

  try {
    saving.value = true;

    const bookData = {
      title: book.value.title,
      authorId: selectedAuthor.value.id,
      description: book.value.description,
      coverImageUrl: book.value.coverImageUrl,
      quantity: book.value.quantity,
    };

    if (editingBook.value) {
      await apiClient.updateBook(book.value.id, bookData);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book updated successfully",
        life: 3000,
      });
    } else {
      await apiClient.createBook(bookData);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book created successfully",
        life: 3000,
      });
    }
    hideDialog();
    loadBooks();
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to save book",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
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
    if (error.response.status === 400) {
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
