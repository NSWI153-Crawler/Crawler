<script setup lang="ts">
import { watch, computed, onMounted, ref } from 'vue'
import { useWebsiteRecordStore } from '@/stores/records'
import WebsiteRecordFormComponent from '@/components/WebsiteRecordFormComponent.vue'

const store = useWebsiteRecordStore()
const records = computed(() => store.records)

const formComponent: any = ref(null)
const showCreateForm = () => {
  formComponent.value.showCreationForm()
}

const showEditForm = (
  id: string,
  label: string,
  url: string,
  regexp: string,
  tags: string[],
  periodicity: number,
  status: boolean
) => {
  formComponent.value.showEditForm(id, label, url, regexp, tags, periodicity, status)
}

const currentPage = ref(1)
const pageSize = ref(8)
const filter = ref({ url: '', label: '', tag: '' })
const sort = ref('url')

const totalPages = computed(() => Math.ceil(filteredRecords.value.length / pageSize.value))

function sortRecords() {
  records.value.sort((a, b) => {
    if (sort.value === 'url') {
      return a.url.localeCompare(b.url)
    } else if (sort.value === 'label') {
      return a.label.localeCompare(b.label)
    } else if (sort.value === 'periodicity') {
      return a.periodicity - b.periodicity
    } else if (sort.value === 'lastCrawled') {
      if (!a.lastExecutionTime || !b.lastExecutionTime) return 0
      return new Date(b.lastExecutionTime).getTime() - new Date(a.lastExecutionTime).getTime()
    }
    return 0
  })
}

const filteredRecords = computed(() => {
  return records.value.filter((record) => {
    const urlFilter = filter.value.url ? record.url.includes(filter.value.url) : true
    const labelFilter = filter.value.label ? record.label.includes(filter.value.label) : true
    const tagsFilter =
      filter.value.tag.length > 0 ? record.tags.some((tag) => tag.includes(filter.value.tag)) : true
    return urlFilter && labelFilter && tagsFilter
  })
})

const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredRecords.value.slice(start, end)
})

const formatDate = (dateString: Date | null | undefined) => {
  if (!dateString) return ''
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'numeric',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
    second: '2-digit'
  }
  return dateString.toLocaleString('cs-CZ', options)
}

watch(() => sort.value, sortRecords)

onMounted(() => {
  store.fetchRecords()
})
</script>

<template>
  <div class="min-w-wmin">
    <h1 class="text-4xl font-bold text-center mt-8">Website Records</h1>
    <div class="flex justify-center my-4">
      <button
        class="px-4 py-2 select-none cursor-pointer border-2 border-dark-bg dark:border-dark-fg rounded bg-[#ff0] dark:bg-[#550] dark:hover:bg-[#990] text-black dark:text-white dark:font-bold hover:bg-[#fd0] font-bold"
        @click="showCreateForm()"
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
    <div class="flex flex-wrap justify-center">
      <div v-for="record in paginatedRecords" :key="record.id" class="flex justify-center px-4">
        <div
          class="px-4 pb-4 bg-white dark:bg-dark-bg dark:text-dark-fg border-2 border-dark-bg dark:border-dark-fg rounded shadow my-4"
        >
          <h2 class="py-1 text-xl font-bold text-center my-2">{{ record.label }}</h2>
          <div class="flex justify-center space-x-4 mb-2">
            <button
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-orange-500 text-black dark:bg-[#a30] dark:text-white dark:font-bold dark:hover:bg-[#e70] hover:bg-[#fd9a66]"
              @click="
                showEditForm(
                  record.id!,
                  record.label,
                  record.url,
                  record.regexp,
                  record.tags,
                  record.periodicity,
                  record.isActive
                )
              "
            >
              Edit
            </button>
            <button
              v-if="!record.addedToGraph"
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-green-400 dark:bg-[#161] dark:hover:bg-[#1a1] text-black dark:text-white dark:font-bold hover:bg-green-300"
              @click="record.addedToGraph = !record.addedToGraph"
            >
              Add to Graph
            </button>
            <button
              v-else
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#f00] dark:text-white dark:bg-[#a00] dark:hover:bg-[#c00] dark:font-bold hover:bg-[#f50]"
              @click="record.addedToGraph = !record.addedToGraph"
            >
              Remove from Graph
            </button>
            <button
              class="button-style border-2 border-dark-bg dark:border-dark-fg rounded bg-[#d4f] dark:bg-[#51a] dark:hover:bg-[#72e] dark:text-white dark:font-bold hover:bg-[#e6f]"
              @click="store.runExecution(record.id!)"
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
            <strong>Tags:</strong> {{ record.tags.join(', ') ?? "" }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Periodicity: </strong>Every {{ record.periodicity }} minutes
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Last Execution Time:</strong> {{ formatDate(record.lastExecutionTime) }}
          </p>
          <p class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <strong>Last Execution Status:</strong> {{ record.lastExecutionStatus }}
          </p>
          <div class="text-gray-700 dark:bg-dark-bg dark:text-dark-fg">
            <div class="flex space-x-1 pr-4">
              <strong class="dark:text-dark-fg">Status:</strong>
              <div>
                {{ record.isActive ? 'active' : 'inactive' }}
              </div>
              <div
                class="flex justify-center content-center align-middle w-4 h-4 border-none rounded-[50%] bg-white dark:bg-dark-bg mt-[6px] overflow-auto"
              >
                <span
                  class="rounded-full m-auto"
                  :class="[record.isActive ? 'shadow-green-light' : 'shadow-red-light']"
                />
              </div>
            </div>
          </div>

          <div class="flex justify-center mt-6">
            <button
              class="button-style border-2 border-dark-bg rounded bg-red-700 text-white dark:border-dark-fg hover:bg-red-600 font-bold"
              @click="store.deleteRecord(record.id!)"
            >
              Delete Record
            </button>
          </div>
        </div>
      </div>
    </div>

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
    <WebsiteRecordFormComponent ref="formComponent" />
  </div>
</template>
