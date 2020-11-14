import React, { useState } from 'react'
import cx from "classnames";
import ConnecttionButton from '../Component/connectionButton';
import { connect } from "react-redux";
import { changeServiceUuid, changeCharacteristicUuid, flushLogOutputText } from "../redux/actions";
import { bindActionCreators } from 'redux';
import * as ConnectionController from '../Controller/connectionController';

class Debug extends React.Component {
    constructor(props) {
        super(props);
        this.state = { dataToSend: '' };
        this.refToConsoleOutput = React.createRef();
    }

    hanleOnDataTextChange(data) {
        this.setState({ dataToSend: data })
    }

    componentDidUpdate() {
        let elem = this.refToConsoleOutput.current;
        elem.scrollTop = elem.scrollHeight;
    }

    render() {
        const isConncting = this.props.connectionStatus.isConnecting;
        const isDisconnected = this.props.connectionStatus.isDisconnected;
        const isConnected = this.props.connectionStatus.isConnected;
        const isSearching = this.props.connectionStatus.isSearching;

        const outputStlye = {
            backgroundColor: '#f0f0f0',
            borderRadius: '0.75em',
            display: 'block',
            margin: '0.5em',
            padding: '0.5em',
        };

        return <div className="debug">
            <h2>Bluetooth</h2>
            <h5>Connection <span className={cx("badge",
                isDisconnected && "badge-danger",
                isConnected && "badge-success",
                (isConncting || isSearching) && "badge-warning")}>
                {isDisconnected && "Disconnected"}
                {isConnected && "Connected"}
                {isSearching && "Searching"}
                {isConncting && "Connecting"}
            </span></h5>
            <div className="row">
                <div className="col-sm">
                    <label htmlFor="serviceUuid">Service UUID</label>
                    <input type="text" className="form-control" id="serviceUuid" value={this.props.serviceUuid} onChange={e => this.props.changeServiceUuid(e.currentTarget.value)}></input>
                    <small id="emailHelp" className="form-text text-muted">It must be a valid UUID alias (e.g. 0x1234)</small>
                </div>
                <div className="col-sm">
                    <label htmlFor="characteristicUuid">Characteristic UUID</label>
                    <input type="text" className="form-control" id="characteristicUuid" value={this.props.characteristicUuid} onChange={e => this.props.changeCharacteristicUuid(e.currentTarget.value)}></input>
                    <small id="emailHelp" className="form-text text-muted">It must be a valid UUID alias (e.g. 0x1234)</small>
                </div>
                <div className="col-sm">
                    <ConnecttionButton connectionStatus={this.props.connectionStatus} />
                </div>
            </div>
            <h5>Device information</h5>
            <div className="row">
                <div className="col-sm">
                    Name: {this.props.deviceName}
                </div>
                <div className="col-sm">
                    ID: {this.props.deviceId}
                </div>
            </div>
            <h5>Transmission</h5>
            <div className="row">
                <div className="col-sm">
                    <label htmlFor="dataToSend">Data to send</label>
                    <input type="text" className="form-control" value={this.state.dataToSend} id="dataToSend" onChange={(e) => this.hanleOnDataTextChange(e.target.value)}></input>
                </div>
                <div className="col-sm">
                    <button type="submit" className="btn btn-primary" onClick={() => ConnectionController.send(this.state.dataToSend)}>Send</button>
                </div>
            </div>
            <div className="row">
                <div className="col-sm">
                    <label>Output</label>
                    <div id="output" style={outputStlye}>
                        <pre id="log"  ref={this.refToConsoleOutput} style={{ whiteSpace: 'pre-wrap', margin: '.5em 0', maxHeight: '200px' }}>
                            {this.props.logOutputText.map((value, index) => {
                                return <div key={index}><span>{{
                                    system: <span style={{ fontWeight: "bold" }}>[SYSTEM] </span>,
                                    in: <span style={{ color: "#00979d", fontWeight: "bold" }}>[IN] </span>,
                                    out: <span style={{ color: "#61dafb", fontWeight: "bold" }}>[OUT] </span>,
                                }[value.type]}</span>
                                    <span>{value.text}</span></div>
                            })}
                        </pre>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col-sm">
                    <button type="submit" className="btn btn-outline-primary btn-sm" onClick={() => this.props.flushLogOutputText()}>Flush</button>
                </div>
            </div>
        </div>
    }
}


const mapStateToProps = (state) => {
    return {
        serviceUuid: state.connection.serviceUuid,
        characteristicUuid: state.connection.characteristicUuid,
        logOutputText: state.connection.logOutputText,
        deviceName: state.connection.deviceName,
        deviceId: state.connection.deviceId
    };
};

export default connect(
    mapStateToProps,
    { changeServiceUuid, changeCharacteristicUuid, flushLogOutputText }
)(Debug);