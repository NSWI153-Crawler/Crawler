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
      formFields.tags.trim().split(/[,\s]+/)
    )
  } else {
    await store.updateRecord(
      formFields.id,
      formFields.url,
      formFields.regexp,
      formFields.periodicity,
      formFields.label,
      formFields.status,
      formFields.tags.trim().split(/[,\s]+/)
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
  tags: string[],
  periodicity: number,
  status: boolean
) {
  formFields.creation = false
  formFields.id = id
  formFields.label = label
  formFields.url = url
  formFields.regexp = regexp
  formFields.tags = tags.join(', ')
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
  <Teleport to="body">
    <div class="" :class="[showForm ? 'block' : 'hidden']">
      <div
        class="z-40 fixed top-0 bottom-0 left-0 right-0 bg-[rgba(0,0,0,0.5)]"
        :class="[showForm ? 'block' : 'hidden']"
        @click="hideForm"
      />
      <div
        class="z-50 bg-gray-200 dark:bg-dark-bg border-2 border-dark-bg dark:border-dark-fg rounded-2xl py-4 px-8 fixed top-1/2 left-1/2 translate-x-[-50%] translate-y-[-50%]"
      >
        <h1
          v-if="formFields.creation"
          class="py-2 text-xl font-bold text-center mb-4 dark:text-dark-fg"
        >
          Create New Record
        </h1>
        <h1 v-else class="py-2 text-xl font-bold text-center mb-4">Edit Record</h1>
        <form @submit.prevent="handleSubmit()" class="space-y-2">
          <div>
            <label
              for="label"
              class="w-[100px] float-left dark:text-dark-fg"
              title="Name of your website record"
              >Label:</label
            >
            <input
              type="text"
              id="label"
              name="label"
              required
              class="rounded text-black pl-1"
              v-model="formFields.label"
            />
          </div>
          <div>
            <label for="url" class="w-[100px] float-left dark:text-dark-fg" title="Starting URL"
              >URL:</label
            >
            <input
              type="text"
              id="url"
              name="url"
              required
              class="rounded text-black pl-1"
              v-model="formFields.url"
            />
          </div>
          <div>
            <label
              for="regex"
              class="w-[100px] float-left dark:text-dark-fg"
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
              class="w-[100px] float-left dark:text-dark-fg"
              title="Separate tags with a comma"
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
            <label
              for="periodicity"
              class="w-[100px] float-left dark:text-dark-fg"
              title="How often is the record executed"
              >Periodicity:</label
            >
            <span class="mr-1 dark:text-dark-fg">Every</span>
            <input
              type="number"
              id="periodicity"
              name="periodicity"
              min="0"
              class="rounded text-black pl-1 w-16 text-right"
              v-model="formFields.periodicity"
            />
            <span class="ml-1 dark:text-dark-fg">minutes.</span>
          </div>
          <!-- status buttons container -->
          <div class="flex space-x-1 pr-4">
            <label
              for="status"
              class="p-1 select-none cursor-default dark:text-dark-fg w-[94px]"
              title="Indicates, whether or not should the record be periodically executed. Can be changed later"
              >Status:</label
            >
            <div class="toggle-container" id="status">
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
  </Teleport>
</template>
