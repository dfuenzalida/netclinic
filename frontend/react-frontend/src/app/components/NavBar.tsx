import { HashProps } from "../types/Types";
import T from "./Translations";
  
export default function NavBar({ hash, setHash }: HashProps) {

  const handleNavClick = (e: React.MouseEvent<HTMLAnchorElement>, viewName: string) => {
    e.preventDefault();
    setHash(viewName);
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark" role="navigation">
      <div className="container-fluid">
        <a className="navbar-brand" href="#">
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
              <a className={`nav-link ${hash === '#welcome' ? 'active' : ''}`} href="#welcome" onClick={(e) => handleNavClick(e, 'welcome')} title={T("home")}>
                <span className="fa fa-home"></span>
                &nbsp;
                <span>{T("home")}</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${hash.startsWith('#owners') ? 'active' : ''}`} href="#owners" onClick={(e) => handleNavClick(e, 'owners/search')} title={T("findOwners")}>
                <span className="fa fa-search"></span>
                &nbsp;
                <span>{T("findOwners")}</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${hash.startsWith('#vets') ? 'active' : ''}`} href="#vets" onClick={(e) => handleNavClick(e, 'vets')} title={T("vets")}>
                <span className="fa fa-th-list"></span>
                &nbsp;
                <span>{T("vets")}</span>
              </a>
            </li>

            <li className="nav-item">
              <a className={`nav-link ${hash === 'oops' ? 'active' : ''}`} href="#oops" onClick={(e) => handleNavClick(e, 'oops')} title="trigger a RuntimeException to see how it is handled">
                <span className="fa fa-exclamation-triangle"></span>
                &nbsp;
                <span>{T("error")}</span>
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}
