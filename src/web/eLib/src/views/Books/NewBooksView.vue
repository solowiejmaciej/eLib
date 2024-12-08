<template>
  <div class="bg-gray-900 p-8">
    <h2 class="text-white text-xl font-medium mb-2">New Books</h2>
    <p class="text-gray-400 text-sm mb-8">Discover our latest releases</p>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
      <div
        v-for="book in books"
        :key="book.id"
        class="flex flex-col group relative"
        @mouseenter="book.showDetails = true"
        @mouseleave="book.showDetails = false"
      >
        <!-- Book Image with Hover Overlay -->
        <div class="relative overflow-hidden rounded-md">
          <img
            :src="book.image"
            :alt="book.title"
            class="w-full aspect-[3/4] object-cover rounded-md mb-4 transition-transform duration-300 group-hover:scale-105"
          />

          <!-- Hover Overlay -->
          <div
            v-show="book.showDetails"
            class="absolute inset-0 bg-black/80 p-4 transition-opacity duration-300 overflow-y-auto"
          >
            <p class="text-white text-sm leading-relaxed mb-4">
              {{ book.description }}
            </p>
            <div class="absolute bottom-4 left-4 right-4">
              <Button
                label="Details"
                class="p-button-rounded p-button-outlined w-full text-cyan-400 border-cyan-400 hover:bg-cyan-400 hover:text-white"
                @click="selectBook(book)"
              />
            </div>
          </div>
        </div>

        <!-- Book Info -->
        <h3 class="text-white font-medium mb-1">{{ book.title }}</h3>
        <p class="text-gray-400 text-sm mb-2">{{ book.author }}</p>

        <Rating v-model="book.rating" readonly :cancel="false" class="mt-1" />
      </div>
    </div>

    <!-- Improved Dialog -->
    <Dialog
      v-model:visible="dialogVisible"
      modal
      :header="selectedBook?.title"
      :style="{ width: '800px', maxWidth: '90vw' }"
      class="book-dialog"
    >
      <div v-if="selectedBook" class="flex flex-col md:flex-row gap-8">
        <img
          :src="selectedBook.image"
          :alt="selectedBook.title"
          class="w-full md:w-72 rounded-lg object-cover"
        />
        <div class="flex-1">
          <h3 class="text-2xl font-bold text-white mb-2">
            {{ selectedBook.title }}
          </h3>
          <p class="text-gray-400 mb-4">by {{ selectedBook.author }}</p>
          <Rating
            :modelValue="selectedBook.rating"
            readonly
            class="mb-4"
            :cancel="false"
          />
          <p class="text-gray-300 mb-6 leading-relaxed">
            {{ selectedBook.description }}
          </p>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="genre in selectedBook.genres"
              :key="genre"
              class="text-xs px-3 py-1 rounded-full bg-gray-700 text-cyan-400"
            >
              {{ genre }}
            </span>
          </div>
        </div>
      </div>
    </Dialog>
  </div>
</template>

<script setup>
import { ref } from "vue";

const dialogVisible = ref(false);
const selectedBook = ref(null);

const selectBook = (book) => {
  selectedBook.value = book;
  dialogVisible.value = true;
};

const books = ref([
  {
    id: 1,
    title: "The Lord of the Rings",
    author: "J.R.R. Tolkien",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1566425108i/33.jpg",
    rating: 4,
    showDetails: false,
    description:
      "An epic high-fantasy novel that follows the quest of hobbits and their allies to destroy the One Ring and defeat the Dark Lord Sauron. A masterpiece that defined the fantasy genre.",
    genres: ["Fantasy", "Epic", "Classic"],
  },
  {
    id: 2,
    title: "Dune",
    author: "Frank Herbert",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1555447414i/44767458.jpg",
    rating: 4,
    showDetails: false,
    description:
      "Set in the distant future, Dune tells the story of Paul Atreides, whose family accepts stewardship of the planet Arrakis, the only source of the most valuable substance in the universe - the spice melange.",
    genres: ["Science Fiction", "Space Opera"],
  },
  {
    id: 3,
    title: "The Witcher",
    author: "Andrzej Sapkowski",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1529591917i/40603587.jpg",
    rating: 4,
    showDetails: false,
    description:
      "Follow the adventures of Geralt of Rivia, a monster hunter with supernatural abilities. In a world filled with dark fantasy and moral ambiguity, Geralt must navigate political intrigue and face dangerous creatures.",
    genres: ["Fantasy", "Dark Fantasy", "Adventure"],
  },
  {
    id: 4,
    title: "Foundation",
    author: "Isaac Asimov",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1417900846i/29579.jpg",
    rating: 4,
    showDetails: false,
    description:
      "The first novel in Asimov's Foundation series, it follows the efforts of mathematician Hari Seldon, who develops psychohistory to predict and influence humanity's future across the galactic empire.",
    genres: ["Science Fiction", "Classic"],
  },
]);
</script>

<style scoped>
:deep(.p-rating .p-rating-item.p-rating-item-active .p-rating-icon) {
  color: #06b6d4;
}

:deep(.p-rating .p-rating-item .p-rating-icon) {
  color: #4b5563;
}

:deep(.p-dialog.book-dialog) {
  background: #1e2837;
  border-radius: 8px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

:deep(.book-dialog .p-dialog-header) {
  background: #1e2837;
  color: white;
  border: none;
  padding: 1.5rem;
}

:deep(.book-dialog .p-dialog-content) {
  background: #1e2837;
  color: white;
  padding: 0 1.5rem 1.5rem 1.5rem;
  border: none;
}

:deep(.book-dialog .p-dialog-header-close) {
  color: white;
}

:deep(.book-dialog .p-dialog-header-close:hover) {
  background: rgba(255, 255, 255, 0.1);
}
</style>
