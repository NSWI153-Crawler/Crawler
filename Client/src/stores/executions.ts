import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import type { WebsiteRecord } from './records'
import { useWebsiteRecordStore } from '@/stores/records'

interface Execution {
  id: string
  recordId: string
  recordLabel: string
  executionTime: Date
  executionStatus?: string | null
  sitesCrawled: number
}

const serverUrl: string = import.meta.env.VITE_SERVER_URL || 'http://localhost:8080'

function transformExecutionFromData(data: any) {
  return {
    id: data.id,
    recordId: data.websiteRecord.id,
    recordLabel: data.websiteRecord.label,
    executionTime: new Date(data.startTime),
    executionStatus:
      data.status === 0
        ? 'Success'
        : data.status === 1
          ? 'Failure'
          : 'In Progress',
    sitesCrawled: data.sitesCrawled
  }
}

export const useExecutionStore = defineStore('execution', () => {
  // State
  const executions = ref<Execution[]>([])
  const websiteRecordStore = useWebsiteRecordStore()
  const websiteRecords = computed(() => websiteRecordStore.records)

  const fetchExecutions = async () => {
    const response = await fetch(`${serverUrl}/api/Execution`)
    const data = await response.json()
    executions.value = data.map((execution: any) => {
      return transformExecutionFromData(execution)
    })
  }

  let continueFetching: boolean

  const periodicalFetchExecutions = async () => {
    continueFetching = true
    while (continueFetching) {
      await fetchExecutions()
      for (const execution of executions.value) {
        const record = websiteRecords.value.find((r: WebsiteRecord) => r.id === execution.recordId)
        if (
          record &&
          (!record.lastExecutionTime || record.lastExecutionTime < execution.executionTime)
        ) {
          // Update the last execution time and status of the record
          websiteRecordStore.changeExecutionTimeStatus(
            record.id!,
            execution.executionTime,
            execution.executionStatus ?? ''
          )
        }
      }
      await new Promise((resolve) => setTimeout(resolve, 5000))
    }
  }

  const stopPeriodicalFetchExecutions = () => {
    continueFetching = false
  }

  // Getters
  const getExecutions = computed(() => executions.value)

  const getUniqueLabels = computed(() => {
    return websiteRecordStore.getLabels()
  })

  return {
    fetchExecutions,
    periodicalFetchExecutions,
    stopPeriodicalFetchExecutions,
    getExecutions,
    getUniqueLabels,
    executions
  }
})
