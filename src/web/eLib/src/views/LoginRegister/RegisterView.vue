<template>
  <div class="flex min-h-screen items-center justify-center p-4 bg-slate-900">
    <Card class="w-full max-w-md bg-slate-800">
      <template #title>
        <h2 class="text-center text-2xl font-semibold text-white mb-6">
          Create your account
        </h2>
      </template>
      <template #content>
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="field relative">
            <span class="p-float-label">
              <InputText
                id="name"
                v-model="user.name"
                class="w-full bg-slate-700 text-white border-slate-600 h-12"
                :class="{ 'p-invalid': errors.name }"
                required
              />
              <label for="name" class="text-slate-300">First Name</label>
            </span>
            <small class="text-red-400" v-if="errors.name">{{
              errors.name
            }}</small>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <InputText
                id="surname"
                v-model="user.surname"
                class="w-full bg-slate-700 text-white border-slate-600 h-12"
                :class="{ 'p-invalid': errors.surname }"
                required
              />
              <label for="surname" class="text-slate-300">Last Name</label>
            </span>
            <small class="text-red-400" v-if="errors.surname">{{
              errors.surname
            }}</small>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <InputText
                id="email"
                v-model="user.email"
                type="email"
                class="w-full bg-slate-700 text-white border-slate-600 h-12"
                :class="{ 'p-invalid': errors.email }"
                required
              />
              <label for="email" class="text-slate-300">Email</label>
            </span>
            <small class="text-red-400" v-if="errors.email">{{
              errors.email
            }}</small>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <InputText
                id="phoneNumber"
                v-model="user.phoneNumber"
                class="w-full bg-slate-700 text-white border-slate-600 h-12"
                :class="{ 'p-invalid': errors.phoneNumber }"
                required
              />
              <label for="phoneNumber" class="text-slate-300"
                >Phone Number</label
              >
            </span>
            <small class="text-red-400" v-if="errors.phoneNumber">{{
              errors.phoneNumber
            }}</small>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <Password
                id="password"
                v-model="user.password"
                class="w-full"
                :class="{ 'p-invalid': errors.password }"
                :feedback="true"
                required
                toggleMask
                :pt="{
                  input: {
                    class:
                      'w-full bg-slate-700 text-white border-slate-600 h-12',
                  },
                  showIcon: {
                    class: 'text-slate-300',
                  },
                  hideIcon: {
                    class: 'text-slate-300',
                  },
                }"
              />
              <label for="password" class="text-slate-300">Password</label>
            </span>
            <small class="text-red-400" v-if="errors.password">{{
              errors.password
            }}</small>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <Password
                id="confirmPassword"
                v-model="confirmPassword"
                class="w-full"
                :class="{ 'p-invalid': errors.confirmPassword }"
                required
                toggleMask
                :feedback="false"
                :pt="{
                  input: {
                    class:
                      'w-full bg-slate-700 text-white border-slate-600 h-12',
                  },
                  showIcon: {
                    class: 'text-slate-300',
                  },
                  hideIcon: {
                    class: 'text-slate-300',
                  },
                }"
              />
              <label for="confirmPassword" class="text-slate-300"
                >Confirm Password</label
              >
            </span>
            <small class="text-red-400" v-if="errors.confirmPassword">{{
              errors.confirmPassword
            }}</small>
          </div>

          <div class="field relative">
            <label class="block text-slate-300 mb-2"
              >Preferred Notification Channel</label
            >
            <Dropdown
              v-model="user.notificationChannel"
              :options="notificationChannels"
              optionLabel="name"
              optionValue="value"
              placeholder="Select channel"
              class="w-full bg-slate-700 border-slate-600"
              :class="{ 'p-invalid': errors.notificationChannel }"
              required
            />
            <small class="text-red-400" v-if="errors.notificationChannel">{{
              errors.notificationChannel
            }}</small>
          </div>

          <Button
            type="submit"
            label="Register"
            class="w-full bg-cyan-400 hover:bg-cyan-500 border-none h-12"
            :loading="loading"
          />

          <div class="mt-6 text-center border-t border-slate-700 pt-6">
            <p class="text-slate-300">
              Already have an account?
              <router-link
                to="/login"
                class="text-cyan-400 hover:text-cyan-300 hover:underline ml-1 font-medium"
              >
                Sign in
              </router-link>
            </p>
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup>
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { useToast } from "primevue/usetoast";
import apiClient from "../../clients/eLibApiClient";

const router = useRouter();
const toast = useToast();
const loading = ref(false);
const confirmPassword = ref("");

const notificationChannels = [
  { name: "Email", value: 1 },
  { name: "SMS", value: 2 },
  { name: "System", value: 3 },
];

const user = reactive({
  name: "",
  surname: "",
  email: "",
  phoneNumber: "",
  password: "",
  notificationChannel: null,
});

const errors = reactive({
  name: "",
  surname: "",
  email: "",
  phoneNumber: "",
  password: "",
  confirmPassword: "",
  notificationChannel: "",
});

const validateForm = () => {
  let isValid = true;

  // Reset errors
  Object.keys(errors).forEach((key) => (errors[key] = ""));

  if (!user.name.trim()) {
    errors.name = "First name is required";
    isValid = false;
  }

  if (!user.surname.trim()) {
    errors.surname = "Last name is required";
    isValid = false;
  }

  if (!user.email.trim()) {
    errors.email = "Email is required";
    isValid = false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(user.email)) {
    errors.email = "Please enter a valid email";
    isValid = false;
  }

  if (!user.phoneNumber.trim()) {
    errors.phoneNumber = "Phone number is required";
    isValid = false;
  } else {
    const phoneNumber = user.phoneNumber.replace(/\D/g, "");
    if (phoneNumber.length < 6 || phoneNumber.length > 11) {
      errors.phoneNumber = "Phone number must be between 6 and 11 digits";
      isValid = false;
    }
  }

  if (!user.password) {
    errors.password = "Password is required";
    isValid = false;
  } else if (user.password.length < 8) {
    errors.password = "Password must be at least 8 characters long";
    isValid = false;
  }

  if (!confirmPassword.value) {
    errors.confirmPassword = "Please confirm your password";
    isValid = false;
  } else if (confirmPassword.value !== user.password) {
    errors.confirmPassword = "Passwords do not match";
    isValid = false;
  }

  if (!user.notificationChannel) {
    errors.notificationChannel = "Please select notification preference";
    isValid = false;
  }

  return isValid;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  try {
    loading.value = true;

    const formattedUser = {
      ...user,
      phoneNumber: user.phoneNumber.replace(/\D/g, ""),
    };

    await apiClient.registerUser(formattedUser);

    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Registration successful! Please sign in.",
      life: 3000,
    });

    router.push("/login");
  } catch (error) {
    let errorMessage = "Registration failed. Please try again.";
    if (error.response?.data?.message) {
      errorMessage = error.response.data.message;
    }

    toast.add({
      severity: "error",
      summary: "Error",
      detail: errorMessage,
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
:deep(.p-dropdown-panel) {
  background-color: theme("colors.slate.700");
  border-color: theme("colors.slate.600");
}

:deep(.p-dropdown-item) {
  color: theme("colors.white");
}

:deep(.p-dropdown-item:hover) {
  background-color: theme("colors.slate.600");
}

:deep(.p-password-panel) {
  background-color: theme("colors.slate.700");
  border-color: theme("colors.slate.600");
  color: theme("colors.white");
}

:deep(.p-inputtext:enabled:focus) {
  border-color: theme("colors.cyan.400");
  box-shadow: 0 0 0 1px theme("colors.cyan.400");
}

:deep(.p-password) {
  width: 100%;
}

:deep(.p-password input) {
  width: 100%;
}

:deep(.p-password-panel) {
  background-color: theme("colors.slate.700");
  border-color: theme("colors.slate.600");
  color: theme("colors.white");
}

:deep(.p-password .p-password-input) {
  width: 100%;
  padding-right: 2.5rem !important;
}

:deep(.p-password i) {
  right: 0.75rem;
}
</style>
