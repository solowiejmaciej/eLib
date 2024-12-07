<template>
  <form @submit.prevent="updateProfile" class="space-y-6">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-300">
          First Name
        </label>
        <InputText
          v-model="props.user.name"
          class="w-full bg-gray-700 border-gray-600"
          placeholder="Enter your first name"
        />
      </div>
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-300">
          Last Name
        </label>
        <InputText
          v-model="props.user.surname"
          class="w-full bg-gray-700 border-gray-600"
          placeholder="Enter your last name"
        />
      </div>
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-300"> Email </label>
        <div class="flex gap-3">
          <InputText
            v-model="props.user.email"
            class="w-full bg-gray-700 border-gray-600"
            placeholder="Enter your email"
          />
        </div>
      </div>
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-300"> Phone </label>
        <div class="flex gap-3">
          <InputText
            v-model="props.user.phoneNumber"
            class="w-full bg-gray-700 border-gray-600"
            placeholder="Enter your phone number"
          />
        </div>
      </div>
    </div>
    <div class="flex justify-end gap-4 mt-6">
      <Button
        v-if="
          !props.user.details.hasEmailVerified &&
          store.getters.currentUser.id == props.user.id
        "
        severity="secondary"
        raised
        label="Confirm email"
        @click="showEmailVerification"
      />
      <Button
        v-if="
          !props.user.details.hasPhoneNumberVerified &&
          store.getters.currentUser.id == props.user.id
        "
        severity="secondary"
        raised
        label="Confirm phone"
        @click="showSmsVerification"
      />
      <Button type="submit" :loading="saving" raised label="Save" />
    </div>
    <TwoStepCodeDialog
      v-model:visible="isVerificationVisible"
      :type="verificationType"
      @email-verified="handleEmailVerified"
      @sms-verified="handleSmsVerified"
    />
  </form>
</template>

<script setup>
import { ref } from "vue";
import { useToast } from "primevue/usetoast";
import apiClient from "../clients/eLibApiClient";
import store from "../store/store";
import TwoStepCodeDialog from "./TwoStepCodeDialog.vue";

const toast = useToast();

const saving = ref(false);

const emit = defineEmits(["update", "verified"]);

const props = defineProps({
  user: {
    type: Object,
    required: true,
  },
});

const updateProfile = async () => {
  console.log("updateProfile");

  if (!props.user.id) return;
  saving.value = true;

  try {
    await apiClient.updateUser(props.user.id, props.user);
    emit("update");
    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Profile updated successfully",
      life: 3000,
    });
  } catch (error) {
    console.error("Error updating profile:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to update profile",
      life: 3000,
    });
  } finally {
    saving.value = false;
  }
};

const isVerificationVisible = ref(false);
const verificationType = ref("email");

const showEmailVerification = () => {
  verificationType.value = "email";
  isVerificationVisible.value = true;
};

const showSmsVerification = () => {
  verificationType.value = "sms";
  isVerificationVisible.value = true;
};

const handleEmailVerified = (code) => {
  isVerificationVisible.value = false;
  toast.add({
    severity: "success",
    summary: "Success",
    detail: "Email verified successfully",
    life: 3000,
  });
  emit("verified");
};

const handleSmsVerified = (code) => {
  isVerificationVisible.value = false;
  toast.add({
    severity: "success",
    summary: "Success",
    detail: "Phone verified successfully",
    life: 3000,
  });
  emit("verified");
};
</script>
