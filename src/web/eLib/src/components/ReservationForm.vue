<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    header="Book Reservation"
    modal
    class="p-fluid reservation-dialog"
    :style="{ width: '50rem' }"
    @hide="hideDialog"
  >
    <div class="flex flex-col gap-6">
      <!-- Book Details -->
      <div
        v-if="book"
        class="flex gap-6 items-start bg-gray-800 rounded-lg p-4"
      >
        <img
          :src="book.details.coverUrl"
          :alt="book.title"
          class="w-32 h-auto object-cover rounded-lg shadow-md"
        />
        <div class="flex flex-col gap-2">
          <h3 class="text-xl font-semibold text-gray-100">{{ book.title }}</h3>
          <div class="text-gray-300 flex items-center gap-2">
            <i class="pi pi-user text-sm"></i>
            {{ author.name }} {{ author.surname }}
          </div>
          <div class="mt-2 text-gray-300 flex items-center gap-2">
            <i class="pi pi-box text-sm"></i>
            <span>Available copies: {{ book.details.quantity }}</span>
          </div>
        </div>
      </div>

      <!-- Reservation Form -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="flex flex-col gap-3">
          <label for="startDate" class="font-medium text-gray-200"
            >Start Date</label
          >
          <Calendar
            v-model="startDate"
            id="startDate"
            :minDate="new Date()"
            dateFormat="dd/mm/yy"
            :showIcon="true"
            class="w-full"
            :class="{ 'p-invalid': startDateError }"
          />
          <small v-if="startDateError" class="text-red-400">{{
            startDateError
          }}</small>
        </div>

        <div class="flex flex-col gap-3">
          <label for="endDate" class="font-medium text-gray-200"
            >End Date</label
          >
          <Calendar
            v-model="endDate"
            id="endDate"
            :minDate="startDate || new Date()"
            dateFormat="dd/mm/yy"
            :showIcon="true"
            class="w-full"
            :class="{ 'p-invalid': endDateError }"
          />
          <small v-if="endDateError" class="text-red-400">{{
            endDateError
          }}</small>
        </div>
      </div>

      <!-- Terms and Duration Info -->
      <div class="bg-gray-800 rounded-lg p-4">
        <div v-if="startDate && endDate" class="mb-4">
          <h4 class="text-gray-200 font-medium mb-2">Reservation Details</h4>
          <div class="grid grid-cols-2 gap-4 text-sm text-gray-300">
            <div>
              <span class="text-gray-400">Duration:</span>
              <span class="ml-2">{{ calculateDuration }} days</span>
            </div>
            <div>
              <span class="text-gray-400">Return by:</span>
              <span class="ml-2">{{ formatDate(endDate) }}</span>
            </div>
          </div>
        </div>

        <div class="flex items-start gap-2 mt-4">
          <Checkbox
            v-model="acceptTerms"
            :binary="true"
            id="acceptTerms"
            :class="{ 'p-invalid': termsError }"
          />
          <label for="acceptTerms" class="text-sm text-gray-300 cursor-pointer">
            I understand that I am responsible for returning the book by the end
            date and maintaining its condition
          </label>
        </div>
        <small v-if="termsError" class="block text-red-400 mt-2">{{
          termsError
        }}</small>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-end gap-3">
        <Button
          label="Cancel"
          icon="pi pi-times"
          @click="hideDialog"
          text
          class="text-gray-300 hover:text-gray-100"
        />
        <Button
          v-if="book.details.quantity > 0"
          label="Reserve Book"
          icon="pi pi-check"
          @click="saveBook"
          :loading="saving"
          :disabled="!isFormValid"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup>
import { ref, computed } from "vue";
import apiClient from "../clients/eLibApiClient";
import { useToast } from "primevue/usetoast";
import store from "../store/store";

const props = defineProps({
  visible: {
    type: Boolean,
    required: true,
  },
  book: {
    type: Object,
    default: null,
  },
  author: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["update:visible", "reservation-saved"]);
const toast = useToast();

const startDate = ref(null);
const endDate = ref(null);
const acceptTerms = ref(false);
const saving = ref(false);

const startDateError = ref("");
const endDateError = ref("");
const termsError = ref("");

const isFormValid = computed(() => {
  return (
    startDate.value &&
    endDate.value &&
    acceptTerms.value &&
    startDate.value <= endDate.value
  );
});

const calculateDuration = computed(() => {
  if (!startDate.value || !endDate.value) return 0;
  const diff = endDate.value.getTime() - startDate.value.getTime();
  return Math.ceil(diff / (1000 * 60 * 60 * 24));
});

// Helper functions
const formatDate = (date) => {
  if (!date) return "";
  return new Intl.DateTimeFormat("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  }).format(date);
};

const validateForm = () => {
  let isValid = true;

  startDateError.value = "";
  endDateError.value = "";
  termsError.value = "";

  if (!startDate.value) {
    startDateError.value = "Please select a start date";
    isValid = false;
  }

  if (!endDate.value) {
    endDateError.value = "Please select an end date";
    isValid = false;
  }

  if (startDate.value && endDate.value && startDate.value > endDate.value) {
    endDateError.value = "End date must be after start date";
    isValid = false;
  }

  if (!acceptTerms.value) {
    termsError.value = "You must agree to the terms to proceed";
    isValid = false;
  }

  return isValid;
};

const resetForm = () => {
  startDate.value = null;
  endDate.value = null;
  acceptTerms.value = false;
  startDateError.value = "";
  endDateError.value = "";
  termsError.value = "";
  saving.value = false;
};

const hideDialog = () => {
  emit("update:visible", false);
  resetForm();
};

const saveBook = async () => {
  if (!validateForm()) return;

  try {
    saving.value = true;

    await apiClient.createReservation(
      props.book.id,
      store.getters.currentUser.id,
      startDate.value,
      endDate.value
    );

    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Your book reservation has been confirmed",
      life: 3000,
    });

    emit("reservation-saved");
    hideDialog();
  } catch (error) {
    console.error("Error saving reservation:", error);
    toast.add({
      severity: "error",
      summary: "Reservation Failed",
      detail: "Unable to complete your reservation. Please try again.",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
};
</script>

<style scoped></style>
