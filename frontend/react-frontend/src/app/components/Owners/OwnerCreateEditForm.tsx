import { OwnersViewNames, OwnerCreateEditFormProps, OwnerCreateErrors, OwnerDetails } from "../../types/Types";
import { useEffect, useState } from "react";
import { fetchOwnerById } from "../Api";

export default function OwnerCreateEditForm({ ownerId, setOwnerId, setOwnersView, errors, setErrors }: OwnerCreateEditFormProps) {

  const [firstName, setFirstName] = useState<string>('');
  const [lastName, setLastName] = useState<string>('');
  const [address, setAddress] = useState<string>('');
  const [city, setCity] = useState<string>('');
  const [telephone, setTelephone] = useState<string>('');

  // When editing an existing owner, fetch and populate fields
  useEffect(() => {
    const fetchOwnerDetails = async () => {
      try {
        const data: OwnerDetails = await fetchOwnerById(ownerId!);
        setFirstName(data.firstName);
        setLastName(data.lastName);
        setAddress(data.address);
        setCity(data.city);
        setTelephone(data.telephone);
      } catch (err) {
        console.warn('Error fetching owner details for edit:', err);
      }
    };

    // Only fetch if editing an existing owner
    if (ownerId !== null) {
      fetchOwnerDetails();
    }
  }, [firstName, lastName, address, city, telephone, ownerId]);

  function upsertOwner(e: React.FormEvent) {
    e.preventDefault();
    const endpoint = ownerId ? `/api/owners/${ownerId}` : '/api/owners';
    const method = ownerId ? 'PUT' : 'POST';
    const flashMessage = ownerId ? 'Owner Values Updated' : 'New Owner Created';

    fetch(endpoint, {
      method: method,
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        firstName,
        lastName,
        address,
        city,
        telephone,
      }),
    })
    .then(async (response) => {
      if (response.ok) {
        const data = await response.json();
        setOwnerId(data.id);
        setOwnersView('ownerDetails');
      } else if (response.status === 400) {
        const errorResponse = await response.json();
        console.log('Validation errors:', errorResponse);
        setErrors(errorResponse as OwnerCreateErrors);
      } else {
        console.error('Failed to create owner:', response.statusText);
      }
    })
    .catch((error) => {
      console.error('Error creating owner:', error);
    });
  }

  return (<div>
    <h2>Owner</h2>
      <form className="form-horizontal" id="add-owner-form" action="#">
        <div className="form-group has-feedback">

          <div className="form-group has-error">
            <label htmlFor="firstName" className="col-sm-2 control-label">First Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="firstName"
                  value={firstName} onChange={(e) => setFirstName(e.target.value)} />
              </div>
              {errors?.firstName ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.firstName}</span>
              </>) :
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>}
            </div>
          </div>

          <div className="form-group has-error">
            <label htmlFor="lastName" className="col-sm-2 control-label">Last Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="lastName"
                  value={lastName} onChange={(e) => setLastName(e.target.value)} />
              </div>
              {errors?.lastName ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.lastName}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
            </div>
          </div>

          <div className="form-group has-error">
            <label htmlFor="address" className="col-sm-2 control-label">Address</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="address"
                   value={address} onChange={(e) => setAddress(e.target.value)} />
              </div>
              {errors?.address ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.address}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
            </div>
          </div>

          <div className="form-group has-error">
            <label htmlFor="city" className="col-sm-2 control-label">City</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="city"
                  value={city} onChange={(e) => setCity(e.target.value)} />
              </div>
              {errors?.city ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.city}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
            </div>
          </div>

          <div className="form-group has-error">
            <label htmlFor="telephone" className="col-sm-2 control-label">Telephone</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="telephone"
                  value={telephone} onChange={(e) => setTelephone(e.target.value)} />
              </div>
              {errors?.telephone ? (
              <>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                &nbsp;
                <span className="help-inline">{errors.telephone}</span>
              </>) : (
              <span className="fa fa-ok form-control-feedback" aria-hidden="true"></span>)}
            </div>
          </div>

        </div>
        <div className="form-group">
          <div className="col-sm-offset-2 col-sm-10">
            <button className="btn btn-primary" onClick={(e) => upsertOwner(e)}>{ownerId ? 'Update Owner' : 'Add Owner'}</button>
          </div>
        </div>
      </form>
    </div>);
}
