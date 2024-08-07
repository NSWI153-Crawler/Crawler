import { ref } from 'vue'
import { defineStore } from 'pinia'

export interface WebsiteRecord {
  id?: string
  url: string
  regexp: string
  periodicity: number
  label: string
  isActive: boolean
  tags: string[]
  lastExecutionTime?: Date | null
  lastExecutionStatus?: string | null
  addedToGraph?: boolean
}

function transformWebsiteRecordFromData(data: any): WebsiteRecord {
  return {
    id: data.id,
    url: data.url,
    regexp: data.boundaryRegexp,
    periodicity: data.periodicity,
    label: data.label,
    isActive: data.state === 0,
    tags: data['tags'].map((tag: any) => tag.name).filter((name: string) => name),
    lastExecutionTime: new Date(data.lastExecution?.startTime),
    lastExecutionStatus:
      data.lastExecution?.status === 0
        ? 'Success'
        : data.lastExecution?.status === 1
          ? 'Failure'
          : data.lastExecution?.status === 2
            ? 'In Progress'
            : null,
    addedToGraph: false
  }
}

function transformWebsiteRecordToData(record: WebsiteRecord): any {
  return {
    url: record.url,
    boundaryRegexp: record.regexp,
    periodicity: record.periodicity,
    label: record.label,
    state: record.isActive ? 0 : 1,
    tags: record.tags.filter((name: string) => name).map((name) => ({ name }))
  }
}

const serverUrl: string = import.meta.env.VITE_SERVER_URL || 'http://localhost:8080'

export const useWebsiteRecordStore = defineStore('websiteRecord', () => {
  // State
  const records = ref<WebsiteRecord[]>([])

  // Actions
  const fetchRecords = async () => {
    const response = await fetch(`${serverUrl}/api/WebsiteRecord`)
    const data = await response.json()
    records.value = data.map((record: any) =>
      transformWebsiteRecordFromData(record)
    )
  }

  const createRecord = async (
    url: string,
    regexp: string,
    periodicity: number,
    label: string,
    isActive: boolean,
    tags: string[]
  ) => {
    const record = {
      url,
      regexp,
      periodicity,
      label,
      isActive,
      tags
    }
    const response = await fetch(`${serverUrl}/api/WebsiteRecord`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(transformWebsiteRecordToData(record))
    })
    const data = await response.json()
    const newRecord = transformWebsiteRecordFromData(data)
    records.value.push(newRecord)
  }

  const updateRecord = async (
    id: string,
    url: string,
    regexp: string,
    periodicity: number,
    label: string,
    isActive: boolean,
    tags: string[]
  ) => {
    if (!records.value.find((record) => record.id === id)) {
      return
    }
    const updatedRecord = transformWebsiteRecordToData({
      url,
      regexp,
      periodicity,
      label,
      isActive,
      tags
    })

    await fetch(`${serverUrl}/api/WebsiteRecord/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedRecord)
    })
    const index = records.value.findIndex((record) => record.id === id)
    if (index !== -1) {
      records.value[index].url = url
      records.value[index].regexp = regexp
      records.value[index].periodicity = periodicity
      records.value[index].label = label
      records.value[index].isActive = isActive
      records.value[index].tags = tags
    }
  }

  const deleteRecord = async (id: string) => {
    await fetch(`${serverUrl}/api/WebsiteRecord/${id}`, {
      method: 'DELETE'
    }).then(() => {
      records.value = records.value.filter((record) => record.id !== id)
    })
  }

  const runExecution = async (id: string) => {
    await fetch(`${serverUrl}/api/WebsiteRecord/${id}`, {
      method: 'POST'
    })
  }

  const changeExecutionTimeStatus = (id: string, executionTime: Date, executionStatus: string) => {
    const index = records.value.findIndex((record) => record.id === id)
    if (index !== -1) {
      records.value[index].lastExecutionTime = executionTime
      records.value[index].lastExecutionStatus = executionStatus
    }
  }

  // Getters
  const getLabels = () => {
    return records.value.map((record) => record.label)
  }

  return {
    fetchRecords,
    createRecord,
    updateRecord,
    deleteRecord,
    runExecution,
    changeExecutionTimeStatus,
    getLabels,
    records
  }
})
