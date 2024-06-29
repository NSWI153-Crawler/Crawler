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
  lastExecutionTime?: string
  lastExecutionStatus?: string
}

const serverUrl: string = /*process.env.VUE_APP_SERVER_URL ||*/ 'http://localhost:3600'

export const useWebsiteRecordStore = defineStore('websiteRecord', () => {
  // State
  const records = ref<WebsiteRecord[]>([])
  const totalRecords = ref(0)

  // Actions
  const fetchRecords = async () => {
    const response = await fetch(`${serverUrl}/api/WebsiteRecord`)
    const data = await response.json()
    records.value = data.records
    totalRecords.value = data.total
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
      body: JSON.stringify(record)
    })
    const data = await response.json()
    records.value.push(data)
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
    const updatedRecord = {
      // ...record,
      url,
      regexp,
      periodicity,
      label,
      isActive,
      tags
    }
    const response = await fetch(`${serverUrl}/api/WebsiteRecord/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedRecord)
    })
    const data = await response.json()
    const index = records.value.findIndex((record) => record.id === id)
    if (index !== -1) {
      records.value[index] = data
    }
  }

  const deleteRecord = async (id: string) => {
    await fetch(`${serverUrl}/api/WebsiteRecord/${id}`, {
      method: 'DELETE'
    })
    records.value = records.value.filter((record) => record.id !== id)
  }

  return {
    records,
    totalRecords,
    fetchRecords,
    createRecord,
    updateRecord,
    deleteRecord
  }
})
