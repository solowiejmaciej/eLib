<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold">Notifications List {{ 123 }}</h1>
  </div>
</template>

<script setup>
defineProps({
  userId: {
    type: String,
    required: true,
  },
});
</script>
