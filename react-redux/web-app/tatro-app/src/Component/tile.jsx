import React from 'react';


const tile = ({ connectionStatus, x, y, index, color, onTileClick }) => {
  /*const isDisconnected = connectionStatus.isDisconnected;
  const isConnected = connectionStatus.isConnected;*/

  return <div className="tile" style={{ background: `rgb(${color.rgb.r}, ${color.rgb.g}, ${color.rgb.b})` }} onClick={() => onTileClick(index)}>
  </div>
}

export default tile;