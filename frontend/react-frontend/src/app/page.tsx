"use client";

import { useState, useEffect } from "react";

import NavBar from "./components/NavBar";
import Oops from "./components/Oops";
import Owners from "./components/Owners";
import Vets from "./components/Vets";
import Welcome from "./components/Welcome";
import { PageNames } from "./types/Types";
import useHash from "./components/Hash";

export default function App() {
  const [hash, setHash] = useHash();

  // Set initial hash if there's none
  useEffect(() => {
    if (!hash || hash === '#') {
      setHash('#welcome');
    }
  }, [hash, setHash]);

  const renderCurrentView = () => {

    // TODO implement proper routing logic
    if (hash.startsWith('#owners')) {
        return <Owners />;
    } else if (hash.startsWith('#vets')) {
        return <Vets hash={hash} setHash={setHash} />;
    } else if (hash === '#oops') {
        return <Oops />;
    } else {
        return <Welcome />;
    }
  };

  return (
    <>
      <NavBar hash={hash} setHash={setHash} />
      <div className="container-fluid">
        <div className="container xd-container">
          {renderCurrentView()}
          <br />
          <br />        
          <div className="container">
            <div className="row">
              <div className="col-12 text-center">
                <img src="logo.svg" alt="Footer Logo" height={31} width={151} className="logo" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
