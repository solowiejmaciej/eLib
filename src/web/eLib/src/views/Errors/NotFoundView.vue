<template>
  <div class="flex items-center justify-center min-h-screen">
    <Card class="w-[500px] shadow-2xl">
      <template #title>
        <div class="flex flex-col items-center gap-4">
          <i class="pi pi-exclamation-circle text-6xl text-red-500"></i>
          <div
            class="text-5xl font-bold text-center text-slate-800 dark:text-white"
          >
            404 - Not Found
          </div>
        </div>
      </template>
      <template #content>
        <div class="space-y-6 p-4">
          <p
            class="text-center text-xl leading-relaxed text-slate-600 dark:text-slate-200"
          >
            <i class="pi pi-search mr-2"></i>
            Page you are looking for is not found.
          </p>
          <div
            class="text-center font-medium text-lg text-slate-700 dark:text-slate-300 mt-4"
          >
            <i class="pi pi-clock mr-2"></i>
            Redirecting to home page in
            <span class="text-blue-600 dark:text-blue-400 font-bold">{{
              count
            }}</span>
            seconds.
          </div>
          <div class="flex justify-center mt-6">
            <Button
              icon="pi pi-home"
              label="Go Home"
              class="p-button-rounded"
              @click="goHome"
            />
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup>
import Card from "primevue/card";
import Button from "primevue/button";
import { ref, onMounted } from "vue";

const count = ref(10);

const goHome = () => {
  window.location.href = "/";
};

onMounted(() => {
  const interval = setInterval(() => {
    count.value--;
    if (count.value === 0) {
      clearInterval(interval);
      window.location.href = "/";
    }
  }, 1000);
});
</script>