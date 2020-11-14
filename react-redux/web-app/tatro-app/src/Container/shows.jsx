import React from 'react'
import { SHOWS, SHOW_SPEED } from '../Utility/constants';
import * as ArduinoCmdController from '../Controller/arduinoCmdController'
import Slider from 'rc-slider';
import { handle } from '../Component/sliderHandle'
import { setColor, setDelay, setAnimation } from "../redux/actions";
import { HuePicker } from 'react-color';
import { connect } from "react-redux";
import cx from "classnames";

class Shows extends React.Component {
  constructor(props) {
    super(props)
  }

  onShowSelectClick = (show) => {
    this.props.setAnimation(show);
  }

  onPlayClick = () => {
    if (this.props.selectedAnimation !== SHOWS.THEATER_CHASE) {
      ArduinoCmdController.playShow(this.props.selectedAnimation, this.getDelayValue());
    } else {
      ArduinoCmdController.playShow(this.props.selectedAnimation, this.getDelayValue(), this.props.selectedColor.hsv.h);
    }
  }

  onStopClick = () => {
    ArduinoCmdController.stopShow();
  }

  getDelayValue = () => {
    var showSpeeds = this.getShowSpeeds()

    switch (this.props.selectedDelay) {
      case SHOW_SPEED.NORMAL: return showSpeeds.normal;
      case SHOW_SPEED.SLOW: return showSpeeds.slow;
      case SHOW_SPEED.SLOWER: return showSpeeds.slower;
      default:
        break;
    }
  }

  onDelaySelect = (delay) => {
    this.props.setDelay(delay);
  }

  onColorHueChange = (color) => {
    this.props.setColor(color);
  }

  getShowSpeeds = () => {
    switch (this.props.selectedAnimation) {
      case SHOWS.RANDOM:
        return { normal: 500, slow: 1000, slower: 2000 }
      case SHOWS.TRANSITION:
        return { normal: 256, slow: 128, slower: 64 }
      case SHOWS.RAINBOW_ONE:
        return { normal: 256, slow: 128, slower: 64 }
      case SHOWS.RAINBOW_TWO:
        return { normal: 256, slow: 128, slower: 64 }
      case SHOWS.THEATER_CHASE:
        return { normal: 256, slow: 512, slower: 1024 }
      case SHOWS.THEATER_CHASE_RAINBOW:
        return { normal: 256, slow: 512, slower: 1024 }
      default:
        break;
    }
  }

  render() {
    return <div className='shows'>
      <h2>Animations </h2>
      <div className="row">
        <div className="col-1">
          Animation:
        </div>
        <div className="col-11">
          <div className="btn-group">
            <button type="button" className="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              {this.props.selectedAnimation.name}
            </button>
            <div className="dropdown-menu">
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.RANDOM)}>{SHOWS.RANDOM.name}</a>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.TRANSITION)}>{SHOWS.TRANSITION.name}</a>
              <div className="dropdown-divider"></div>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.RAINBOW_ONE)}>{SHOWS.RAINBOW_ONE.name}</a>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.RAINBOW_TWO)}>{SHOWS.RAINBOW_TWO.name}</a>
              <div className="dropdown-divider"></div>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.THEATER_CHASE)}>{SHOWS.THEATER_CHASE.name}</a>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(SHOWS.THEATER_CHASE_RAINBOW)}>{SHOWS.THEATER_CHASE_RAINBOW.name}</a>
            </div>
          </div>
        </div>
      </div>
      <div className="row">
        <div className="col-1">
          Speed:
        </div>
        <div className="col-3">
          <div className="btn-group btn-group-toggle" data-toggle="buttons">
            <label className={cx("btn btn-secondary", this.props.selectedDelay === SHOW_SPEED.NORMAL && "active")} onClick={() => this.onDelaySelect(SHOW_SPEED.NORMAL)}>
              <input type="radio" name="options" id="option1" checked={this.props.selectedDelay === SHOW_SPEED.NORMAL} />{SHOW_SPEED.NORMAL}
            </label>
            <label className={cx("btn btn-secondary", this.props.selectedDelay === SHOW_SPEED.SLOW && "active")} onClick={() => this.onDelaySelect(SHOW_SPEED.SLOW)}>
              <input type="radio" name="options" id="option2" checked={this.props.selectedDelay === SHOW_SPEED.SLOW} />{SHOW_SPEED.SLOW}
            </label>
            <label className={cx("btn btn-secondary", this.props.selectedDelay === SHOW_SPEED.SLOWER && "active")} onClick={() => this.onDelaySelect(SHOW_SPEED.SLOWER)}>
              <input type="radio" name="options" id="option3" checked={this.props.selectedDelay === SHOW_SPEED.SLOWER} />{SHOW_SPEED.SLOWER}
            </label>
          </div>
        </div>
      </div>
      {this.props.selectedAnimation === SHOWS.THEATER_CHASE &&
        <div className="row">
          <div className="col-1">
            Color:
        </div>
          <div className="col-3">
            <span className="animations-selected-color" style={{ background: `rgb(${this.props.selectedColor.rgb.r}, ${this.props.selectedColor.rgb.g}, ${this.props.selectedColor.rgb.b})` }}></span>
            <HuePicker color={this.props.selectedColor.rgb} onChange={(color) => { this.onColorHueChange(color) }} />
          </div>
        </div>
      }

      <div className="row">
        <div className="col-1">
          <button type="button" disabled={this.props.isAnimationOrGameOrMusicPlaying || this.props.isAnimationPlaying} className="btn btn-primary" onClick={this.onPlayClick}><span className="shows-play-icon"></span>Start</button>
          <button type="button" disabled={this.props.isAnimationOrGameOrMusicPlaying && !this.props.isAnimationPlaying} className="btn btn-secondary" onClick={this.onStopClick}><span className="shows-stop-icon"></span>Stop</button>
        </div>
      </div>
    </div>
  }
}

const mapStateToProps = (state) => {
  return {
    selectedDelay: state.animations.delay,
    selectedColor: state.animations.color,
    selectedAnimation: state.animations.animation
  };
};


const mapDispatchToProps = dispatch => ({
  setColor: color => dispatch(setColor(color)),
  setDelay: delay => dispatch(setDelay(delay)),
  setAnimation: animation => dispatch(setAnimation(animation)),
})

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Shows);