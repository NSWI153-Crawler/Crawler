<template>
  <div class="bg-white dark:bg-dark-bg">
    <div class="min-w-[500px] select-none cursor-pointer" @click="isExpanded = !isExpanded">
      <button
        class="absolute right-8 mt-[9px] bg-[url('/arrow_down.png')] bg-cover bg-center bg-no-repeat h-4 w-8 dark:invert"
        :class="[isExpanded ? 'transform rotate-180' : '']"
      />
      <h1 class="text-2xl text-center dark:text-dark-fg">Graph</h1>
    </div>
    <div v-if="isExpanded">
      <hr class="mt-2 border-dark-bg dark:border-dark-fg" />
      <!-- bar holding top buttons -->
      <div class="w-1/2 min-w-[500px] mx-auto my-4">
        <!-- view buttons container-->
        <div class="pl-4 float-left">
          <div class="toggle-container">
            <label
              :class="[
                viewToggle === viewDomains ? 'bg-yellow-400 text-black' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleView(viewDomains)"
            >
              Domain View
            </label>
            <label
              :class="[
                viewToggle === viewWebsites ? 'bg-blue-600 text-white' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleView(viewWebsites)"
            >
              Website View
            </label>
          </div>
        </div>
        <!-- mode buttons container -->
        <div class="flex space-x-1 pr-4 justify-end">
          <div class="p-1 select-none cursor-default dark:text-dark-fg">Mode:</div>
          <div class="toggle-container">
            <label
              :class="[
                modeToggle === modeStatic ? 'bg-orange-500 text-black' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleMode(modeStatic)"
            >
              Static
            </label>
            <label
              :class="[
                modeToggle === modeLive ? 'bg-green-500 text-black' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleMode(modeLive)"
            >
              Live
            </label>
          </div>
        </div>
      </div>

      <!-- graph container -->
      <div
        class="w-1/2 min-w-[500px] bg-white border border-black rounded-lg overflow-hidden relative mx-auto h-[500px]"
      >
        <v-network-graph
          class="graph"
          ref="graph"
          :nodes="nodes"
          :edges="edges"
          :layouts="layouts"
          :configs="configs"
          :eventHandlers="eventHandlers"
        />
        <div
          ref="tooltip"
          class="t-0 l-0 opacity-0 absolute p-2 grid place-content-center text-xs bg-gray-200 border border-dark-bg rounded-xl shadow"
          :class="{ 'pointer-events-none': tooltipOpacity === 0 }"
          :style="{ ...tooltipPos, opacity: tooltipOpacity }"
        >
          <div v-if="nodes[targetNodeId]?.crawled">
            <p><b>Title:</b> {{ nodes[targetNodeId]?.title ?? "" }}</p>
            <p><b>Url:</b> {{ nodes[targetNodeId]?.name ?? "" }}</p>
            <p><b>Crawled at:</b> {{ nodes[targetNodeId]?.crawledTime ?? "unknown" }}</p>
            <p title="Click on website record to start new execution"><b>Crawled by:</b>    ⓘ</p>
            <ul class="list-disc list-inside">
<!--              TODO crawl records on click; find record id by name in store?-->
              <li v-for="record in nodes[targetNodeId]?.websiteRecords ?? []" :key="record">{{ record }}</li>
            </ul>
          </div>
          <div v-else>
            <i>Not crawled due to boundary RegExp</i>
            <p><b>Url:</b> {{ nodes[targetNodeId]?.name ?? "" }}</p>
            <div class="flex justify-center my-1">
              <button
                class="p-1 mx-auto select-none cursor-pointer border-2 border-dark-bg bg-green-500 rounded"
                @click="showCreateForm(nodes[targetNodeId]?.name)"
              >
                Create Website Record
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- bar holding bottom buttons -->
      <div class="w-1/2 min-w-[500px] mx-auto my-4">
        <!-- layout buttons container -->
        <div class="flex space-x-1 pr-4 justify-center">
          <div class="p-1 select-none cursor-default dark:text-dark-fg">Layout:</div>
          <div class="toggle-container">
            <label
              :class="[
                layoutToggle === layoutLR ? 'bg-[#0ff] text-black' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleLayout(layoutLR)"
            >
              Left to Right
            </label>
            <label
              :class="[
                layoutToggle === layoutTB ? 'bg-[#0ff] text-black' : 'disabled-state',
                'button-style'
              ]"
              @click="toggleLayout(layoutTB)"
            >
              Top to Bottom
            </label>
          </div>
        </div>
      </div>
    </div>
    <WebsiteRecordFormComponent ref="formComponent" />
  </div>
</template>

<script setup lang="ts">
import * as vNG from 'v-network-graph'
import { computed, nextTick, watch, onMounted, onUnmounted, reactive, ref, defineComponent } from 'vue'
import { useWebsiteRecordStore } from '@/stores/records'
import dagre from '@dagrejs/dagre'
import WebsiteRecordFormComponent from '@/components/WebsiteRecordFormComponent.vue'

const isExpanded = ref(false)
const formComponent: any = ref(null)
const graph = ref<vNG.VNetworkGraphInstance>()
const store = useWebsiteRecordStore()

defineComponent({
  name: 'VisualizationComponent'
})

const showCreateForm = (url:string) => {
  formComponent.value.showCreationForm(url)
  tooltipOpacity.value = 0
}

interface Node extends vNG.Node {
  name: string
  crawled: boolean
  title?: string
  crawledTime?: string
  websiteRecords?: string[]
}
type NodePlaceHolder = [string, boolean]

interface Edge extends vNG.Edge {
  color?: string
}
type EdgePlaceHolder = [string, string]

const nodes: Record<string, Node> = reactive({
  node1: { name: 'example.com', crawled: true, title: 'example.com', crawledTime: '2021-10-02T13:00:00Z', websiteRecords: ['Record', 'Example'] },
  node2: { name: 'http://example.com/home', crawled: true, title:'Example', crawledTime: '2021-10-02T13:00:00Z', websiteRecords: ['Record', 'Example'] },
  node3: { name: 'https://exam.com/info', crawled: true, title:'Exam', crawledTime: '2021-10-02T13:00:00Z', websiteRecords: ['Record', 'Example'] },
  node4: { name: 'https://www.exampl.com/about', crawled: false },
  node5: { name: 'example.com/contact', crawled: false }
})

const edges: Record<string, Edge> = reactive({
  edge1: { source: 'node1', target: 'node2' },
  edge2: { source: 'node1', target: 'node3' },
  edge3: { source: 'node1', target: 'node4' },
  edge4: { source: 'node3', target: 'node5' },
  edge5: { source: 'node3', target: 'node4' },
  edge6: { source: 'node4', target: 'node3' }
})

const configs = reactive(
  vNG.defineConfigs<Node, Edge>({
    node: {
      normal: { color: (node) => (node.crawled ? '#000' : '#999') },
      hover: { color: (node) => (node.crawled ? '#000' : '#999') },
      label: { visible: (node) => !!node.name, color: (node) => (node.crawled ? '#000' : '#666') },
      focusring: { visible: false },
      selectable: true
    },

    edge: {
      normal: { color: '#999' },
      hover: { color: '#999' },
      selectable: false,
      gap: 5,
      marker: {
        target: {
          type: 'arrow',
          width: 4,
          height: 4
        }
      }
    }
  })
)

const layouts: vNG.Layouts = reactive({
  nodes: {}
})

const eventHandlers: vNG.EventHandlers = {
  'node:dblclick': ({ node }) => {
    if (viewToggle.value !== viewWebsites)
      return
    targetNodeId.value = node
    tooltipOpacity.value = 1
    nextTick(() => {
      adjustTooltipPosition()
    })
  },
  'node:click': () => {
    if (viewToggle.value === viewWebsites && tooltipOpacity.value === 1)
      tooltipOpacity.value = 0
  },
  'view:zoom': () => {
    adjustTooltipPosition()
  }
}

const tooltip = ref<HTMLDivElement>()
const targetNodeId = ref<string>("")
const tooltipOpacity = ref<number>(0)
const tooltipPos = ref({ left: "px", top: "0px" })

const targetNodePos = computed(() => {
  const nodePos = layouts.nodes[targetNodeId.value]
  return nodePos || { x: 0, y: 0 }
})

function adjustTooltipPosition() {
  if (!graph.value || !tooltip.value) return

  const domPoint = graph.value.translateFromSvgToDomCoordinates(targetNodePos.value)
  tooltipPos.value = {
    left: domPoint.x - tooltip.value.offsetWidth / 2 + "px",
    top: domPoint.y - tooltip.value.offsetHeight - 30 + "px",
  }
}

const checkIfClickedOutside = (event: MouseEvent) => {
  // Hides tooltip if clicked outside
  if (!tooltip.value || !tooltip.value.contains(event.target as HTMLElement)) {
    tooltipOpacity.value = 0
  }
}

let websiteNodes: NodePlaceHolder[] = []
let websiteEdges: EdgePlaceHolder[] = []
let domainNodes: NodePlaceHolder[]
let domainEdges: EdgePlaceHolder[]

for (const [_, node] of Object.entries(nodes)) {
  websiteNodes.push([node.name, node.crawled])
}
for (const [_, edge] of Object.entries(edges)) {
  websiteEdges.push([nodes[edge.source].name, nodes[edge.target].name])
}
domainNodes = convertWebsiteNodesToDomainNodes(websiteNodes)
domainEdges = convertWebsiteEdgesToDomainEdges(websiteEdges)

const modeStatic = 'static'
const modeLive = 'live'
const modeToggle = ref(modeStatic)

const layoutLR = 'LR'
const layoutTB = 'TB'
const layoutToggle = ref(layoutLR)

const viewDomains = 'domain'
const viewWebsites = 'website'
const viewToggle = ref(viewWebsites)

const nodeSize = 40

function toggleMode(mode: 'static' | 'live') {
  if (mode === modeToggle.value) return
  modeToggle.value = mode
  if (mode === modeStatic) {
    //TODO
  } else if (mode === modeLive) {
    //TODO
  }
}

function toggleLayout(direction: 'LR' | 'TB') {
  updateLayout(direction)
  if (direction !== layoutToggle.value) layoutToggle.value = direction
}

function toggleView(viewType: 'domain' | 'website') {
  if (viewType === viewToggle.value) return
  viewToggle.value = viewType
  if (viewType === 'domain') changeView(domainNodes, domainEdges)
  if (viewType === 'website') changeView(websiteNodes, websiteEdges)
  layout(layoutToggle.value === layoutLR ? 'LR' : 'TB')
}

function updateLayout(direction: 'TB' | 'LR') {
  // Animates the movement of an element.
  graph.value?.transitionWhile(() => {
    layout(direction)
  })
}

function layout(direction: 'TB' | 'LR') {
  if (Object.keys(nodes).length <= 1 || Object.keys(edges).length == 0) {
    return
  }

  // convert graph
  // ref: https://github.com/dagrejs/dagre/wiki
  const g = new dagre.graphlib.Graph()
  // Set an object for the graph label
  g.setGraph({
    rankdir: direction,
    nodesep: nodeSize * 2,
    edgesep: nodeSize,
    ranksep: nodeSize * 2
  })
  // Default to assigning a new object as a label for each new edge.
  g.setDefaultEdgeLabel(() => ({}))

  // Add nodes to the graph. The first argument is the node id. The second is
  // metadata about the node. In this case we're going to add labels to each of
  // our nodes.
  Object.entries(nodes).forEach(([nodeId, node]) => {
    g.setNode(nodeId, { label: node.name, width: nodeSize, height: nodeSize })
  })

  // Add edges to the graph.
  Object.values(edges).forEach((edge) => {
    g.setEdge(edge.source, edge.target)
  })

  dagre.layout(g)

  g.nodes().forEach((nodeId: string) => {
    // update node position
    const x = g.node(nodeId).x
    const y = g.node(nodeId).y
    layouts.nodes[nodeId] = { x, y }
  })
}

function changeView(Nodes: NodePlaceHolder[], Edges: EdgePlaceHolder[]) {
  for (const [nodeID, _] of Object.entries(nodes)) {
    delete nodes[nodeID]
  }
  for (const [edgeID, _] of Object.entries(edges)) {
    delete edges[edgeID]
  }
  for (const node of Nodes) {
    const nodeName = `node${Object.keys(nodes).length + 1}`
    nodes[nodeName] = {
      name: node[0],
      crawled: node[1]
    }
  }
  for (const edge of Edges) {
    const edgeName = `edge${Object.keys(edges).length + 1}`
    edges[edgeName] = {
      source: `node${Object.keys(nodes).findIndex((nodeID) => nodes[nodeID].name === edge[0]) + 1}`,
      target: `node${Object.keys(nodes).findIndex((nodeID) => nodes[nodeID].name === edge[1]) + 1}`
    }
  }
}

function convertWebsiteNodesToDomainNodes(websiteNodes: NodePlaceHolder[]): NodePlaceHolder[] {
  const domainNodes: NodePlaceHolder[] = []
  for (const [_, node] of Object.entries(websiteNodes)) {
    const domain = extractDomain(node[0])
    if (!domain) {
      continue
    }
    if (!domainNodes.some(([domainName, _]) => domainName === domain)) {
      domainNodes.push([domain, node[1]])
    } else {
      const index = domainNodes.findIndex(([domainName, _]) => domainName === domain)
      domainNodes[index][1] = domainNodes[index][1] || node[1]
    }
  }
  return domainNodes
}

function convertWebsiteEdgesToDomainEdges(websiteEdges: EdgePlaceHolder[]): EdgePlaceHolder[] {
  const domainEdges: EdgePlaceHolder[] = []
  for (const [_, edge] of Object.entries(websiteEdges)) {
    const sourceDomain = extractDomain(edge[0]) ?? ''
    const targetDomain = extractDomain(edge[1]) ?? ''
    if (sourceDomain !== targetDomain) {
      domainEdges.push([sourceDomain, targetDomain])
    }
  }
  return domainEdges
}

function extractDomain(url: string): string | null {
  try {
    const matches = url.match(/^(https?:\/\/)?(www.)?([^/?#]+)(?:[/?#]|$)/i)
    return matches && matches[3]
  } catch (error) {
    console.error('Invalid URL:', error)
    return null
  }
}

onMounted(() => {
  layout('LR')
  window.addEventListener('click', checkIfClickedOutside)
})

onUnmounted(() => {
  window.removeEventListener('click', checkIfClickedOutside)
})

watch(
  () => [targetNodePos.value, tooltipOpacity.value],
  () => {
    adjustTooltipPosition()
  },
  { deep: true }
)
</script>
