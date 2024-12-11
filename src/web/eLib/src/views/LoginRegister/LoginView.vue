<template>
  <div class="flex min-h-screen items-center justify-center p-4 bg-slate-900">
    <Card class="w-full max-w-md bg-slate-800">
      <template #title>
        <h2 class="text-center text-2xl font-semibold text-white mb-6">
          Sign in to eLib
        </h2>
      </template>
      <template #content>
        <div class="mb-8">
          <SelectButton
            v-model="loginMethod"
            :options="loginMethods"
            optionLabel="name"
            class="w-full"
            :pt="{
              button: {
                class: 'text-white hover:bg-cyan-500/20 flex-1',
              },
              buttonIcon: {
                class: 'text-white',
              },
            }"
          />
        </div>

        <form @submit.prevent="handleLogin" class="space-y-8">
          <div class="field relative">
            <span class="p-float-label">
              <InputText
                :id="loginMethod.id"
                v-model="credentials.identifier"
                :type="loginMethod.inputType"
                class="w-full bg-slate-700 text-white border-slate-600 h-12"
                required
                :placeholder="loginMethod.placeholder"
              />
              <label :for="loginMethod.id" class="text-slate-300">{{
                loginMethod.label
              }}</label>
            </span>
          </div>

          <div class="field relative">
            <span class="p-float-label">
              <Password
                id="password"
                v-model="credentials.password"
                :feedback="false"
                toggleMask
                class="w-full"
                required
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
          </div>

          <div class="flex justify-between items-center">
            <div class="flex items-center">
              <Checkbox
                v-model="rememberMe"
                :binary="true"
                inputId="remember"
                class="mr-2"
                :pt="{
                  root: {
                    class: 'text-cyan-400',
                  },
                }"
              />
              <label for="remember" class="text-sm text-slate-300"
                >Remember me</label
              >
            </div>
            <router-link
              to="/forgot-password"
              class="text-sm text-cyan-400 hover:text-cyan-300 hover:underline"
            >
              Forgot password?
            </router-link>
          </div>

          <Button
            type="submit"
            :label="loading ? 'Signing in...' : 'Sign in'"
            :loading="loading"
            class="w-full bg-cyan-400 hover:bg-cyan-500 border-none h-12"
          />
        </form>

        <!-- Dodana sekcja rejestracji -->
        <div class="mt-6 text-center border-t border-slate-700 pt-6">
          <p class="text-slate-300">
            Don't have an account?
            <router-link
              to="/register"
              class="text-cyan-400 hover:text-cyan-300 hover:underline ml-1 font-medium"
            >
              Sign up
            </router-link>
          </p>
        </div>
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
  @apply shadow-lg bg-slate-800 border-none;
}

:deep(.p-selectbutton) {
  display: flex;
  background-color: theme("colors.slate.700");
  border-radius: theme("borderRadius.lg");
  padding: theme("spacing.1");
  border: none;

  .p-button {
    flex: 1;
    background-color: transparent;
    border: none;
    border-radius: theme("borderRadius.md");
    color: theme("colors.white");

    &.p-highlight {
      background-color: theme("colors.cyan.400");
    }

    &:not(.p-highlight):hover {
      background-color: theme("colors.slate.600");
    }
  }
}

:deep(.p-float-label) {
  position: relative;
  margin-top: theme("spacing.6");
}

:deep(.p-float-label label) {
  background-color: transparent;
  padding: 0 theme("spacing.2");
  margin-top: -0.5rem;
}

:deep(.p-inputtext:enabled:focus) {
  @apply border-cyan-400 ring-1 ring-cyan-400;
}

:deep(.p-password-panel) {
  @apply bg-slate-700 border-slate-600;
}
</style>
