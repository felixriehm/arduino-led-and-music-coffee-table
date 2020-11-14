import React from 'react';
import cx from "classnames";
import { connect } from "react-redux";
import * as ConnectionController from '../Controller/connectionController';

const ConnecttionButton = ( {connectionStatus , serviceUuid ,characteristicUuid} ) => {
    const isConncting = connectionStatus.isConnecting;
    const isDisconnected = connectionStatus.isDisconnected;
    const isConnected = connectionStatus.isConnected;
    const isSearching = connectionStatus.isSearching;

    return <button type="button" className={cx("btn",
    isDisconnected && "btn-success",
    isConnected && "btn-danger",
    (isConncting || isSearching) && "btn-warning")} 
    disabled={isConncting || isSearching} onClick={
      isDisconnected ? () => ConnectionController.connect(serviceUuid, characteristicUuid) : () => ConnectionController.disconnect()
      }>
        <span className="connection-btn-icon"></span>
        {(isSearching || isConncting) && <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>}
        {isDisconnected && "Connect"}
        {isConnected && "Disconnect"}
        {isSearching && "Searching..."}
        {isConncting && "Connecting..."}
        </button>
}

const mapStateToProps = (state) => {
  return {
      serviceUuid: state.connection.serviceUuid,
      characteristicUuid: state.connection.characteristicUuid,
   };
};

export default connect(
  mapStateToProps,
  null
)(ConnecttionButton);