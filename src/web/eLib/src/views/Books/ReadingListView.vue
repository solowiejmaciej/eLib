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
          v-for="(book, index) in readingList.items"
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
                  class="text-gray-400"
                  @click="(e) => showProgressPanel(e, book, index)"
                />
                <OverlayPanel :ref="(el) => (overlayPanels[index] = el)">
                  <div class="p-4 w-64">
                    <div class="flex gap-2">
                      <InputNumber
                        v-model="progressValues[index]"
                        :min="0"
                        :max="100"
                        class="w-24"
                        :inputClass="'w-full'"
                        suffix="%"
                      />
                      <Button
                        label="Update"
                        @click="updateProgress(book, index)"
                        :disabled="!isProgressValid(index)"
                        class="!bg-cyan-400 hover:!bg-cyan-500 !border-none !text-slate-900 font-medium"
                      />
                    </div>
                    <small
                      class="block mt-2 text-gray-400"
                      v-if="!isProgressValid(index)"
                    >
                      Please enter a value between 0 and 100
                    </small>
                  </div>
                </OverlayPanel>

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
import { onMounted, ref, computed } from "vue";
import { useConfirm } from "primevue/useconfirm";
import { useRouter } from "vue-router";
import OverlayPanel from "primevue/overlaypanel";
import InputNumber from "primevue/inputnumber";
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

// Refs for progress update
const overlayPanels = ref([]);
const progressValues = ref([]);

const isProgressValid = (index) => {
  const value = progressValues.value[index];
  return value >= 0 && value <= 100;
};

const loadReadingList = async (page = 1) => {
  loading.value = true;
  try {
    readingList.value = await apiClient.getReadingList(
      store.getters.currentUser.id,
      page,
      pageSize
    );
    currentPage.value = page;

    // Initialize progress values for each book
    progressValues.value = readingList.value.items.map((book) => book.progress);
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

const showProgressPanel = (event, book, index) => {
  progressValues.value[index] = book.progress;
  overlayPanels.value[index]?.show(event);
};

const updateProgress = async (book, index) => {
  if (!isProgressValid(index)) return;

  try {
    await apiClient.updateReadingProgress(
      book.bookId,
      progressValues.value[index]
    );
    await loadReadingList(currentPage.value);
    overlayPanels.value[index]?.hide();
  } catch (error) {
    console.error("Error updating progress:", error);
  }
};

const toggleFinished = async (book) => {
  try {
    if (book.isFinished) {
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

:deep(.p-overlaypanel) {
  background: rgb(30, 41, 59) !important;
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

:deep(.p-overlaypanel-content) {
  background: rgb(30, 41, 59) !important;
  color: white !important;
}

:deep(.p-overlaypanel-close) {
  background: transparent !important;
  color: rgb(148, 163, 184) !important;
}

:deep(.p-overlaypanel-close:hover) {
  background: rgba(255, 255, 255, 0.1) !important;
}

:deep(.p-inputnumber-input) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: white !important;
}

:deep(.p-button.p-button-text:hover) {
  background: rgba(255, 255, 255, 0.1);
}

:deep(.p-button:enabled:hover) {
  background: rgb(34 211 238) !important;
  border: none;
}

:deep(.p-button:focus) {
  box-shadow: none !important;
  border: none;
}
</style>
