import React from 'react'
import { MUSIC } from '../Utility/constants';
import * as ArduinoCmdController from '../Controller/arduinoCmdController'
import { connect } from "react-redux";
import { setBrightness } from "../redux/actions";
import Slider from 'rc-slider';
import { handle } from '../Component/sliderHandle'

class Settings extends React.Component {
  constructor(props) {
    super(props)
  }

  onSelectBrightness = (brightness) => {
    this.props.setBrightness(brightness);
  }

  render() {
    return <div className='music'>
      <h2>Settings </h2>
      <div className="row">
        <div className="col-1">
          Brightness:
        </div>
        <div className="col-11">
        <Slider min={0} max={255} defaultValue={this.props.brightness} handle={handle} onAfterChange={(value) => this.onSelectBrightness(value)}/>
        </div>
      </div>

      <div className="row">
        <div className="col-1">
          <button type="button" className="btn btn-primary" onClick={() => {ArduinoCmdController.setBrightness(this.props.brightness)}}><span className="settings-save-icon"></span>Übernehmen</button>
        </div>
      </div>
    </div>
  }
}


const mapStateToProps = (state) => {
    return {
        brightness: state.settings.brightness
    };
};

export default connect(
    mapStateToProps,
    { setBrightness }
)(Settings);