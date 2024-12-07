<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div class="flex gap-4 items-center">
        <InputText
          v-model="searchPhrase"
          placeholder="Search reviews..."
          class="bg-gray-800 border-gray-700 text-white w-64"
          @input="debouncedSearch"
        />
        <Dropdown
          v-model="pageSize"
          :options="[10, 20, 50]"
          class="w-24"
          @change="loadReviews"
        />
      </div>
    </div>

    <div class="bg-gray-900 rounded-lg p-4">
      <div class="grid grid-cols-12 gap-4 mb-4 text-gray-300 font-medium">
        <div class="col-span-8">Review Content</div>
        <div class="col-span-4">Date</div>
      </div>

      <div v-if="loading" class="flex justify-center py-8">
        <ProgressSpinner />
      </div>

      <div v-else class="space-y-4">
        <div
          v-for="review in reviews"
          :key="review.id"
          class="grid grid-cols-12 gap-4 py-4 border-b border-gray-800 items-center"
        >
          <div class="col-span-8">
            <p class="text-white mb-2">{{ review.content }}</p>
            <Rating :modelValue="review.rating" readonly :cancel="false" />
          </div>
          <div class="col-span-3 text-gray-400">
            {{ formatDate(review.createdAt) }}
          </div>
          <div class="col-span-1 flex justify-end">
            <Button
              icon="pi pi-trash"
              severity="danger"
              text
              rounded
              @click="confirmDelete(review)"
            />
          </div>
        </div>
      </div>

      <!-- Custom Pagination -->
      <div class="flex justify-between items-center mt-4">
        <div class="flex gap-2">
          <Button
            icon="pi pi-angle-double-left"
            text
            @click="goToPage(1)"
            :disabled="currentPage === 1"
          />
          <Button
            icon="pi pi-angle-left"
            text
            @click="goToPage(currentPage - 1)"
            :disabled="currentPage === 1"
          />
          <Button
            v-for="pageNum in displayedPages"
            :key="pageNum"
            :text="currentPage !== pageNum"
            :outlined="currentPage === pageNum"
            @click="goToPage(pageNum)"
          >
            {{ pageNum }}
          </Button>
          <Button
            icon="pi pi-angle-right"
            text
            @click="goToPage(currentPage + 1)"
            :disabled="currentPage === totalPages"
          />
          <Button
            icon="pi pi-angle-double-right"
            text
            @click="goToPage(totalPages)"
            :disabled="currentPage === totalPages"
          />
        </div>
      </div>
    </div>

    <Dialog
      v-model:visible="deleteDialogVisible"
      header="Confirm Delete"
      modal
      :style="{ width: '350px' }"
    >
      <div class="flex flex-col gap-4">
        <p>Are you sure you want to delete this review?</p>
        <div class="flex justify-end gap-2">
          <Button label="No" @click="deleteDialogVisible = false" text />
          <Button
            label="Yes"
            @click="deleteReview"
            severity="danger"
            :loading="deleteLoading"
          />
        </div>
      </div>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useToast } from "primevue/usetoast";
import debounce from "lodash/debounce";
import apiClient from "../clients/eLibApiClient";

const toast = useToast();

const props = defineProps({
  userId: {
    type: String,
    required: true,
  },
});

// State
const reviews = ref([]);
const loading = ref(false);
const deleteLoading = ref(false);
const deleteDialogVisible = ref(false);
const reviewToDelete = ref(null);
const totalRecords = ref(0);
const pageSize = ref(10);
const currentPage = ref(1);
const searchPhrase = ref("");

// Computed
const totalPages = computed(() =>
  Math.ceil(totalRecords.value / pageSize.value)
);

const displayedPages = computed(() => {
  const delta = 2;
  const range = [];
  const rangeWithDots = [];
  let l;

  for (let i = 1; i <= totalPages.value; i++) {
    if (
      i === 1 ||
      i === totalPages.value ||
      (i >= currentPage.value - delta && i <= currentPage.value + delta)
    ) {
      range.push(i);
    }
  }

  range.forEach((i) => {
    if (l) {
      if (i - l === 2) {
        rangeWithDots.push(l + 1);
      } else if (i - l !== 1) {
        rangeWithDots.push("...");
      }
    }
    rangeWithDots.push(i);
    l = i;
  });

  return range;
});

const loadReviews = async () => {
  loading.value = true;
  try {
    const response = await apiClient.getBooksReviewsForUser(
      searchPhrase.value,
      currentPage.value,
      pageSize.value,
      props.userId
    );
    reviews.value = response.items;
    totalRecords.value = response.totalCount;
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to load reviews",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};

const debouncedSearch = debounce(() => {
  currentPage.value = 1;
  loadReviews();
}, 300);

const goToPage = (page) => {
  currentPage.value = page;
  loadReviews();
};

const onPageSizeChange = () => {
  currentPage.value = 1;
  loadReviews();
};

const confirmDelete = (review) => {
  reviewToDelete.value = review;
  deleteDialogVisible.value = true;
};

const deleteReview = async () => {
  if (!reviewToDelete.value) return;

  deleteLoading.value = true;
  try {
    await apiClient.deleteReview(reviewToDelete.value.id);
    deleteDialogVisible.value = false;
    reviewToDelete.value = null;
    loadReviews();

    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Review deleted successfully",
      life: 3000,
    });
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to delete review",
      life: 3000,
    });
  } finally {
    deleteLoading.value = false;
  }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

// Initial load
onMounted(() => {
  loadReviews();
});
</script>
