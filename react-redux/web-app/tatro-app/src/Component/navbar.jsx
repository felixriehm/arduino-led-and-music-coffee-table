import React from 'react';
import { Link, withRouter } from 'react-router-dom';
import ConnecttionButton from './connectionButton';
import cx from "classnames";

const Navbar = ({connectionStatus,location}) => {
    return <nav className="navbar navbar-expand-lg navbar-light bg-light">
        <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
        </button>
        <a className="navbar-brand" href="/">Tatro-App</a>

        <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav">
                <li className={cx("nav-item", (location.pathname ==="/draw" || location.pathname ==="/") && "active")}>
                    <Link className="nav-link" to="/draw"><span className="nav-item-icon nav-draw-icon"></span>Draw</Link>
                </li>
                <li className={cx("nav-item", location.pathname ==="/shows" && "active")}>
                    <Link className="nav-link" to="/shows"><span className="nav-item-icon nav-shows-icon"></span>Animations</Link>
                </li>
                <li className={cx("nav-item", location.pathname ==="/games" && "active")}>
                    <Link className="nav-link" to="/games"><span className="nav-item-icon nav-games-icon"></span>Games</Link>
                </li>
                <li className={cx("nav-item", location.pathname ==="/music" && "active")}>
                    <Link className="nav-link" to="/music"><span className="nav-item-icon nav-music-icon"></span>Music</Link>
                </li>
                <li className={cx("nav-item", location.pathname ==="/debug" && "active")}>
                    <Link className="nav-link" to="/debug"><span className="nav-item-icon nav-bluetooth-icon"></span>Bluetooth</Link>
                </li>
                <li className={cx("nav-item", location.pathname ==="/settings" && "active")}>
                    <Link className="nav-link" to="/settings"><span className="nav-item-icon nav-settings-icon"></span>Settings</Link>
                </li>
            </ul>
        </div>
        <ConnecttionButton connectionStatus={connectionStatus}/>
    </nav>
  }

  export default withRouter(Navbar);