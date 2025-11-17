interface NavBarProps {
  currentView: string;
  setCurrentView: (viewName: string) => void;
}

export default function NavBar({ currentView, setCurrentView }: NavBarProps) {
  const handleNavClick = (e: React.MouseEvent<HTMLAnchorElement>, viewName: string) => {
    // e.preventDefault();
    setCurrentView(viewName);
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark" role="navigation">
      <div className="container-fluid">
        <a className="navbar-brand" href="/">
          <span></span>
        </a>
        <button 
          className="navbar-toggler" 
          type="button" 
          data-bs-toggle="collapse" 
          data-bs-target="#main-navbar"
          aria-controls="main-navbar"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="main-navbar">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <a className={`nav-link ${currentView === 'welcome' ? 'active' : ''}`} href="#" onClick={(e) => handleNavClick(e, 'welcome')} title="home page">
                <span className="fa fa-home"></span>
                <span>Home</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${currentView === 'owners' ? 'active' : ''}`} href="#owners" onClick={(e) => handleNavClick(e, 'owners')} title="find owners">
                <span className="fa fa-search"></span>
                <span>Find Owners</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${currentView === 'vets' ? 'active' : ''}`} href="#vets" onClick={(e) => handleNavClick(e, 'vets')} title="veterinarians">
                <span className="fa fa-th-list"></span>
                <span>Veterinarians</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${currentView === 'oops' ? 'active' : ''}`} href="#oops" onClick={(e) => handleNavClick(e, 'oops')} title="trigger a RuntimeException to see how it is handled">
                <span className="fa fa-exclamation-triangle"></span>
                <span>Error</span>
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}
