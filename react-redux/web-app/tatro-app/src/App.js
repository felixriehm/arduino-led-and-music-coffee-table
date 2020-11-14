import React from 'react';
import {
  BrowserRouter as Router,
  Switch,
  Route
} from 'react-router-dom';
import logo from './logo.svg';
import './App.css';
import './loader.js'
import Navbar from './Component/navbar';
import Draw from './Container/draw';
import Shows from './Container/shows';
import Games from './Container/games';
import Music from './Container/music';
import Settings from './Container/settings';
import { connect } from "react-redux";
import { CONNECTION_STATUS } from './Utility/constants';
import NotificationBar from './Component/notification';
import Debug from './Container/debug';

const App = ({isConnected = false, isDisconnected = false, isConnecting = false, isSearching = false, isAnimationOrGameOrMusicPlaying, isAnimationPlaying, isGamePlaying}) => {
  const connectionStatus = {
    isConnected: isConnected,
    isDisconnected: isDisconnected,
    isConnecting: isConnecting,
    isSearching: isSearching,
  }
  return (
    <Router>
      <div className="App">
        <Navbar connectionStatus={connectionStatus}/>
        <div className="container">
          <NotificationBar connectionStatus={connectionStatus}/>
          <Switch>
            <Route path="/draw">
              <Draw connectionStatus={connectionStatus}/>
            </Route>
            <Route path="/shows">
              <Shows isAnimationOrGameOrMusicPlaying={isAnimationOrGameOrMusicPlaying} isAnimationPlaying={isAnimationPlaying}/>
            </Route>
            <Route path="/games">
              <Games isAnimationOrGameOrMusicPlaying={isAnimationOrGameOrMusicPlaying} isGamePlaying={isGamePlaying}/>
            </Route>
            <Route path="/music">
              <Music />
            </Route>
            <Route path="/debug">
              <Debug connectionStatus={connectionStatus}/>
            </Route>
            <Route path="/settings">
              <Settings/>
            </Route>
            <Route path="/">
              <Draw />
            </Route>
          </Switch>
        </div>
      </div>
    </Router>
  );
}

const mapStateToProps = (state) => {
  return {
    isConnected: state.connection.connectionStatus === CONNECTION_STATUS.CONNECTED,
    isDisconnected: state.connection.connectionStatus === CONNECTION_STATUS.DISCONNECTED,
    isConncting: state.connection.connectionStatus === CONNECTION_STATUS.CONNECTING,
    isSearching: state.connection.connectionStatus === CONNECTION_STATUS.SEARCHING,
    isAnimationOrGameOrMusicPlaying: state.animations.animationIsPlaying || state.games.gameIsPlaying,
    isAnimationPlaying: state.animations.animationIsPlaying,
    isGamePlaying: state.games.gameIsPlaying,
   };
};

export default connect(
  mapStateToProps,
  null
)(App);
