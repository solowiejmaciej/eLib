<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    :header="isEditing ? 'Edit Book' : 'New Book'"
    modal
    class="p-fluid"
    :style="{ width: '50rem' }"
    @hide="hideDialog"
  >
    <div class="grid grid-cols-1 gap-4">
      <div class="field">
        <label for="title">Title</label>
        <InputText
          id="title"
          v-model="bookData.title"
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
          v-model="bookData.description"
          rows="3"
          autoResize
        />
      </div>

      <div class="field">
        <label for="quantity">Quantity</label>
        <InputNumber id="quantity" v-model="bookData.quantity" :min="0" />
      </div>

      <div class="field">
        <label for="coverImageUrl">Cover URL</label>
        <InputText id="coverImageUrl" v-model="bookData.coverImageUrl" />
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
</template>

<script setup>
import { ref, computed, watch } from "vue";
import { useToast } from "primevue/usetoast";
import apiClient from "../clients/eLibApiClient";

const props = defineProps({
  visible: {
    type: Boolean,
    required: true,
  },
  bookToEdit: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["update:visible", "book-saved"]);

const toast = useToast();
const saving = ref(false);
const loadingAuthors = ref(false);
const authorSuggestions = ref([]);
const selectedAuthor = ref(null);

const bookData = ref({
  title: "",
  authorId: null,
  description: "",
  coverImageUrl: "",
  quantity: 0,
});

const isEditing = computed(() => !!props.bookToEdit);

watch(
  () => props.bookToEdit,
  async (newBook) => {
    if (newBook) {
      bookData.value = {
        id: newBook.id,
        title: newBook.title,
        authorId: newBook.authorId,
        description: newBook.details.description,
        coverImageUrl: newBook.details.coverUrl,
        quantity: newBook.details.quantity,
      };
      await loadAuthorDetails(newBook.authorId);
    } else {
      resetForm();
    }
  },
  { immediate: true }
);

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

const resetForm = () => {
  bookData.value = {
    title: "",
    authorId: null,
    description: "",
    coverImageUrl: "",
    quantity: 0,
  };
  selectedAuthor.value = null;
};

const hideDialog = () => {
  emit("update:visible", false);
  resetForm();
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

    const bookPayload = {
      title: bookData.value.title,
      authorId: selectedAuthor.value.id,
      description: bookData.value.description,
      coverImageUrl: bookData.value.coverImageUrl,
      quantity: bookData.value.quantity,
    };

    if (isEditing.value) {
      await apiClient.updateBook(bookData.value.id, bookPayload);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book updated successfully",
        life: 3000,
      });
    } else {
      await apiClient.createBook(bookPayload);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book created successfully",
        life: 3000,
      });
    }

    emit("book-saved");
    hideDialog();
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
</script>
