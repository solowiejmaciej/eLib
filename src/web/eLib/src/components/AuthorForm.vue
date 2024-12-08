<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    :header="isEditing ? 'Edit Author' : 'New Author'"
    modal
    class="p-fluid"
    :style="{ width: '40rem' }"
    @hide="hideDialog"
  >
    <div class="grid grid-cols-1 gap-4">
      <div class="field">
        <label for="name" class="font-medium">Name</label>
        <InputText id="name" v-model="authorData.name" required autofocus />
      </div>

      <div class="field">
        <label for="surname" class="font-medium">Surname</label>
        <InputText id="surname" v-model="authorData.surname" required />
      </div>

      <div class="field">
        <label for="birthday" class="font-medium">Birthday</label>
        <Calendar
          id="birthday"
          v-model="authorData.birthday"
          dateFormat="dd/mm/yy"
          :maxDate="new Date()"
          :showIcon="true"
          required
        />
      </div>

      <div class="field">
        <label for="biography" class="font-medium">Biography</label>
        <Textarea
          id="biography"
          v-model="authorData.biography"
          rows="3"
          autoResize
        />
      </div>

      <div class="field">
        <label for="photoUrl" class="font-medium">Photo URL</label>
        <InputText id="photoUrl" v-model="authorData.photoUrl" />
      </div>

      <div v-if="authorData.photoUrl" class="field">
        <img
          :src="authorData.photoUrl"
          :alt="authorData.name"
          class="w-32 h-32 object-cover rounded-lg"
          @error="handleImageError"
        />
      </div>
    </div>

    <template #footer>
      <div class="flex justify-end gap-2">
        <Button label="Cancel" icon="pi pi-times" @click="hideDialog" text />
        <Button
          label="Save"
          icon="pi pi-check"
          @click="saveAuthor"
          :loading="saving"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup>
import { ref, computed, watch, onMounted } from "vue";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Button from "primevue/button";
import Calendar from "primevue/calendar";
import Textarea from "primevue/textarea";
import { useToast } from "primevue/usetoast";
import apiClient from "../clients/eLibApiClient";

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  authorToEdit: {
    type: Object,
    default: () => null,
  },
});

const emit = defineEmits(["update:visible", "author-saved"]);

const toast = useToast();
const saving = ref(false);
const isEditing = computed(() => Boolean(props.authorToEdit));

const authorData = ref({
  id: null,
  name: "",
  surname: "",
  birthday: null,
  biography: "",
  photoUrl: "",
});

const resetForm = () => {
  authorData.value = {
    id: null,
    name: "",
    surname: "",
    birthday: null,
    biography: "",
    photoUrl: "",
  };
};

const handleImageError = (event) => {
  event.target.style.display = "none";
  toast.add({
    severity: "warn",
    summary: "Warning",
    detail: "Failed to load image preview",
    life: 3000,
  });
};

const hideDialog = () => {
  emit("update:visible", false);
  resetForm();
};

const updateFormData = (author) => {
  if (!author) {
    resetForm();
    return;
  }

  try {
    authorData.value = {
      id: author.id || null,
      name: author.name || "",
      surname: author.surname || "",
      birthday: author.birthday ? new Date(author.birthday) : null,
      biography: author.details?.biography || "",
      photoUrl: author.details?.photoUrl || "",
    };
  } catch (error) {
    console.error("Error updating form data:", error);
    resetForm();
  }
};

watch(() => props.authorToEdit, updateFormData, { immediate: true });

watch(
  () => props.visible,
  (newVisible) => {
    if (!newVisible) {
      resetForm();
    }
  }
);

const validateForm = () => {
  if (
    !authorData.value.name ||
    !authorData.value.surname ||
    !authorData.value.birthday
  ) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Please fill in all required fields",
      life: 3000,
    });
    return false;
  }
  return true;
};

const saveAuthor = async () => {
  if (!validateForm()) return;

  try {
    saving.value = true;

    if (isEditing.value) {
      await apiClient.updateAuthor(
        authorData.value.id,
        authorData.value.name,
        authorData.value.surname,
        authorData.value.birthday,
        authorData.value.biography,
        authorData.value.photoUrl
      );
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Author updated successfully",
        life: 3000,
      });
    } else {
      await apiClient.createAuthor(
        authorData.value.name,
        authorData.value.surname,
        authorData.value.birthday,
        authorData.value.biography,
        authorData.value.photoUrl
      );
      toast.add({
        severity: "success",
        summary: "Success",
        detail: "Author created successfully",
        life: 3000,
      });
    }

    emit("author-saved");
    hideDialog();
  } catch (error) {
    console.error("Failed to save author:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to save author",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
};

// Cleanup on unmount
onMounted(() => {
  return () => {
    resetForm();
  };
});
</script>

<style scoped>
.field {
  @apply mb-4;
}

:deep(.p-calendar) {
  @apply w-full;
}

:deep(.p-inputtext) {
  @apply w-full;
}
</style>
