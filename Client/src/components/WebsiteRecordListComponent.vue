<template>
  <div>
    <div class="grid grid-cols-1 gap-4">
      <div>
        <label for="filter" class="block text-gray-700 dark:text-white text-sm font-bold mb-2">Filter by:</label>
        <input
          id="filter"
          v-model="filter.url"
          placeholder="URL"
          class="shadow appearance-none border rounded w-full py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500"
        />
        <input
          id="filter"
          v-model="filter.label"
          placeholder="Label"
          class="shadow appearance-none border rounded w-full py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline mt-2 hover:border-blue-500"
        />
        <input
          id="filter"
          v-model="filter.tags"
          placeholder="Tags"
          class="shadow appearance-none border rounded w-full py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline mt-2 hover:border-blue-500"
        />
      </div>

      <div>
        <label for="sort" class="block text-gray-700 dark:text-white text-sm font-bold mb-2">Sort by:</label>
        <select
          id="sort"
          v-model="sort"
          class="form-select block w-full mt-1 shadow appearance-none border rounded py-2 px-3 dark:bg-dark-fg text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500"
        >
          <option value="url">URL</option>
          <option value="label">Label</option>
          <option value="periodicity">Periodicity</option>
          <option value="tags">Tags</option>
          <option value="lastExecutionTime">Last Execution Time</option>
          <option value="lastCrawled">Last Crawled</option>
        </select>
      </div>
    </div>
    <button class="button-style border-2 border-dark-bg rounded-2xl">Create New Record</button>
    <div class="flex justify-center space-x-4">
    <button
      class="button-style border border-black rounded-lg"
      :class="[currentPage === 1 ? 'disabled-state cursor-not-allowed' : 'bg-blue-500 hover:bg-blue-700 text-white font-bold']"
      @click="currentPage > 1 && currentPage--"
    >
      Previous
    </button>
    <span class="py-1 dark:text-dark-fg">{{ currentPage }} / {{ totalPages }}</span>
    <button
      class="button-style border border-black rounded-lg"
      :class="[((currentPage === Math.ceil(records.length / pageSize)) || records.length == 0) ? 'disabled-state cursor-not-allowed' : 'bg-blue-500 hover:bg-blue-700 text-white font-bold']"
      @click="currentPage < Math.ceil(records.length / pageSize) && currentPage++"
    >
      Next
    </button>
    </div>



    <div v-for="record in paginatedRecords" :key="record.id" class="flex justify-center">
      <div class="px-4 pb-4 bg-white dark:bg-dark-bg dark:text-dark-fg border-2 border-dark-bg dark:border-dark-bg rounded shadow my-4">
        <h2 class="button-style text-xl font-bold text-center my-2">{{ record.label }}</h2>
        <div class="flex justify-center space-x-4 mb-2">
          <button class="button-style border-2 border-dark-bg rounded bg-orange-500 hover:bg-orange-400">Edit</button>
          <button class="button-style border-2 border-dark-bg rounded bg-green-400 hover:bg-green-300">Add to Graph</button>
          <button class="button-style font-bold border-2 border-dark-bg rounded bg-[#a2f] text-white hover:bg-[#b3f]">Crawl</button>
        </div>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg"><strong>URL:</strong> {{ record.url }}</p>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg"><strong>Tags:</strong> {{ record.tags.join(', ') }}</p>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg"><strong>RegExp:</strong> {{ record.regexp }}</p>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg"><strong>Periodicity:</strong> {{ record.periodicity }}</p>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
          <strong >Last Execution Time:</strong> {{ record.lastExecutionTime }}
        </p>
        <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
          <strong>Last Execution Status:</strong> {{ record.lastExecutionStatus }}
        </p>
        <div class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
          <div class="flex space-x-1 pr-4">
            <strong class="dark:text-dark-fg">Status:</strong>
            <div @click="activateRecord(record.id, !record.isActive)">
              <a :class="[
                  record.isActive ? '' : '',
                  'cursor-pointer',
                ]"
              >
                {{ record.isActive ? 'active' : 'inactive'}}
              </a>
            </div>
            <button
              class="flex justify-center content-center align-middle w-4 h-4 border-none rounded-[50%] bg-white mt-[6px] overflow-auto"
              @click="activateRecord(record.id, !record.isActive)"
            >
              <span
                class="rounded-full m-auto"
                :class="[
                  record.isActive ? 'shadow-green-light' : 'shadow-red-light',
                ]"
              />
            </button>
          </div>
        </div>

        <div class="flex justify-center mt-6">
          <button class="button-style border-2 border-dark-bg rounded bg-red-700 text-white hover:bg-red-600 font-bold">
            Delete Record
          </button>
        </div>
      </div>
    </div>




    <div class="flex justify-center space-x-4">
      <button
        class="button-style border border-black rounded-lg"
        :class="[currentPage === 1 ? 'disabled-state cursor-not-allowed' : 'bg-blue-500 hover:bg-blue-700 text-white font-bold']"
        @click="currentPage > 1 && currentPage--"
      >
        Previous
      </button>
      <span class="py-1 dark:text-dark-fg">{{ currentPage }} / {{ totalPages }}</span>
      <button
        class="button-style border border-black rounded-lg"
        :class="[((currentPage === Math.ceil(records.length / pageSize)) || records.length == 0) ? 'disabled-state cursor-not-allowed' : 'bg-blue-500 hover:bg-blue-700 text-white font-bold']"
        @click="currentPage < Math.ceil(records.length / pageSize) && currentPage++"
      >
        Next
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, onMounted, ref, defineComponent } from 'vue'
import { useWebsiteRecordStore } from '@/stores/records'

const store = useWebsiteRecordStore()
const currentPage = ref(1)
const pageSize = ref(4)
const filter = ref({ url: '', label: '', tags: [] as string[] })
const sort = ref('url')

const fetchRecords = async () => {
  await store.fetchRecords(currentPage.value, pageSize.value, filter.value, sort.value)
}

async function activateRecord(id: number, isActive: boolean) {
  await new Promise(resolve => setTimeout(resolve, 1000))
  if (!records.find(record => record.id === id)) return
  records.find(record => record.id === id)!.isActive = isActive
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
    periodicity: 'daily',
    lastExecutionTime: '2021-10-02T13:00:00Z',
    lastExecutionStatus: 'unknown',
    isActive: true,
  },
  {
    id: 2,
    url: 'https://www.bing.com',
    label: 'Bing',
    tags: ['search', 'engine'],
    regexp: '.*',
    periodicity: 'daily',
    lastExecutionTime: '2021-10-02T13:00:00Z',
    lastExecutionStatus: 'unknown',
    isActive: true,
  },
  {
    id: 3,
    url: 'https://www.yahoo.com',
    label: 'Yahoo',
    tags: ['search', 'engine'],
    regexp: '.*',
    periodicity: 'daily',
    lastExecutionTime: '2021-10-02T13:00:00Z',
    lastExecutionStatus: 'unknown',
    isActive: true,
  },
  {
    id: 4,
    url: 'https://www.duckduckgo.com',
    label: 'DuckDuckGo',
    tags: ['search', 'engine'],
    regexp: '.*',
    periodicity: 'daily',
    lastExecutionTime: '2021-10-02T13:00:00Z',
    lastExecutionStatus: 'unknown',
    isActive: true,
  },
  // TODO: communication with server
])
const totalPages = computed(() => Math.ceil(filteredRecords.value.length / pageSize.value))

const filteredRecords = computed(() => {
  return records.filter(record => {
    const urlFilter = filter.value.url ? record.url.includes(filter.value.url) : true;
    const labelFilter = filter.value.label ? record.label.includes(filter.value.label) : true;
    const tagsFilter = filter.value.tags.length > 0 ? record.tags.some(tag => filter.value.tags.includes(tag)) : true;
    return urlFilter && labelFilter && tagsFilter;
  })
})

const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredRecords.value.slice(start, end)
})
</script>