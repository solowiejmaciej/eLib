<template>
  <div class="relative w-full" ref="searchContainer">
    <span class="p-input-icon-left w-full">
      <IconField class="w-full">
        <InputIcon class="pi pi-search" />
        <InputText
          placeholder="Search"
          class="w-full bg-gray-800 border-gray-700 text-gray-200"
          size="large"
          @update:modelValue="handleSearch"
          v-model="searchPhrase"
        />
      </IconField>
    </span>
    <div
      v-if="books.length > 0 && showResults"
      class="absolute mt-1 w-full bg-gray-800 rounded-lg shadow-lg overflow-hidden z-50"
    >
      <div v-if="loading" class="flex justify-center p-4">
        <ProgressSpinner />
      </div>

      <div v-else>
        <div
          v-for="book in books"
          :key="book.id"
          class="flex items-center gap-3 p-2 border-b border-gray-700 hover:bg-gray-700 transition-colors cursor-pointer"
          @click="openBookDetails(book.id)"
        >
          <img
            :src="book.details.coverUrl"
            :alt="book.title"
            class="w-10 h-14 object-cover rounded"
          />
          <div class="flex-1 min-w-0">
            <h3 class="text-sm font-medium text-gray-200 truncate">
              {{ book.title }}
            </h3>
            <span class="text-xs text-gray-400">
              Available: {{ book.details.quantity }}
            </span>
          </div>
        </div>

        <div @click.stop class="border-t border-gray-700">
          <Paginator
            v-if="totalPages > 1"
            :rows="pageSize"
            :totalRecords="totalCount"
            :first="(currentPage - 1) * pageSize"
            @page="onPageChange"
            :template="'PrevPageLink PageLinks NextPageLink'"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import InputText from "primevue/inputtext";
import Paginator from "primevue/paginator";
import ProgressSpinner from "primevue/progressspinner";
import { ref, onMounted, onUnmounted } from "vue";
import apiClient from "../clients/eLibApiClient";
import { useRouter } from "vue-router";

var router = useRouter();

const searchPhrase = ref("");
const books = ref([]);
const loading = ref(false);
const currentPage = ref(1);
const pageSize = ref(5);
const totalCount = ref(0);
const totalPages = ref(0);
const showResults = ref(false);
const searchContainer = ref(null);

async function loadBooks(phrase = "", page = 1) {
  try {
    loading.value = true;
    const response = await apiClient.getBooks(phrase, page, pageSize.value);
    books.value = response.items;
    totalCount.value = response.totalCount;
    totalPages.value = response.totalPages;
    currentPage.value = response.pageNumber;
  } catch (error) {
    console.error("Error loading books:", error);
  } finally {
    loading.value = false;
  }
}

async function handleSearch() {
  currentPage.value = 1;
  showResults.value = true;
  await loadBooks(searchPhrase.value);
}

function onPageChange(event) {
  const newPage = Math.floor(event.first / event.rows) + 1;
  loadBooks(searchPhrase.value, newPage);
}

function handleClickOutside(event) {
  if (searchContainer.value && !searchContainer.value.contains(event.target)) {
    showResults.value = false;
  }
}

onMounted(() => {
  document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});

function openBookDetails(bookId) {
  searchPhrase.value = "";
  showResults.value = false;
  router.push("/books/" + bookId);
}
</script>

<style scoped>
:deep(.p-inputtext) {
  @apply bg-gray-800 border-gray-700 text-gray-200;
}

:deep(.p-inputtext:enabled:focus) {
  @apply border-blue-500 shadow-none;
}

:deep(.p-paginator) {
  @apply bg-transparent;
}

:deep(.p-paginator .p-paginator-page) {
  @apply text-gray-200;
}

:deep(.p-paginator .p-paginator-prev),
:deep(.p-paginator .p-paginator-next),
:deep(.p-paginator .p-paginator-last) {
  @apply text-gray-200;
}

:deep(.p-paginator .p-paginator-page.p-highlight) {
  @apply bg-gray-700;
}

:deep(.pi-search) {
  @apply text-gray-400;
}
</style>
