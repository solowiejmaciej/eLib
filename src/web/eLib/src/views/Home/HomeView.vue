<template>
  <div>
    <NavBar /> 
  </div>
  <div>
    <Gallery />
  </div>
</template>

<script>
import NavBar from '../../components/NavBar.vue';
import Gallery from '../../components/Gallery.vue';
export default {
  components: {
    NavBar,
    Gallery,
  },
};
</script>
