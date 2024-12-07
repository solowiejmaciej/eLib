<template>
  <div class="card">
    <Carousel
      :value="reviews"
      :numVisible="2"
      :numScroll="2"
      circular
      :autoplayInterval="5000"
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
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import Carousel from "primevue/carousel";
import Rating from "primevue/rating";
import apiClient from "../clients/eLibApiClient";

const props = defineProps({
  bookId: {
    type: String,
    required: true,
  },
});

const reviews = ref([]);

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
