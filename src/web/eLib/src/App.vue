<template>
  <Toast></Toast>
  <RouterView />
</template>

<script setup>
import { RouterView } from "vue-router";
import Toast from "primevue/toast";
import { emitter } from "./clients/eLibApiClient";
import { onMounted, onUnmounted } from "vue";
import store from "./store/store";
const handleUnauthorized = () => {
  store.dispatch("logout");
  router.push("/login");
};

onMounted(() => {
  emitter.on("unauthorized", handleUnauthorized);
});

onUnmounted(() => {
  emitter.off("unauthorized", handleUnauthorized);
});
</script>
