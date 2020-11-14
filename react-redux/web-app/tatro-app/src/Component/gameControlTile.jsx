import React from 'react';
import cx from "classnames";
import {lightOrDark} from '../Utility/colorConversion'

const gameControlTile = ({ connectionStatus, color, index, onGameControlClick, gameInput, classes }) => {
    /*const isDisconnected = connectionStatus.isDisconnected;
    const isConnected = connectionStatus.isConnected;*/

    return <div className={cx("tile game-control", classes)} style={{ background: `rgb(${color.r}, ${color.g}, ${color.b})` }} onClick={() => onGameControlClick(gameInput)}>
    </div>
}

export default gameControlTile;