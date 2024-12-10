<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <!-- Header Section -->
    <div class="flex justify-between items-center mb-8">
      <h1 class="text-3xl font-bold text-white">
        {{ author?.name }} {{ author?.surname }}
      </h1>
      <Button icon="pi pi-arrow-left" label="Back" link @click="goBack" />
    </div>

    <div v-if="author" class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Author Info Section -->
      <div class="lg:col-span-1">
        <div class="bg-slate-800/50 rounded-xl p-6 shadow-lg">
          <Image
            :src="author.details.photoUrl"
            :alt="author.name"
            imageClass="w-full rounded-lg shadow-md"
            :pt="{
              image: { class: 'w-full h-[400px] object-cover' },
            }"
            preview
          />

          <div class="mt-6 space-y-4">
            <div class="flex items-center text-gray-300">
              <i class="pi pi-calendar mr-3 text-teal-300"></i>
              <span>{{ formatDate(author.birthday) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Biography & Books Section -->
      <div class="lg:col-span-2 space-y-8">
        <!-- Biography -->
        <div class="bg-slate-800/50 rounded-xl p-6 shadow-lg">
          <h2 class="text-2xl font-semibold text-white mb-4">Biography</h2>
          <p class="text-gray-300 leading-relaxed">
            {{ author.details.biography }}
          </p>
        </div>

        <!-- Books Carousel -->
        <div class="bg-slate-800/50 rounded-xl p-6 shadow-lg">
          <h2 class="text-2xl font-semibold text-white mb-4">
            Other books by this author
          </h2>
          <Carousel
            :value="books"
            :numVisible="3"
            :numScroll="1"
            :responsiveOptions="carouselResponsiveOptions"
            :autoplayInterval="3000"
            :showIndicators="false"
            class="books-carousel"
          >
            <template #item="slotProps">
              <div class="p-4">
                <div
                  class="bg-slate-700/50 rounded-lg p-4 h-full flex flex-col cursor-pointer transition-all duration-300 hover:bg-slate-600/50 hover:shadow-xl hover:-translate-y-1"
                  @click="navigateToBook(slotProps.data.id)"
                >
                  <img
                    :src="slotProps.data.details.coverUrl"
                    :alt="slotProps.data.title"
                    class="w-full h-64 object-cover rounded-lg shadow-md mb-4"
                  />
                  <h3 class="text-lg font-medium text-white mb-2">
                    {{ slotProps.data.title }}
                  </h3>
                  <p class="text-gray-300 text-sm line-clamp-3 flex-grow">
                    {{ slotProps.data.details.description }}
                  </p>
                  <div class="mt-4 text-sm text-teal-300">
                    Available copies: {{ slotProps.data.details.quantity }}
                  </div>
                </div>
              </div>
            </template>
          </Carousel>
        </div>
      </div>
    </div>

    <!-- Loading Spinner -->
    <div
      v-if="loading"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
    >
      <ProgressSpinner strokeWidth="4" class="w-16 h-16" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import apiClient from "../../clients/eLibApiClient";

// Components
import Image from "primevue/image";
import Button from "primevue/button";
import ProgressSpinner from "primevue/progressspinner";
import Carousel from "primevue/carousel";

const route = useRoute();
const router = useRouter();
const author = ref(null);
const loading = ref(true);
const books = ref([]);

const carouselResponsiveOptions = ref([
  {
    breakpoint: "1199px",
    numVisible: 2,
    numScroll: 1,
  },
  {
    breakpoint: "767px",
    numVisible: 1,
    numScroll: 1,
  },
]);

const loadAuthor = async () => {
  try {
    loading.value = true;
    const response = await apiClient.getAuthorById(route.params.id);
    author.value = response;
    await loadBooks();
  } catch (error) {
    console.error("Error loading author:", error);
  } finally {
    loading.value = false;
  }
};

const loadBooks = async () => {
  try {
    const response = await apiClient.getBooksByAuthorId(
      null, // searchPhrase
      1, // pageNumber
      50, // pageSize
      route.params.id
    );
    books.value = response.items;
  } catch (error) {
    console.error("Error loading books:", error);
  }
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

const goBack = () => {
  router.back();
};

const navigateToBook = (bookId) => {
  router.push({ path: `/books/${bookId}` });
};

onMounted(() => {
  loadAuthor();
});
</script>

<style scoped>
:deep(
    .p-carousel .p-carousel-indicators .p-carousel-indicator.p-highlight button
  ) {
  background-color: rgb(94 234 212); /* teal-300 */
}

:deep(.p-carousel .p-carousel-indicators button) {
  background-color: rgb(148 163 184); /* slate-400 */
}

:deep(.p-carousel-prev),
:deep(.p-carousel-next) {
  color: rgb(94 234 212) !important; /* teal-300 */
}

:deep(.p-carousel-prev:hover),
:deep(.p-carousel-next:hover) {
  color: rgb(45 212 191) !important; /* teal-400 */
}

:deep(.p-image-preview-indicator) {
  background: rgba(0, 0, 0, 0.5);
}
</style>
