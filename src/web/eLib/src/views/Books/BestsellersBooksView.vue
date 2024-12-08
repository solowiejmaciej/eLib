<template>
  <div class="bg-gray-900 p-8">
    <h2 class="text-white text-xl font-medium mb-2">Bestsellers</h2>
    <p class="text-gray-400 text-sm mb-8">Most popular books this month</p>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
      <div
        v-for="(book, index) in books"
        :key="book.id"
        class="flex flex-col group relative"
        @mouseenter="book.showDetails = true"
        @mouseleave="book.showDetails = false"
      >
        <!-- Bestseller Ranking Badge -->
        <div
          class="absolute -top-4 -left-4 z-10 bg-cyan-400 text-white w-8 h-8 rounded-full flex items-center justify-center font-bold shadow-lg"
        >
          #{{ index + 1 }}
        </div>

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
            <div class="flex flex-wrap gap-2 mb-4">
              <span
                v-for="genre in book.genres"
                :key="genre"
                class="text-xs px-2 py-1 rounded-full bg-gray-700 text-cyan-400"
              >
                {{ genre }}
              </span>
            </div>
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

        <div class="flex items-center gap-4">
          <Rating v-model="book.rating" :cancel="false" readonly />
          <span class="text-cyan-400 text-sm">({{ book.reviewCount }})</span>
        </div>
      </div>
    </div>

    <!-- Book Details Dialog -->
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
          <div class="flex items-center gap-3 mb-4">
            <Rating
              :modelValue="selectedBook.rating"
              readonly
              :cancel="false"
            />
            <span class="text-cyan-400"
              >({{ selectedBook.reviewCount }} reviews)</span
            >
          </div>
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
    title: "The Hunger Games",
    author: "Suzanne Collins",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1586722975i/2767052.jpg",
    rating: 4.5,
    reviewCount: "8,203",
    showDetails: false,
    description:
      "In a dark vision of the near future, twelve boys and twelve girls are forced to appear in a live TV show called The Hunger Games. There is only one rule: kill or be killed.",
    genres: ["Young Adult", "Dystopian", "Action"],
  },
  {
    id: 2,
    title: "1984",
    author: "George Orwell",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1657781256i/61439040.jpg",
    rating: 4.8,
    reviewCount: "6,932",
    showDetails: false,
    description:
      "Among the seminal texts of the 20th century, Nineteen Eighty-Four is a rare work that grows more haunting as its futuristic purgatory becomes more real.",
    genres: ["Classics", "Dystopian", "Fiction"],
  },
  {
    id: 3,
    title: "Pride and Prejudice",
    author: "Jane Austen",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1320399351i/1885.jpg",
    rating: 4.7,
    reviewCount: "7,541",
    showDetails: false,
    description:
      "Since its immediate success in 1813, Pride and Prejudice has remained one of the most popular novels in the English language.",
    genres: ["Classics", "Romance", "Literature"],
  },
  {
    id: 4,
    title: "The Silent Patient",
    author: "Alex Michaelides",
    image:
      "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1668782119i/40097951.jpg",
    rating: 4.6,
    reviewCount: "5,821",
    showDetails: false,
    description:
      "Alicia Berenson's life is seemingly perfect. A famous painter married to an in-demand fashion photographer, she lives in a grand house overlooking a park in one of London's most desirable areas.",
    genres: ["Thriller", "Mystery", "Fiction"],
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
