<template>
  <div class="flex min-h-screen items-center justify-center p-4">
    <Card class="w-full max-w-md">
      <template #title>
        <h2 class="text-center text-2xl font-semibold text-gray-800">
          Sign in to eLib
        </h2>
      </template>
      <template #content>
        <div class="mb-4">
          <SelectButton
            v-model="loginMethod"
            :options="loginMethods"
            optionLabel="name"
            class="w-full"
          />
        </div>

        <form @submit.prevent="handleLogin" class="space-y-6">
          <div class="field">
            <span class="p-float-label">
              <InputText
                :id="loginMethod.id"
                v-model="credentials.identifier"
                :type="loginMethod.inputType"
                class="w-full"
                required
                :placeholder="loginMethod.placeholder"
              />
              <label :for="loginMethod.id">{{ loginMethod.label }}</label>
            </span>
          </div>

          <div class="field">
            <span class="p-float-label">
              <Password
                id="password"
                v-model="credentials.password"
                :feedback="false"
                toggleMask
                class="w-full"
                required
                inputClass="w-full"
              />
              <label for="password">Password</label>
            </span>
          </div>

          <div class="flex justify-between items-center">
            <div class="flex items-center">
              <Checkbox
                v-model="rememberMe"
                :binary="true"
                inputId="remember"
                class="mr-2"
              />
              <label for="remember" class="text-sm text-gray-600"
                >Remember me</label
              >
            </div>
            <router-link
              to="/forgot-password"
              class="text-sm text-primary-600 hover:underline"
            >
              Forgot password?
            </router-link>
          </div>

          <Button
            type="submit"
            :label="loading ? 'Signing in...' : 'Sign in'"
            :loading="loading"
            class="w-full"
          />
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup>
import { ref, reactive } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { useToast } from "primevue/usetoast";
import SelectButton from "primevue/selectbutton";

const store = useStore();
const router = useRouter();
const toast = useToast();

const loginMethods = [
  {
    id: "email",
    name: "Email",
    label: "Email",
    inputType: "email",
    placeholder: "Enter your email",
  },
  {
    id: "phone",
    name: "Phone",
    label: "Phone Number",
    inputType: "tel",
    placeholder: "Enter your phone number",
  },
];

const loginMethod = ref(loginMethods[0]);
const credentials = reactive({
  identifier: "",
  password: "",
});
const rememberMe = ref(false);
const loading = ref(false);

const handleLogin = async () => {
  if (!credentials.identifier || !credentials.password) {
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Please fill in all fields",
      life: 3000,
    });
    return;
  }

  try {
    loading.value = true;

    if (loginMethod.value.id === "email") {
      await store.dispatch("loginWithEmail", {
        email: credentials.identifier,
        password: credentials.password,
      });
    } else {
      await store.dispatch("loginWithPhone", {
        phoneNumber: credentials.identifier,
        password: credentials.password,
      });
    }

    toast.add({
      severity: "success",
      summary: "Success",
      detail: "Logged in successfully",
      life: 3000,
    });

    router.push("/");
  } catch (error) {
    let errorMessage = "Invalid credentials";
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
:deep(.p-password input) {
  width: 100%;
}

:deep(.p-card) {
  @apply shadow-lg;
}

:deep(.p-selectbutton) {
  display: flex;
  .p-button {
    flex: 1;
  }
}
</style>
