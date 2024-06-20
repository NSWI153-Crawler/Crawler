<template>
  <div class="execution-list bg-white dark:bg-dark-bg" ref="executionList">
    <div class="min-w-[500px] select-none cursor-pointer" @click="isExpanded = !isExpanded">
      <button
        class="absolute right-8 mt-[9px] bg-[url('static/arrow_down.png')] bg-cover bg-center bg-no-repeat h-4 w-8 dark:invert"
        :class="[isExpanded ? 'transform rotate-180' : '']"
      />
      <h1 class="text-2xl text-center dark:text-dark-fg">Executions Log</h1>
    </div>
    <div v-if="isExpanded">
      <hr class="mt-2 border-dark-bg dark:border-dark-fg" />
      <div id="dropdown" class="w-64 relative mx-auto my-4">
        <div @click="showCheckboxes" class="cursor-pointer relative">
          <select class="w-full font-bold rounded p-1">
            <option class="">Filter by Website Records</option>
          </select>
          <div class="absolute inset-0" />
        </div>

        <div
          id="checkboxes"
          class="hidden absolute bg-white z-10 border-2 border-dark-bg w-64 overflow-auto rounded shadow max-h-60"
        >
          <label for="all" class="block cursor-pointer select-none hover:bg-[#1e90ff]">
            <input class="ml-2" type="checkbox" id="all" v-model="allChecked" /> Select all
          </label>
          <span v-for="execution in uniqueExecutions" :key="execution.id">
            <label
              class="block cursor-pointer select-none hover:bg-[#1e90ff]"
              :for="`execution-${execution.id}`"
            >
              <input
                class="ml-2"
                type="checkbox"
                :id="`execution-${execution.id}`"
                v-model="execution.checked"
              />
              {{ execution.label }}
            </label>
          </span>
        </div>
      </div>

      <div class="min-w-[500px] h-[500px]">
        <table class="table-auto w-full dark:text-dark-fg">
          <thead class="text-left">
            <tr>
              <th class="px-2 button-style" @click="sortTable('label')">
                Label
                <span v-if="sortColumn === 'label'">
                  <span v-if="sortOrder === 'asc'">↑</span>
                  <span v-else>↓</span>
                </span>
                <span v-else>→</span>
              </th>

              <th class="px-2 button-style" @click="sortTable('time')">
                Time
                <span v-if="sortColumn === 'time'">
                  <span v-if="sortOrder === 'asc'">↑</span>
                  <span v-else>↓</span>
                </span>
                <span v-else>→</span>
              </th>

              <th class="px-2 button-style" @click="sortTable('numberOfSitesCrawled')">
                Sites Crawled
                <span v-if="sortColumn === 'numberOfSitesCrawled'">
                  <span v-if="sortOrder === 'asc'">↑</span>
                  <span v-else>↓</span>
                </span>
                <span v-else>→</span>
              </th>

              <th class="px-2 button-style" @click="sortTable('status')">
                Status
                <span v-if="sortColumn === 'status'">
                  <span v-if="sortOrder === 'asc'">↑</span>
                  <span v-else>↓</span>
                </span>
                <span v-else>→</span>
              </th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="execution in paginatedExecutions" :key="execution.id">
              <td class="border border-dark-bg dark:border-dark-fg px-2 py-2">
                {{ execution.label }}
              </td>
              <td class="border border-dark-bg dark:border-dark-fg px-2 py-2">
                {{ formatDate(execution.time) }}
              </td>
              <td class="border border-dark-bg dark:border-dark-fg px-2 py-2 text-center">
                {{ execution.numberOfSitesCrawled }}
              </td>
              <td
                class="border border-dark-bg px-2 py-2 text-center text-dark-bg dark:border-dark-fg"
                :class="
                  execution.status === 'success'
                    ? 'bg-green-400 dark:bg-[#14e414]'
                    : 'bg-[#ff0000] dark:bg-[#900] dark:text-dark-fg dark:font-bold'
                "
              >
                {{ execution.status }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="flex justify-center space-x-4 my-4">
        <button
          class="button-style border border-black rounded-lg"
          :class="[
            currentPage === 1
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage > 1 && currentPage--"
        >
          Prev
        </button>
        <span class="py-1 dark:text-dark-fg"
          >Page {{ currentPage }} of
          {{
            Math.ceil(filteredExecutions.length / pageSize) == 0
              ? 1
              : Math.ceil(filteredExecutions.length / pageSize)
          }}</span
        >
        <button
          class="button-style border border-black rounded-lg"
          :class="[
            currentPage === Math.ceil(filteredExecutions.length / pageSize) ||
            filteredExecutions.length == 0
              ? 'disabled-state cursor-not-allowed'
              : 'bg-blue-500 hover:bg-blue-700 text-white font-bold'
          ]"
          @click="currentPage < Math.ceil(filteredExecutions.length / pageSize) && currentPage++"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch, onMounted, onUnmounted, computed, ref, defineComponent } from 'vue'
defineComponent({
  name: 'ExecutionListComponent'
})

const isExpanded = ref(false)

const executions = ref(
  [
    {
      id: 15,
      time: '2021-10-02T13:00:00Z',
      label: 'Record',
      numberOfSitesCrawled: 7,
      status: 'unknown'
    }
    // TODO: communication with server
  ].map((execution) => ({ ...execution, checked: true }))
)

const currentPage = ref(1)
const pageSize = ref(11)
const sortColumn = ref('time')
const sortOrder = ref('desc')

const uniqueExecutions = computed(() => {
  const uniqueLabels = new Set()
  return executions.value.filter((execution) => {
    if (!uniqueLabels.has(execution.label)) {
      uniqueLabels.add(execution.label)
      return true
    }
    return false
  })
})

const sortedExecutions = computed(() => {
  if (!sortColumn.value) return executions.value
  const sorted = [...executions.value]
  sorted.sort((a: any, b: any) => {
    if (a[sortColumn.value] < b[sortColumn.value]) return sortOrder.value === 'asc' ? -1 : 1
    if (a[sortColumn.value] > b[sortColumn.value]) return sortOrder.value === 'asc' ? 1 : -1
    return 0
  })
  return sorted
})

const filteredExecutions = computed(() => {
  const checkedLabels = new Set()
  uniqueExecutions.value.forEach((execution) => {
    if (execution.checked) {
      checkedLabels.add(execution.label)
    }
  })
  return sortedExecutions.value.filter((execution) => checkedLabels.has(execution.label))
})

const paginatedExecutions = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredExecutions.value.slice(start, end)
})

const sortTable = (column: string) => {
  if (sortColumn.value === column) sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  else {
    sortColumn.value = column
    sortOrder.value = 'asc'
  }
}

const formatDate = (dateString: string) => {
  const options: Intl.DateTimeFormatOptions = {
    month: 'numeric',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit'
  }
  return new Date(dateString).toLocaleString('cs-CZ', options)
}

const allChecked = computed({
  get: () => executions.value.every((execution) => execution.checked),
  set: (newValue) => executions.value.forEach((execution) => (execution.checked = newValue))
})

const filterBoxExpanded = ref(false)

const showCheckboxes = () => {
  const checkboxes = document.getElementById('checkboxes')
  if (!checkboxes) return
  if (!filterBoxExpanded.value) {
    checkboxes.style.display = 'block'
    filterBoxExpanded.value = true
  } else {
    checkboxes.style.display = 'none'
    filterBoxExpanded.value = false
  }
}

const hideCheckboxes = () => {
  const checkboxes = document.getElementById('checkboxes')
  if (!checkboxes) return
  checkboxes.style.display = 'none'
  filterBoxExpanded.value = false
}

const onClickOutside = (event: MouseEvent) => {
  const checkboxes = document.getElementById('checkboxes')
  const dropdown = document.getElementById('dropdown')
  if (dropdown && dropdown.contains(event.target as Node)) return
  if (checkboxes && !checkboxes.contains(event.target as Node)) hideCheckboxes()
}

const executionList = ref()

onMounted(() => {
  document.addEventListener('click', onClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', onClickOutside)
})

watch(filteredExecutions, () => {
  const totalPages = Math.ceil(filteredExecutions.value.length / pageSize.value)
  if (currentPage.value > totalPages) currentPage.value = totalPages
  if (currentPage.value === 0) currentPage.value = 1
})
</script>
