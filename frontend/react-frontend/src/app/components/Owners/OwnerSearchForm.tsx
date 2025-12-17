import { useState } from 'react';
import { HashProps } from '../../types/Types';
import T from '../Translations';

export default function OwnerSearchForm({ setHash } : HashProps) {

  const [lastName, setLastName] = useState<string>('');
  const [errorMessage] = useState<string|null>(null);

  function search() {
    setHash(`#owners/lastName/${lastName}`);
  }

  return (
    <div>
      <h2>{T("findOwners")}</h2>

      <div className="form-group">
        <div className="control-group" id="lastNameGroup">
          <label className="col-sm-2 control-label">{T("lastName")}</label>
          <div className="col-sm-10">
            <input className="form-control" size={30} maxLength={80} name="lastName" value={lastName}
              onKeyDown={(e) => e.key === 'Enter' && search()}
              onChange={(e) => setLastName(e.target.value)} />
            <span className="help-inline">
              {errorMessage ?
              <div>
                <p>{errorMessage}</p>
              </div> : <span></span>}
            </span>
          </div>
        </div>
      </div>
      <div className="form-group">
        <div className="col-sm-offset-2 col-sm-10">
          <button type="submit" className="btn btn-primary" onClick={search}>{T("findOwner")}</button>
        </div>
      </div>

      <a className="btn btn-primary" onClick={(e) => { e.preventDefault(); setHash('#owners/new'); }} href="#/owners/new">{T("addOwner")}</a>
    </div>
  );
}