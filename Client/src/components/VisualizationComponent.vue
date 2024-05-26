<template>
  <div>

  <h1 class="text-3xl font-bold text-center">Graph</h1>
  <div class="w-1/2 min-w-[500px] mx-auto my-4">    <!-- bar holding top buttons -->

    <div class="pl-4 float-left">                   <!-- view buttons container-->
      <div class="toggle-container">
        <label
          :class="[
            viewToggle === viewDomains
              ? 'bg-yellow-400 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="toggleView(viewDomains)"
        >
          Domain View
        </label>
        <label
          :class="[
            viewToggle === viewWebsites
              ? 'bg-blue-600 text-white'
              : 'disabled-state',
            'button-style',
          ]"
          @click="toggleView(viewWebsites)"
        >
          Website View
        </label>
      </div>
    </div>

    <div class="flex space-x-1 pr-4 justify-end">   <!-- mode buttons container -->
      <div class="p-1 select-none cursor-default">Mode:</div>
      <div class="toggle-container">
        <label
          :class="[
            modeToggle === modeStatic
              ? 'bg-orange-500 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="toggleMode(modeStatic)"
        >
          Static
        </label>
        <label
          :class="[
            modeToggle === modeLive
              ? 'bg-green-500 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="toggleMode(modeLive)"
        >
          Live
        </label>
      </div>
    </div>
  </div>

  <div class="w-1/2 min-w-[500px] border border-black rounded-lg overflow-hidden relative mx-auto h-[500px]"> <!-- graph container -->
    <v-network-graph class="graph"
                     ref="graph"
                     :nodes="nodes"
                     :edges="edges"
                     :layouts="layouts"
                     :configs="configs"
                     :eventHandlers="eventHandlers"
    />
  </div>

  <div class="w-1/2 min-w-[500px] mx-auto my-4">   <!-- bar holding bottom buttons -->
    <div class="flex space-x-1 pr-4 justify-center">    <!-- layout buttons container -->
      <div class="p-1 select-none cursor-default">Layout:</div>
      <div class="toggle-container">
        <label
          :class="[
              layoutToggle === layoutLR
                ? 'bg-[#0ff] text-black'
                : 'disabled-state',
              'button-style',
            ]"
          @click="toggleLayout(layoutLR)"
        >
          Left to Right
        </label>
        <label
          :class="[
              layoutToggle === layoutTB
                ? 'bg-[#0ff] text-black'
                : 'disabled-state',
              'button-style',
            ]"
          @click="toggleLayout(layoutTB)"
        >
          Top to Bottom
        </label>
      </div>
    </div>
  </div>
  </div>
</template>

<script setup lang="ts">
import * as vNG from "v-network-graph"
import { computed, onMounted, reactive, ref, defineComponent } from "vue"
import { useWebsiteRecordStore } from '../stores/records'
import { useRoute } from 'vue-router'
import dagre from "@dagrejs/dagre"

defineComponent({
  name: "VisualizationComponent",
})

const graph = ref<vNG.VNetworkGraphInstance>()
const route = useRoute()
const store = useWebsiteRecordStore()
let id = ""

onMounted(() => {
  id = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id
  // store.fetchRecordById(id)
  layout("LR")
})
// const record = computed(() => store.getRecordById(id)) TODO
const record = { // crawled = matches regexp
  id: "1",
  url: "http://example.com",
  regexp: ".*",
  periodicity: 24,
  label: "Record Label",
  isActive: true,
  tags: ["example", "com"],
  lastExecutionTime: "2021-09-01T00:00:00Z",
  lastExecutionStatus: "Success",
}

interface Node extends vNG.Node {
  name: string
  crawled: boolean
}
type NodePlaceHolder = [string, boolean];

interface Edge extends vNG.Edge {
  color?: string
}
type EdgePlaceHolder = [string, string];

const nodes: Record<string, Node> = reactive({
  node1: { name: "example.com", crawled: true },
  node2: { name: "http://example.com/home", crawled: true },
  node3: { name: "https://exam.com/info", crawled: true },
  node4: { name: "https://www.exampl.com/about", crawled: false },
  node5: { name: "example.com/contact", crawled: false },
})

const edges: Record<string, Edge> = reactive({
  edge1: { source: "node1", target: "node2", },
  edge2: { source: "node1", target: "node3", },
  edge3: { source: "node1", target: "node4", },
  edge4: { source: "node3", target: "node5", },
  edge5: { source: "node3", target: "node4", },
  edge6: { source: "node4", target: "node3", },
})

const configs = reactive(vNG.defineConfigs<Node, Edge>({
    node: {
      normal: { color: (node) => (node.crawled ? "#000" : "#999"), },
      hover:  { color: (node) => (node.crawled ? "#000" : "#999"), },
      label:  { visible: node => !!node.name,
                color: (node) => (node.crawled ? "#000" : "#666"),
      },
      focusring: { visible: false, },
      selectable: true,
    },

    edge: {
      normal: { color: "#999", },
      hover:  { color: "#999", },
      selectable: false,
      gap: 5,
      marker: {
        target: {
          type: "arrow",
          width:  4,
          height: 4,
        },
      },
    },
  })
)

const layouts: vNG.Layouts = reactive({
  nodes: {},
})




const eventHandlers: vNG.EventHandlers = {
  "node:dblclick": ({ node, event }) => {
    if (viewToggle.value !== viewWebsites)
      return
    if (nodes[node].crawled) {
      console.log("open node detail: URL, Crawl Time, list of website record that crawled this node");
    }
    else {
      console.log("open node detail: URL");
    }
  },
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

const modeStatic = "static"
const modeLive = "live"
const modeToggle = ref(modeStatic)

const layoutLR = "LR"
const layoutTB = "TB"
const layoutToggle = ref(layoutLR)

const viewDomains = "domain"
const viewWebsites = "website"
const viewToggle = ref(viewWebsites)

const nodeSize = 40

function toggleMode(mode: "static" | "live") {
  if (mode === modeToggle.value)
    return
  modeToggle.value = mode
  if (mode === modeStatic) {
    //TODO
  }
}

function toggleLayout(direction: "LR" | "TB"){
  updateLayout(direction)
  if (direction !== layoutToggle.value)
    layoutToggle.value = direction
}

function toggleView(viewType: 'domain' | 'website') {
  if (viewType === viewToggle.value)
    return
  viewToggle.value = viewType
  if (viewType === 'domain')
    changeView(domainNodes, domainEdges)
  if (viewType === 'website')
    changeView(websiteNodes, websiteEdges)
  layout(layoutToggle.value === layoutLR ? "LR" : "TB")
}

function updateLayout(direction: "TB" | "LR") {
  // Animates the movement of an element.
  graph.value?.transitionWhile(() => {
    layout(direction)
  })
}

function layout(direction: "TB" | "LR") {
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
    ranksep: nodeSize * 2,
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
  Object.values(edges).forEach(edge => {
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
    const sourceDomain = extractDomain(edge[0]) ?? ""
    const targetDomain = extractDomain(edge[1]) ?? ""
    if (sourceDomain !== targetDomain) {
      domainEdges.push([sourceDomain, targetDomain])
    }
  }
  return domainEdges
}

function extractDomain(url: string): string | null {
  try {
    const matches = url.match(/^(https?:\/\/)?(www.)?([^/?#]+)(?:[/?#]|$)/i);
    const domain = matches && matches[3];
    return domain;
  } catch (error) {
    console.error('Invalid URL:', error);
    return null;
  }
}
</script>