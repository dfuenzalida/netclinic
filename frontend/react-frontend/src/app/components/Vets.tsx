'use client';

import { useState, useEffect } from 'react';
import Pagination from './Pagination';
import { Vet, Specialty, HashProps } from '../../types/types';
import T from './Translations';
import { fetchVetsByPage } from './Api';

export default function Vets({ hash, setHash }: HashProps) {
  const [vets, setVets] = useState<Vet[]>([]);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  let currentPage = 1;
  if (hash && hash.indexOf('?page=') > -1) {
    currentPage = parseInt(hash.substring(hash.indexOf('?page=') + 6)) || 1;
  }

  const setCurrentPage = (page: number) => {
    setHash(`#vets?page=${page}`);
  }

  useEffect(() => {

    if (hash) {
      const pos = hash.indexOf('?page=');
      if (pos > -1) {
        const page = parseInt(hash.substring(pos + 6)) || 1;
        setHash(`#vets?page=${page}`);
      }
    }

    const fetchVets = async () => {
      try {
        setLoading(true);
        const data = await fetchVetsByPage(currentPage);
        setVets(data.vetList);
        setTotalPages(data.totalPages);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchVets();
  }, [hash, setHash, currentPage]); // Add hash and currentPage as dependencies to refetch when they change

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
      <h2>{T("vets")}</h2>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>
              {T("name")}
            </th>
            <th>
              {T("specialties")}
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
                  <span>{T("none")}</span>
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