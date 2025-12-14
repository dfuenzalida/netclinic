import { useEffect, useState } from "react";
import { HashProps, OwnerDetails, PetCreateErrors, PetType } from "../../types/Types";
import { fetchOwnerById, fetchPetById, fetchPetTypes } from "../Api";
import T from "../Translations";

export default function PetCreateEditForm({ hash, setHash }: HashProps) {

    // State for owner details rendered on the top of the form
    const [firstName, setFirstName] = useState<string>('');
    const [lastName, setLastName] = useState<string>('');

    // State for form fields
    const [name, setName] = useState<string>('');
    const [birthDate, setBirthDate] = useState<string>('');
    const [type, setType] = useState<string>('');

    // State for form errors
    const [errors, setErrors] = useState<PetCreateErrors>({});

    // State for pet types dropdown
    const [petTypes, setPetTypes] = useState<PetType[]>([]);

    // Find the ownerId from the hash
    var ownerId : number|null = null;
    var petId: number|null = null;
    const ownerIdStr = hash.split('/')[1];
    ownerId = parseInt(ownerIdStr, 10);
    if (!hash.endsWith('/pets/new')) {
        const petIdStr = hash.split('/')[3];
        if (petIdStr) {
            petId = parseInt(petIdStr, 10);
        }
    }
    
    useEffect(() => {

        const fetchOwnerDetails = async () => {
            try {
                const data: OwnerDetails = await fetchOwnerById(ownerId!);
                setFirstName(data.firstName);
                setLastName(data.lastName);
            } catch (err) {
            console.warn('Error fetching owner details for edit:', err);
            }
        };    

        // Fetch pet types on component mount
        const fetchPetTypeDetails = async () => {
            try {
                const types = await fetchPetTypes();
                setPetTypes(types);
                // Set default pet type if not already set
                if (type == '') { setType(types[0].name); }
            } catch (err) {
                console.warn('Error fetching pet types:', err);
            }
        };

        const fetchPetDetails = async () => {
            if (ownerId && petId) {
                try {
                    const data = await fetchPetById(ownerId, petId);
                    setName(data.name);
                    setBirthDate(data.birthDate);
                    setType(data.type);
                } catch (err) {
                    console.warn('Error fetching pet details for edit:', err);
                }
            }
        };

        fetchOwnerDetails();
        fetchPetTypeDetails();
        fetchPetDetails();
    }, []);

      function upsertPet(e: React.FormEvent) {
        e.preventDefault();
        const endpoint = petId ? `/api/owners/${ownerId}/pets/${petId}` : `/api/owners/${ownerId}/pets`;
        const method = petId ? 'PUT' : 'POST';
        const flashMessage = petId ? 'Pet Values Updated' : 'New Pet Created';

        fetch(endpoint, {
          method: method,
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            name,
            birthDate,
            type,
          }),
        })
        .then(async (response) => {
          if (response.ok) {
            const data = await response.json();
            setHash(`#owners/${ownerId}/pets/${data.id}?flash=${encodeURIComponent(flashMessage)}`);
          } else if (response.status === 400) {
            const errorResponse = await response.json();
            console.log('Validation errors:', errorResponse);
            setErrors(errorResponse as PetCreateErrors);
          } else {
            console.error('Failed to create owner:', response.statusText);
          }
        })
        .catch((error) => {
          console.error('Error creating owner:', error);
        });
      }

    return (<div>
  <h2>{petId ? T("pet") : T("addNewPet")}</h2>
  <form className="form-horizontal" method="post">
    <input type="hidden" name="id" value="" />
    <div className="form-group has-feedback">
      <div className="form-group">
        <label className="col-sm-2 control-label">{T("owner")}</label>
        <div className="col-sm-10">
          <span>{firstName} {lastName}</span>
        </div>
      </div>

      <div className="form-group">
        <label htmlFor="name" className="col-sm-2 control-label">{T("name")}</label>
        <div className="col-sm-10">
          <div>
            <input className="form-control" type="text" id="name" name="name"
                value={name} onChange={(e) => setName(e.target.value)} />
          </div>
            {errors?.name ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.name}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
        </div>
      </div>


      <div className="form-group">
        <label htmlFor="birthDate" className="col-sm-2 control-label">{T("birthDate")}</label>
        <div className="col-sm-10">
          <div>

            <input className="form-control" type="date" id="birthDate" name="birthDate"
                value={birthDate} onChange={(e) => setBirthDate(e.target.value)} />
          </div>
              {errors?.birthDate ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.birthDate}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
        </div>
      </div>


      <div className="form-group">
        <label htmlFor="type" className="col-sm-2 control-label">{T("type")}</label>

        <div className="col-sm-10">
          <select id="type" name="type" onChange={(e) => {setType(e.target.value)}}>
            {petTypes.map((petType) => (
              <option key={petType.id} value={petType.name} selected={petType.name === type}>{petType.name}</option>
            ))}
          </select>
          <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>
        </div>
      </div>

    </div>
    <div className="form-group">
      <div className="col-sm-offset-2 col-sm-10">
        <button className="btn btn-primary" onClick={(e) => upsertPet(e)}>{petId ? T("editPet") : T("addNewPet")}</button>
      </div>
    </div>
  </form></div>);
}
