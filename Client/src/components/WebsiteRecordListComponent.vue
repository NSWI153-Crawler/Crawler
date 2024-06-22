<template>
  <div>
    <div :class="[showForm ? 'block' : 'hidden']">
      <!-- the veil -->
      <!-- prettier-ignore-attribute -->
      <div
        class="fixed top-0 left-0 w-full h-full z-50 bg-[rgba(0,0,0,0.5)]"
        @click="clearForm(); showForm = false"
      />
      <div
        class="bg-gray-200 dark:bg-dark-bg border-2 border-dark-bg dark:border-dark-fg rounded-2xl py-4 px-8 z-50 fixed top-1/2 left-1/2 translate-x-[-50%] translate-y-[-50%]"
      >
        <!-- THIS IS NOT THE YELLOW BUTTON -->
        <h1 v-if="formFields.creation" class="py-2 text-xl font-bold text-center mb-4">
          Create New Record
        </h1>
        <h1 v-else class="py-2 text-xl font-bold text-center mb-4">Edit Record</h1>
        <form @submit.prevent="handleSubmit" class="space-y-2">
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
              title="You can choose to add your own tags for the record"
              >Tags:</label
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
              class="w-[100px] float-left"
              title="How often is record executed"
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
            <!-- prettier-ignore-attribute -->
            <button
              type="button"
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#f00] hover:bg-[#e00] font-bold"
              @click="
                clearForm();
                showForm = false
              "
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

    <h1 class="text-4xl font-bold text-center mt-8">Website Records</h1>
    <div class="flex justify-center my-4">
      <!-- prettier-ignore-attribute -->
      <button
        class="px-4 py-2 select-none cursor-pointer border-2 border-dark-bg dark:border-dark-fg rounded bg-[#ff0] dark:bg-[#550] dark:hover:bg-[#990] text-black dark:text-white dark:font-bold hover:bg-[#fd0] font-bold"
        @click="
            clearForm();
            showForm = true
          "
      >
        Create New Website Record
      </button>
    </div>
    <div class="flex flex-wrap justify-center">
      <div class="p-2 space-y-2 grid grid-cols-1 w-[300px] text-center">
        <label for="filter" class="block text-gray-700 dark:text-white text-sm font-bold"
          >Filter by:</label
        >
        <input
          id="filter"
          v-model="filter.label"
          placeholder="Label"
          class="shadow appearance-none border rounded py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline mt-2 hover:border-blue-500"
        />
        <input
          id="filter"
          v-model="filter.url"
          placeholder="URL"
          class="shadow appearance-none border rounded py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500"
        />
        <input
          id="filter"
          v-model="filter.tag"
          placeholder="Tag"
          class="shadow appearance-none border rounded py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline mb-2 hover:border-blue-500"
        />
      </div>

      <div class="p-2 space-y-2 grid grid-cols-1 w-[300px] text-center self-start">
        <label for="sort" class="block text-gray-700 dark:text-white text-sm font-bold"
          >Sort by:</label
        >

        <select
          id="sort"
          v-model="sort"
          class="form-select mt-1 shadow border rounded py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500 w-full"
        >
          <option value="url">URL</option>
          <option value="label">Label</option>
          <option value="periodicity">Periodicity</option>
          <option value="lastCrawled">Last Crawled</option>
        </select>
      </div>
    </div>
    <!--        <hr class="border border-dark-bg dark:border-dark-fg z-0 mt-10" />-->
    <div class="flex justify-center space-x-4 mt-4" v-if="paginatedRecords.length > 0">
      <div class="bg-white dark:bg-dark-bg">
        <button
          class="button-style border border-black rounded-lg mx-2"
          :class="[
            currentPage === 1
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage > 1 && currentPage--"
        >
          Prev
        </button>
        <span class="py-1 dark:text-dark-fg mx-4">Page {{ currentPage }} of {{ totalPages }}</span>
        <button
          class="button-style border border-black rounded-lg mx-2"
          :class="[
            currentPage === Math.ceil(filteredRecords.length / pageSize) || records.length == 0
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage < Math.ceil(filteredRecords.length / pageSize) && currentPage++"
        >
          Next
        </button>
      </div>
    </div>
    <div v-else-if="records.length > 0">
      <p class="text-center m-6 font-bold text-xl">No records match the filter criteria</p>
    </div>
    <div v-else>
      <p class="text-center m-6 font-bold text-xl">There are no records yet</p>
    </div>
    <!--    <hr class="border border-dark-bg dark:border-dark-fg z-0 mt-4" />-->
    <div class="flex flex-wrap justify-center">
      <div v-for="record in paginatedRecords" :key="record.id" class="flex justify-center px-4">
        <div
          class="px-4 pb-4 bg-white dark:bg-dark-bg dark:text-dark-fg border-2 border-dark-bg dark:border-dark-fg rounded shadow my-4"
        >
          <h2 class="py-1 text-xl font-bold text-center my-2">{{ record.label }}</h2>
          <div class="flex justify-center space-x-4 mb-2">
            <!-- prettier-ignore-attribute -->
            <button
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-orange-500 text-black dark:bg-[#a30] dark:text-white dark:font-bold dark:hover:bg-[#e70] hover:bg-[#fd9a66]"
              @click="
                formFields.creation = false;
                formFields.label = record.label;
                formFields.url = record.url;
                formFields.regexp = record.regexp;
                formFields.tags = record.tags.join(', ');
                formFields.periodicity = record.periodicity;
                formFields.status = record.isActive;
                showForm = true
              "
            >
              Edit
            </button>
            <!-- prettier-ignore-attribute -->
            <button
              v-if="!record.addedToGraph"
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-green-400 dark:bg-[#161] dark:hover:bg-[#1a1] text-black dark:text-white dark:font-bold hover:bg-green-300"
              @click="record.addedToGraph = !record.addedToGraph"
            >
              Add to Graph
            </button>
            <!-- prettier-ignore-attribute -->
            <button
              v-else
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#f00] dark:text-white dark:bg-[#a00] dark:hover:bg-[#c00] dark:font-bold hover:bg-[#f50]"
              @click="record.addedToGraph = !record.addedToGraph"
            >
              Remove from Graph
            </button>
            <button
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#d4f] dark:bg-[#51a] dark:hover:bg-[#72e] dark:text-white dark:font-bold hover:bg-[#e6f]"
            >
              Crawl
            </button>
          </div>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>URL:</strong> {{ record.url }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>RegExp:</strong> {{ record.regexp }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Tags:</strong> {{ record.tags.join(', ') }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Periodicity:</strong> {{ record.periodicity }} a day
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Last Execution Time:</strong> {{ record.lastExecutionTime }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Last Execution Status:</strong> {{ record.lastExecutionStatus }}
          </p>
          <div class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <div class="flex space-x-1 pr-4">
              <strong class="dark:text-dark-fg">Status:</strong>
              <div @click="activateRecord(record.id, !record.isActive)">
                <a class="cursor-pointer">{{ record.isActive ? 'active' : 'inactive' }}</a>
              </div>
              <button
                class="flex justify-center content-center align-middle w-4 h-4 border-none rounded-[50%] bg-white dark:bg-dark-bg mt-[6px] overflow-auto"
                @click="activateRecord(record.id, !record.isActive)"
              >
                <span
                  class="rounded-full m-auto"
                  :class="[record.isActive ? 'shadow-green-light' : 'shadow-red-light']"
                />
              </button>
            </div>
          </div>

          <div class="flex justify-center mt-6">
            <button
              class="button-style border-2 border-dark-bg rounded bg-red-700 text-white dark:border-dark-fg hover:bg-red-600 font-bold"
              @click="deleteRecord(record.id)"
            >
              Delete Record
            </button>
          </div>
        </div>
      </div>
    </div>

    <!--    <hr class="border border-dark-bg dark:border-dark-fg z-0 mt-6" />-->
    <div class="flex justify-center space-x-4 mb-4" v-if="paginatedRecords.length > 0">
      <div class="bg-white dark:bg-dark-bg">
        <button
          class="button-style border border-black rounded-lg mx-2"
          :class="[
            currentPage === 1
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage > 1 && currentPage--"
        >
          Prev
        </button>
        <span class="py-1 dark:text-dark-fg mx-4">Page {{ currentPage }} of {{ totalPages }}</span>
        <button
          class="button-style border border-black rounded-lg mx-2"
          :class="[
            currentPage === Math.ceil(filteredRecords.length / pageSize) || records.length == 0
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage < Math.ceil(filteredRecords.length / pageSize) && currentPage++"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch, reactive, computed, onMounted, ref, defineComponent } from 'vue'
import { useWebsiteRecordStore } from '@/stores/records'

const formFields = reactive({
  creation: true,
  label: '',
  url: '',
  regexp: '',
  tags: '',
  periodicity: 0,
  status: true
})

function clearForm() {
  formFields.creation = true
  formFields.label = ''
  formFields.url = ''
  formFields.regexp = ''
  formFields.tags = ''
  formFields.periodicity = 0
  formFields.status = true
}

const handleSubmit = () => {
  // Handle form submission
}

const store = useWebsiteRecordStore()
const currentPage = ref(1)
const pageSize = ref(8)
const filter = ref({ url: '', label: '', tag: '' })
const sort = ref('url')

const fetchRecords = async () => {
  await store.fetchRecords(currentPage.value, pageSize.value, filter.value, sort.value)
}

async function activateRecord(id: number, isActive: boolean) {
  await new Promise((resolve) => setTimeout(resolve, 1000))
  if (!records.find((record) => record.id === id)) return
  records.find((record) => record.id === id)!.isActive = isActive
}

//onMounted(fetchRecords)

//const records = computed(() => store.records)
const records = reactive([
  {
    id: 1,
    url: 'https://www.google.com',
    label: 'Google',
    tags: ['search', 'engine'],
    regexp: '.*',
    periodicity: 1,
    lastExecutionTime: '2021-10-02T13:00:00Z',
    lastExecutionStatus: 'unknown',
    isActive: true,
    addedToGraph: false
  },
  // TODO: communication with server
])

const showForm = ref(false)

async function deleteRecord(id: number) {
  await store.deleteRecord(id.toString())
  records.splice(
    records.findIndex((record) => record.id === id),
    1
  )
}

const totalPages = computed(() => Math.ceil(filteredRecords.value.length / pageSize.value))

function sortRecords() {
  records.sort((a, b) => {
    if (sort.value === 'url') {
      return a.url.localeCompare(b.url)
    } else if (sort.value === 'label') {
      return a.label.localeCompare(b.label)
    } else if (sort.value === 'periodicity') {
      return a.periodicity - b.periodicity
    } else if (sort.value === 'lastCrawled') {
      return a.lastExecutionTime.localeCompare(b.lastExecutionTime)
    }
    return 0
  })
}

const filteredRecords = computed(() => {
  return records.filter((record) => {
    const urlFilter = filter.value.url ? record.url.includes(filter.value.url) : true
    const labelFilter = filter.value.label ? record.label.includes(filter.value.label) : true
    const tagsFilter =
      filter.value.tag.length > 0
        ? record.tags.some((tag) => tag.includes(filter.value.tag))
        : true
    return urlFilter && labelFilter && tagsFilter
  })
})

const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredRecords.value.slice(start, end)
})

watch(() => sort.value, sortRecords);
</script>
