<template>
  <div class="relative overflow-hidden">
    <div
      class="absolute top-0 left-0 w-full h-full bg-gradient-to-b from-transparent to-black opacity-50 z-10"
    ></div>
    <Carousel
      :value="images"
      :numVisible="1"
      :numScroll="1"
      :autoplayInterval="5000"
      :circular="true"
      class="h-[500px]"
    >
      <template #item="slotProps">
        <div class="relative w-full h-[500px]">
          <img
            :src="slotProps.data.url"
            :alt="slotProps.data.alt"
            class="w-full h-full object-cover"
          />
        </div>
      </template>
    </Carousel>

    <div
      class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-center text-white z-20"
    >
      <h1 class="text-6xl font-bold mb-4 animate-fadeIn">Welcome to eLib</h1>
      <p class="text-xl mb-8 animate-slideUp">
        Your Modern Library Management System
      </p>
    </div>
  </div>
</template>

<script setup>
const images = [
  {
    url: "https://images.unsplash.com/photo-1507842217343-583bb7270b66",
    alt: "Library interior",
  },
  {
    url: "https://images.unsplash.com/photo-1521587760476-6c12a4b040da",
    alt: "Books on shelf",
  },
  {
    url: "https://images.unsplash.com/photo-1481627834876-b7833e8f5570",
    alt: "Study area",
  },
];
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.animate-fadeIn {
  animation: fadeIn 1s ease-out;
}

.animate-slideUp {
  animation: slideUp 1s ease-out 0.5s both;
}
</style>
