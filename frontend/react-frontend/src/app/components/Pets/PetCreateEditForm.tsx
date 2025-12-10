import { useEffect, useState } from "react";
import { HashProps, OwnerDetails, PetType } from "../../types/Types";
import { fetchOwnerById, fetchPetTypes } from "../Api";

export default function PetCreateEditForm({ hash, setHash }: HashProps) {

    // State for owner details rendered on the top of the form
    const [firstName, setFirstName] = useState<string>('');
    const [lastName, setLastName] = useState<string>('');

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
            } catch (err) {
                console.warn('Error fetching pet types:', err);
            }
        };

        fetchOwnerDetails();
        fetchPetTypeDetails();
    }, []);

    return (<div>
  <h2>{ownerId ? 'Edit Pet' : 'Create Pet'}</h2>
  <form className="form-horizontal" method="post">
    <input type="hidden" name="id" value="" />
    <div className="form-group has-feedback">
      <div className="form-group">
        <label className="col-sm-2 control-label">Owner</label>
        <div className="col-sm-10">
          <span>{firstName} {lastName}</span>
        </div>
      </div>

      <div className="form-group">
        <label htmlFor="name" className="col-sm-2 control-label">Name</label>
        <div className="col-sm-10">
          <div>
            <input className="form-control" type="text" id="name" name="name" value="" />

          </div>
          <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>

        </div>
      </div>


      <div className="form-group">
        <label for="birthDate" className="col-sm-2 control-label">Birth Date</label>
        <div className="col-sm-10">
          <div>

            <input className="form-control" type="date" id="birthDate" name="birthDate" value="" />
          </div>
          <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>

        </div>
      </div>


      <div className="form-group">
        <label htmlFor="type" className="col-sm-2 control-label">Type</label>

        <div className="col-sm-10">
          <select id="type" name="type">
            {petTypes.map((petType) => (
              <option key={petType.id} value={petType.id}>{petType.name}</option>
            ))}
          </select>
          <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>

        </div>
      </div>

    </div>
    <div className="form-group">
      <div className="col-sm-offset-2 col-sm-10">
        <button className="btn btn-primary" type="submit">Add Pet</button>
      </div>
    </div>
  </form></div>);
}
