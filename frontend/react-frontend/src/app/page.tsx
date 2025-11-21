"use client";

import { useState } from "react";

import NavBar from "./components/NavBar";
import Oops from "./components/Oops";
import Owners from "./components/Owners";
import Vets from "./components/Vets";
import Welcome from "./components/Welcome";
import { PageNames } from "./types/Types";

export default function App() {
  const [currentView, setCurrentView] = useState<PageNames>('welcome');

  const renderCurrentView = () => {
    switch (currentView) {
      case 'owners':
        return <Owners />;
      case 'vets':
        return <Vets />;
      case 'oops':
        return <Oops />;
      default:
        return <Welcome />;
    }
  };

  return (
    <>
      <NavBar currentView={currentView} setCurrentView={setCurrentView} />
      <div className="container-fluid">
        <div className="container xd-container">
          {renderCurrentView()}
          <br />
          <br />        
          <div className="container">
            <div className="row">
              <div className="col-12 text-center">
                <img src="logo.svg" alt="Footer Logo" height={20} width={100} className="logo" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
