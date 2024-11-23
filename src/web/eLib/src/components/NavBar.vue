
<template>
    <div class="card">
        <Menubar :model="items">
            
            <template #end>
                <div class="flex flex-nowrap pr-6">
                    <div><InputText placeholder="Search" type="text" class="w-32 sm:w-auto" /></div>
                    <div><Menubar :model="[loginItem]"/></div>
                </div>
            </template>
            
        </Menubar>
    </div>
</template>

<script setup>
import { ref } from "vue";
import Menubar from 'primevue/menubar';
const items = ref([
    {
        label: 'Home',
        icon: 'pi pi-home'
    },

    {
        label: 'Books',
        icon: 'pi pi-book'
    },
    {
        label: 'New Arrivals',
        icon: 'pi pi-sparkles'
    },
    {
        label: 'Bestsellers',
        icon: 'pi pi-crown'
    }
]);
    const loginItem = {

        label: 'Login',
        icon: 'pi pi-sign-in'
    };

    

</script>

<script>

import Menubar from 'primevue/menubar';

</script>