'use client';

import { useState, useEffect } from 'react';
import Pagination from './Pagination';
import { OwnerDetailsProps, OwnerOverview, OwnerSearchProps, OwnerSearchResultsProps,
  OwnerDetails, PetDetails, VisitDetails, OwnersViewNames } from '../types/Types';

function OwnerSearchForm({ lastName, setLastName, setOwnersView, errorMessage, setErrorMessage } : OwnerSearchProps) {
  return (
    <div>
      <h2>Find Owners</h2>

      <div className="form-group">
        <div className="control-group" id="lastNameGroup">
          <label className="col-sm-2 control-label">Last Name</label>
          <div className="col-sm-10">
            <input className="form-control" size={30} maxLength={80} name="lastName" value={lastName}
              onKeyDown={(e) => e.key === 'Enter' && setOwnersView('searchResults')}
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
          <button type="submit" className="btn btn-primary" onClick={() => setOwnersView('searchResults')}>Find Owner</button>
        </div>
      </div>

      <a className="btn btn-primary" onClick={() => setOwnersView('ownerCreateForm')} href="#/owners/new">Add Owner</a>

    </div>
  );
}

function OwnerSearchResults({ lastName, ownersView, setOwnersView, errorMessage, setErrorMessage, setOwnerId }: OwnerSearchResultsProps) {

  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [owners, setOwners] = useState<OwnerOverview[]>([]);

  const fetchOwnersByLastName = async (lastName: string) => {
    try {
      setLoading(true);
      const response = await fetch(`/api/owners?lastName=${lastName}&page=${currentPage}`);

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      // If no owners found, set an error message
      if (data.ownerList.length === 0) {
        console.log('No owners found with the given last name.');
        setErrorMessage('has not been found');
        setOwnersView('searchForm');
      } else {
        setErrorMessage(null);
      }

      setOwners(data.ownerList);
      setTotalPages(data.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOwnersByLastName(lastName);
  }, [lastName, ownersView, errorMessage, currentPage]);

  if (loading) {
    return (
      <div>
        <h2>Owners</h2>
        <p>Searching owners...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div>
        <h2>Owners</h2>
        <p style={{ color: 'red' }}>Error loading owners: {error}</p>
      </div>
    );
  }

  return (
    <div>
      <h2>Owners</h2>
      <table id="owners" className="table table-striped">
        <thead>
          <tr>
            <th style={{ width: '150px' }}>Name</th>
            <th style={{ width: '200px' }}>Address</th>
            <th>City</th>
            <th style={{ width: '120px' }}>Telephone</th>
            <th>Pets</th>
          </tr>
        </thead>
        <tbody>
          {owners.map((owner: OwnerOverview) => (
            <tr key={owner.id}>
              <td><a href="#" onClick={() => { setOwnerId(owner.id); setOwnersView('ownerDetails'); }}> {owner.firstName} {owner.lastName}</a></td>
              <td>{owner.address}</td>
              <td>{owner.city}</td>
              <td>{owner.telephone}</td>
              <td>{owner.pets.join(', ')}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <Pagination currentPage={currentPage} setCurrentPage={setCurrentPage} totalPages={totalPages} />
    </div>
  );
}

function OwnerDetailsView({ownerId, setOwnersView}: OwnerDetailsProps) {
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [owner, setOwner] = useState<OwnerDetails | null>(null);

  useEffect(() => {
    const fetchOwnerDetails = async () => {
      try {
        setLoading(true);
        const response = await fetch(`/api/owners/${ownerId}`);

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        setOwner(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchOwnerDetails();
  }, [ownerId]);

  if (loading) {
    return <p>Loading owner details...</p>;
  }

  if (error) {
    return <p style={{ color: 'red' }}>Error loading owner details: {error}</p>;
  }

  return (
    <div>
      <h2>Owner Information</h2>
      <table className="table table-striped">
        <tbody>
          <tr>
            <th>Name</th>
            <td><b>{owner?.firstName} {owner?.lastName}</b></td>
          </tr>
          <tr>
            <th>Address</th>
            <td>{owner?.address}</td>
          </tr>
          <tr>
            <th>City</th>
            <td>{owner?.city}</td>
          </tr>
          <tr>
            <th>Telephone</th>
            <td>{owner?.telephone}</td>
          </tr>
        </tbody>
      </table>

      <a href={`"#${ownerId}/edit"`} className="btn btn-primary">Edit Owner</a>
      &nbsp;
      <a href={`"#${ownerId}/pets/new"`} className="btn btn-primary">Add New Pet</a>
      <br />
      <br />
      <br />
      <h2>Pets and Visits</h2>

      <table className="table table-striped">
        <tbody>
          {
            owner?.pets.map((pet: PetDetails) => (
              <tr key={'pet' + pet.id}>
                <td valign="top">
                  <dl className="dl-horizontal">
                    <dt>Name</dt>
                    <dd>{pet.name}</dd>
                    <dt>Birth Date</dt>
                    <dd>{pet.birthDate}</dd>
                    <dt>Type</dt>
                    <dd>{pet.type}</dd>
                  </dl>
                </td>
                <td valign="top">
                  <table className="table-condensed">
                    <thead>
                      <tr>
                        <th style={{ width: '120px' }}>Visit Date</th>
                        <th>Description</th>
                      </tr>
                    </thead>
                    <tbody>
                      {
                        pet.visits.map((visit: VisitDetails) => (
                          <tr key={'visit' + visit.id}>
                            <td>{visit.visitDate}</td>
                            <td>{visit.description}</td>
                          </tr>
                        ))
                      }
                      <tr>
                        <td><a href="#8/pets/10/edit">Edit Pet</a></td>
                        <td><a href="#8/pets/10/visits/new">Add Visit</a></td>
                      </tr>
                    </tbody>
                  </table>
                </td>
              </tr>            
            ))
          }
        </tbody>
      </table>
    </div>
  );
}

function OwnerCreateForm() {
  return (<div>
    <h2>Owner</h2>
      <form className="form-horizontal" id="add-owner-form" method="post">
        <div className="form-group has-feedback">
          
          <div className="form-group has-error">
            <label htmlFor="firstName" className="col-sm-2 control-label">First Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="firstName" name="firstName" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="lastName" className="col-sm-2 control-label">Last Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="lastName" name="lastName" value="" />                
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>        
          
          <div className="form-group has-error">
            <label htmlFor="address" className="col-sm-2 control-label">Address</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="address" name="address" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="city" className="col-sm-2 control-label">City</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="city" name="city" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="telephone" className="col-sm-2 control-label">Telephone</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="telephone" name="telephone" value="" />                
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">Telephone must be a 10-digit number<br />must not be blank</span>              
            </div>
          </div>
        
        </div>
        <div className="form-group">
          <div className="col-sm-offset-2 col-sm-10">
            <button className="btn btn-primary" type="submit">Add Owner</button>
          </div>
        </div>
      </form>
    </div>);
}

export default function Owners() {

  const [ownerId, setOwnerId] = useState<number>(0);
  const [lastName, setLastName] = useState<string>('');
  const [ownersView, setOwnersView] = useState<OwnersViewNames>('searchForm');
  const [errorMessage, setErrorMessage] = useState<string|null>(null);

  switch (ownersView) {
    case 'ownerDetails':
      return <OwnerDetailsView ownerId={ownerId} setOwnersView={setOwnersView} />;
    case 'searchResults':
      return <OwnerSearchResults lastName={lastName} ownersView={ownersView} setOwnersView={setOwnersView}
                errorMessage={errorMessage} setErrorMessage={setErrorMessage} setOwnerId={setOwnerId} />;
    case 'searchForm':
      return <OwnerSearchForm lastName={lastName} setLastName={setLastName} errorMessage={errorMessage} setErrorMessage={setErrorMessage}
               setOwnersView={setOwnersView} />;
    case 'ownerCreateForm':
      return <OwnerCreateForm />;
  };
}