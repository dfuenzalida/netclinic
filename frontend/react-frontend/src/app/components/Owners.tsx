'use client';

import { useState, useEffect } from 'react';
import Pagination from './Pagination';

interface Owner {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  city: string;
  telephone: string;
  pets: string[];
}

interface OwnerSearchProps {
  lastName: string;
  setLastName: (name: string) => void;
  setOwnersView: (view: string) => void;
  errorMessage?: string;
  setErrorMessage: (message: string | null) => void;
}

interface OwnerSearchResultsProps {
  lastName: string;
  ownersView: string;
  setOwnersView: (view: string) => void;
  errorMessage: string | null;
  setErrorMessage: (message: string | null) => void;
}

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

      <a className="btn btn-primary" href="#/owners/new">Add Owner</a>

    </div>
  );
}

function OwnerSearchResults({ lastName, ownersView, setOwnersView, errorMessage, setErrorMessage }: OwnerSearchResultsProps) {

  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [owners, setOwners] = useState<Owner[]>([]);

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
          {owners.map((owner: Owner) => (
            <tr key={owner.id}>
              <td><a href="#">{owner.firstName} {owner.lastName}</a></td>
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

function OwnerDetails() {
  return (
    <div>
      <h2>Owner Details</h2>
      <p>Owner details will be displayed here.</p>
    </div>
  );
}

export default function Owners() {

  const [lastName, setLastName] = useState<string>('');
  const [ownersView, setOwnersView] = useState<string>('searchForm');
  const [errorMessage, setErrorMessage] = useState<string|null>(null);

  switch (ownersView) {
    case 'ownerDetails':
      return <OwnerDetails />;
    case 'searchResults':
      return <OwnerSearchResults lastName={lastName} ownersView={ownersView} setOwnersView={setOwnersView}
                errorMessage={errorMessage} setErrorMessage={setErrorMessage} />;
    case 'searchForm':
      return <OwnerSearchForm lastName={lastName} setLastName={setLastName} errorMessage={errorMessage} setErrorMessage={setErrorMessage}
               setOwnersView={setOwnersView} />;
  };
}