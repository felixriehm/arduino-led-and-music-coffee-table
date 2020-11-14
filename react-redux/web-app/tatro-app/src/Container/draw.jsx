import React from 'react'
import Tile from '../Component/tile'
import ColorSelectionTile from '../Component/colorSelectionTile'
import { HSVtoRGB, RGBtoHSV } from '../Utility/colorConversion'
import { HuePicker } from 'react-color';
import cx from "classnames";
import { GRID_X, GRID_Y, HSV_HUE_MAX_DEGREE, TOOLS, COLORS } from '../Utility/constants'
import * as ArduinoCmdController from '../Controller/arduinoCmdController'

export default class Draw extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      x: GRID_X,
      y: GRID_Y,
      tiles: [],
      colorselectionTiles: [],
      colorSelection: [],
      selectedTool: TOOLS.BRUSH,
      colorPickerOpen: false,
      tableBackgroundColor: COLORS.BLACK,
      selectedColor: { colorIndex: 200, rgb: HSVtoRGB(200 / HSV_HUE_MAX_DEGREE, 1, 1) },
      selectedColorIndex: 8,
      colorSelectionHsv: [
        COLORS.BLACK,
        COLORS.WHITE,
        { colorIndex: 0, rgb: HSVtoRGB(0 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 36, rgb: HSVtoRGB(36 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 60, rgb: HSVtoRGB(60 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 80, rgb: HSVtoRGB(80 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 116, rgb: HSVtoRGB(116 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 178, rgb: HSVtoRGB(178 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 200, rgb: HSVtoRGB(200 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 240, rgb: HSVtoRGB(240 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 280, rgb: HSVtoRGB(280 / HSV_HUE_MAX_DEGREE, 1, 1) },
        { colorIndex: 300, rgb: HSVtoRGB(300 / HSV_HUE_MAX_DEGREE, 1, 1) }]
    }
  }

  componentDidMount() {
    let newColorSelection = new Array(this.state.colorSelectionHsv.length).fill(false);
    newColorSelection[8] = true;

    this.setState({
      tiles: new Array(this.state.x * this.state.y).fill(null).map((value, index) => {
        let x = index % this.state.x;
        let y = Math.floor(index / this.state.x);
        return {
          x: x,
          y: y,
          index: index,
          color: this.state.tableBackgroundColor,
          component: <Tile key={index} x={x} y={y} index={index} connectionStatus={this.props.connectionStatus} color={this.state.tableBackgroundColor} onTileClick={this.onTileClick} />
        }
      }),
      colorSelection: newColorSelection
    }, () => {
      this.setState({
        colorselectionTiles: this.createColorSelectionTiles()
      })
    })
  }

  onTileClick = (index) => {
    let newTiles = this.state.tiles;
    let x = index % this.state.x;
    let y = Math.floor(index / this.state.x);

    if (this.state.selectedTool === TOOLS.BRUSH) {
      newTiles[index].color = this.state.selectedColor;
      newTiles[index].component = <Tile key={index} x={x} y={y} index={index} connectionStatus={this.props.connectionStatus} color={this.state.selectedColor} onTileClick={this.onTileClick} />
      ArduinoCmdController.setTile(x, y, this.state.selectedColor.colorIndex);
    }

    if (this.state.selectedTool === TOOLS.ERASER) {
      newTiles[index].color = this.state.tableBackgroundColor;
      newTiles[index].component = <Tile key={index} x={x} y={y} index={index} connectionStatus={this.props.connectionStatus} color={this.state.tableBackgroundColor} onTileClick={this.onTileClick} />
      ArduinoCmdController.setTile(x, y, this.state.selectedColor.colorIndex);
    }

    if (this.state.selectedTool === TOOLS.BUCKET) {
      let selectedTileColor = this.state.tiles[index].color;
      this.state.tiles.forEach((value, index) => {
        if (value.color.colorIndex === selectedTileColor.colorIndex) {
          newTiles[index].color = this.state.selectedColor;
          newTiles[index].component = <Tile key={index} x={x} y={y} index={index} connectionStatus={this.props.connectionStatus} color={this.state.selectedColor} onTileClick={this.onTileClick} />;
        }
      });
      ArduinoCmdController.setAllTiles(newTiles);
    }

    this.setState({
      tiles: newTiles
    })
  }

  onColorSelectionClick = (index) => {
    let newColorSelection = new Array(this.state.colorSelection.length).fill(false);
    newColorSelection[index] = true;

    this.setState({
      colorSelection: newColorSelection,
      selectedColor: this.state.colorSelectionHsv[index],
      selectedColorIndex: index
    }, () => {
      this.setState({
        colorselectionTiles: this.createColorSelectionTiles(),
      })
    })
  }

  onColorHueChange = (rgbColor) => {
    let newColorSelectionHsv = this.state.colorSelectionHsv;
    newColorSelectionHsv[this.state.selectedColorIndex] = { colorIndex: rgbColor.hsv.h, rgb: rgbColor.rgb };

    this.setState({
      colorSelectionHsv: newColorSelectionHsv,
      selectedColor: { colorIndex: rgbColor.hsv.h, rgb:rgbColor.rgb}
    }, () => {
      this.setState({
        colorselectionTiles: this.createColorSelectionTiles(),
      })
    })
  }

  createColorSelectionTiles = () => {
    return this.state.colorSelectionHsv.map((value, index) => {
      return <ColorSelectionTile color={value} index={index} colorSelection={this.state.colorSelection} onColorSelectionClick={this.onColorSelectionClick} />
    })
  }

  onToolColorSelectClick = () => {
    this.setState({
      colorPickerOpen: !this.state.colorPickerOpen
    })
  }

  onToolsClick = (tool) => {
    this.setState({ selectedTool: tool })
  }

  onToolsUndoAllClick = () => {
    this.setState({
      tiles: new Array(this.state.x * this.state.y).fill(null).map((value, index) => {
        let x = index % this.state.x;
        let y = Math.floor(index / this.state.x);
        return {
          x: x,
          y: y,
          index: index,
          color: this.state.tableBackgroundColor,
          component: <Tile key={index} x={x} y={y} index={index} connectionStatus={this.props.connectionStatus} color={this.state.tableBackgroundColor} onTileClick={this.onTileClick} />
        }
      })
    })
  }

  onToolsBackgroundClick = (backgroundColor) => {
    let newTiles = this.state.tiles;
    let selectedTileColor = backgroundColor === COLORS.BLACK ? COLORS.WHITE : COLORS.BLACK;

    this.state.tiles.forEach((value, index) => {
      if (value.color.colorIndex === selectedTileColor.colorIndex) {
        newTiles[index].color = backgroundColor;
        newTiles[index].component = <Tile key={index} x={index % this.state.x} y={Math.floor(index / this.state.x)} index={index} connectionStatus={this.props.connectionStatus} color={backgroundColor} onTileClick={this.onTileClick} />;
      }
    });

    this.setState({ tableBackgroundColor: backgroundColor, tiles: newTiles });
  }

  onSetRandomClick = () => {
    ArduinoCmdController.setRandom();
  }

  render() {
    return <div className="draw">
      <h2>Draw</h2>
      <h5>Table</h5>
      <div className={"table"}>
        {this.state.tiles.map((value) => value.component)}
      </div>
      <h5>Tools</h5>
      <div className="row">
        <div className="col-4">
          <div className="btn-group btn-group-toggle" data-toggle="buttons">
            <label className={cx("btn btn-secondary", this.state.selectedTool === TOOLS.BRUSH && "active")} onClick={() => this.onToolsClick(TOOLS.BRUSH)}>
              <input type="radio" name="options" id="option1" checked={this.state.selectedTool === TOOLS.BRUSH} /><span className="tools-brush-icon"></span> {TOOLS.BRUSH.name}
            </label>
            <label className={cx("btn btn-secondary", this.state.selectedTool === TOOLS.BUCKET && "active")} onClick={() => this.onToolsClick(TOOLS.BUCKET)}>
              <input type="radio" name="options" id="option2" checked={this.state.selectedTool === TOOLS.BUCKET} /><span className="tools-bucket-icon"></span> {TOOLS.BUCKET.name}
            </label>
            <label className={cx("btn btn-secondary", this.state.selectedTool === TOOLS.ERASER && "active")} onClick={() => this.onToolsClick(TOOLS.ERASER)}>
              <input type="radio" name="options" id="option3" checked={this.state.selectedTool === TOOLS.ERASER} /><span className="tools-ereaser-icon"></span> {TOOLS.ERASER.name}
            </label>
          </div>
        </div>
        <div className="col-6">
          <button type="button" className="btn btn-secondary" data-toggle="button" onClick={this.onToolColorSelectClick}>
            <span className="tools-tint-icon"></span> Color <span className="tools-selected-color" style={{ background: `rgb(${this.state.selectedColor.rgb.r}, ${this.state.selectedColor.rgb.g}, ${this.state.selectedColor.rgb.b})` }}></span>
          </button>
          Background:
          <div className="btn-group btn-group-toggle" data-toggle="buttons">
            <label className={cx("btn btn-secondary", this.state.tableBackgroundColor === COLORS.BLACK && "active")} onClick={() => this.onToolsBackgroundClick(COLORS.BLACK)}>
              <input type="radio" name="backgroundColor" id="backgroundColor2" checked={this.state.tableBackgroundColor === COLORS.BLACK} /> <span className="tools-selected-color" style={{ background: `rgb(${COLORS.BLACK.rgb.r}, ${COLORS.BLACK.rgb.g}, ${COLORS.BLACK.rgb.b})` }}></span>
            </label>
            <label className={cx("btn btn-secondary", this.state.tableBackgroundColor === COLORS.WHITE && "active")} onClick={() => this.onToolsBackgroundClick(COLORS.WHITE)}>
              <input type="radio" name="backgroundColor" id="backgroundColor3" checked={this.state.tableBackgroundColor === COLORS.WHITE} /> <span className="tools-selected-color" style={{ background: `rgb(${COLORS.WHITE.rgb.r}, ${COLORS.WHITE.rgb.g}, ${COLORS.WHITE.rgb.b})` }}></span>
            </label>
          </div>
        </div>
        <div className="col-2">
          <button type="button" className="btn btn-outline-secondary" onClick={this.onSetRandomClick}> Set Random</button>
          <button type="button" className="btn btn-outline-secondary" onClick={this.onToolsUndoAllClick}><span className="tools-undo-all-icon"></span> Undo all</button>
        </div>
      </div>
      <div className={cx("color-picker", this.state.colorPickerOpen && "color-picker-open")}>
        <h5>Color picker</h5>
        <div className={"colorSelection"}>
          {this.state.colorselectionTiles}
        </div>
        {this.state.selectedColor.colorIndex <= 360 &&
          <HuePicker color={this.state.selectedColor.rgb} onChange={(color) => { this.onColorHueChange(color) }} />
        }
      </div>
    </div>
  }
}