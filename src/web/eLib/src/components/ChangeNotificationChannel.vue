<template>
  <div class="flex flex-col gap-6">
    <div class="flex flex-col gap-4 p-6 rounded-lg">
      <div
        class="flex items-center p-3 cursor-pointer"
        @click="handleRadioChange('1')"
      >
        <RadioButton
          :model-value="String(selectedChannel)"
          :value="'1'"
          @change="handleRadioChange('1')"
        />
        <label class="ml-3 cursor-pointer flex-1">
          <div class="text-gray-200">Email</div>
          <div class="text-sm text-gray-400">
            Receive notifications via email
          </div>
        </label>
      </div>

      <div
        class="flex items-center p-3 cursor-pointer"
        @click="handleRadioChange('2')"
      >
        <RadioButton
          :model-value="String(selectedChannel)"
          :value="'2'"
          @change="handleRadioChange('2')"
        />
        <label class="ml-3 cursor-pointer flex-1">
          <div class="text-gray-200">SMS</div>
          <div class="text-sm text-gray-400">
            Get instant notifications on your phone
          </div>
        </label>
      </div>

      <div
        class="flex items-center p-3 cursor-pointer"
        @click="handleRadioChange('3')"
      >
        <RadioButton
          :model-value="String(selectedChannel)"
          :value="'3'"
          @change="handleRadioChange('3')"
        />
        <label class="ml-3 cursor-pointer flex-1">
          <div class="text-gray-200">System</div>
          <div class="text-sm text-gray-400">
            Notifications in the application
          </div>
        </label>
      </div>
    </div>

    <div class="flex justify-center">
      <Button @click="handleSave" label="Save" />
    </div>
  </div>
</template>
<script setup>
import { ref, watch } from "vue";
import apiClient from "../clients/eLibApiClient";

const props = defineProps({
  selected: {
    type: [String, Number],
    required: true,
  },
  userId: {
    required: true,
  },
});

const emit = defineEmits(["update"]);
const selectedChannel = ref(String(props.selected));

watch(
  () => props.selected,
  (newValue) => {
    selectedChannel.value = String(newValue);
  }
);

const handleRadioChange = (value) => {
  selectedChannel.value = value;
  emit("update", value);
};

const handleSave = async () => {
  try {
    await apiClient.changeNotificationChannel(
      selectedChannel.value,
      props.userId
    );
    emit("update", selectedChannel.value);
  } catch (error) {
    console.error("Failed to update notification channel", error);
  }
};
</script>
