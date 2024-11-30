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
                :label="getInitials(slotProps.data.user)"
                size="large"
                shape="circle"
                class="mr-4"
              />
              <div>
                <p class="text-lg font-semibold text-white">
                  {{ slotProps.data.user.name }}
                  {{ slotProps.data.user.surname }}
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
              {{ slotProps.data.comment }}
            </p>
            <p class="text-sm text-gray-500">
              {{ formatDate(slotProps.data.date) }}
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

const props = defineProps({
  bookId: {
    type: String,
    required: true,
  },
});

const reviews = ref([]);

const mockReviews = [
  {
    id: 1,
    bookId: props.bookId,
    rating: 4,
    comment: "Great book, really enjoyed reading it!",
    user: {
      name: "John",
      surname: "Doe",
    },
    date: "2024-12-09",
  },
  {
    id: 2,
    bookId: props.bookId,
    rating: 5,
    comment: "One of the best books I've ever read!",
    user: {
      name: "Jane",
      surname: "Smith",
    },
    date: "2023-06-10",
  },
  {
    id: 3,
    bookId: props.bookId,
    rating: 3,
    comment: "It was okay, but not great.",
    user: {
      name: "Alice",
      surname: "Johnson",
    },
    date: "2023-06-11",
  },
  {
    id: 4,
    bookId: props.bookId,
    rating: 5,
    comment: "I loved it",
    user: {
      name: "Bob",
      surname: "Brown",
    },
    date: "2024-03-15",
  },
];

onMounted(() => {
  reviews.value = mockReviews;
});

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
