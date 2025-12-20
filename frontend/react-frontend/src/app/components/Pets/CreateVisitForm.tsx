import { HashProps, PetDetails, PetVisit } from "../../types/Types";
import { useEffect, useState } from "react";
import { VisitCreateErrors, OwnerDetails } from "../../types/Types";
import { fetchOwnerById, fetchPetById, fetchPetVisitsById } from "../Api";
import T from "../Translations";

export default function CreateVisitForm({ hash, setHash }: HashProps) {

    // State for owner details rendered above/below the new visit form
    const [ownerFirstName, setOwnerFirstName] = useState<string>('');
    const [ownerLastName, setOwnerLastName] = useState<string>('');
    const [pet, setPet] = useState<PetDetails | null>(null);
    const [petVisits, setPetVisits] = useState<PetVisit[]>([]);

    // State for form fields
    const [visitDate, setVisitDate] = useState<string>('');
    const [description, setDescription] = useState<string>('');

    // State for form errors
    const [errors, setErrors] = useState<VisitCreateErrors>({});

    // Find the ownerId and petId from the hash
    let ownerId : number|null = null;
    let petId: number|null = null;
    const ownerIdStr = hash.split('/')[1];
    ownerId = parseInt(ownerIdStr, 10);
    const petIdStr = hash.split('/')[3];
    petId = parseInt(petIdStr, 10);
    
    useEffect(() => {

        const fetchOwnerDetails = async () => {
            try {
                const data: OwnerDetails = await fetchOwnerById(ownerId!);
                setOwnerFirstName(data.firstName);
                setOwnerLastName(data.lastName);
            } catch (err) {
                console.warn('Error fetching owner details for visit creation:', err);
            }
        };

        const fetchPetDetails = async () => {
            try {
                if (ownerId && petId) {
                    const pet: PetDetails = await fetchPetById(ownerId, petId);
                    setPet(pet);
                }
            } catch (err) {
                console.warn('Error fetching pet details for visit creation:', err);
            }
        };

        const fetchPetVisits = async () => {
            try {
                if (ownerId && petId) {
                    const visits : PetVisit[] = await fetchPetVisitsById(ownerId, petId);
                    setPetVisits(visits);
                }
            } catch (err) {
                console.warn('Error fetching pet visits for visit creation:', err);
            }
        };

        // Fetch owner and pet details on component mount
        if (ownerId && petId) {
            fetchOwnerDetails();
            fetchPetDetails();
            fetchPetVisits();
        }
    }, [ownerId, petId]);

    function createVisit(e: React.FormEvent) {
        e.preventDefault();
        const endpoint = `/api/owners/${ownerId}/pets/${petId}/visits`;

        fetch(endpoint, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            visitDate,
            description
          }),
        })
        .then(async (response) => {
          if (response.ok) {
            setHash(`#owners/${ownerId}/pets/${petId}?flash=Visit Created`);
          } else if (response.status === 400) {
            const errorData = await response.json();
            setErrors(errorData);
          } else {
            console.error('Failed to create visit:', response.statusText);
          }
        })
        .catch((err) => {
          console.error('Error creating visit:', err);
        });
    }

    return (<>
      <h2>{T("addVisit")}</h2>

      <b>{T("pet")}</b>
      <table className="table table-striped">
        <thead>
            <tr>
                <th>{T("name")}</th>
                <th>{T("birthDate")}</th>
                <th>{T("type")}</th>
                <th>{T("owner")}</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>{pet?.name}</td>
                <td>{pet?.birthDate}</td>
                <td>{pet?.type}</td>
                <td>{ownerFirstName} {ownerLastName}</td>
            </tr>
        </tbody>
      </table>

      <form className="form-horizontal" method="post" onSubmit={createVisit}>
        <div className="form-group has-feedback">
          <label htmlFor="date" className="col-sm-2 control-label">{T("date")}</label>

          <div className="col-sm-10">
            <div>
                <input type="date" className="form-control" id="date" name="date"
                    value={visitDate} onChange={(e) => setVisitDate(e.target.value)} />
            </div>
              {errors?.visitDate ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.visitDate}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}

          </div>
        </div>

        <div className="form-group has-feedback">
          <label htmlFor="description" className="col-sm-2 control-label">{T("description")}</label>

          <div className="col-sm-10">
            <input type="text" className="form-control" id="description" name="description"
                value={description} onChange={(e) => setDescription(e.target.value)} />
          </div>
            {errors?.description ? (
            <>
            <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
            &nbsp;
            <span className="help-inline">{errors.description}</span>
            </>) : (
            <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
        </div>

        <div className="form-group">
          <div className="col-sm-offset-2 col-sm-10">
            <button onClick={createVisit} className="btn btn-primary">{T("addVisit")}</button>
          </div>
        </div>
      </form>

        <br />
        <b>{T("previousVisits")}</b>
        <table className="table table-striped">
          <tbody>
            <tr>
                <th>{T("date")}</th>
                <th>{T("description")}</th>
            </tr>
            {
            petVisits.map((visit, index) => (
                <tr key={index}>
                    <td>{visit.visitDate}</td>
                    <td>{visit.description}</td>
                </tr>))
            }
            </tbody>
        </table>
      </>);
}