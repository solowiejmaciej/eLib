<template>
  <div class="author-container p-4">
    <div class="flex justify-content-between align-items-center mb-4">
      <h1 class="text-2xl font-medium text-white m-0">
        {{ author?.name }} {{ author?.surname }}
      </h1>
      <Button label="← Back" link class="author-back-btn" @click="goBack" />
    </div>

    <div v-if="author" class="author-content p-4">
      <Image
        :src="author.details.photoUrl"
        :alt="author.name"
        imageClass="w-full rounded-lg mb-4"
        preview
      />

      <div class="mt-4">
        <h2 class="text-lg font-medium mb-2">Personal Information</h2>
        <div class="flex align-items-center mb-4">
          <i class="pi pi-calendar mr-2"></i>
          <span>Birthday: {{ formatDate(author.birthday) }}</span>
        </div>

        <h2 class="text-lg font-medium mb-2">Biography</h2>
        <p class="line-height-3">
          {{ author.details.biography }}
        </p>
      </div>
    </div>

    <ProgressSpinner
      v-if="loading"
      class="fixed top-50 left-50"
      strokeWidth="4"
    />
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

const route = useRoute();
const router = useRouter();
const author = ref(null);
const loading = ref(true);

const loadAuthor = async () => {
  try {
    loading.value = true;
    const response = await apiClient.getAuthorById(route.params.id);
    author.value = response;
  } catch (error) {
    console.error("Error loading author:", error);
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

const goBack = () => {
  router.back();
};

onMounted(() => {
  loadAuthor();
});
</script>

<style scoped>
.author-container {
  max-width: 1200px;
  margin: 0 auto;
}

.author-content {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
}

.author-back-btn {
  color: #80cbc4 !important;
}

.author-back-btn:hover {
  color: #b2dfdb !important;
}

:deep(.p-image-preview-indicator) {
  background: rgba(0, 0, 0, 0.5);
}

h2 {
  color: #e0e0e0;
}

p {
  color: #b0bec5;
}
</style>
