<template>
  <div class="relative">
    <Button
      v-show="showBackToTop"
      icon="pi pi-arrow-up"
      @click="scrollToTop"
      class="fixed bottom-8 right-8 z-50 p-button-rounded p-button-lg shadow-lg"
      aria-label="Go to top"
    />

    <div class="relative h-[600px] w-full overflow-x-hidden">
      <div class="absolute inset-0 bg-black/50 z-10"></div>

      <div class="absolute inset-0 w-full">
        <Carousel
          :value="images"
          :numVisible="1"
          :numScroll="1"
          :autoplayInterval="5000"
          :circular="true"
          class="h-full w-full"
          :showNavigators="false"
        >
          <template #item="slotProps">
            <div class="overflow-y-auto overflow-x-auto">
              <img
                :src="slotProps.data.url"
                :alt="slotProps.data.alt"
                class="max-w-full max-h-full object-contain"
              />
            </div>
          </template>
        </Carousel>
      </div>

      <div
        class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-center z-20"
      >
        <h1 class="text-6xl font-bold mb-4 text-white animate-on-scroll">
          Welcome to eLib
        </h1>
        <p class="text-xl mb-8 text-white animate-on-scroll">
          Discover Your Next Great Read
        </p>
        <Button
          label="Start Exploring"
          @click="scrollToContent"
          class="p-button-lg animate-on-scroll"
        />
      </div>
    </div>

    <div
      id="content"
      class="bg-gradient-to-b from-gray-900 to-gray-800 text-white min-h-screen"
    >
      <div class="container mx-auto px-4 py-16">
        <!-- Feature cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 mb-16">
          <div
            v-for="(feature, index) in features"
            :key="feature.title"
            class="animate-on-scroll bg-gray-800/50 backdrop-blur p-8 rounded-xl border border-gray-700 hover:border-primary transition-all duration-300"
          >
            <div
              class="bg-primary/20 rounded-full w-16 h-16 flex items-center justify-center mb-6"
            >
              <i :class="feature.icon" class="text-3xl text-primary"></i>
            </div>
            <h3 class="text-2xl font-semibold mb-4 text-white">
              {{ feature.title }}
            </h3>
            <p class="text-gray-300 leading-relaxed">
              {{ feature.description }}
            </p>
          </div>
        </div>

        <div class="mb-16">
          <TabView>
            <TabPanel header="Reader Features">
              <div
                class="p-8 bg-gray-800/50 backdrop-blur rounded-b-xl border border-gray-700"
              >
                <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div v-for="(list, index) in readerFeatures" :key="index">
                    <div
                      v-for="item in list"
                      :key="item.text"
                      class="flex items-center space-x-3 mb-4 animate-on-scroll"
                    >
                      <i :class="item.icon" class="text-primary text-xl"></i>
                      <span class="text-gray-300">{{ item.text }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </TabPanel>

            <TabPanel header="Library Management">
              <div
                class="p-8 bg-gray-800/50 backdrop-blur rounded-b-xl border border-gray-700"
              >
                <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div v-for="(list, index) in librarianFeatures" :key="index">
                    <div
                      v-for="item in list"
                      :key="item.text"
                      class="flex items-center space-x-3 mb-4 animate-on-scroll"
                    >
                      <i :class="item.icon" class="text-primary text-xl"></i>
                      <span class="text-gray-300">{{ item.text }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </TabPanel>
          </TabView>
        </div>

        <div class="mb-16">
          <div
            class="animate-on-scroll bg-gray-800/50 backdrop-blur p-8 rounded-xl border border-gray-700 text-center"
          >
            <i class="pi pi-cog text-5xl text-primary mb-4"></i>
            <h2 class="text-3xl font-bold mb-4">Powered by AI*</h2>
            <p class="text-gray-300 mb-4 text-lg">
              Experience our cutting-edge AI-powered recommendation system that
              understands your reading preferences and suggests your next
              perfect book!
            </p>
            <p class="text-sm text-gray-400 italic">
              * Marketing disclaimer: While we do use smart algorithms for
              recommendations, there's no magical AI here - just good
              old-fashioned library science combined with modern technology! We
              believe in honest tech marketing.
            </p>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div
            v-for="stat in statistics"
            :key="stat.title"
            class="animate-on-scroll bg-gray-800/50 backdrop-blur p-8 rounded-xl border border-gray-700 text-center transition-all duration-300 hover:border-primary"
          >
            <i :class="stat.icon" class="text-4xl text-primary mb-4"></i>
            <div class="text-4xl font-bold text-primary mb-2">
              {{ stat.value }}
            </div>
            <div class="text-xl text-white mb-2">{{ stat.title }}</div>
            <p class="text-gray-300">{{ stat.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";

const showBackToTop = ref(false);
const animatedElements = ref([]);

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
    alt: "Reading area",
  },
];

const features = [
  {
    icon: "pi pi-book",
    title: "Find Your Perfect Book",
    description:
      "Browse through our carefully curated collection of books. From bestsellers to hidden gems, find your next favorite read with ease.",
  },
  {
    icon: "pi pi-envelope",
    title: "Stay Connected",
    description:
      "Receive personalized email updates about new arrivals, due dates, and book recommendations tailored to your interests.",
  },
  {
    icon: "pi pi-heart",
    title: "Personal Collections",
    description:
      "Create your own reading lists, track your progress, and keep all your favorite books organized in one place.",
  },
  {
    icon: "pi pi-star",
    title: "Rate & Review",
    description:
      "Share your thoughts on books you've read and discover what other readers think about your next potential read.",
  },
  {
    icon: "pi pi-users",
    title: "Join the Community",
    description:
      "Connect with fellow book lovers, share recommendations, and be part of our growing reading community.",
  },
  {
    icon: "pi pi-bell",
    title: "Smart Notifications",
    description:
      "Get timely reminders about due dates, book availability, and personalized recommendations.",
  },
];

const readerFeatures = [
  [
    {
      icon: "pi pi-search",
      text: "Smart book recommendations based on your interests",
    },
    {
      icon: "pi pi-bookmark",
      text: "Create and organize personal reading lists",
    },
    { icon: "pi pi-chart-line", text: "Track your reading progress" },
    { icon: "pi pi-calendar", text: "Set reading goals and get achievements" },
  ],
  [
    { icon: "pi pi-comments", text: "Join book discussions and reviews" },
    {
      icon: "pi pi-envelope",
      text: "Get email notifications about new arrivals",
    },
    { icon: "pi pi-star", text: "Rate books and share your opinions" },
    { icon: "pi pi-share-alt", text: "Share your favorite reads with friends" },
  ],
];

const librarianFeatures = [
  [
    { icon: "pi pi-database", text: "Efficiently manage book collections" },
    { icon: "pi pi-users", text: "Handle member accounts and permissions" },
    { icon: "pi pi-chart-bar", text: "Access detailed analytics and reports" },
    { icon: "pi pi-calendar", text: "Track reservations and due dates" },
  ],
  [
    { icon: "pi pi-cog", text: "Customize system settings and preferences" },
    { icon: "pi pi-envelope", text: "Manage communication with members" },
    { icon: "pi pi-tags", text: "Organize books by categories and tags" },
    { icon: "pi pi-clock", text: "Monitor library activities in real-time" },
  ],
];

const statistics = [
  {
    icon: "pi pi-users",
    title: "Happy Readers",
    value: "5,000+",
    description: "Join our thriving community of book lovers",
  },
  {
    icon: "pi pi-book",
    title: "Books Available",
    value: "10,000+",
    description: "Find your next adventure in our growing collection",
  },
  {
    icon: "pi pi-check-circle",
    title: "Daily Checkouts",
    value: "200+",
    description: "New reading journeys starting every day",
  },
];

// Methods
const handleScroll = () => {
  showBackToTop.value = window.scrollY > 500;
};

const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: "smooth" });
};

const scrollToContent = () => {
  document.getElementById("content").scrollIntoView({ behavior: "smooth" });
};

// Intersection Observer setup
onMounted(() => {
  // Znajdujemy wszystkie elementy do animacji
  animatedElements.value = document.querySelectorAll(".animate-on-scroll");

  // Tworzymy intersection observer
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          entry.target.classList.add("is-visible");
        }
      });
    },
    { threshold: 0.1 }
  );

  animatedElements.value.forEach((el) => observer.observe(el));

  window.addEventListener("scroll", handleScroll);

  return () => {
    animatedElements.value.forEach((el) => observer.unobserve(el));
    window.removeEventListener("scroll", handleScroll);
  };
});
</script>

<style scoped>
.animate-on-scroll {
  opacity: 0;
  transform: translateY(20px);
  transition: all 0.6s ease-out;
}

.animate-on-scroll.is-visible {
  opacity: 1;
  transform: translateY(0);
}

:deep(.p-tabview-nav) {
  background-color: transparent !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}

:deep(.p-tabview-nav-link) {
  color: white !important;
  background-color: transparent !important;
}

:deep(.p-tabview-selected) {
  border-color: var(--primary-color) !important;
}

:deep(.p-tabview-panels) {
  background-color: transparent !important;
  border: none !important;
}

:deep(.p-carousel-indicators) {
  background-color: transparent !important;
}

:deep(.p-carousel) {
  width: 100%;
  overflow-x: hidden;
}

:deep(.p-carousel-container) {
  width: 100%;
}
</style>
