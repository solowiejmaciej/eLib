<template>
  <div class="card space-y-6">
    <div
      v-if="store.getters.isAuthenticated"
      class="bg-slate-900 p-6 rounded-lg"
    >
      <h3 class="text-xl font-semibold text-white mb-4">Add Your Review</h3>
      <div class="space-y-4">
        <div class="flex flex-col gap-2">
          <label class="text-white">Rating</label>
          <Rating v-model="newReview.rating" :cancel="false" />
        </div>

        <div class="flex flex-col gap-2">
          <label class="text-white">Review</label>
          <Textarea
            v-model="newReview.content"
            rows="3"
            class="w-full"
            placeholder="Share your thoughts about this book..."
          />
        </div>

        <div class="flex justify-end">
          <Button
            label="Submit Review"
            @click="submitReview"
            :loading="submitting"
            :disabled="!isValidReview"
          />
        </div>
      </div>
    </div>

    <Carousel
      :value="reviews"
      :numVisible="2"
      :numScroll="2"
      circular
      :autoplayInterval="5000"
      v-if="reviews.length > 0"
    >
      <template #item="slotProps">
        <div
          class="border border-surface-200 dark:border-surface-700 rounded m-2 p-4"
        >
          <div class="mb-4 bg-slate-900 shadow-md rounded-lg p-6">
            <div class="flex items-center mb-4">
              <Avatar
                :label="getInitials(slotProps.data)"
                size="large"
                shape="circle"
                class="mr-4"
              />
              <div>
                <p class="text-lg font-semibold text-white">
                  {{ slotProps.data.name }}
                  {{ slotProps.data.surname }}
                </p>
                <Rating
                  v-model="slotProps.data.rating"
                  readonly
                  :cancel="false"
                  class="mt-1"
                />
              </div>
            </div>
            <p class="text-white leading-relaxed mb-2">
              {{ slotProps.data.content }}
            </p>
            <p class="text-sm text-gray-500">
              {{ formatDate(slotProps.data.createdAt) }}
            </p>
          </div>
        </div>
      </template>
    </Carousel>

    <div v-else>
      <p class="text-gray-500 text-center">No reviews yet</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useStore } from "vuex";
import { useToast } from "primevue/usetoast";
import Carousel from "primevue/carousel";
import Rating from "primevue/rating";
import Textarea from "primevue/textarea";
import apiClient from "../clients/eLibApiClient";

const store = useStore();
const toast = useToast();

const props = defineProps({
  bookId: {
    type: String,
    required: true,
  },
});

const reviews = ref([]);
const submitting = ref(false);
const newReview = ref({
  rating: 0,
  content: "",
});

const isValidReview = computed(() => {
  return (
    newReview.value.rating > 0 && newReview.value.content.trim().length >= 3
  );
});

onMounted(async () => {
  await fetchReviews();
});

async function fetchReviews() {
  try {
    const response = await apiClient.getBookReviews(null, 1, 20, props.bookId);
    reviews.value = response.items;
  } catch (error) {
    console.error("Error fetching reviews:", error);
    reviews.value = [];
  }
}

async function submitReview() {
  if (!isValidReview.value) return;

  submitting.value = true;
  try {
    const reviewData = {
      bookId: props.bookId,
      content: newReview.value.content.trim(),
      rating: newReview.value.rating,
    };

    await apiClient.createReview(reviewData);

    newReview.value = {
      rating: 0,
      content: "",
    };

    await fetchReviews();

    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Your review has been added successfully",
      life: 3000,
    });
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to submit your review",
      life: 3000,
    });
  } finally {
    submitting.value = false;
  }
}

function formatDate(dateString) {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function getInitials(user) {
  const firstNameInitial = user.name.charAt(0).toUpperCase();
  const lastNameInitial = user.surname.charAt(0).toUpperCase();
  return firstNameInitial + lastNameInitial;
}
</script>
