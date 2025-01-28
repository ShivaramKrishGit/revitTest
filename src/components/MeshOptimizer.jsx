import React, { useState, useCallback } from 'react';
import { Upload, Settings2, RefreshCw } from 'lucide-react';

const Alert = ({ title, children, variant = 'default' }) => (
  <div className={`p-4 mb-4 rounded-lg ${variant === 'destructive' ? 'bg-red-100 text-red-700' : 'bg-blue-100 text-blue-700'}`}>
    {title && <h5 className="font-bold mb-1">{title}</h5>}
    <p>{children}</p>
  </div>
);

const MeshOptimizer = () => {
  const [file, setFile] = useState(null);
  const [reductionPercent, setReductionPercent] = useState(50);
  const [quality, setQuality] = useState(0.8);
  const [isProcessing, setIsProcessing] = useState(false);
  const [stats, setStats] = useState(null);
  const [error, setError] = useState(null);

  const handleFileDrop = useCallback((e) => {
    e.preventDefault();
    const droppedFile = e.dataTransfer?.files[0];
    if (droppedFile && (droppedFile.name.endsWith('.obj') || droppedFile.name.endsWith('.stl'))) {
      setFile(droppedFile);
      setError(null);
    } else {
      setError('Please upload .obj or .stl files only');
    }
  }, []);

  const handleFileSelect = useCallback((e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile && (selectedFile.name.endsWith('.obj') || selectedFile.name.endsWith('.stl'))) {
      setFile(selectedFile);
      setError(null);
    } else {
      setError('Please upload .obj or .stl files only');
    }
  }, []);

  const handleOptimize = useCallback(() => {
    setIsProcessing(true);
    // Simulating processing - in real implementation, this would call your backend
    setTimeout(() => {
      setStats({
        originalTriangles: 50000,
        optimizedTriangles: Math.round(50000 * (1 - reductionPercent / 100)),
        reductionAchieved: reductionPercent,
        fileSize: '2.5 MB'
      });
      setIsProcessing(false);
    }, 2000);
  }, [reductionPercent]);

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">3D Mesh Optimizer</h1>
        
        {/* Upload Area */}
        <div 
          className="border-2 border-dashed border-gray-300 rounded-lg p-8 mb-8 text-center"
          onDragOver={(e) => e.preventDefault()}
          onDrop={handleFileDrop}
        >
          {!file ? (
            <div>
              <Upload className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-600 mb-2">Drag and drop your 3D model here</p>
              <p className="text-gray-400 mb-4">Supports .obj and .stl files</p>
              <label className="bg-blue-500 text-white px-4 py-2 rounded-md cursor-pointer hover:bg-blue-600">
                Browse Files
                <input 
                  type="file" 
                  className="hidden" 
                  accept=".obj,.stl"
                  onChange={handleFileSelect}
                />
              </label>
            </div>
          ) : (
            <div>
              <p className="text-gray-600 mb-2">✓ {file.name} uploaded</p>
              <button 
                className="text-red-500 hover:text-red-600"
                onClick={() => setFile(null)}
              >
                Remove
              </button>
            </div>
          )}
        </div>

        {error && (
          <Alert title="Error" variant="destructive">
            {error}
          </Alert>
        )}

        {/* Optimization Settings */}
        {file && (
          <div className="bg-white rounded-lg shadow p-6 mb-8">
            <h2 className="text-xl font-semibold mb-4 flex items-center">
              <Settings2 className="w-5 h-5 mr-2" />
              Optimization Settings
            </h2>
            
            <div className="mb-4">
              <label className="block text-gray-700 mb-2">
                Triangle Reduction ({reductionPercent}%)
              </label>
              <input
                type="range"
                min="0"
                max="90"
                value={reductionPercent}
                onChange={(e) => setReductionPercent(Number(e.target.value))}
                className="w-full"
              />
            </div>

            <div className="mb-6">
              <label className="block text-gray-700 mb-2">
                Quality Threshold ({quality})
              </label>
              <input
                type="range"
                min="0"
                max="1"
                step="0.1"
                value={quality}
                onChange={(e) => setQuality(Number(e.target.value))}
                className="w-full"
              />
            </div>

            <button
              className="bg-green-500 text-white px-6 py-2 rounded-md hover:bg-green-600 flex items-center justify-center w-full"
              onClick={handleOptimize}
              disabled={isProcessing}
            >
              {isProcessing ? (
                <RefreshCw className="w-5 h-5 animate-spin mr-2" />
              ) : null}
              {isProcessing ? 'Optimizing...' : 'Optimize Model'}
            </button>
          </div>
        )}

        {/* Results */}
        {stats && (
          <div className="bg-white rounded-lg shadow p-6">
            <h2 className="text-xl font-semibold mb-4">Results</h2>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <p className="text-gray-600">Original Triangles</p>
                <p className="text-2xl font-bold">{stats.originalTriangles.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-gray-600">Optimized Triangles</p>
                <p className="text-2xl font-bold">{stats.optimizedTriangles.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-gray-600">Reduction Achieved</p>
                <p className="text-2xl font-bold">{stats.reductionAchieved}%</p>
              </div>
              <div>
                <p className="text-gray-600">File Size</p>
                <p className="text-2xl font-bold">{stats.fileSize}</p>
              </div>
            </div>
            <button className="mt-6 bg-blue-500 text-white px-6 py-2 rounded-md hover:bg-blue-600 w-full">
              Download Optimized Model
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default MeshOptimizer;