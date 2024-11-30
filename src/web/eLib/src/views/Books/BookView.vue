<template>
  <div class="container mx-auto py-8">
    <Card v-if="loading">
      <template #content>
        <div class="text-center">
          <ProgressSpinner />
          <p class="mt-4">Loading book details...</p>
        </div>
      </template>
    </Card>
    <div v-else>
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <div>
          <Card>
            <template #header>
              <img
                :src="book.details.coverUrl"
                :alt="book.title"
                class="w-full h-auto shadow-lg rounded-lg"
              />
            </template>
          </Card>
        </div>
        <div>
          <Card>
            <template #title>{{ book.title }}</template>
            <template #content>
              <div class="mb-6">
                <router-link
                  :to="'/authors/' + author.id"
                  class="text-blue-600 hover:underline"
                >
                  {{ author.name }} {{ author.surname }}
                </router-link>
              </div>
              <p class="text-gray-200 mb-4 line-clamp-6">
                {{ book.details.description }}
              </p>
              <div class="mb-4">
                <span class="font-bold">Quantity:</span>
                {{ book.details.quantity }}
              </div>
            </template>
            <template #footer>
              <div class="flex items-center justify-between">
                <Button
                  @click="viewAuthorBooks"
                  icon="pi pi-user"
                  label="View Books by This Author"
                  class="p-button-outlined"
                />

                <Button
                  v-if="!isBookInReadingList"
                  @click="addToReadingList"
                  icon="pi pi-star"
                  label="Add to Reading List"
                  class="p-button-outlined"
                />
                <Button
                  v-else
                  @click="removeFromReadingList"
                  icon="pi pi-star-fill"
                  filled
                  label="Already in reading list"
                />
              </div>
            </template>
          </Card>
        </div>
      </div>
      <Card class="mt-8">
        <template #title>About the Author</template>
        <template #content>
          <div class="flex flex-col md:flex-row items-center">
            <div class="md:w-1/3">
              <img
                :src="author.details.photoUrl"
                :alt="author.name + ' ' + author.surname"
                class="w-32 h-32 rounded-full shadow-lg mb-4 md:mb-0"
              />
            </div>
            <div class="md:w-2/3 md:ml-8">
              <p class="text-gray-200 line-clamp-6">
                {{ author.details.biography }}
              </p>
            </div>
          </div>
          <h3 class="text-xl font-semibold text-gray-200">
            {{ author.name }} {{ author.surname }}
          </h3>
          <p class="text-gray-400">{{ author.birthday }}</p>
        </template>
      </Card>
      <Card class="mt-8">
        <template #title>Reviews</template>
        <template #content>
          <BookReviews :bookId="bookId" />
        </template>
      </Card>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter, onBeforeRouteUpdate } from "vue-router";
import apiClient from "../../clients/eLibApiClient";
import Card from "primevue/card";
import ProgressSpinner from "primevue/progressspinner";
import Button from "primevue/button";
import BookReviews from ".././../components/BookReviews.vue";

const route = useRoute();
const router = useRouter();
const bookId = route.params.id;

const loading = ref(true);
const book = ref(null);
const author = ref(null);

const isBookInReadingList = ref(false);

const checkReadingListStatus = async () => {
  console.log("Checking reading list status...");
  try {
    // const readingList = await apiClient.getReadingList();
    // const isInList = readingList.some(book => book.id === bookId);
    // isBookInReadingList.value = isInList;
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
</script>
