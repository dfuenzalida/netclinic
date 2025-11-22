import { useEffect, useState } from 'react';
import { OwnerOverview, OwnerSearchResultsProps } from '../../types/Types';
import Pagination from '../Pagination';

export default function OwnerSearchResults({ lastName, ownersView, setOwnersView, errorMessage, setErrorMessage, setOwnerId }: OwnerSearchResultsProps) {

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
      } if (data.ownerList.length === 1 && currentPage === 1) {
        // When there's exactly one owner in the first page ofsearch results, go to the details view for that owner
        setOwnerId(data.ownerList[0].id);
        setOwnersView('ownerDetails');
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
