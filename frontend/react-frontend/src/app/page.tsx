"use client";

import { useEffect } from "react";

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

  // Route configuration with pattern matching
  const routes = [
    { pattern: /^#vets/, component: Vets },
    { pattern: /^#owners\/search/, component: OwnerSearchForm },
    { pattern: /^#owners\/lastName/, component: OwnerSearchResults },
    { pattern: /\/visits\/new$/, component: CreateVisitForm },
    { pattern: /^#owners\/new$/, component: OwnerCreateEditForm },
    { pattern: /^#owners\/[^\/]+\/pets\/new$/, component: PetCreateEditForm },
    { pattern: /^#owners\/[^\/]+\/pets\/[^\/]+\/edit$/, component: PetCreateEditForm },
    { pattern: /^#owners\/[^\/]+\/edit$/, component: OwnerCreateEditForm },
    { pattern: /^#owners\/[^\/]+$/, component: OwnerDetailsView },
    { pattern: /^#oops$/, component: Oops },
  ];

  const renderCurrentView = () => {
    // Find matching route
    const matchedRoute = routes.find(route => route.pattern.test(hash));
    
    if (matchedRoute) {
      const Component = matchedRoute.component;
      return <Component hash={hash} setHash={setHash} />;
    }
    
    // Default to Welcome
    return <Welcome />;
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
