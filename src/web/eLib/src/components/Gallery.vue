<template>
    <div class="card">
        <Carousel :value="products" :numVisible="1" :numScroll="1" :responsiveOptions="responsiveOptions" circular :autoplayInterval="3000">
            <template #item="slotProps">
                <div class="border border-surface-200 dark:border-surface-700 rounded m-2 p-4">
                    <div class="mb-4">
                        <div class="relative mx-auto">
                            <img :src="slotProps.data.image" :alt="slotProps.data.image" class="w-full rounded" />
                        </div>
                    </div>
                </div>
            </template>
        </Carousel>
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";

// Mock product data to simulate the API call
const mockProducts = [
    { image: 'https://wydajenamsie.pl/wp-content/uploads/2024/11/wns_1920x570.jpg' },
    { image: 'https://wydajenamsie.pl/wp-content/uploads/2023/01/b_darmowa-dostawa_1920x570_3p3.png' },
];

const products = ref([]);

// Simulate API call to populate product data
onMounted(() => {
    products.value = mockProducts;  // Mock data for now
});

// Responsive options for the carousel (if you want to make it responsive on different screen sizes)
const responsiveOptions = [
    {
        breakpoint: '1024px',
        numVisible: 1,
        numScroll: 1
    },
    {
        breakpoint: '768px',
        numVisible: 1,
        numScroll: 1
    },
    {
        breakpoint: '560px',
        numVisible: 1,
        numScroll: 1
    }
];
</script>
