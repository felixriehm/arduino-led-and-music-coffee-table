import React from 'react';
import cx from "classnames";
import {lightOrDark} from '../Utility/colorConversion'

const colorSelectionTile = ({ connectionStatus, color, index, onColorSelectionClick, colorSelection }) => {
    /*const isDisconnected = connectionStatus.isDisconnected;
    const isConnected = connectionStatus.isConnected;*/

    return <div className={cx("tile color-select-tile", colorSelection[index] && "color-selected", lightOrDark(color.rgb))} style={{ background: `rgb(${color.rgb.r}, ${color.rgb.g}, ${color.rgb.b})` }} onClick={() => onColorSelectionClick(index)}>
    </div>
}

export default colorSelectionTile;