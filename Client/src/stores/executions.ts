import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { WebsiteRecord } from './records'
import { useWebsiteRecordStore } from '@/stores/records'

interface Execution {
  id: string
  recordId: string
  recordLabel: string
  executionTime: string
  executionStatus?: string | null
  sitesCrawled: number
  checked: boolean
}

const serverUrl: string = import.meta.env.VITE_SERVER_URL || 'http://localhost:8080'

function transformExecutionFromData(data: any): Execution {
  return {
    id: data.id,
    recordId: data.websiteRecord.id,
    recordLabel: data.websiteRecord.label,
    executionTime: data.startTime,
    executionStatus:
      data.executionStatus === 0
        ? 'Success'
        : data.executionStatus === 1
          ? 'Failure'
          : 'In Progress',
    sitesCrawled: data.sitesCrawled,
    checked: true
  }
}

export const useExecutionStore = defineStore('execution', () => {
  // State
  const executions = ref<Execution[]>([])
  const websiteRecordStore = useWebsiteRecordStore()
  const websiteRecords = computed(() => websiteRecordStore.records)

  // Actions
  const fetchExecutions = async () => {
    const response = await fetch(`${serverUrl}/api/Execution`)
    const data = await response.json()
    const newExecutions: Array<Execution> = data.map((execution: any) =>
      transformExecutionFromData(execution)
    )
    executions.value = newExecutions
  }

  let continueFetching

  const periodicalFetchExecutions = async () => {
    continueFetching = true
    while (continueFetching) {
      const response = await fetch(`${serverUrl}/api/Execution`)
      const data = await response.json()
      const newExecutions = data.map((execution: any) => transformExecutionFromData(execution))

      for (const execution of newExecutions) {
        if (!executions.value.find((e: Execution) => e.id === execution.id)) {
          // If the execution is newly created
          const record = websiteRecords.value.find(
            (r: WebsiteRecord) => r.id === execution.recordId
          )
          if (
            record &&
            (!record.lastExecutionTime || record.lastExecutionTime < execution.executionTime)
          ) {
            // Update the last execution time and status of the record
            websiteRecordStore.changeExecutionTimeStatus(
              record.id!,
              execution.executionTime,
              execution.executionStatus
            )
          }
        }
      }
      executions.value = newExecutions
      await new Promise((resolve) => setTimeout(resolve, 5000))
    }
  }

  const stopPeriodicalFetchExecutions = () => {
    continueFetching = false
  }

  // Getters
  const getExecutions = computed(() => executions.value)

  return {
    fetchExecutions,
    periodicalFetchExecutions,
    stopPeriodicalFetchExecutions,
    getExecutions,
    executions
  }
})
