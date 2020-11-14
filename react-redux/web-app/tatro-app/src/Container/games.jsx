import React from 'react';
import { GAMES, GAME_CONTROL, GAME_SPEED } from '../Utility/constants';
import GameControlTile from '../Component/gameControlTile';
import * as ArduinoCmdController from '../Controller/arduinoCmdController'
import { setGameSpeed, setGame } from "../redux/actions";
import { connect } from "react-redux";
import cx from "classnames";

class Games extends React.Component {
  constructor(props) {
    super(props)
  }

  onPlayClick = () => {
      ArduinoCmdController.playArduinoGame(this.props.selectedGame, this.getDelayValue());
  }

  onStopClick = () => {
    ArduinoCmdController.stopArduinoGame();
  }

  onGameSelectClick = (game) => {
    this.props.setGame(game);
  }

  onGameControlClick = (control) => {
    ArduinoCmdController.doGameControl(control);
  }

  getDelayValue = () => {
    var showSpeeds = this.getShowSpeeds()

    switch (this.props.selectedDelay) {
      case GAME_SPEED.NORMAL: return showSpeeds.normal;
      case GAME_SPEED.SLOW: return showSpeeds.slow;
      case GAME_SPEED.SLOWER: return showSpeeds.slower;
      default:
        break;
    }
  }

  onDelaySelect = (speed) => {
    this.props.setGameSpeed(speed);
  }

  getShowSpeeds = () => {
    switch (this.props.selectedGame) {
      case GAMES.SNAKE:
        return { normal: 256, slow: 512, slower: 1024 }
      default:
        break;
    }
  }

  render() {
    return <div className='games'>
      <h2>Games </h2>
      <div className="row">
        <div className="col-1">
          Game:
        </div>
        <div className="col-11">
          <div className="btn-group">
            <button type="button" className="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              {this.props.selectedGame.name}
            </button>
            <div className="dropdown-menu">
              <a className="dropdown-item" href="#" onClick={() => this.onGameSelectClick(GAMES.SNAKE)}>{GAMES.SNAKE.name}</a>
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
            <label className={cx("btn btn-secondary", this.props.selectedDelay === GAME_SPEED.NORMAL && "active")} onClick={() => this.onDelaySelect(GAME_SPEED.NORMAL)}>
              <input type="radio" name="options" id="option1" checked={this.props.selectedDelay === GAME_SPEED.NORMAL} />{GAME_SPEED.NORMAL}
            </label>
            <label className={cx("btn btn-secondary", this.props.selectedDelay === GAME_SPEED.SLOW && "active")} onClick={() => this.onDelaySelect(GAME_SPEED.SLOW)}>
              <input type="radio" name="options" id="option2" checked={this.props.selectedDelay === GAME_SPEED.SLOW} />{GAME_SPEED.SLOW}
            </label>
            <label className={cx("btn btn-secondary", this.props.selectedDelay === GAME_SPEED.SLOWER && "active")} onClick={() => this.onDelaySelect(GAME_SPEED.SLOWER)}>
              <input type="radio" name="options" id="option3" checked={this.props.selectedDelay === GAME_SPEED.SLOWER} />{GAME_SPEED.SLOWER}
            </label>
          </div>
        </div>
      </div>
      <div className="row">
        <div className="col-1">
          <button type="button" disabled={this.props.isAnimationOrGameOrMusicPlaying || this.props.isGamePlaying} className="btn btn-primary" onClick={this.onPlayClick}><span className="shows-play-icon"></span>Start</button>
          <button type="button" disabled={this.props.isAnimationOrGameOrMusicPlaying && !this.props.isGamePlaying} className="btn btn-secondary" onClick={this.onStopClick}><span className="shows-stop-icon"></span>Stop</button>
        </div>
      </div>
      <h5>Game controls </h5>
      <div className="game-controls">
        <GameControlTile color={{ r: 0, g: 0, b: 0 }} classes="btn-up" onGameControlClick={this.onGameControlClick} gameInput={GAME_CONTROL.UP} />
        <GameControlTile color={{ r: 0, g: 0, b: 0 }} classes="btn-left" onGameControlClick={this.onGameControlClick} gameInput={GAME_CONTROL.LEFT} />
        <GameControlTile color={{ r: 0, g: 0, b: 0 }} classes="btn-right" onGameControlClick={this.onGameControlClick} gameInput={GAME_CONTROL.RIGHT} />
        <GameControlTile color={{ r: 0, g: 0, b: 0 }} classes="btn-action" onGameControlClick={this.onGameControlClick} gameInput={GAME_CONTROL.ACTION} />
        <GameControlTile color={{ r: 0, g: 0, b: 0 }} classes="btn-down" onGameControlClick={this.onGameControlClick} gameInput={GAME_CONTROL.DOWN} />
      </div>
    </div>
  }
}

const mapStateToProps = (state) => {
  return {
    selectedDelay: state.games.delay,
    selectedGame: state.games.game
  };
};


const mapDispatchToProps = dispatch => ({
  setGameSpeed: speed => dispatch(setGameSpeed(speed)),
  setGame: game => dispatch(setGame(game)),
})

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Games);