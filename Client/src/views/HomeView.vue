<template>

  <div class="p-6">
    <div class="flex flex-wrap">
      <ExecutionListComponent class="border-2 border-dark-bg dark:border-dark-fg px-6 py-2 rounded-2xl xl:ml-auto xl:mr-5 mx-auto mb-6" />
      <visualization-component class="border-2 border-dark-bg dark:border-dark-fg px-6 py-2 rounded-2xl xl:mr-auto xl:ml-5 mx-auto mb-6" />
    </div>
    <WebsiteRecordListComponentVue class="mb-4 p-4 bg-white dark:bg-[#0A1124] dark:text-white shadow rounded flex-grow" />

  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, defineComponent } from 'vue'
import { useWebsiteRecordStore } from '../stores/records'
import WebsiteRecordListComponentVue from '../components/WebsiteRecordListComponent.vue'
import ExecutionListComponent from '../components/ExecutionListComponent.vue'
import VisualizationComponent from '../components/VisualizationComponent.vue'

defineComponent({
  components: {
    WebsiteRecordListComponentVue,
    ExecutionListComponent,
    VisualizationComponent,
  },
})

// const store = useWebsiteRecordStore()

// const fetchRecords = async () => {
//   await store.fetchRecords(page.value, pageSize.value, filter.value, sort.value)
// }

// onMounted(fetchRecords)

// const records = computed(() => store.records)

</script>
