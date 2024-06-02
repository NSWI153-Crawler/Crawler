import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

interface WebsiteRecord {
  id: string
  url: string
  regexp: string
  periodicity: number
  label: string
  isActive: boolean
  tags: string[]
  lastExecutionTime: string
  lastExecutionStatus: string
}

interface ExecutionRecord {
  id: string
  title: string
  crawlTime: string
  links?: string[]
  owner?: WebsiteRecord
}

interface FilterCriteria {
  url?: string
  label?: string
  tags?: string[]
}

export const useWebsiteRecordStore = defineStore('websiteRecord', () => {
  // State
  const records = ref<WebsiteRecord[]>([])
  const totalRecords = ref(0)

  // Actions
  const fetchRecords = async (
    page = 1,
    pageSize = 10,
    filter: FilterCriteria = {},
    sort = 'url'
  ) => {
    const response = await fetch(
      `/api/records?page=${page}&pageSize=${pageSize}&filter=${JSON.stringify(filter)}&sort=${sort}`
    )
    const data = await response.json()
    records.value = data.records
    totalRecords.value = data.total
  }

  const addRecord = async (record: WebsiteRecord) => {
    const response = await fetch('/api/records', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(record)
    })
    const data = await response.json()
    records.value.push(data)
  }

  const updateRecord = async (updatedRecord: WebsiteRecord) => {
    const response = await fetch(`/api/records/${updatedRecord.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedRecord)
    })
    const data = await response.json()
    const index = records.value.findIndex((record) => record.id === updatedRecord.id)
    if (index !== -1) {
      records.value[index] = data
    }
  }

  const deleteRecord = async (id: string) => {
    await fetch(`/api/records/${id}`, {
      method: 'DELETE'
    })
    records.value = records.value.filter((record) => record.id !== id)
  }

  return {
    records,
    totalRecords,
    fetchRecords,
    addRecord,
    updateRecord,
    deleteRecord
  }
})
