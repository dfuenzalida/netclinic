'use client';

import { useState } from 'react';
import { OwnerCreateErrors, OwnersViewNames } from '../types/Types';
import OwnerSearchForm from './Owners/OwnerSearchForm';
import OwnerSearchResults from './Owners/OwnerSearchResults';
import OwnerDetailsView from './Owners/OwnerDetailsView';
import OwnerCreateEditForm from './Owners/OwnerCreateEditForm';

export default function Owners() {

  const [ownerId, setOwnerId] = useState<number|null>(null);
  const [lastName, setLastName] = useState<string>('');
  const [ownersView, setOwnersView] = useState<OwnersViewNames>('searchForm');
  const [errorMessage, setErrorMessage] = useState<string|null>(null);
  const [ownerCreateErrors, setOwnerCreateErrors] = useState<OwnerCreateErrors>({});

  switch (ownersView) {
    case 'ownerDetails':
      return <OwnerDetailsView ownerId={ownerId ? ownerId : 0} setOwnerId={setOwnerId} setOwnersView={setOwnersView} />;
    case 'searchResults':
      return <OwnerSearchResults lastName={lastName} ownersView={ownersView} setOwnersView={setOwnersView}
                errorMessage={errorMessage} setErrorMessage={setErrorMessage} setOwnerId={setOwnerId} />;
    case 'searchForm':
      return <OwnerSearchForm lastName={lastName} setLastName={setLastName}
                errorMessage={errorMessage} setOwnersView={setOwnersView} />;
    case 'ownerCreateForm':
      return <OwnerCreateEditForm setOwnerId={setOwnerId} setOwnersView={setOwnersView}
                errors={ownerCreateErrors} setErrors={setOwnerCreateErrors} ownerId={ownerId} />;
  };
}