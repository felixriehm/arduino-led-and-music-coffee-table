import React from 'react'
import { MUSIC } from '../Utility/constants';
import * as ArduinoCmdController from '../Controller/arduinoCmdController'

export default class Music extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      selectedShow: MUSIC.GET_LUCKY,
      isPlaying: false,
      isPaused: false,
      currentlyPlaying: "",
    }
    
    this.timer = null;
  }

  onShowSelectClick = (show) => {
    switch (show) {
      case MUSIC.GET_LUCKY: this.setState({ selectedShow: MUSIC.GET_LUCKY });
        break;
      case MUSIC.WHAT_IS_LOVE: this.setState({ selectedShow: MUSIC.WHAT_IS_LOVE });
        break;
      case MUSIC.CANTINA_THEME: this.setState({ selectedShow: MUSIC.CANTINA_THEME });
        break;
      default:
        break;
    }
  }

  onPlayClick = () => {
    if (!this.state.isPlaying) {
      ArduinoCmdController.playMusic(this.state.selectedShow);
      this.setState({
        isPlaying: true,
        isPaused: false,
        currentlyPlaying: this.state.selectedShow.name
      });
    }
  }

  onStopClick = () => {
    if (this.state.isPlaying) {
      ArduinoCmdController.stopMusic();
      this.setState({
        isPlaying: false,
        isPaused: false,
        currentlyPlaying: ""
      });
    }
  }

  onPauseClick = () => {
    if (this.state.isPlaying) {
      ArduinoCmdController.pauseMusic();
      this.setState({
        isPlaying: false,
        isPaused: true
      });
    }
  }

  render() {
    return <div className='music'>
      <h2>Music </h2>
      <div className="row">
        <div className="col-1">
          Preview:
        </div>
        <div className="col-11">
          <button type="button" className="btn btn-secondary"><span className="shows-play-icon"></span>Play on browser</button>
        </div>
      </div>
      <div className="row">
        <div className="col-1">
          Duration:
        </div>
        <div className="col-11">
          {this.state.selectedShow.duration}
        </div>
      </div>
      <div className="row">
        <div className="col-1">
          Music:
        </div>
        <div className="col-11">
          <div className="btn-group">
            <button type="button" className="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              {this.state.selectedShow.name}
            </button>
            <div className="dropdown-menu">
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(MUSIC.GET_LUCKY)}>{MUSIC.GET_LUCKY.name}</a>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(MUSIC.WHAT_IS_LOVE)}>{MUSIC.WHAT_IS_LOVE.name}</a>
              <a className="dropdown-item" href="#" onClick={() => this.onShowSelectClick(MUSIC.CANTINA_THEME)}>{MUSIC.CANTINA_THEME.name}</a>
            </div>
          </div>
        </div>
      </div>
      <div className="row">
        <div className="col-1">
          <button type="button" className="btn btn-primary" onClick={() => this.onPlayClick()} disabled={this.state.isPlaying }><span className="shows-play-icon"></span>Play</button>
        </div>
        <div className="col-1">
          <button type="button" className="btn btn-secondary" onClick={() => this.onStopClick()}><span className="shows-stop-icon"></span>Stop</button>
        </div>
        <div className="col-1">
          <button type="button" className="btn btn-secondary" onClick={() => this.onPauseClick()}><span className="shows-pause-icon"></span>Pause</button>
        </div>
        <div className="col-1">
          Currently playing: {this.state.currentlyPlaying}
        </div>
      </div>
    </div>
  }
}