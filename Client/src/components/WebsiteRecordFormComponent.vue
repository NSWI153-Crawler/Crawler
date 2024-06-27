<script setup lang="ts">
import { useWebsiteRecordStore } from '@/stores/records'
import { ref, reactive } from 'vue'

const store = useWebsiteRecordStore()
const showForm = ref(false)
const formFields = reactive({
  creation: true,
  id: '',
  label: '',
  url: '',
  regexp: '',
  tags: '',
  periodicity: 1,
  status: true
})

function clearForm() {
  formFields.creation = true
  formFields.id = ''
  formFields.label = ''
  formFields.url = ''
  formFields.regexp = ''
  formFields.tags = ''
  formFields.periodicity = 1
  formFields.status = true
}

function hideForm() {
  clearForm()
  showForm.value = false
}

async function handleSubmit() {
  if (formFields.creation) {
    await store.createRecord(
      formFields.url,
      formFields.regexp,
      formFields.periodicity,
      formFields.label,
      formFields.status,
      formFields.tags.split(/[,\s]+/)
    )
  } else {
    await store.updateRecord(
      formFields.id,
      formFields.url,
      formFields.regexp,
      formFields.periodicity,
      formFields.label,
      formFields.status,
      formFields.tags.split(/[,\s]+/)
    )
  }
  hideForm()
}

function showCreationForm(url: string = '') {
  clearForm()
  if (url) formFields.url = url
  showForm.value = true
}

function showEditForm(
  id: string,
  label: string,
  url: string,
  regexp: string,
  tags: string,
  periodicity: number,
  status: boolean
) {
  formFields.creation = false
  formFields.id = id
  formFields.label = label
  formFields.url = url
  formFields.regexp = regexp
  formFields.tags = tags
  formFields.periodicity = periodicity
  formFields.status = status
  showForm.value = true
}

defineExpose({
  showCreationForm,
  showEditForm
})
</script>

<template>
  <div :class="[showForm ? 'block' : 'hidden']">
    <!-- the veil -->
    <div class="fixed top-0 bottom-0 left-0 right-0 z-50 bg-[rgba(0,0,0,0.5)]" @click="hideForm" />
    <div
      class="bg-gray-200 dark:bg-dark-bg border-2 border-dark-bg dark:border-dark-fg rounded-2xl py-4 px-8 z-50 fixed top-1/2 left-1/2 translate-x-[-50%] translate-y-[-50%]"
    >
      <h1 v-if="formFields.creation" class="py-2 text-xl font-bold text-center mb-4">
        Create New Record
      </h1>
      <h1 v-else class="py-2 text-xl font-bold text-center mb-4">Edit Record</h1>
      <form @submit.prevent="handleSubmit()" class="space-y-2">
        <div>
          <label for="label" class="w-[100px] float-left" title="Name of your website record"
            >Label:</label
          >
          <input
            type="text"
            id="label"
            name="label"
            class="rounded text-black pl-1"
            v-model="formFields.label"
          />
        </div>
        <div>
          <label for="url" class="w-[100px] float-left" title="Starting URL">URL:</label>
          <input
            type="text"
            id="url"
            name="url"
            class="rounded text-black pl-1"
            v-model="formFields.url"
          />
        </div>
        <div>
          <label
            for="regex"
            class="w-[100px] float-left"
            title="May restrict what pages should be crawled"
            >RegEx:</label
          >
          <input
            type="text"
            id="regex"
            name="regex"
            class="rounded text-black pl-1"
            v-model="formFields.regexp"
          />
        </div>
        <div>
          <label
            for="tags"
            class="w-[100px] float-left"
            title="For multiple tags, separate them by comma"
            >Tags: ⓘ</label
          >
          <input
            type="text"
            id="tags"
            name="tags"
            class="rounded text-black pl-1"
            v-model="formFields.tags"
          />
        </div>
        <div>
          <label for="periodicity" class="w-[100px] float-left" title="How often is record executed"
            >Periodicity:</label
          >
          <input
            type="number"
            id="periodicity"
            name="periodicity"
            min="0"
            class="rounded text-black pl-1 w-10"
            v-model="formFields.periodicity"
          />
          /day
        </div>
        <!-- status buttons container -->
        <div class="flex space-x-1 pr-4">
          <label
            class="p-1 select-none cursor-default dark:text-dark-fg w-[94px]"
            title="Indicates, whether or not should the record be periodically executed. Can be changed later"
            >Status:</label
          >
          <div class="toggle-container">
            <label
              @click="formFields.status = true"
              :class="[
                formFields.status ? 'bg-[#0f0] text-black' : 'disabled-state',
                'button-style'
              ]"
            >
              Active
            </label>
            <label
              @click="formFields.status = false"
              :class="[
                !formFields.status ? 'bg-[#f00] text-black' : 'disabled-state',
                'button-style'
              ]"
            >
              Inactive
            </label>
          </div>
        </div>
        <div class="flex justify-center space-x-4 mb-2 pt-4">
          <button
            type="button"
            class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#f00] hover:bg-[#e00] font-bold"
            @click="hideForm"
          >
            Cancel
          </button>
          <button
            v-if="formFields.creation"
            type="submit"
            class="button-style border-2 border-dark-bg dark:border-[#0f0] rounded bg-[#0f0] hover:bg-[#0d0] font-bold text-black"
          >
            Save
          </button>

          <button
            v-else
            type="submit"
            class="button-style border-2 border-dark-bg dark:border-[#0cf] rounded bg-[#0cf] hover:bg-[#0bd] font-bold text-black"
          >
            Update
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
