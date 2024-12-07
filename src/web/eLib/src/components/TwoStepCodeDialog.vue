<template>
  <Dialog
    v-model:visible="isDialogVisible"
    :header="dialogHeader"
    modal
    class="max-w-sm"
    :draggable="false"
  >
    <div class="p-4">
      <p class="text-gray-300 mb-4">
        {{ verificationMessage }}
      </p>

      <InputOtp
        v-model="otpValue"
        :input-type="inputType"
        :length="6"
        separator=""
        class="justify-center"
        @change="handleComplete"
      >
        <template #input="{ active, value }">
          <input
            :class="[
              'w-10 h-12 mx-1 text-center border rounded-md bg-gray-700 border-gray-600',
              'focus:border-cyan-400 focus:ring-1 focus:ring-cyan-400 focus:outline-none',
              { 'border-cyan-400': active },
            ]"
            :value="value"
          />
        </template>
      </InputOtp>

      <div class="mt-4 flex justify-center">
        <template v-if="resendTimeout > 0">
          <span class="text-gray-400">Resend code in {{ resendTimeout }}s</span>
        </template>
        <template v-else>
          <Button
            class="text-cyan-400 hover:text-cyan-500"
            text
            @click="handleResendCode"
          >
            Resend code
          </Button>
        </template>
      </div>
    </div>
  </Dialog>
</template>

<script setup>
import { ref, computed, watch, onUnmounted } from "vue";
import Dialog from "primevue/dialog";
import Button from "primevue/button";
import InputOtp from "primevue/inputotp";
import apiClient from "../clients/eLibApiClient";
import { useToast } from "primevue/usetoast";

const toast = useToast();

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: (value) => ["email", "sms"].includes(value),
  },
  visible: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:visible", "email-verified", "sms-verified"]);

const isDialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit("update:visible", value),
});

const otpValue = ref("");
const resendTimeout = ref(0);
let timeoutInterval = null;

const dialogHeader = computed(() => {
  return props.type === "email" ? "Email Verification" : "Phone Verification";
});

const verificationMessage = computed(() => {
  return props.type === "email"
    ? "Please enter the verification code sent to your email"
    : "Please enter the verification code sent to your phone";
});

const inputType = computed(() => {
  return props.type === "email" ? "text" : "number";
});

const startResendTimeout = () => {
  resendTimeout.value = 60;
  timeoutInterval = setInterval(() => {
    if (resendTimeout.value > 0) {
      resendTimeout.value--;
    } else {
      clearInterval(timeoutInterval);
    }
  }, 1000);
};

const sendVerificationCode = async () => {
  console.log(props.type);
  if (props.type === "email") {
    try {
      await apiClient.sendEmailVerificationCode();
    } catch (error) {
      toast.add({
        severity: "error",
        summary: "Error",
        detail: "Failed to send email verification code",
        life: 3000,
      });
    }
  } else {
    try {
      await apiClient.sendSmsVerificationCode();
    } catch (error) {
      toast.add({
        severity: "error",
        summary: "Error",
        detail: "Failed to send SMS verification code",
        life: 3000,
      });
    }
    startResendTimeout();
  }
};

watch(
  () => props.visible,
  (newValue) => {
    if (newValue) {
      sendVerificationCode();
    } else {
      otpValue.value = "";
      clearInterval(timeoutInterval);
      resendTimeout.value = 0;
    }
  }
);

const handleResendCode = () => {
  if (resendTimeout.value === 0) {
    sendVerificationCode();
  }
};

const handleComplete = (event) => {
  if (event.value.length === 6) {
    console.log("Verification code entered:", event.value);
    if (props.type == "email") {
      handleEmail(event.value);
    } else {
      handleSms(event.value);
    }
  }
};

const handleEmail = async (code) => {
  try {
    await apiClient.verifyEmail(code);
    emit("email-verified", code);
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to verify email",
      life: 3000,
    });
    return;
  }
};

const handleSms = async (code) => {
  try {
    await apiClient.verifyPhoneNumber(code);
    emit("sms-verified", code);
  } catch (error) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Failed to verify phone number",
      life: 3000,
    });
    return;
  }
};

onUnmounted(() => {
  if (timeoutInterval) {
    clearInterval(timeoutInterval);
  }
});
</script>
