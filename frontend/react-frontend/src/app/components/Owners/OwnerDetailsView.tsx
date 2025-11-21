import { OwnerDetails, OwnerDetailsProps, PetDetails, VisitDetails } from '../../types/Types';
import { useEffect, useState } from "react";

export default function OwnerDetailsView({ownerId, setOwnersView}: OwnerDetailsProps) {
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
