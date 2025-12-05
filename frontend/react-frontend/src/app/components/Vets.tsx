'use client';

import { useState, useEffect } from 'react';
import Pagination from './Pagination';
import { Vet, Specialty, HashProps } from '../types/Types';

export default function Vets({ hash, setHash }: HashProps) {
  const [vets, setVets] = useState<Vet[]>([]);
  // const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  var currentPage = 1;
  if (hash && hash.indexOf('?page=') > -1) {
    currentPage = parseInt(hash.substring(hash.indexOf('?page=') + 6)) || 1;
  }

  const setCurrentPage = (page: number) => {
    setHash(`#vets?page=${page}`);
  }

  const fetchVets = async () => {
    try {
      setLoading(true);
      const response = await fetch(`/api/vets?page=${currentPage}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      setVets(data.vetList);
      setTotalPages(data.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (hash) {
      var pos = hash.indexOf('?page=');
      if (pos > -1) {
        var page = parseInt(hash.substring(pos + 6)) || 1;
        setCurrentPage(page);
      }
    }
    fetchVets();
  }, [hash]); // Add hash as dependency to refetch when it changes

  if (loading) {
    return (
      <div>
        <h2>Vets</h2>
        <p>Loading veterinarians...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div>
        <h2>Vets</h2>
        <p style={{ color: 'red' }}>Error loading vets: {error}</p>
      </div>
    );
  }

  return (
    <div>
      <h2>Veterinarians</h2>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>
              Name
            </th>
            <th>
              Specialties
            </th>
          </tr>
        </thead>
        <tbody>
          {vets.map((vet: Vet) => (
            <tr key={vet.id}>
              <td>
                {vet.firstName} {vet.lastName}
              </td>
              <td>
                {vet.specialties.length > 0 ? (
                  vet.specialties.map((specialty: Specialty) => specialty.name).join(', ')
                ) : (
                  <span>none</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <Pagination linkBase='#vets' currentPage={currentPage} setCurrentPage={setCurrentPage} totalPages={totalPages} />
      {vets.length === 0 && (
        <p>No veterinarians found.</p>
      )}
    </div>
  );
}