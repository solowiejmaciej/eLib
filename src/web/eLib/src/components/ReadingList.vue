<template>
  <div class="space-y-6">
    <div class="bg-gray-800 rounded-lg p-4 border border-gray-700">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold">
          Reading List ({{ readingList.totalCount }})
        </h2>
        <Button
          icon="pi pi-refresh"
          class="p-button-text p-button-sm text-white"
          @click="() => loadReadingList(currentPage)"
          :loading="loading"
        />
      </div>

      <DataTable
        v-if="!loading"
        :value="readingList.items"
        :rows="pageSize"
        :totalRecords="readingList.totalCount"
        :lazy="true"
        :paginator="true"
        :loading="loading"
        @page="onPageChange"
        :first="(currentPage - 1) * pageSize"
        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport"
        currentPageReportTemplate="{first} to {last} of {totalRecords}"
        class="p-datatable-sm"
        stripedRows
        removableSort
        :rowHover="true"
      >
        <Column field="title" header="Book" style="width: 250px">
          <template #body="slotProps">
            <div class="flex items-center gap-2">
              <Avatar
                :image="slotProps.data.coverUrl"
                size="normal"
                shape="circle"
                class="bg-indigo-500"
              />
              <RouterLink
                :to="{ path: `/books/${slotProps.data.bookId}` }"
                class="text-sm hover:text-cyan-400 transition-colors"
              >
                {{ slotProps.data.title }}
              </RouterLink>
            </div>
          </template>
        </Column>

        <Column field="authorName" header="Author" style="width: 200px">
          <template #body="slotProps">
            <RouterLink
              :to="{ path: `/authors/${slotProps.data.authorId}` }"
              class="text-sm hover:text-cyan-400 transition-colors flex items-center"
            >
              <i class="pi pi-user mr-2"></i>
              {{ slotProps.data.authorName }}
            </RouterLink>
          </template>
        </Column>

        <Column field="progress" header="Progress" style="width: 200px">
          <template #body="slotProps">
            <div class="space-y-2">
              <div class="flex justify-between text-sm text-gray-400">
                <span>{{ slotProps.data.progress }}%</span>
                <Tag
                  v-if="slotProps.data.isFinished"
                  severity="success"
                  value="Finished"
                  class="ml-2"
                />
              </div>
              <div class="relative h-2 bg-slate-700/50 rounded overflow-hidden">
                <div
                  class="absolute h-full rounded transition-all duration-300"
                  :class="getProgressBarClass(slotProps.data)"
                  :style="{ width: `${slotProps.data.progress}%` }"
                ></div>
              </div>
            </div>
          </template>
        </Column>

        <Column field="dateAdded" header="Added Date" style="width: 200px">
          <template #body="slotProps">
            <div class="text-sm text-gray-400">
              <i class="pi pi-calendar mr-2"></i>
              {{ formatDate(slotProps.data.dateAdded) }}
            </div>
          </template>
        </Column>
      </DataTable>

      <ProgressSpinner
        v-else
        style="width: 40px; height: 40px"
        class="flex justify-center p-4"
      />
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useToast } from "primevue/usetoast";
import { useConfirm } from "primevue/useconfirm";
import { useRoute } from "vue-router";
import apiClient from "../clients/eLibApiClient";

const toast = useToast();
const confirm = useConfirm();
const route = useRoute();
const loading = ref(true);
const currentPage = ref(1);
const pageSize = 5;

const readingList = ref({
  items: [],
  totalCount: 0,
  totalPages: 0,
  pageNumber: 1,
});

const loadReadingList = async (page = 1) => {
  loading.value = true;
  try {
    const pageNumber = typeof page === "number" ? page : 1;
    readingList.value = await apiClient.getReadingList(
      route.params.id,
      pageNumber,
      pageSize
    );
    currentPage.value = pageNumber;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load reading list",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

const getProgressBarClass = (book) => {
  return {
    "bg-emerald-500/50": book.isFinished,
    "bg-cyan-500/50": !book.isFinished,
  };
};

const updateProgress = async (book) => {
  try {
    loading.value = true;
    const newProgress = Math.min(100, book.progress + 10);
    await apiClient.updateReadingProgress(book.bookId, newProgress);
    await loadReadingList(currentPage.value);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Progress updated successfully",
      life: 3000,
    });
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to update progress",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const toggleFinished = async (book) => {
  try {
    loading.value = true;
    if (book.isFinished) {
      await apiClient.markReadingListEntryAsUnfinished(book.bookId);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book marked as unfinished",
        life: 3000,
      });
    } else {
      await apiClient.markReadingListEntryAsFinished(book.bookId);
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Book marked as finished",
        life: 3000,
      });
    }
    await loadReadingList(currentPage.value);
  } catch (error) {
    console.error("Error toggling finished status:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to update book status",
      life: 3000,
    });
  } finally {
    loading.value = false;
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
    loading.value = true;
    await apiClient.removeFromReadingList(bookId);
    await loadReadingList(currentPage.value);
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Book removed from reading list",
      life: 3000,
    });
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to remove book",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const onPageChange = (event) => {
  const newPage = Math.floor(event.first / pageSize) + 1;
  loadReadingList(newPage);
};

onMounted(() => {
  loadReadingList(1);
});
</script>

<style scoped>
:deep(.p-button.p-button-text) {
  padding: 0.5rem;
}

:deep(.p-button.p-button-text:hover) {
  background: rgba(255, 255, 255, 0.1);
}

:deep(.p-button.p-button-text.text-red-500:hover) {
  background: rgba(239, 68, 68, 0.1);
}

:deep(.p-datatable) {
  background: transparent;
}

:deep(.p-datatable .p-datatable-thead > tr > th) {
  background: transparent;
  color: #94a3b8;
  border-color: rgba(148, 163, 184, 0.2);
}

:deep(.p-datatable .p-datatable-tbody > tr) {
  background: transparent;
  color: #e2e8f0;
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
  border-color: rgba(148, 163, 184, 0.2);
}

:deep(.p-datatable .p-datatable-tbody > tr:hover) {
  background: rgba(255, 255, 255, 0.03);
}

:deep(.p-badge) {
  background: rgba(16, 185, 129, 0.2);
  color: rgb(16, 185, 129);
}

:deep(.p-tag.p-tag-success) {
  background: rgba(16, 185, 129, 0.2);
  color: rgb(16, 185, 129);
}
</style>
