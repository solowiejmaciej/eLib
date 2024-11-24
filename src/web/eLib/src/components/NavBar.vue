<template>
    <div class="card">
        <Menubar :model="items" class="custom-menubar">
            <template #start>
                <!-- Left items -->
            </template>

            <template #end>
                <div class="end-content">
                    <div class="search-bar-container">
                        <InputText placeholder="Search" type="text" class="w-96" />
                    </div>
                    <Menubar :model="[loginItem]" />
                </div>
            </template>
        </Menubar>
    </div>
</template>

<script setup>
import { ref } from "vue";
import Menubar from "primevue/menubar";
import InputText from "primevue/inputtext";

const items = ref([
    { label: "Home", icon: "pi pi-home" },
    { label: "Books", icon: "pi pi-book" },
    { label: "New Arrivals", icon: "pi pi-sparkles" },
    { label: "Bestsellers", icon: "pi pi-crown" },
]);

const loginItem = { label: "Login", icon: "pi pi-sign-in" };
</script>

<style>
.custom-menubar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: relative;
    padding: 0 1rem;
}

.search-bar-container {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    text-align: center;
}

.w-96 {
    max-width: 24rem;
}
.end-content {
    display: flex;
    align-items: center;
    gap: 1rem;
}
</style>
