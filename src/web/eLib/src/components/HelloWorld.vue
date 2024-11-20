<script setup>
import { ref } from "vue";
import Button from "primevue/button";

const count = ref(0);
</script>

<template>
  <h1>Hello eLib!</h1>
  <Button type="button" @click="count++">count is {{ count }}</Button>
</template>

<style scoped></style>
