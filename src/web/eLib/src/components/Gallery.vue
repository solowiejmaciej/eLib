<template>
    <div class="card">
        <Carousel :value="products" :numVisible="1" :numScroll="1" :responsiveOptions="responsiveOptions" circular :autoplayInterval="3000">
            <template #item="slotProps">
                <div class="border border-surface-200 dark:border-surface-700 rounded m-2 p-4">
                    <div class="mb-4">
                        <div class="relative mx-auto">
                            <img :src="slotProps.data.image" :alt="slotProps.data.name" class="w-full rounded" />
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
    // This would be an actual API call in a real app, using `ProductService.getProductsSmall()`
    products.value = mockProducts;  // Mock data for now
});
</script>
