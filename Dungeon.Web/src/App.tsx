import { useState, useEffect } from 'react'
import './App.css'

interface DungeonMap {
  id: string
  name: string
  width: number
  height: number
  startPosition: { x: number; y: number }
  goalPosition: { x: number; y: number }
  obstacles: { x: number; y: number }[]
  createdAt: string
}

interface PathResponse {
  path: { x: number; y: number }[]
  distance: number
  pathFound: boolean
}

function App() {
  const [maps, setMaps] = useState<DungeonMap[]>([])
  const [selectedMap, setSelectedMap] = useState<DungeonMap | null>(null)
  const [path, setPath] = useState<PathResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [apiBase, setApiBase] = useState<string>('http://localhost:5000/api')

  useEffect(() => {
    const initializeApp = async () => {
      try {
        const config = await fetch("/config.json").then(r => r.json())
        setApiBase(config.apiBase)
      } catch (err) {
        console.error('Failed to load config, using default API base')
      }
      fetchMaps()
    }
    initializeApp()
  }, [])

  const fetchMaps = async () => {
    try {
      setLoading(true)
      const response = await fetch(`${apiBase}/maps`)
      if (response.ok) {
        const mapsData = await response.json()
        setMaps(mapsData)
      } else {
        setError('Failed to fetch maps')
      }
    } catch (err) {
      setError('Error connecting to API')
    } finally {
      setLoading(false)
    }
  }

  const fetchPath = async (mapId: string) => {
    try {
      setLoading(true)
      setError(null)
      const response = await fetch(`${apiBase}/maps/${mapId}/path`)
      if (response.ok) {
        const responseText = await response.text()
        if (responseText.trim() === '') {
          setError('Server returned empty response')
          return
        }
        try {
          const pathData = JSON.parse(responseText)
          setPath(pathData)
        } catch (jsonErr) {
          setError(`Invalid JSON response: ${responseText}`)
        }
      } else {
        const errorText = await response.text().catch(() => 'No response body')
        setError(`Failed to compute path (${response.status}): ${errorText}`)
      }
    } catch (err) {
      setError(`Error computing path: ${err}`)
    } finally {
      setLoading(false)
    }
  }

  const createSampleMap = async () => {
    const sampleMap = {
      name: 'Sample Dungeon',
      width: 10,
      height: 8,
      startPosition: { x: 0, y: 0 },
      goalPosition: { x: 9, y: 7 },
      obstacles: [
        { x: 2, y: 3 },
        { x: 5, y: 6 },
        { x: 4, y: 4 },
        { x: 7, y: 2 }
      ]
    }

    try {
      setLoading(true)
      const response = await fetch(`${apiBase}/maps`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(sampleMap)
      })

      if (response.ok) {
        fetchMaps() // Refresh the maps list
      } else {
        setError('Failed to create map')
      }
    } catch (err) {
      setError('Error creating map')
    } finally {
      setLoading(false)
    }
  }

  const renderGrid = (map: DungeonMap) => {
    const grid = []
    
    for (let y = 0; y < map.height; y++) {
      for (let x = 0; x < map.width; x++) {
        const isStart = x === map.startPosition.x && y === map.startPosition.y
        const isGoal = x === map.goalPosition.x && y === map.goalPosition.y
        const isObstacle = map.obstacles.some(obs => obs.x === x && obs.y === y)
        const isPath = path?.path.some(p => p.x === x && p.y === y) || false

        let cellClass = 'grid-cell'
        let cellContent = ''

        if (isStart) {
          cellClass += ' start'
          cellContent = 'S'
        } else if (isGoal) {
          cellClass += ' goal'
          cellContent = 'G'
        } else if (isObstacle) {
          cellClass += ' obstacle'
          cellContent = '█'
        } else if (isPath) {
          cellClass += ' path'
          cellContent = '•'
        }

        grid.push(
          <div
            key={`${x}-${y}`}
            className={cellClass}
            title={`(${x}, ${y})`}
          >
            {cellContent}
          </div>
        )
      }
    }

    return (
      <div 
        className="grid" 
        style={{
          gridTemplateColumns: `repeat(${map.width}, 1fr)`,
          gridTemplateRows: `repeat(${map.height}, 1fr)`
        }}
      >
        {grid}
      </div>
    )
  }

  return (
    <div className="App">
      <header className="App-header">
        <h1>🏰 Dungeon Pathfinder</h1>
        <p>Interactive dungeon map pathfinding visualization</p>
      </header>

      <main className="App-main">
        {error && (
          <div className="error-message">
            <strong>Error:</strong> {error}
            <button onClick={() => setError(null)}>✕</button>
          </div>
        )}

        <div className="controls">
          <button onClick={fetchMaps} disabled={loading}>
            {loading ? '⟳ Loading...' : '🔄 Refresh Maps'}
          </button>
          <button onClick={createSampleMap} disabled={loading}>
            ➕ Create Sample Map
          </button>
        </div>

        <div className="content">
          <div className="maps-section">
            <h2>Available Maps ({maps.length})</h2>
            {maps.length === 0 ? (
              <p className="no-maps">
                No maps available. Create a sample map to get started!
              </p>
            ) : (
              <div className="maps-list">
                {maps.map((map) => (
                  <div
                    key={map.id}
                    className={`map-card ${selectedMap?.id === map.id ? 'selected' : ''}`}
                    onClick={() => setSelectedMap(map)}
                  >
                    <h3>{map.name}</h3>
                    <div className="map-info">
                      <span>Size: {map.width}×{map.height}</span>
                      <span>Obstacles: {map.obstacles.length}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {selectedMap && (
            <div className="visualization-section">
              <div className="visualization-header">
                <h2>Map: {selectedMap.name}</h2>
                <button
                  onClick={() => fetchPath(selectedMap.id)}
                  disabled={loading}
                  className="find-path-btn"
                >
                  {loading ? '⟳ Computing...' : '🎯 Find Path'}
                </button>
              </div>
              <div className="legend">
                <span><span className="legend-start">S</span> Start</span>
                <span><span className="legend-goal">G</span> Goal</span>
                <span><span className="legend-obstacle">█</span> Obstacle</span>
                <span><span className="legend-path">•</span> Path</span>
              </div>
              {renderGrid(selectedMap)}
              
              {path && (
                <div className="path-info">
                  {path.pathFound ? (
                    <div className="path-found">
                      ✅ Path found! Distance: <strong>{path.distance}</strong> steps
                      <br />
                      Path length: <strong>{path.path.length}</strong> coordinates
                    </div>
                  ) : (
                    <div className="path-not-found">
                      ❌ No path found between start and goal positions
                    </div>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      </main>
    </div>
  )
}

export default App