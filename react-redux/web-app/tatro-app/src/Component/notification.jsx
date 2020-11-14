import React from 'react';
import cx from "classnames";
import { connect } from "react-redux";
import * as ConnectionController from '../Controller/connectionController';
import { CONNECTION_STATUS } from '../Utility/constants';


const NotificationBar = ( {connectionStatus} ) => {
      const isDisconnected = connectionStatus.isDisconnected;
      const isConnected = connectionStatus.isConnected;

    return <div className="alert alert-info alert-dismissible" style={{display: 'none', position: 'absolute', bottom: '0'}} role="alert">
        {isDisconnected && 'Your are <strong>disconnected!</strong> You can use no functionalities now.'}
        {isConnected && '<strong>Connecting</strong> to bluetooth device.'}
        <button type="button" className="close" data-dismiss="alert" aria-label="Close">
        <span aria-hidden="true">&times;</span>
        </button>
    </div>
  }

  export default NotificationBar;