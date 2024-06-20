<template>
<div class="xl:h-12 h-36" />
    <ExecutionListComponent class="absolute max-sm:left-1 max-sm:translate-x-0 xl:top-40 max-xl:top-[166px] xl:left-12 max-xl:left-1/2 max-xl:translate-x-[-50%] max-xl:z-50 border-2 border-dark-bg dark:border-dark-fg px-6 py-2 rounded-2xl xl:mr-auto xl:ml-5 mx-auto mb-6" />
    <visualization-component class="absolute max-sm:left-1 max-sm:translate-x-0 xl:top-40 xl:right-12 max-xl:left-1/2 max-xl:translate-x-[-50%] max-xl:top-60 border-2 border-dark-bg dark:border-dark-fg px-6 py-2 rounded-2xl xl:mr-auto xl:ml-5 mx-auto mb-6" />
  <div class="p-6 min-w-[660px]">
    <WebsiteRecordListComponent class="mb-4 p-4 bg-white dark:bg-dark-bg dark:text-white shadow rounded flex-grow" />

    </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, defineComponent } from 'vue'
import { useWebsiteRecordStore } from '../stores/records'
import WebsiteRecordListComponent from '../components/WebsiteRecordListComponent.vue'
import ExecutionListComponent from '../components/ExecutionListComponent.vue'
import VisualizationComponent from '../components/VisualizationComponent.vue'

defineComponent({
  components: {
    WebsiteRecordListComponent,
    ExecutionListComponent,
    VisualizationComponent,
  },
})

</script>
