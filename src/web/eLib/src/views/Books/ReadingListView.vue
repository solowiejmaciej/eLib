<template>
  <div class="min-h-screen bg-slate-900 p-6">
    <div class="max-w-7xl mx-auto">
      <div class="flex items-center justify-between mb-8">
        <h1 class="text-2xl font-bold text-white">
          Reading List ({{ readingList.totalCount }})
        </h1>
      </div>

      <div v-if="loading" class="flex justify-center p-8">
        <ProgressSpinner />
      </div>

      <div
        v-else-if="readingList.items?.length === 0"
        class="text-center text-gray-400 text-lg p-8"
      >
        No books found in your reading list.
      </div>

      <div v-else class="space-y-4">
        <div
          v-for="book in readingList.items"
          :key="book.bookId"
          class="bg-slate-800/30 backdrop-blur-sm rounded-lg overflow-hidden"
          :class="{ 'border border-emerald-500/30': book.isFinished }"
        >
          <div class="flex p-4 gap-6">
            <img
              :src="book.coverUrl"
              :alt="book.title"
              class="w-24 h-36 object-cover rounded shadow-lg flex-shrink-0"
            />

            <div class="flex-grow space-y-4">
              <div>
                <div class="flex items-start justify-between">
                  <RouterLink
                    :to="{ path: `/books/${book.bookId}` }"
                    class="text-xl font-semibold text-white hover:text-cyan-400 transition-colors mb-2"
                  >
                    {{ book.title }}
                  </RouterLink>
                  <Badge
                    v-if="book.isFinished"
                    severity="success"
                    value="Finished"
                    class="ml-2"
                  />
                </div>
                <RouterLink
                  :to="{ path: `/authors/${book.authorId}` }"
                  class="text-gray-400 mb-2 hover:text-cyan-400 transition-colors inline-flex items-center"
                >
                  <i class="pi pi-user mr-2"></i>
                  {{ book.authorName }}
                </RouterLink>
                <div class="text-gray-400 text-sm">
                  <i class="pi pi-calendar mr-2"></i>
                  Added: {{ formatDate(book.dateAdded) }}
                </div>
              </div>

              <div>
                <div class="flex justify-between text-sm text-gray-400 mb-2">
                  <span>Progress:</span>
                  <span>{{ book.progress }}%</span>
                </div>
                <div
                  class="relative h-2 bg-slate-700/50 rounded overflow-hidden"
                >
                  <div
                    class="absolute h-full rounded transition-all duration-300"
                    :class="
                      book.isFinished ? 'bg-emerald-500/50' : 'bg-cyan-500/50'
                    "
                    :style="{ width: `${book.progress}%` }"
                  ></div>
                </div>
              </div>

              <div class="flex items-center gap-2">
                <Button
                  icon="pi pi-refresh"
                  label="Update Progress"
                  text
                  @click="mockProgressUpdate(book)"
                  class="text-gray-400"
                />
                <Button
                  icon="pi pi-check-circle"
                  :label="
                    book.isFinished ? 'Mark as Unfinished' : 'Mark as Finished'
                  "
                  text
                  @click="toggleFinished(book)"
                  class="text-gray-400"
                />
                <Button
                  icon="pi pi-trash"
                  severity="danger"
                  text
                  rounded
                  @click="confirmRemove(book)"
                  class="!p-3 hover:bg-red-500/20"
                />
              </div>
            </div>
          </div>
        </div>

        <Paginator
          v-if="readingList.totalCount > 0"
          :rows="pageSize"
          :totalRecords="readingList.totalCount"
          :first="(currentPage - 1) * pageSize"
          @page="onPageChange($event)"
          :template="paginatorTemplate"
          class="bg-slate-800/30 backdrop-blur-sm rounded-lg p-2 mt-6"
        />
      </div>
    </div>

    <ConfirmDialog />
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useConfirm } from "primevue/useconfirm";
import { useRouter } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import store from "../../store/store";

const confirm = useConfirm();
const router = useRouter();
const readingList = ref({
  items: [],
  totalCount: 0,
  totalPages: 0,
  pageNumber: 1,
});
const loading = ref(true);
const currentPage = ref(1);
const pageSize = 5;
const paginatorTemplate =
  "FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink";

const loadReadingList = async (page = 1) => {
  loading.value = true;
  try {
    readingList.value = await apiClient.getReadingList(
      store.getters.currentUser.id,
      page,
      pageSize
    );
    currentPage.value = page;
  } catch (error) {
    console.error("Error loading reading list:", error);
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const mockProgressUpdate = (book) => {
  console.log(
    `Updating progress for "${book.title}" to ${Math.min(
      100,
      book.progress + 10
    )}%`
  );
};

const toggleFinished = async (book) => {
  console.log(book.isFinished);
  try {
    if (book.isFinished) {
      console.log("Marking as unfinished");
      await apiClient.markReadingListEntryAsUnfinished(book.bookId);
    } else {
      await apiClient.markReadingListEntryAsFinished(book.bookId);
    }
    await loadReadingList(currentPage.value);
  } catch (error) {
    console.error("Error toggling finished status:", error);
  }
};

const confirmRemove = (book) => {
  confirm.require({
    message: `Are you sure you want to remove "${book.title}" from your reading list?`,
    header: "Confirm Removal",
    icon: "pi pi-exclamation-triangle",
    accept: () => removeFromList(book.bookId),
    reject: () => console.log("Removal cancelled"),
  });
};

const removeFromList = async (bookId) => {
  try {
    await apiClient.removeFromReadingList(bookId);
    // Reload current page
    await loadReadingList(currentPage.value);
  } catch (error) {
    console.error("Error removing book:", error);
  }
};

const onPageChange = (event) => {
  const newPage = Math.floor(event.first / pageSize) + 1;
  loadReadingList(newPage);
};

onMounted(() => {
  loadReadingList();
});
</script>

<style scoped>
:deep(.p-button.p-button-text:hover) {
  background: rgba(255, 255, 255, 0.1);
}

:deep(.p-paginator) {
  background: transparent;
  border: none;
}

:deep(.p-paginator .p-paginator-page.p-highlight) {
  background: rgba(6, 182, 212, 0.2);
  color: rgb(6, 182, 212);
}

:deep(.p-paginator .p-paginator-element) {
  color: #94a3b8;
}

:deep(.p-paginator .p-paginator-element:hover) {
  background: rgba(255, 255, 255, 0.1);
}

:deep(.p-badge) {
  background: rgba(16, 185, 129, 0.2);
  color: rgb(16, 185, 129);
}
</style>
