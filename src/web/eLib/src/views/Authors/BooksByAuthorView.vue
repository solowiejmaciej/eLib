<template>
  <div class="books-container p-4">
    <div class="flex justify-content-between align-items-center mb-4">
      <span class="p-input-icon-left flex-1">
        <IconField class="w-full">
          <InputIcon class="pi pi-search" />
          <InputText
            v-model="searchPhrase"
            placeholder="Search books..."
            class="w-full"
            @input="debounceSearch"
          />
        </IconField>
      </span>
    </div>

    <div v-if="loading" class="flex justify-content-center my-8">
      <ProgressSpinner strokeWidth="4" />
    </div>

    <div v-else-if="books.length === 0" class="text-center p-4 text-gray-400">
      No books found
    </div>

    <div v-else class="grid">
      <div
        v-for="book in books"
        :key="book.id"
        class="col-12 md:col-6 lg:col-4"
      >
        <div class="book-card p-3 m-2">
          <div class="relative">
            <Image
              :src="book.details.coverUrl"
              :alt="book.title"
              imageClass="w-full h-64 object-cover rounded-lg"
              preview
            />
            <div class="absolute bottom-2 right-2">
              <Tag
                :value="`${book.details.quantity} left`"
                :severity="getQuantitySeverity(book.details.quantity)"
              />
            </div>
          </div>

          <div class="mt-3">
            <h3
              class="text-lg font-medium text-white mb-2 line-clamp-1"
              :title="book.title"
            >
              {{ book.title }}
            </h3>
            <p
              class="text-gray-400 line-clamp-3 text-sm mb-3"
              :title="book.details.description"
            >
              {{ book.details.description }}
            </p>
            <div class="flex justify-content-end">
              <Button
                icon="pi pi-external-link"
                label="Details"
                text
                @click="router.push(`/books/${book.id}`)"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <Paginator
      v-if="totalRecords > pageSize"
      v-model:rows="pageSize"
      v-model:first="first"
      :total-records="totalRecords"
      :rows-per-page-options="[10, 20, 30]"
      class="mt-4"
      @page="onPageChange"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import { debounce } from "lodash";

// Components
import Image from "primevue/image";
import InputText from "primevue/inputtext";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Tag from "primevue/tag";
import Paginator from "primevue/paginator";

const route = useRoute();
const router = useRouter();
const searchPhrase = ref("");
const pageNumber = ref(1);
const pageSize = ref(10);
const first = ref(0);
const books = ref([]);
const loading = ref(true);
const totalRecords = ref(0);

const loadBooks = async () => {
  try {
    loading.value = true;
    const authorId = route.params.id;
    const response = await apiClient.getBooksByAuthorId(
      searchPhrase.value,
      pageNumber.value,
      pageSize.value,
      authorId
    );
    books.value = response.items;
    totalRecords.value = response.totalCount;
  } catch (error) {
    console.error("Error loading books:", error);
  } finally {
    loading.value = false;
  }
};

const getQuantitySeverity = (quantity) => {
  if (quantity <= 3) return "danger";
  if (quantity <= 5) return "warning";
  return "success";
};

const debounceSearch = debounce(() => {
  pageNumber.value = 1;
  first.value = 0;
  loadBooks();
}, 300);

const onPageChange = (event) => {
  first.value = event.first;
  pageNumber.value = event.page + 1;
  pageSize.value = event.rows;
  loadBooks();
};

onMounted(() => {
  loadBooks();
});
</script>

<style scoped>
.books-container {
  max-width: 1400px;
  margin: 0 auto;
}

.book-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  transition: transform 0.2s ease;
  height: 100%;
}

.book-card:hover {
  transform: translateY(-4px);
}

.line-clamp-1 {
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
