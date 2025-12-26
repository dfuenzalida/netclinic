import { useEffect, useState } from 'react';
import { OwnerDetails, HashProps } from '../../../types/types';
import Pagination from '../Pagination';
import { fetchPetsForOwner } from '../Api';
import { replaceHash } from '../Hash';
import T from '../Translations';

export default function OwnerSearchResults({ hash, setHash } : HashProps) {

  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [owners, setOwners] = useState<OwnerDetails[]>([]);

  // Parse the last name from the hash
  let lastName : string = hash.split('/').pop() ?? '';
  if (lastName.includes('?')) {
    lastName = lastName.split('?')[0];
  }

  useEffect(() => {

    const fetchOwnersByLastName = async (lastName: string, page: number) => {
      try {
        setLoading(true);
        const response = await fetch(`/api/owners?lastName=${lastName}&page=${page}`);

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        // If no owners found, set an error message
        if (data.ownerList.length === 0) {
          console.log('No owners found with the given last name.');
          setHash('#owners/search');
          return;
        } 
        
        if (data.ownerList.length === 1 && page === 1) {
          // When there's exactly one owner in the first page of search results, go to the details view for that owner
          replaceHash(`#owners/${data.ownerList[0].id}`);
          return;
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

    // Parse the page number from the hash
    let pageToUse = 1;
    if (hash.includes('?page=')) {
      const pageParam = new URLSearchParams(hash.split('?')[1]).get('page');
      if (pageParam) {
        pageToUse = parseInt(pageParam, 10);
        console.log(`Parsed page number from hash: ${pageParam}`);
      }
    }
    
    // Update current page if it's different
    if (pageToUse !== currentPage) {
      setCurrentPage(pageToUse);
    }
    
    fetchOwnersByLastName(lastName, pageToUse);
  }, [hash, setHash, currentPage, lastName]);

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
      <h2>{T("owners")}</h2>
      <table id="owners" className="table table-striped">
        <thead>
          <tr>
            <th style={{ width: '150px' }}>{T("name")}</th>
            <th style={{ width: '200px' }}>{T("address")}</th>
            <th>{T("city")}</th>
            <th style={{ width: '120px' }}>{T("telephone")}</th>
            <th>{T("pets")}</th>
          </tr>
        </thead>
        <tbody>
          {owners.map((owner: OwnerDetails) => (
            <tr key={owner.id}>
              <td><a href={`#/owners/${owner.id}`}
                     onClick={(e) => { e.preventDefault(); setHash(`#owners/${owner.id}`); }}>{owner.firstName} {owner.lastName}</a></td>
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
      <Pagination linkBase={`#owners/lastName/${lastName}`} currentPage={currentPage} setCurrentPage={setCurrentPage} totalPages={totalPages} />
    </div>
  );
}
