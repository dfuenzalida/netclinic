import { useEffect, useState } from 'react';
import { OwnerDetails, OwnerSearchResultsProps } from '../../types/Types';
import Pagination from '../Pagination';
import { fetchPetsForOwner } from '../Api';

export default function OwnerSearchResults({ lastName, ownersView, setOwnersView, errorMessage, setErrorMessage, setOwnerId }: OwnerSearchResultsProps) {

  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [owners, setOwners] = useState<OwnerDetails[]>([]);

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
        return;
      } 
      
      if (data.ownerList.length === 1 && currentPage === 1) {
        // When there's exactly one owner in the first page of search results, go to the details view for that owner
        setOwnerId(data.ownerList[0].id);
        setOwnersView('ownerDetails');
        return;
      } else {
        setErrorMessage(null);
      }

      // Convert owners to the extended type with loading state
      const ownersWithDetailedPets: OwnerDetails[] = data.ownerList.map((owner: OwnerDetails) => ({
        ...owner,
        pets: []
      }));

      setOwners(ownersWithDetailedPets);
      setTotalPages(data.totalPages);

      // Fetch pets for each owner
      const updatedOwners = await Promise.all(
        ownersWithDetailedPets.map(async (owner) => {
          const pets = await fetchPetsForOwner(owner.id);
          return {
            ...owner,
            pets
          };
        })
      );

      setOwners(updatedOwners);
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
          {owners.map((owner: OwnerDetails) => (
            <tr key={owner.id}>
              <td><a href={`#/owners/${owner.id}`}
                     onClick={() => { setOwnerId(owner.id); setOwnersView('ownerDetails'); }}> {owner.firstName} {owner.lastName}</a></td>
              <td>{owner.address}</td>
              <td>{owner.city}</td>
              <td>{owner.telephone}</td>
              <td>
                {owner.pets.map(pet => pet.name).sort().join(', ')}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <Pagination linkBase={`#owners/lastName/${lastName}/`} currentPage={currentPage} setCurrentPage={setCurrentPage} totalPages={totalPages} />
    </div>
  );
}
