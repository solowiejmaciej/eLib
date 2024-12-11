<template>
  <div class="container mx-auto py-8 px-4">
    <Card v-if="loading">
      <template #content>
        <div class="flex flex-col items-center justify-center min-h-[400px]">
          <ProgressSpinner class="w-16 h-16" />
          <p class="mt-4 text-gray-300">Loading book details...</p>
        </div>
      </template>
    </Card>

    <div v-else class="space-y-8">
      <!-- Book Details Section -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- Book Cover -->
        <div>
          <Card class="shadow-xl">
            <template #header>
              <div class="relative pb-[133%] overflow-hidden rounded-lg">
                <img
                  :src="book.details.coverUrl"
                  :alt="book.title"
                  class="absolute inset-0 w-full h-full object-cover hover:scale-105 transition-transform duration-300"
                />
              </div>
            </template>
          </Card>
        </div>

        <!-- Book Info -->
        <div>
          <Card class="h-full">
            <template #title>
              <h1 class="text-2xl font-bold text-gray-100 mb-2">
                {{ book.title }}
              </h1>
            </template>
            <template #subtitle>
              <router-link
                :to="'/authors/' + author.id"
                class="text-blue-400 hover:text-blue-300 text-lg font-medium hover:underline inline-flex items-center gap-2"
              >
                <i class="pi pi-user"></i>
                {{ author.name }} {{ author.surname }}
              </router-link>
            </template>
            <template #content>
              <div class="space-y-6">
                <!-- Description -->
                <div>
                  <h3 class="text-gray-300 font-medium mb-2">About the Book</h3>
                  <p
                    class="text-gray-200 leading-relaxed line-clamp-6 hover:line-clamp-none transition-all duration-300"
                  >
                    {{ book.details.description }}
                  </p>
                </div>

                <!-- Book Details -->
                <div class="grid grid-cols-2 gap-4 p-4 bg-gray-800 rounded-lg">
                  <div class="flex items-center gap-2">
                    <i class="pi pi-box text-gray-400"></i>
                    <span class="text-gray-300">
                      <strong>Available:</strong> {{ book.details.quantity }}
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="pi pi-calendar text-gray-400"></i>
                    <span class="text-gray-300">
                      <strong>Published:</strong> {{ book.details.publishDate }}
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="pi pi-bookmark text-gray-400"></i>
                    <span class="text-gray-300">
                      <strong>ISBN:</strong> {{ book.details.isbn }}
                    </span>
                  </div>
                  <div class="flex items-center gap-2">
                    <i class="pi pi-tag text-gray-400"></i>
                    <span class="text-gray-300">
                      <strong>Genre:</strong> {{ book.details.genre }}
                    </span>
                  </div>
                </div>
              </div>
            </template>
            <template #footer>
              <div class="flex flex-wrap gap-3 justify-between">
                <Button
                  @click="viewAuthorBooks"
                  icon="pi pi-list"
                  label="More by Author"
                  class="p-button-outlined p-button-secondary"
                />
                <div class="flex gap-3">
                  <div v-if="store.getters.isAuthenticated">
                    <Button
                      v-if="book.details.quantity > 0"
                      @click="openNewReservationDialog"
                      icon="pi pi-cart-plus"
                      label="Reserve"
                      raised
                    />
                    <Button
                      v-else
                      label="Out of Stock"
                      icon="pi pi-times"
                      class="p-button-danger"
                      disabled
                    />
                  </div>

                  <div v-if="store.getters.isAuthenticated">
                    <Button
                      v-if="!isBookInReadingList"
                      @click="addToReadingList"
                      icon="pi pi-star"
                      label="Add to List"
                      class="p-button-outlined"
                    />
                    <Button
                      v-else
                      @click="removeFromReadingList"
                      icon="pi pi-star-fill"
                      severity="warning"
                      label="In Reading List"
                      raised
                    />
                  </div>
                </div>
              </div>
            </template>
          </Card>
        </div>
      </div>

      <!-- Author Section -->
      <Card>
        <template #title>
          <h2 class="text-xl font-bold text-gray-100 flex items-center gap-2">
            <i class="pi pi-user-edit"></i> About the Author
          </h2>
        </template>
        <template #content>
          <div
            class="flex flex-col md:flex-row items-center md:items-start gap-8"
          >
            <div class="md:w-1/4 flex flex-col items-center text-center">
              <img
                :src="author.details.photoUrl"
                :alt="author.name + ' ' + author.surname"
                class="w-40 h-40 rounded-full shadow-lg object-cover mb-4 ring-2 ring-gray-700"
              />
              <h3 class="text-lg font-semibold text-gray-200">
                {{ author.name }} {{ author.surname }}
              </h3>
              <p class="text-gray-400 text-sm">
                <i class="pi pi-calendar mr-2"></i>{{ formattedBirthday }}
              </p>
            </div>
            <div class="md:w-3/4">
              <p class="text-gray-200 leading-relaxed">
                {{ author.details.biography }}
              </p>
            </div>
          </div>
        </template>
      </Card>

      <!-- Reviews Section -->
      <Card>
        <template #title>
          <h2 class="text-xl font-bold text-gray-100 flex items-center gap-2">
            <i class="pi pi-comments"></i> Reader Reviews
          </h2>
        </template>
        <template #content>
          <BookReviews :bookId="bookId" />
        </template>
      </Card>
    </div>
  </div>

  <ReservationForm
    :book="book"
    :visible="isBookReservationDialogVisible"
    :author="author"
    @update:visible="closeReservationDialog"
  />
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter, onBeforeRouteUpdate } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import Card from "primevue/card";
import ProgressSpinner from "primevue/progressspinner";
import Button from "primevue/button";
import BookReviews from ".././../components/BookReviews.vue";
import ReservationForm from "../../components/ReservationForm.vue";
import store from "../../store/store";

const route = useRoute();
const router = useRouter();
const bookId = route.params.id;

const loading = ref(true);
const book = ref(null);
const author = ref(null);

const isBookInReadingList = ref(false);
const isBookReservationDialogVisible = ref(false);

const checkReadingListStatus = async () => {
  console.log("Checking reading list status...");
  try {
    const isInList = await apiClient.existsInReadingList(bookId);
    isBookInReadingList.value = isInList;
    return isBookInReadingList.value;
  } catch (error) {
    console.error("Error checking reading list status:", error);
    return false;
  }
};

const addToReadingList = async () => {
  try {
    console.log("Adding book to reading list...");
    await apiClient.addToReadingList(bookId);
    isBookInReadingList.value = true;
    console.log(
      "Book added to reading list, new status:",
      isBookInReadingList.value
    );
  } catch (error) {
    console.error("Error adding book to reading list:", error);
  }
};

const removeFromReadingList = async () => {
  try {
    console.log("Removing book from reading list...");
    await apiClient.removeFromReadingList(bookId);
    isBookInReadingList.value = false;
    console.log(
      "Book removed from reading list, new status:",
      isBookInReadingList.value
    );
  } catch (error) {
    console.error("Error removing book from reading list:", error);
  }
};

onMounted(async () => {
  await loadBookDetails();
});

onBeforeRouteUpdate(async (to, from) => {
  if (to.params.id !== from.params.id) {
    loading.value = true;
    try {
      book.value = await apiClient.getBookById(to.params.id);
      author.value = await apiClient.getAuthorById(book.value.authorId);
      loading.value = false;
    } catch (error) {
      console.error("Error loading book details:", error);
    }
  }
});

function viewAuthorBooks() {
  router.push({ path: `/authors/${author.value.id}/books` });
}

const openNewReservationDialog = () => {
  isBookReservationDialogVisible.value = true;
};

const closeReservationDialog = async (isVisible) => {
  isBookReservationDialogVisible.value = isVisible;
  await loadBookDetails();
};

async function loadBookDetails() {
  try {
    loading.value = true;
    const [bookData, bookStatus] = await Promise.all([
      apiClient.getBookById(bookId),
      checkReadingListStatus(),
    ]);
    book.value = bookData;
    author.value = await apiClient.getAuthorById(book.value.authorId);
    console.log("Initial reading list status:", isBookInReadingList.value);
  } catch (error) {
    console.error("Error in component mount:", error);
  } finally {
    loading.value = false;
  }
}

const formattedBirthday = computed(() => {
  if (!author.value?.birthday) return "";
  return new Date(author.value.birthday).toLocaleDateString("en-GB", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
});
</script>
