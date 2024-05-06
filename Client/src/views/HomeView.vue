<template>
  <div class="p-6">
    <div class="mb-4 p-4 bg-white shadow rounded">
      <div class="grid grid-cols-1 gap-4">
        <div>
          <label for="filter" class="block text-gray-700 text-sm font-bold mb-2">Filter by:</label>
          <input
            id="filter"
            v-model="filter.url"
            placeholder="URL"
            @input="fetchRecords"
            class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500"
          />
          <input
            id="filter"
            v-model="filter.label"
            placeholder="Label"
            @input="fetchRecords"
            class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline mt-2 hover:border-blue-500"
          />
          <input
            id="filter"
            v-model="filter.tags"
            placeholder="Tags"
            @input="fetchRecords"
            class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline mt-2 hover:border-blue-500"
          />
        </div>

        <div>
          <label for="sort" class="block text-gray-700 text-sm font-bold mb-2">Sort by:</label>
          <select
            id="sort"
            v-model="sort"
            @change="fetchRecords"
            class="form-select block w-full mt-1 shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline hover:border-blue-500"
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
    </div>
    <div v-for="record in records" :key="record.id" class="mb-4 p-4 bg-white rounded shadow">
      <h2 class="text-xl font-bold mb-2">{{ record.label }}</h2>
      <p class="text-gray-700"><strong>URL:</strong> {{ record.url }}</p>
      <p class="text-gray-700"><strong>RegExp:</strong> {{ record.regexp }}</p>
      <p class="text-gray-700"><strong>Periodicity:</strong> {{ record.periodicity }}</p>
      <p class="text-gray-700"><strong>Tags:</strong> {{ record.tags.join(', ') }}</p>
      <p class="text-gray-700">
        <strong>Last Execution Time:</strong> {{ record.lastExecutionTime }}
      </p>
      <p class="text-gray-700">
        <strong>Last Execution Status:</strong> {{ record.lastExecutionStatus }}
      </p>
      <p class="text-gray-700">
        <strong>Status:</strong> {{ record.isActive ? 'Active' : 'Inactive' }}
      </p>
    </div>

    <div class="flex justify-center space-x-4">
  <button
    @click="prevPage"
    :disabled="page === 1"
    class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
  >
    Previous
  </button>
  <button
    @click="nextPage"
    :disabled="page === totalPages"
    class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
  >
    Next
  </button>
</div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useWebsiteRecordStore } from '../stores/records'

const store = useWebsiteRecordStore()
const page = ref(1)
const pageSize = ref(10)
const filter = ref({ url: '', label: '', tags: [] })
const sort = ref('url')

const fetchRecords = async () => {
  await store.fetchRecords(page.value, pageSize.value, filter.value, sort.value)
}

onMounted(fetchRecords)

const records = computed(() => store.records)
const totalPages = computed(() => Math.ceil(store.totalRecords / pageSize.value))

const nextPage = () => {
  if (page.value < totalPages.value) {
    page.value++
    fetchRecords()
  }
}

const prevPage = () => {
  if (page.value > 1) {
    page.value--
    fetchRecords()
  }
}
</script>
