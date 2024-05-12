<script setup lang="ts">
import * as vNG from "v-network-graph"

const nodes = {
  node1: { name: "example.com" },
  node2: { name: "example.com/home" },
  node3: { name: "example.com/info" },
  node4: { name: "example.com/about" },
  node5: { name: "example.com/contact" },
}

const edges = {
  edge1: { source: "node1", target: "node2" },
  edge2: { source: "node1", target: "node3" },
  edge3: { source: "node1", target: "node4" },
  edge4: { source: "node3", target: "node5" },
  edge5: { source: "node3", target: "node4" },
  edge6: { source: "node4", target: "node3" }
}

const layouts = {
  nodes: {
    node1: { x: 0, y: 100 },
    node2: { x: 100, y: 0 },
    node3: { x: 100, y: 200 },
    node4: { x: 100, y: 100 },
    node5: { x: 200, y: 200 },
  },
}

const configs = vNG.defineConfigs({
  node: {
    selectable: true,
  },
  edge: {
    selectable: false,
    normal: { color: "#5555dd" },
    hover: { color: "#dd5555" },
    selected: { color: "#dddd55" },
    gap: 10,
  },
})

/**
 * Make `transform` value of the object placing on the edge.
 * @param center - center position
 * @param edgePos - edge source and target positions
 * @param scale - zooming scale
 */
function makeTransform(center: vNG.Point, edgePos: vNG.EdgePosition, scale: number) {
  const radian = Math.atan2(
    edgePos.target.y - edgePos.source.y,
    edgePos.target.x - edgePos.source.x
  )
  const degree = (radian * 180.0) / Math.PI

  return [
    `translate(${center.x} ${center.y})`,
    `scale(${scale}, ${scale})`,
    `rotate(${degree})`,
  ].join(" ")
}

</script>

<template>
  <div class="graph">

    <v-network-graph
      :nodes="nodes"
      :edges="edges"
      :layouts="layouts"
      :configs="configs"
    >
      <!-- To use CSS within SVG, <defs> is used -->
      <defs>
        <!-- Cannot use <style> directly due to restrictions of Vue. -->
        <component is="style">
          <!-- prettier-ignore -->
          .marker {
          fill: {{ configs.edge.normal.color }};
          transition: fill 0.1s linear;
          pointer-events: none;
          }
          .marker.hovered { fill: {{ configs.edge.hover.color }}; }
          .marker.selected { fill: {{ configs.edge.selected.color }}; }
        </component>
      </defs>
      <template #edge-overlay="{ scale, center, position, hovered, selected }">
        <!-- Place the triangle at the center of the edge -->
        <path
          class="marker"
          :class="{ hovered, selected }"
          d="M-5 -5 L5 0 L-5 5 Z"
          :transform="makeTransform(center, position, scale)"
        />
      </template>
    </v-network-graph>
  </div>
</template>

<style>
.graph {
  width: 50%;
  height: 50vh;
  border: 1px solid #000;
  overflow: hidden;
  position: relative;
  margin: 0 auto;
}
</style>
