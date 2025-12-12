"use client";

import { useState, useEffect } from "react";

import NavBar from "./components/NavBar";
import Oops from "./components/Oops";
import Vets from "./components/Vets";
import Welcome from "./components/Welcome";
import useHash from "./components/Hash";
import OwnerSearchForm from "./components/Owners/OwnerSearchForm";
import OwnerSearchResults from "./components/Owners/OwnerSearchResults";
import OwnerDetailsView from "./components/Owners/OwnerDetailsView";
import OwnerCreateEditForm from "./components/Owners/OwnerCreateEditForm";
import PetCreateEditForm from "./components/Pets/PetCreateEditForm";
import CreateVisitForm from "./components/Pets/CreateVisitForm";

export default function App() {
  const [hash, setHash] = useHash();

  // Subscribe to hash changes and update current view accordingly
  // Set initial hash if there's none
  useEffect(() => {
    if (!hash || hash === '#') {
      setHash('#welcome');
    }
  }, [hash, setHash]);

  const renderCurrentView = () => {

    // TODO implement proper routing logic
    if (hash.startsWith('#vets')) {
        return <Vets hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/search')) {
        return <OwnerSearchForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/lastName')) {
        return <OwnerSearchResults hash={hash} setHash={setHash} />;
    } else if (hash.endsWith('/visits/new')) {
        return <CreateVisitForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/new')) {
        return <OwnerCreateEditForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/') && hash.endsWith('/pets/new')) {
        return <PetCreateEditForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/') && hash.indexOf('/pets/') > 0 && hash.endsWith('/edit')) {
        return <PetCreateEditForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/') && hash.endsWith('/edit')) {
        return <OwnerCreateEditForm hash={hash} setHash={setHash} />;
    } else if (hash.startsWith('#owners/')) {
        return <OwnerDetailsView hash={hash} setHash={setHash} />;
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
